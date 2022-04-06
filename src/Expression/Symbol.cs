using System;
using System.Collections.Generic;
using System.Linq;

namespace Symbolica.Expression;

public sealed record Symbol : IBitVectorExpression
{
    private Symbol(Bits size, string name, IExpression<IType>[] assertions)
    {
        Type = new BitVector(size);
        Name = name;
        Assertions = assertions;
    }

    public IExpression<IType>[] Assertions { get; }

    public string Name { get; }

    public BitVector Type { get; }

    IInteger IExpression<IInteger>.Type => Type;

    public bool Equals(IExpression<IType>? other)
    {
        return Equals(other as Symbol);
    }

    public T Map<T>(IArityMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public T Map<T>(ITypeMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public T Map<T>(IIntegerMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public T Map<T>(IBitVectorMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public static Symbol Create(Bits size, string? name = null)
    {
        return Create(size, name, Enumerable.Empty<Func<IExpression<IType>, IExpression<IType>>>());
    }

    public static Symbol Create(
        Bits size,
        string? name,
        IEnumerable<Func<IExpression<IType>, IExpression<IType>>> assertions)
    {
        name ??= Guid.NewGuid().ToString();
        var unconstrained = new Symbol(size, name, Array.Empty<IExpression<IType>>());

        return new Symbol(size, name, assertions
            .Select(a => a(unconstrained))
            .ToArray());
    }
}
