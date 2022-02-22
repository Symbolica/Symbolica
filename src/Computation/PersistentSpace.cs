using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Symbolica.Collection;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal sealed class PersistentSpace<TContext> : IPersistentSpace
    where TContext : IContext, new()
{
    private readonly IPersistentStack<IValue> _assertions;
    private readonly ICollectionFactory _collectionFactory;
    private readonly bool _useSymbolicGarbage;

    private PersistentSpace(Bits pointerSize, bool useSymbolicGarbage, ICollectionFactory collectionFactory,
        IPersistentStack<IValue> assertions)
    {
        PointerSize = pointerSize;
        _useSymbolicGarbage = useSymbolicGarbage;
        _collectionFactory = collectionFactory;
        _assertions = assertions;
    }

    public Bits PointerSize { get; }

    public IConstantValue GetConstant(IValue value)
    {
        using var constraints = GetConstraints();

        return ConstantUnsigned.Create(value.Size, constraints.Evaluate(value));
    }

    public IProposition GetProposition(IValue value)
    {
        return value is IConstantValue v
            ? ConstantProposition.Create(this, v)
            : SymbolicProposition.Create(this, value);
    }

    public IPersistentSpace Assert(IValue assertion)
    {
        return new PersistentSpace<TContext>(PointerSize, _useSymbolicGarbage, _collectionFactory,
            _assertions.Push(assertion));
    }

    public IConstraints GetConstraints()
    {
        return Constraints.Create<TContext>(_assertions);
    }

    public IExample GetExample()
    {
        return Example.Create(this);
    }

    public IExpression CreateConstant(Bits size, BigInteger value)
    {
        return new Expression<TContext>(_collectionFactory,
            ConstantUnsigned.Create(size, value));
    }

    public IExpression CreateConstantFloat(Bits size, string value)
    {
        return new Expression<TContext>(_collectionFactory,
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
        return Expression<TContext>.CreateSymbolic(_collectionFactory,
            size, name ?? Guid.NewGuid().ToString(), assertions);
    }

    public static ISpace Create(Bits pointerSize, bool useSymbolicGarbage, ICollectionFactory collectionFactory)
    {
        return new PersistentSpace<TContext>(pointerSize, useSymbolicGarbage, collectionFactory,
            collectionFactory.CreatePersistentStack<IValue>());
    }
}
