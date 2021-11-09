using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.StateActions
{
    public class ReallocateMemory : IStateAction
    {
        private readonly InstructionId _callerId;
        private readonly IExpression _address;
        private readonly Bytes _size;

        public ReallocateMemory(InstructionId callerId, IExpression address, Bytes size)
        {
            _callerId = callerId;
            _address = address;
            _size = size;
        }

        public Unit Run(IState state)
        {
            if (_size == Bytes.Zero)
                FreeMemory(state);
            else
                AllocateMemory(state);

            return new Unit();
        }

        private void FreeMemory(IState state)
        {
            state.Memory.Free(_address);
            state.Stack.SetVariable(_callerId, state.Space.CreateConstant(_address.Size, BigInteger.Zero));
        }

        private void AllocateMemory(IState state)
        {
            state.Fork(
                _address,
                new SetVariable(_callerId, new MoveMemory(_address, _size.ToBits())),
                new SetVariable(_callerId, new AllocateMemory(_size)));
        }
    }

    public class ReallocateMemoryOfSize : IFunc<BigInteger, IStateAction>
    {
        private readonly InstructionId _callerId;
        private readonly IExpression _address;

        public ReallocateMemoryOfSize(InstructionId callerId, IExpression address)
        {
            _callerId = callerId;
            _address = address;
        }

        public IStateAction Run(BigInteger value) =>
            new ReallocateMemory(_callerId, _address, (Bytes)(uint)value);
    }
}
