using System;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.StateActions
{
    [Serializable]
    public class MoveMemory : IFunc<IState, IExpression>
    {
        private readonly IExpression _address;
        private readonly Bits _size;


        public MoveMemory(IExpression address, Bits size)
        {
            _address = address;
            _size = size;
        }

        public IExpression Run(IState state)
        {
            return state.Memory.Move(_address, _size);
        }
    }
}
