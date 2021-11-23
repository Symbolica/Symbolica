using System;
using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.StateActions
{
    [Serializable]
    public class CopyMemory : IStateAction
    {
        private readonly IExpression _destination;
        private readonly IExpression _source;
        private readonly Bytes _length;

        public CopyMemory(IExpression destination, IExpression source, Bytes length)
        {
            _destination = destination;
            _source = source;
            _length = length;
        }

        public Unit Run(IState state)
        {
            if (_length != Bytes.Zero)
                state.Memory.Write(_destination, state.Memory.Read(_source, _length.ToBits()));
            return new Unit();
        }
    }

    [Serializable]
    public class CopyMemoryOfLength : IFunc<BigInteger, IStateAction>
    {
        private readonly IExpression _destination;
        private readonly IExpression _source;

        public CopyMemoryOfLength(IExpression destination, IExpression source)
        {
            _destination = destination;
            _source = source;
        }

        public IStateAction Run(BigInteger value) =>
            new CopyMemory(_destination, _source, (Bytes)(uint)value);
    }
}
