using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation;

public sealed class BasicBlock : IBasicBlock
{
    private readonly IInstruction[] _instructions;
    private readonly Lazy<int> _equivalencyHash;
    private readonly Lazy<int> _mergeHash;

    public BasicBlock(BasicBlockId id, IInstruction[] instructions)
    {
        Id = id;
        _instructions = instructions;
        _equivalencyHash = new(() =>
        {
            var instructionsHash = new HashCode();
            foreach (var instruction in _instructions)
                instructionsHash.Add(instruction.GetEquivalencyHash());

            return HashCode.Combine(
                Id.GetEquivalencyHash(),
                instructionsHash.ToHashCode());
        });
        _mergeHash = new(() =>
        {
            var instructionsHash = new HashCode();
            foreach (var instruction in _instructions)
                instructionsHash.Add(instruction.GetMergeHash());

            return HashCode.Combine(
                Id.GetMergeHash(),
                instructionsHash.ToHashCode());
        });
    }

    public BasicBlockId Id { get; }

    public IInstruction GetInstruction(int index)
    {
        return _instructions[index];
    }

    public (HashSet<ExpressionSubs> subs, bool) IsEquivalentTo(IBasicBlock other)
    {
        return other is BasicBlock b
            ? Id.IsEquivalentTo(b.Id)
                .And(_instructions.IsSequenceEquivalentTo<ExpressionSubs, IInstruction>(b._instructions))
            : (new(), false);
    }

    public int GetEquivalencyHash()
    {
        return _equivalencyHash.Value;
    }

    public object ToJson()
    {
        return new
        {
            Id = Id.ToJson(),
            Instructions = _instructions.Select(i => i.ToJson()).ToArray()
        };
    }

    public int GetMergeHash()
    {
        return _mergeHash.Value;
    }

    public bool TryMerge(IBasicBlock other, IExpression predicate, [MaybeNullWhen(false)] out IBasicBlock merged)
    {
        if (other is BasicBlock bb
            && Id.TryMerge(bb.Id, predicate, out var mergedId)
            && _instructions.TryMerge(bb._instructions, predicate, out var mergedInstructions))
        {
            merged = new BasicBlock(mergedId, mergedInstructions.ToArray());
            return true;
        }

        merged = null;
        return false;
    }
}
