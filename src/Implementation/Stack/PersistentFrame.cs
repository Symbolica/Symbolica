using System;
using System.Collections.Generic;
using System.Linq;
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
    private readonly Lazy<int> _equivalencyHash;
    private readonly Lazy<int> _mergeHash;

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
        _equivalencyHash = new(() => EquivalencyHash(false));
        _mergeHash = new(() => EquivalencyHash(true));

        int EquivalencyHash(bool includeSubs)
        {
            var allocationsHash = new HashCode();
            foreach (var allocation in _allocations)
                allocationsHash.Add(allocation.GetEquivalencyHash(includeSubs));

            var formalsHash = new HashCode();
            foreach (var formal in _formals)
                formalsHash.Add(formal.GetEquivalencyHash(includeSubs));

            return HashCode.Combine(
                allocationsHash.ToHashCode(),
                formalsHash.ToHashCode(),
                _jumps.GetEquivalencyHash(includeSubs),
                _programCounter.GetEquivalencyHash(includeSubs),
                _vaList.GetEquivalencyHash(includeSubs),
                _variables.GetEquivalencyHash(includeSubs));
        }
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

    public (HashSet<ExpressionSubs> subs, bool) IsEquivalentTo(IPersistentFrame other)
    {
        return other is PersistentFrame pf
            ? IsEquivalentTo(pf)
            : (new(), false);
    }

    public (HashSet<ExpressionSubs> subs, bool) IsEquivalentTo(ISavedFrame other)
    {
        return other is PersistentFrame pf
            ? IsEquivalentTo(pf)
            : (new(), false);
    }

    private (HashSet<ExpressionSubs> subs, bool) IsEquivalentTo(PersistentFrame other)
    {
        return _allocations.Concat(_formals)
            .IsSequenceEquivalentTo(
                other._allocations.Concat(other._formals),
                (x, y) => x.IsEquivalentTo(y).ToHashSet())
            .And(_jumps.IsEquivalentTo(other._jumps))
            .And(_programCounter.IsEquivalentTo(other._programCounter))
            .And(_vaList.IsEquivalentTo(other._vaList))
            .And(_variables.IsEquivalentTo(other._variables));
    }

    public object ToJson()
    {
        return new
        {
            Allocations = _allocations.Select(a => a.ToJson()).ToArray(),
            Formals = _formals.Select(f => f.ToJson()).ToArray(),
            Jumps = _jumps.ToJson(),
            ProgramCounter = _programCounter.ToJson(),
            VaList = _vaList.ToJson(),
            Variables = _variables.ToJson()
        };
    }

    public int GetEquivalencyHash(bool includeSubs)
    {
        return includeSubs
            ? _mergeHash.Value
            : _equivalencyHash.Value;
    }
}
