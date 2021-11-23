using System;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.StateActions
{
    [Serializable]
    public class FreeMemory : IStateAction
    {
        private readonly IExpression _address;

        public FreeMemory(IExpression address)
        {
            _address = address;
        }

        public Unit Run(IState state)
        {
            state.Memory.Free(_address);
            return new Unit();
        }
    }
}
