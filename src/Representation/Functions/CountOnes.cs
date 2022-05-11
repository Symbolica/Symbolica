using System.Linq;
using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Functions;

internal sealed class CountOnes : IFunction
{
    public CountOnes(FunctionId id, IParameters parameters)
    {
        Id = id;
        Parameters = parameters;
    }

    public FunctionId Id { get; }
    public IParameters Parameters { get; }

    public void Call(IExpressionFactory exprFactory, IState state, ICaller caller, IArguments arguments)
    {
        var expression = arguments.Get(0);

        var result = Enumerable.Range(0, (int) (uint) expression.Size)
            .Aggregate(exprFactory.CreateZero(expression.Size), (l, r) =>
                l.Add(expression.LogicalShiftRight(exprFactory.CreateConstant(expression.Size, r))
                    .And(exprFactory.CreateConstant(expression.Size, BigInteger.One))));

        state.Stack.SetVariable(caller.Id, result);
    }
}
