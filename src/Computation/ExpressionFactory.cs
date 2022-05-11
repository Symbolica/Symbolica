using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Symbolica.Collection;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation;

public sealed class ExpressionFactory : IExpressionFactory
{
    private readonly ICollectionFactory _collectionFactory;
    private readonly bool _useSymbolicGarbage;

    public ExpressionFactory(Bits pointerSize, bool useSymbolicGarbage, ICollectionFactory collectionFactory)
    {
        PointerSize = pointerSize;
        _useSymbolicGarbage = useSymbolicGarbage;
        _collectionFactory = collectionFactory;
    }

    public Bits PointerSize { get; }

    public IExpression CreateZero(Bits size)
    {
        return new Expression(_collectionFactory, ConstantUnsigned.CreateZero(size));
    }

    public IExpression CreateConstant(Bits size, BigInteger value)
    {
        return new Expression(_collectionFactory, ConstantUnsigned.Create(size, value));
    }

    public IExpression CreateConstantFloat(Bits size, string value)
    {
        return new Expression(_collectionFactory,
            (uint) size switch
            {
                32U => new ConstantSingle(float.Parse(value)),
                64U => new ConstantDouble(double.Parse(value)),
                _ => new NormalFloat(size, value)
            });
    }

    public IExpression CreateGarbage(Bits size)
    {
        return _useSymbolicGarbage
            ? CreateSymbolic(size, null)
            : CreateZero(size);
    }

    public IExpression CreateSymbolic(Bits size, string? name)
    {
        return CreateSymbolic(size, name, Enumerable.Empty<Func<IExpression, IExpression>>());
    }

    public IExpression CreateSymbolic(Bits size, string? name, IEnumerable<Func<IExpression, IExpression>> assertions)
    {
        return Expression.CreateSymbolic(_collectionFactory,
            size, name ?? Guid.NewGuid().ToString(), assertions);
    }
}
