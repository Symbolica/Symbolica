using System.Collections.Generic;
using Symbolica.Expression;

namespace Symbolica.Abstraction;

public interface IInstruction : IMergeable<IExpression, IInstruction>
{
    InstructionId Id { get; }

    void Execute(IExpressionFactory exprFactory, IState state);

    (HashSet<(IExpression, IExpression)> subs, bool) IMergeable<IExpression, IInstruction>.IsEquivalentTo(IInstruction other)
        => (new(), Id == other.Id);
}
