using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Symbolica.Expression;

namespace Symbolica.Abstraction;

public interface ICaller : IEquivalent<ExpressionSubs, ICaller>, IMergeable<ICaller>
{
    InstructionId Id { get; }
    Bits Size { get; }
    IAttributes ReturnAttributes { get; }

    void Return(IState state);

    (HashSet<ExpressionSubs> subs, bool) IEquivalent<ExpressionSubs, ICaller>.IsEquivalentTo(ICaller other)
        => (new(), Id == other.Id);

    object IEquivalent<ExpressionSubs, ICaller>.ToJson() => (ulong) Id;

    int IEquivalent<ExpressionSubs, ICaller>.GetEquivalencyHash() => Id.GetEquivalencyHash();

    int IMergeable<ICaller>.GetMergeHash() => Id.GetMergeHash();

    bool IMergeable<ICaller>.TryMerge(
        ICaller other,
        IExpression predicate,
        [MaybeNullWhen(false)] out ICaller merged)
    {
        merged = this;
        return Id.TryMerge(other.Id, predicate, out var _);
    }
}
