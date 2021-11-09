using System.Linq;
using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.StateActions
{
    public class SetMemory : IStateAction
    {
        private readonly IExpression _destination;
        private readonly IExpression _value;
        private readonly int _length;

        public SetMemory(IExpression destination, IExpression value, int length)
        {
            _destination = destination;
            _value = value;
            _length = length;
        }

        public Unit Run(IState state)
        {
            foreach (var offset in Enumerable.Range(0, _length))
            {
                var address = _destination.Add(state.Space.CreateConstant(_destination.Size, offset));
                state.Memory.Write(address, _value);
            }
            return new Unit();
        }
    }

    public class SetMemoryOfLength : IFunc<BigInteger, IStateAction>
    {
        private readonly IExpression _destination;
        private readonly IExpression _value;

        public SetMemoryOfLength(IExpression destination, IExpression value)
        {
            _destination = destination;
            _value = value;
        }

        public IStateAction Run(BigInteger value) =>
            new SetMemory(_destination, _value, (int)value);
    }
}
