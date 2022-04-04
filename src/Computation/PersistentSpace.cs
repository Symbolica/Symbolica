using System.Numerics;
using Symbolica.Collection;
using Symbolica.Expression;
using Symbolica.Expression.Values;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Computation;

internal sealed class PersistentSpace : IPersistentSpace
{
    private readonly IPersistentStack<IExpression<IType>> _assertions;
    private readonly ICollectionFactory _collectionFactory;
    private readonly bool _useSymbolicGarbage;

    private PersistentSpace(Bits pointerSize, bool useSymbolicGarbage, ICollectionFactory collectionFactory,
        IPersistentStack<IExpression<IType>> assertions)
    {
        PointerSize = pointerSize;
        _useSymbolicGarbage = useSymbolicGarbage;
        _collectionFactory = collectionFactory;
        _assertions = assertions;
    }

    public Bits PointerSize { get; }

    public IPersistentSpace Assert(IExpression<IType> assertion)
    {
        return assertion is IConstantValue<IType>
            ? this
            : new PersistentSpace(PointerSize, _useSymbolicGarbage, _collectionFactory,
                _assertions.Push(assertion));
    }

    public IExpression<IType> CreateGarbage(Bits size)
    {
        return _useSymbolicGarbage
            ? Symbol.Create(size)
            : ConstantUnsigned.CreateZero(size);
    }

    public IProposition CreateProposition(IExpression<IType> assertion)
    {
        return Proposition.Create(this, assertion);
    }

    public ISolver CreateSolver()
    {
        var solver = new LazySolver();
        solver.Assert(_assertions);

        return solver;
    }

    public Example GetExample()
    {
        using var solver = CreateSolver();

        return solver.GetExample();
    }

    public BigInteger GetExampleValue(IExpression<IType> expression)
    {
        using var solver = CreateSolver();

        return solver.GetExampleValue(expression);
    }

    public BigInteger GetSingleValue(IExpression<IType> expression)
    {
        using var solver = CreateSolver();

        return solver.GetSingleValue(expression);
    }

    public IExpression<IType> Read(IExpression<IType> buffer, Address offset, Bits size)
    {
        return Expression.Values.Read.Create(_collectionFactory, this, buffer, offset, size);
    }

    public IExpression<IType> Write(IExpression<IType> buffer, Address offset, IExpression<IType> value)
    {
        return Expression.Values.Write.Create(_collectionFactory, this, buffer, offset, value);
    }

    public bool TryGetSingleValue(IExpression<IType> expression, out BigInteger constant)
    {
        using var solver = CreateSolver();

        return solver.TryGetSingleValue(expression, out constant);
    }

    public static ISpace Create(Bits pointerSize, bool useSymbolicGarbage, ICollectionFactory collectionFactory)
    {
        return new PersistentSpace(pointerSize, useSymbolicGarbage, collectionFactory,
            collectionFactory.CreatePersistentStack<IExpression<IType>>());
    }
}
