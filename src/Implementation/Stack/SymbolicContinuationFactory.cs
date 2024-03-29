﻿using Symbolica.Expression;

namespace Symbolica.Implementation.Stack;

internal sealed class SymbolicContinuationFactory : IPersistentContinuationFactory
{
    public (IExpression, IPersistentContinuationFactory) Create(ISpace space, Bits size)
    {
        var continuation = space.CreateSymbolic(size, null);

        return (continuation, this);
    }
}
