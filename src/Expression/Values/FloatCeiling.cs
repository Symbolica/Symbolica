﻿using System;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Expression.Values;

public sealed record FloatCeiling : IUnaryExpr, IFloat
{
    private FloatCeiling(IExpression value)
    {
        Value = value;
    }

    public IExpression Value { get; }

    public bool Equals(IExpression? other)
    {
        return Equals(other as FloatCeiling);
    }

    public T Map<T>(IExprMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public T Map<T>(IUnaryExprMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public static IExpression Create(IExpression value)
    {
        return IFloat.Unary(value,
            v => new ConstantSingle(MathF.Ceiling(v)),
            v => new ConstantDouble(Math.Ceiling(v)),
            v => new FloatCeiling(v));
    }
}
