using System.Collections.Generic;
using Symbolica.Expression;

namespace Symbolica.Abstraction;

public interface IInstruction : IMergeable<IExpression, IInstruction>
{
    InstructionId Id { get; }

    void Execute(IExpressionFactory exprFactory, IState state);

    (HashSet<(IExpression, IExpression)> subs, bool) IMergeable<IExpression, IInstruction>.IsEquivalentTo(IInstruction other)
        => (new(), Id == other.Id);

    object IMergeable<IExpression, IInstruction>.ToJson() => (ulong) Id;

    int IMergeable<IExpression, IInstruction>.GetEquivalencyHash(bool includeSubs) => Id.GetEquivalencyHash(includeSubs);
}
