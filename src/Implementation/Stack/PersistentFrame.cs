using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Implementation.Stack;

internal sealed class PersistentFrame : IPersistentFrame, ISavedFrame
{
    private readonly IPersistentAllocations _allocations;
    private readonly IArguments _formals;
    private readonly IPersistentJumps _jumps;
    private readonly IPersistentProgramCounter _programCounter;
    private readonly IVaList _vaList;
    private readonly IPersistentVariables _variables;

    public PersistentFrame(ICaller caller, IArguments formals, IVaList vaList,
        IPersistentJumps jumps, IPersistentProgramCounter programCounter,
        IPersistentVariables variables, IPersistentAllocations allocations)
    {
        Caller = caller;
        _formals = formals;
        _vaList = vaList;
        _jumps = jumps;
        _programCounter = programCounter;
        _variables = variables;
        _allocations = allocations;
    }

    public ICaller Caller { get; }
    public BasicBlockId PredecessorId => _programCounter.PredecessorId;
    public IInstruction Instruction => _programCounter.Instruction;

    public IPersistentFrame Save(IExpression continuation, bool useJumpBuffer)
    {
        return new PersistentFrame(Caller, _formals, _vaList,
            _jumps.Add(continuation, useJumpBuffer, this), _programCounter,
            _variables, _allocations);
    }

    public Result<IPersistentFrame> TryRestore(ISpace space, IExpression continuation, bool useJumpBuffer)
    {
        var result = _jumps.TryGet(space, continuation, useJumpBuffer);

        return result.IsSuccess
            ? Result<IPersistentFrame>.Success(
                result.Value.Restore(useJumpBuffer,
                    _jumps, _programCounter, _variables))
            : Result<IPersistentFrame>.Failure();
    }

    public IPersistentFrame TransferBasicBlock(BasicBlockId id)
    {
        return new PersistentFrame(Caller, _formals, _vaList,
            _jumps, _programCounter.TransferBasicBlock(id),
            _variables.TransferBasicBlock(), _allocations);
    }

    public IPersistentFrame MoveNextInstruction()
    {
        return new PersistentFrame(Caller, _formals, _vaList,
            _jumps, _programCounter.MoveNextInstruction(),
            _variables, _allocations);
    }

    public IExpression GetFormal(int index)
    {
        return _formals.Get(index);
    }

    public IExpression GetInitializedVaList(ISpace space, IStructType vaListType)
    {
        return _vaList.Initialize(space, vaListType);
    }

    public IExpression GetVariable(InstructionId id, bool useIncomingValue)
    {
        return _variables.Get(id, useIncomingValue);
    }

    public IPersistentFrame SetVariable(InstructionId id, IExpression variable)
    {
        return new PersistentFrame(Caller, _formals, _vaList,
            _jumps, _programCounter,
            _variables.Set(id, variable), _allocations);
    }

    public IPersistentFrame AddAllocation(IExpression allocation)
    {
        return new PersistentFrame(Caller, _formals, _vaList,
            _jumps, _programCounter,
            _variables, _allocations.Add(allocation));
    }

    public IAllocations GetAllocations()
    {
        return _allocations;
    }

    public IPersistentFrame Restore(bool useJumpBuffer,
        IPersistentJumps jumps, IPersistentProgramCounter programCounter, IPersistentVariables variables)
    {
        return useJumpBuffer
            ? new PersistentFrame(Caller, _formals, _vaList,
                jumps, _programCounter,
                _variables, _allocations)
            : new PersistentFrame(Caller, _formals, _vaList,
                jumps, programCounter,
                variables, _allocations);
    }
}
