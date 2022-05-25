using System;
using System.Collections.Generic;
using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Abstraction.Memory;
using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Implementation.Stack;

internal sealed class PersistentStack : IPersistentStack
{
    private readonly IPersistentContinuationFactory _continuationFactory;
    private readonly IPersistentFrame _currentFrame;
    private readonly IFrameFactory _frameFactory;
    private readonly IModule _module;
    private readonly IPersistentStack<IPersistentFrame> _pushedFrames;
    private readonly Lazy<int> _equivalencyHash;
    private readonly Lazy<int> _mergeHash;

    private PersistentStack(IModule module, IFrameFactory frameFactory,
        IPersistentContinuationFactory continuationFactory,
        IPersistentStack<IPersistentFrame> pushedFrames, IPersistentFrame currentFrame)
    {
        _module = module;
        _frameFactory = frameFactory;
        _continuationFactory = continuationFactory;
        _pushedFrames = pushedFrames;
        _currentFrame = currentFrame;
        _equivalencyHash = new(() => EquivalencyHash(false));
        _mergeHash = new(() => EquivalencyHash(true));

        int EquivalencyHash(bool includeSubs)
        {
            var hash = new HashCode();
            foreach (var frame in AllFrames)
                hash.Add(frame.GetEquivalencyHash(includeSubs));
            return hash.ToHashCode();
        }
    }

    public bool IsInitialFrame => !_pushedFrames.Any();
    public BasicBlockId PredecessorId => _currentFrame.PredecessorId;
    public IInstruction Instruction => _currentFrame.Instruction;
    private IEnumerable<IPersistentFrame> AllFrames => new[] { _currentFrame }.Concat(_pushedFrames);

    public IPersistentStack Wind(ISpace space, IMemory memory, ICaller caller, IInvocation invocation)
    {
        return new PersistentStack(_module, _frameFactory,
            _continuationFactory,
            _pushedFrames.Push(_currentFrame), _frameFactory.Create(space, memory, caller, invocation));
    }

    public (ICaller, IPersistentStack) Unwind(ISpace space, IMemory memory)
    {
        return (_currentFrame.Caller, Pop(space, memory));
    }

    public IPersistentStack Save(ISpace space, IMemory memory, IExpression address, bool useJumpBuffer)
    {
        var size = GetContinuationSize(useJumpBuffer);
        var (continuation, continuationFactory) = _continuationFactory.Create(size);

        memory.Write(space, address, continuation);

        return new PersistentStack(_module, _frameFactory,
            continuationFactory,
            _pushedFrames, _currentFrame.Save(continuation, useJumpBuffer));
    }

    public IPersistentStack Restore(ISpace space, IMemory memory, IExpression address, bool useJumpBuffer)
    {
        var size = GetContinuationSize(useJumpBuffer);
        var continuation = memory.Read(space, address, size);

        var stack = this;

        while (true)
        {
            var result = stack._currentFrame.TryRestore(space, continuation, useJumpBuffer);

            if (result.IsSuccess)
            {
                var currentFrame = result.Value;

                Free(space, memory, stack._currentFrame.GetAllocations()
                    .SkipLast(currentFrame.GetAllocations().Count()));

                return new PersistentStack(_module, _frameFactory,
                    _continuationFactory,
                    stack._pushedFrames, currentFrame);
            }

            if (stack.IsInitialFrame)
                throw new StateException(StateError.InvalidJump, space);

            stack = stack.Pop(space, memory);
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

    public (IExpression, IPersistentStack) Allocate(IMemory memory, Bits size)
    {
        var address = memory.Allocate(Section.Stack, size);

        return (address, new PersistentStack(_module, _frameFactory,
            _continuationFactory,
            _pushedFrames, _currentFrame.AddAllocation(address)));
    }

    public (HashSet<(IExpression, IExpression)> subs, bool) IsEquivalentTo(IPersistentStack other)
    {
        return other is PersistentStack ps
            ? AllFrames.IsSequenceEquivalentTo<IExpression, IPersistentFrame>(ps.AllFrames)
            : (new(), false);
    }

    private PersistentStack Pop(ISpace space, IMemory memory)
    {
        Free(space, memory, _currentFrame.GetAllocations());

        return new PersistentStack(_module, _frameFactory,
            _continuationFactory,
            _pushedFrames.Pop(out var currentFrame), currentFrame);
    }

    private Bits GetContinuationSize(bool useJumpBuffer)
    {
        return useJumpBuffer
            ? _module.JumpBufferType.Size.ToBits()
            : Bytes.One.ToBits();
    }

    private static void Free(ISpace space, IMemory memory, IEnumerable<IExpression> allocations)
    {
        foreach (var allocation in allocations)
            memory.Free(space, Section.Stack, allocation);
    }

    public static IPersistentStack Create(IModule module, IFrameFactory frameFactory,
        IPersistentContinuationFactory continuationFactory, ICollectionFactory collectionFactory)
    {
        var main = module.GetMain();

        return new PersistentStack(module, frameFactory,
            continuationFactory,
            collectionFactory.CreatePersistentStack<IPersistentFrame>(), frameFactory.CreateInitial(main));
    }

    public object ToJson()
    {
        return AllFrames.Select(f => f.ToJson()).ToArray();
    }

    public int GetEquivalencyHash(bool includeSubs)
    {
        return includeSubs
            ? _mergeHash.Value
            : _equivalencyHash.Value;
    }
}
