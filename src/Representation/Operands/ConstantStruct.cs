﻿using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Operands;

public sealed class ConstantStruct : IOperand
{
    private readonly IOperand[] _elements;
    private readonly IStructType _structType;

    public ConstantStruct(IStructType structType, IOperand[] elements)
    {
        _structType = structType;
        _elements = elements;
    }

    public IExpression Evaluate(IExpressionFactory exprFactory, IState state)
    {
        return _elements
            .Select((o, i) => (o, i))
            .Aggregate(_structType.CreateStruct(exprFactory, exprFactory.CreateGarbage), (s, e) =>
                s.Write(state.Space, e.i, e.o.Evaluate(exprFactory, state)))
            .Expression;
    }
}
