﻿using System;
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
        IPersistentStack<IValue> assertions)
    {
        PointerSize = pointerSize;
        _useSymbolicGarbage = useSymbolicGarbage;
        _collectionFactory = collectionFactory;
        _assertions = assertions;
    }

    public Bits PointerSize { get; }

    public IPersistentSpace Assert(IValue assertion)
    {
        return assertion is IConstantValue
            ? this
            : new PersistentSpace(PointerSize, _useSymbolicGarbage, _collectionFactory,
                _assertions.Push(assertion));
    }

    public ISolver CreateSolver()
    {
        var solver = new LazySolver();
        solver.Assert(_assertions);

        return solver;
    }

    public IExample GetExample()
    {
        using var solver = CreateSolver();

        return solver.GetExample();
    }

    public IExpression CreateZero(Bits size)
    {
        return new Expression(_collectionFactory, ConstantUnsigned.CreateZero(size));
    }

    public IExpression CreateConstant(Bits size, BigInteger value)
    {
        return new Expression(_collectionFactory,
            ConstantUnsigned.Create(size, value));
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

    public static ISpace Create(Bits pointerSize, bool useSymbolicGarbage, ICollectionFactory collectionFactory)
    {
        return new PersistentSpace(pointerSize, useSymbolicGarbage, collectionFactory,
            collectionFactory.CreatePersistentStack<IValue>());
    }
}
