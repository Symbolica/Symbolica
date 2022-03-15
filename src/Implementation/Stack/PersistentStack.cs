using System.Collections.Generic;
using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Collection;
using Symbolica.Expression;
using Symbolica.Implementation.Memory;

namespace Symbolica.Implementation.Stack;

internal sealed class PersistentStack : IPersistentStack
{
    private readonly IPersistentContinuationFactory _continuationFactory;
    private readonly IPersistentFrame _currentFrame;
    private readonly IFrameFactory _frameFactory;
    private readonly IModule _module;
    private readonly IPersistentStack<IPersistentFrame> _pushedFrames;

    private PersistentStack(IModule module, IFrameFactory frameFactory,
        IPersistentContinuationFactory continuationFactory,
        IPersistentStack<IPersistentFrame> pushedFrames, IPersistentFrame currentFrame)
    {
        _module = module;
        _frameFactory = frameFactory;
        _continuationFactory = continuationFactory;
        _pushedFrames = pushedFrames;
        _currentFrame = currentFrame;
    }

    public bool IsInitialFrame => !_pushedFrames.Any();
    public BasicBlockId PredecessorId => _currentFrame.PredecessorId;
    public IInstruction Instruction => _currentFrame.Instruction;

    public IPersistentStack Wind(ISpace space, IMemoryProxy memory, ICaller caller, IInvocation invocation)
    {
        return new PersistentStack(_module, _frameFactory,
            _continuationFactory,
            _pushedFrames.Push(_currentFrame), _frameFactory.Create(space, memory, caller, invocation));
    }

    public (ICaller, IPersistentStack) Unwind(IMemoryProxy memory)
    {
        return (_currentFrame.Caller, Pop(memory));
    }

    public IPersistentStack Save(ISpace space, IMemory memory, IExpression address, bool useJumpBuffer)
    {
        var size = GetContinuationSize(useJumpBuffer);
        var (continuation, continuationFactory) = _continuationFactory.Create(space, size);

        memory.Write(address, continuation);

        return new PersistentStack(_module, _frameFactory,
            continuationFactory,
            _pushedFrames, _currentFrame.Save(continuation, useJumpBuffer));
    }

    public IPersistentStack Restore(ISpace space, IMemoryProxy memory, IExpression address, bool useJumpBuffer)
    {
        var size = GetContinuationSize(useJumpBuffer);
        var continuation = memory.Read(address, size);

        var stack = this;

        while (true)
        {
            var result = stack._currentFrame.TryRestore(space, continuation, useJumpBuffer);

            if (result.IsSuccess)
            {
                var currentFrame = result.Value;

                Free(memory, stack._currentFrame.GetAllocations()
                    .SkipLast(currentFrame.GetAllocations().Count()));

                return new PersistentStack(_module, _frameFactory,
                    _continuationFactory,
                    stack._pushedFrames, currentFrame);
            }

            if (stack.IsInitialFrame)
                throw new StateException(StateError.InvalidJump, space.GetExample());

            stack = stack.Pop(memory);
        }
    }

    public IPersistentStack TransferBasicBlock(BasicBlockId id)
    {
        return new PersistentStack(_module, _frameFactory,
            _continuationFactory,
            _pushedFrames, _currentFrame.TransferBasicBlock(id));
    }

    public IPersistentStack MoveNextInstruction()
    {
        return new PersistentStack(_module, _frameFactory,
            _continuationFactory,
            _pushedFrames, _currentFrame.MoveNextInstruction());
    }

    public IExpression GetFormal(int index)
    {
        return _currentFrame.GetFormal(index);
    }

    public IExpression GetInitializedVaList(ISpace space)
    {
        return _currentFrame.GetInitializedVaList(space, _module.VaListType);
    }

    public IExpression GetVariable(InstructionId id, bool useIncomingValue)
    {
        return _currentFrame.GetVariable(id, useIncomingValue);
    }

    public IPersistentStack SetVariable(InstructionId id, IExpression variable)
    {
        return new PersistentStack(_module, _frameFactory,
            _continuationFactory,
            _pushedFrames, _currentFrame.SetVariable(id, variable));
    }

    public (IExpression, IPersistentStack) Allocate(IMemoryProxy memory, Bits size)
    {
        var address = memory.Allocate(Section.Stack, size);

        return (address, new PersistentStack(_module, _frameFactory,
            _continuationFactory,
            _pushedFrames, _currentFrame.AddAllocation(address)));
    }

    private PersistentStack Pop(IMemoryProxy memory)
    {
        Free(memory, _currentFrame.GetAllocations());

        return new PersistentStack(_module, _frameFactory,
            _continuationFactory,
            _pushedFrames.Pop(out var currentFrame), currentFrame);
    }

    private Bits GetContinuationSize(bool useJumpBuffer)
    {
        return useJumpBuffer
            ? _module.JumpBufferType.Size
            : Bytes.One.ToBits();
    }

    private static void Free(IMemoryProxy memory, IEnumerable<IExpression> allocations)
    {
        foreach (var allocation in allocations)
            memory.Free(Section.Stack, allocation);
    }

    public static IPersistentStack Create(IModule module, IFrameFactory frameFactory,
        IPersistentContinuationFactory continuationFactory, ICollectionFactory collectionFactory)
    {
        var main = module.GetMain();

        return new PersistentStack(module, frameFactory,
            continuationFactory,
            collectionFactory.CreatePersistentStack<IPersistentFrame>(), frameFactory.CreateInitial(main));
    }
}
