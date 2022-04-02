using System.Linq;
using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Expression.Values;
using Symbolica.Expression.Values.Constants;

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

    public void Call(IState state, ICaller caller, IArguments arguments)
    {
        var expression = arguments.Get(0);

        var result = Enumerable.Range(0, (int) (uint) expression.Size)
            .Aggregate(ConstantUnsigned.CreateZero(expression.Size) as IExpression<IType>, (l, r) =>
                Add.Create(
                    l,
                    And.Create(
                        LogicalShiftRight.Create(expression, ConstantUnsigned.Create(expression.Size, r)),
                        ConstantUnsigned.Create(expression.Size, BigInteger.One))));

        state.Stack.SetVariable(caller.Id, result);
    }
}
