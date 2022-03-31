using System;
using System.Collections.Generic;
using System.Linq;

namespace Symbolica.Expression;

public sealed record Symbol : IBitVector, IExpression
{
    private Symbol(Bits size, string name, IExpression[] assertions)
    {
        Size = size;
        Name = name;
        Assertions = assertions;
    }

    public IExpression[] Assertions { get; }

    public string Name { get; }

    public Bits Size { get; }

    public bool Equals(IExpression? other)
    {
        return Equals(other as Symbol);
    }

    public T Map<T>(IExprMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public static IExpression Create(Bits size, string name)
    {
        return Create(size, name, Enumerable.Empty<Func<IExpression, IExpression>>());
    }

    public static IExpression Create(Bits size, string name, IEnumerable<Func<IExpression, IExpression>> assertions)
    {
        var unconstrained = new Symbol(size, name, Array.Empty<IExpression>());

        return new Symbol(size, name, assertions
            .Select(a => a(unconstrained))
            .ToArray());
    }
}
