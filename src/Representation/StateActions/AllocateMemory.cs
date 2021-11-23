using System;
using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.StateActions
{
    [Serializable]
    public class AllocateMemory : IFunc<IState, IExpression>
    {
        private readonly Bytes _size;

        public AllocateMemory(Bytes size)
        {
            _size = size;
        }

        public IExpression Run(IState state)
        {
            return _size == Bytes.Zero
                ? state.Space.CreateConstant(state.Space.PointerSize, BigInteger.Zero)
                : state.Memory.Allocate(_size.ToBits());
        }
    }

    [Serializable]
    public class AllocateMemoryOfSize : IFunc<BigInteger, IFunc<IState, IExpression>>
    {
        public IFunc<IState, IExpression> Run(BigInteger value) =>
            new AllocateMemory((Bytes)(uint)value);
    }
}
