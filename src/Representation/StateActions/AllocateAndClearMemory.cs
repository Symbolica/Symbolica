using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.StateActions
{
    public class AllocateAndClearMemory : IFunc<IState, IExpression>
    {
        private readonly Bytes _size;

        public AllocateAndClearMemory(Bytes size)
        {
            _size = size;
        }

        public IExpression Run(IState state)
        {
            IExpression AllocateMemory(IState state, Bits size)
            {
                var address = state.Memory.Allocate(size);
                state.Memory.Write(address, state.Space.CreateConstant(size, BigInteger.Zero));
                return address;
            }

            return _size == Bytes.Zero
                ? state.Space.CreateConstant(state.Space.PointerSize, BigInteger.Zero)
                : AllocateMemory(state, _size.ToBits());
        }
    }

    public class AllocateAndClearMemoryOfSize : IFunc<BigInteger, IFunc<IState, IExpression>>
    {
        public IFunc<IState, IExpression> Run(BigInteger value) =>
            new AllocateAndClearMemory((Bytes)(uint)value);
    }
}
