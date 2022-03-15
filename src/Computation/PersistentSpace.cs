using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Symbolica.Collection;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal sealed class PersistentSpace : IPersistentSpace
{
    private readonly IPersistentStack<IValue> _assertions;
    private readonly ICollectionFactory _collectionFactory;
    private readonly bool _useSymbolicGarbage;

    private PersistentSpace(Bits pointerSize, bool useSymbolicGarbage, ICollectionFactory collectionFactory,
        IConstraints constraints, IPersistentStack<IValue> assertions)
    {
        PointerSize = pointerSize;
        _useSymbolicGarbage = useSymbolicGarbage;
        _collectionFactory = collectionFactory;
        Constraints = constraints;
        _assertions = assertions;
    }

    public Bits PointerSize { get; }
    public IConstraints Constraints { get; }

    public void Dispose()
    {
        Constraints.Dispose();
    }

    public IPersistentSpace Assert(IValue assertion)
    {
        var assertions = _assertions.Push(assertion);

        var constraints = Computation.Constraints.Create();
        constraints.Assert(assertions);

        return new PersistentSpace(PointerSize, _useSymbolicGarbage, _collectionFactory,
            constraints, assertions);
    }

    public IExample GetExample()
    {
        return Example.Create(this);
    }

    public IExpression CreateConstant(Bits size, BigInteger value)
    {
        return new Expression(_collectionFactory,
            ConstantUnsigned.Create(size, value));
    }

    public IExpression CreateConstantFloat(Bits size, string value)
    {
        return new Expression(_collectionFactory,
            size.ParseFloat(value));
    }

    public IExpression CreateGarbage(Bits size)
    {
        return _useSymbolicGarbage
            ? CreateSymbolic(size, null)
            : CreateConstant(size, BigInteger.Zero);
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

    public static ISpace Create(Bits pointerSize, bool useSymbolicGarbage, ICollectionFactory collectionFactory)
    {
        return new PersistentSpace(pointerSize, useSymbolicGarbage, collectionFactory,
            Computation.Constraints.Create(), collectionFactory.CreatePersistentStack<IValue>());
    }
}
