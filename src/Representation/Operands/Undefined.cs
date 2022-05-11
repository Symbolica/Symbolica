﻿using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Operands;

public sealed class Undefined : IOperand
{
    private readonly Bits _size;

    public Undefined(Bits size)
    {
        _size = size;
    }

    public IExpression Evaluate(IExpressionFactory exprFactory, IState state)
    {
        return exprFactory.CreateGarbage(_size);
    }
}
