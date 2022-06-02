using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Symbolica.Expression;

namespace Symbolica.Abstraction;

public interface IInstruction : IEquivalent<ExpressionSubs, IInstruction>, IMergeable<IInstruction>
{
    InstructionId Id { get; }

    void Execute(IExpressionFactory exprFactory, IState state);

    (HashSet<ExpressionSubs> subs, bool) IEquivalent<ExpressionSubs, IInstruction>.IsEquivalentTo(IInstruction other)
        => (new(), Id == other.Id);

    object IEquivalent<ExpressionSubs, IInstruction>.ToJson() => (ulong) Id;

    int IEquivalent<ExpressionSubs, IInstruction>.GetEquivalencyHash() => Id.GetEquivalencyHash();

    int IMergeable<IInstruction>.GetMergeHash() => Id.GetMergeHash();

    bool IMergeable<IInstruction>.TryMerge(
        IInstruction other,
        IExpression predicate,
        [MaybeNullWhen(false)] out IInstruction merged)
    {
        merged = this;
        return Id.TryMerge(other.Id, predicate, out var _);
    }
}
