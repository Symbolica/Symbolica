using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Functions
{
    internal sealed class Reallocate : IFunction
    {
        public Reallocate(FunctionId id, IParameters parameters)
        {
            Id = id;
            Parameters = parameters;
        }

        public FunctionId Id { get; }
        public IParameters Parameters { get; }

        public void Call(IState state, ICaller caller, IArguments arguments)
        {
            var address = arguments.Get(0);
            var size = arguments.Get(1);

            state.ForkAll(size, new CallValueAction(caller.Id, address));
        }

        private class CallValueAction : IForkAllAction
        {
            private readonly InstructionId _callerId;
            private readonly IExpression _address;

            public CallValueAction(InstructionId callerId, IExpression address)
            {
                _callerId = callerId;
                _address = address;
            }

            public IStateAction Run(BigInteger value) =>
                new CallAction(_callerId, _address, (Bytes)(uint)value);
        }

        private class CallAction : IStateAction
        {
            private readonly InstructionId _callerId;
            private readonly IExpression _address;
            private readonly Bytes _size;

            public CallAction(InstructionId callerId, IExpression address, Bytes size)
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
                    AllocateMemory(state, _size.ToBits());

                return new Unit();
            }

            private void FreeMemory(IState state)
            {
                state.Memory.Free(_address);
                state.Stack.SetVariable(_callerId, state.Space.CreateConstant(_address.Size, BigInteger.Zero));
            }

            private void AllocateMemory(IState state, Bits size)
            {
                state.Fork(_address, new Move(_callerId, _address, size), new Allocate(_callerId, size));
            }
        }

        private class Move : IStateAction
        {
            private readonly InstructionId _callerId;
            private readonly IExpression _address;
            private readonly Bits _size;


            public Move(InstructionId callerId, IExpression address, Bits size)
            {
                _callerId = callerId;
                _address = address;
                _size = size;
            }

            public Unit Run(IState state)
            {
                state.Stack.SetVariable(_callerId, state.Memory.Move(_address, _size));
                return new Unit();
            }
        }

        private class Allocate : IStateAction
        {
            private readonly InstructionId _callerId;
            private readonly Bits _size;


            public Allocate(InstructionId callerId, Bits size)
            {
                _callerId = callerId;
                _size = size;
            }

            public Unit Run(IState state)
            {
                state.Stack.SetVariable(_callerId, state.Memory.Allocate(_size));
                return new Unit();
            }
        }
    }
}
