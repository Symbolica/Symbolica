using System.Linq;
using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Functions;

internal sealed class MemorySet : IFunction
{
    public MemorySet(FunctionId id, IParameters parameters)
    {
        Id = id;
        Parameters = parameters;
    }

    public FunctionId Id { get; }
    public IParameters Parameters { get; }

    public void Call(IExpressionFactory exprFactory, IState state, ICaller caller, IArguments arguments)
    {
        var destination = arguments.Get(0);
        var value = arguments.Get(1);
        var length = arguments.Get(2);

        state.ForkAll(exprFactory, length, new SetMemory(exprFactory, destination, value));
    }

    private sealed class SetMemory : IParameterizedStateAction
    {
        private readonly IExpressionFactory _exprFactory;
        private readonly IExpression _destination;
        private readonly IExpression _value;

        public SetMemory(IExpressionFactory exprFactory, IExpression destination, IExpression value)
        {
            _exprFactory = exprFactory;
            _destination = destination;
            _value = value;
        }

        public void Invoke(IState state, BigInteger value)
        {
            var destination = _destination is IAddress d ? d.AddImplicitOffsets() : _destination;
            foreach (var offset in Enumerable.Range(0, (int) value))
            {
                var address = destination.Add(_exprFactory.CreateConstant(_destination.Size, offset));
                state.Memory.Write(state.Space, address, _value);
            }
        }
    }
}
