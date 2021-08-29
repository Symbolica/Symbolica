using System;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation
{
    [Serializable]
    public class UndefinedShiftException : StateException
    {
        public UndefinedShiftException(ISpace space)
            : base(space)
        {
        }
    }
}
