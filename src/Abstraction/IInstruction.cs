using System.Collections.Generic;
using Symbolica.Expression;

namespace Symbolica.Abstraction;

public interface IInstruction : IMergeable<ExpressionSubs, IInstruction>
{
    InstructionId Id { get; }

    void Execute(IExpressionFactory exprFactory, IState state);

    (HashSet<ExpressionSubs> subs, bool) IMergeable<ExpressionSubs, IInstruction>.IsEquivalentTo(IInstruction other)
        => (new(), Id == other.Id);

    object IMergeable<ExpressionSubs, IInstruction>.ToJson() => (ulong) Id;

    int IMergeable<ExpressionSubs, IInstruction>.GetEquivalencyHash(bool includeSubs) => Id.GetEquivalencyHash(includeSubs);
}
