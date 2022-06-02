using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        _equivalencyHash = new(() =>
        {
            return HashCode.Combine(
                _allocations.GetEquivalencyHash(),
                Caller.GetEquivalencyHash(),
                _formals.GetEquivalencyHash(),
                _jumps.GetEquivalencyHash(),
                _programCounter.GetEquivalencyHash(),
                _vaList.GetEquivalencyHash(),
                _variables.GetEquivalencyHash());
        });
        _mergeHash = new(() =>
        {
            return HashCode.Combine(
                _allocations.GetMergeHash(),
                Caller.GetMergeHash(),
                _formals.GetMergeHash(),
                _jumps.GetMergeHash(),
                _programCounter.GetMergeHash(),
                _vaList.GetMergeHash(),
                _variables.GetMergeHash());
        });
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
        return _allocations.IsEquivalentTo(other._allocations)
            .And(_formals.IsEquivalentTo(other._formals))
            .And(Caller.IsEquivalentTo(other.Caller))
            .And(_jumps.IsEquivalentTo(other._jumps))
            .And(_programCounter.IsEquivalentTo(other._programCounter))
            .And(_vaList.IsEquivalentTo(other._vaList))
            .And(_variables.IsEquivalentTo(other._variables));
    }

    public object ToJson()
    {
        return new
        {
            Allocations = _allocations.ToJson(),
            Caller = Caller.ToJson(),
            Formals = _formals.ToJson(),
            Jumps = _jumps.ToJson(),
            ProgramCounter = _programCounter.ToJson(),
            VaList = _vaList.ToJson(),
            Variables = _variables.ToJson()
        };
    }

    public int GetEquivalencyHash()
    {
        return _equivalencyHash.Value;
    }

    public int GetMergeHash()
    {
        return _mergeHash.Value;
    }

    public bool TryMerge(IPersistentFrame other, IExpression predicate, [MaybeNullWhen(false)] out IPersistentFrame merged)
    {
        merged = null;
        return other is PersistentFrame pf
            && TryMerge(pf, predicate, out merged);
    }

    public bool TryMerge(ISavedFrame other, IExpression predicate, [MaybeNullWhen(false)] out ISavedFrame merged)
    {
        merged = null;
        return other is PersistentFrame pf
            && TryMerge(pf, predicate, out merged);
    }

    private bool TryMerge(PersistentFrame other, IExpression predicate, [MaybeNullWhen(false)] out IPersistentFrame merged)
    {
        if (_allocations.TryMerge(other._allocations, predicate, out var mergedAllocations)
            && Caller.TryMerge(other.Caller, predicate, out var mergedCaller)
            && _formals.TryMerge(other._formals, predicate, out var mergedFormals)
            && _jumps.TryMerge(other._jumps, predicate, out var mergedJumps)
            && _programCounter.TryMerge(other._programCounter, predicate, out var mergedProgramCounter)
            && _vaList.TryMerge(other._vaList, predicate, out var mergedVaList)
            && _variables.TryMerge(other._variables, predicate, out var mergedVariables))
        {
            merged = new PersistentFrame(
                mergedCaller,
                mergedFormals,
                mergedVaList,
                mergedJumps,
                mergedProgramCounter,
                mergedVariables,
                mergedAllocations);
            return true;
        }

        merged = null;
        return false;
    }
}
