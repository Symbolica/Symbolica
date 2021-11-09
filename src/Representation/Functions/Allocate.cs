using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Functions
{
    internal sealed class Allocate : IFunction
    {
        public Allocate(FunctionId id, IParameters parameters)
        {
            Id = id;
            Parameters = parameters;
        }

        public FunctionId Id { get; }
        public IParameters Parameters { get; }

        public void Call(IState state, ICaller caller, IArguments arguments)
        {
            var size = arguments.Get(0);

            state.ForkAll(
                size,
                new MapFunc<BigInteger, IFunc<IState, IExpression>, IStateAction>(
                    new StateActions.AllocateMemoryOfSize(),
                    new StateActions.SetVariableFromFunc(caller.Id)));
        }
    }
}
