using System;
using Symbolica.Expression;

namespace Symbolica.Implementation.Stack
{
    [Serializable]
    internal sealed class SymbolicContinuationFactory : IPersistentContinuationFactory
    {
        public (IExpression, IPersistentContinuationFactory) Create(ISpace space, Bits size)
        {
            return (space.CreateSymbolic(size, null, null), this);
        }
    }
}
