using System;
using System.Numerics;
using Symbolica.Collection;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal sealed class PersistentSpace : IPersistentSpace
{
    private readonly IPersistentStack<IExpression> _assertions;
    private readonly ICollectionFactory _collectionFactory;
    private readonly bool _useSymbolicGarbage;

    private PersistentSpace(Bits pointerSize, bool useSymbolicGarbage, ICollectionFactory collectionFactory,
        IPersistentStack<IExpression> assertions)
    {
        PointerSize = pointerSize;
        _useSymbolicGarbage = useSymbolicGarbage;
        _collectionFactory = collectionFactory;
        _assertions = assertions;
    }

    public Bits PointerSize { get; }

    public IPersistentSpace Assert(IExpression assertion)
    {
        return assertion is IConstantValue
            ? this
            : new PersistentSpace(PointerSize, _useSymbolicGarbage, _collectionFactory,
                _assertions.Push(assertion));
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
            ? Symbolica.Expression.Symbol.Create(size, Guid.NewGuid().ToString())
            : Symbolica.Expression.Values.Constants.ConstantUnsigned.CreateZero(size);
    }

    public IProposition CreateProposition(IExpression assertion)
    {
        return Proposition.Create(this, assertion);
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

    public BigInteger GetSingleValue(IExpression expression)
    {
        using var solver = CreateSolver();

        return solver.GetSingleValue(expression);
    }

    public static ISpace Create(Bits pointerSize, bool useSymbolicGarbage, ICollectionFactory collectionFactory)
    {
        return new PersistentSpace(pointerSize, useSymbolicGarbage, collectionFactory,
            collectionFactory.CreatePersistentStack<IExpression>());
    }
}
