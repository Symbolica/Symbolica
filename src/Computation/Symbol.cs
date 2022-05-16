using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal sealed record Symbol : BitVector
{
    private readonly IValue[] _assertions;
    private readonly string _name;

    private Symbol(Bits size, string name, IValue[] assertions)
        : base(size)
    {
        _name = name;
        _assertions = assertions;
    }

    public override BitVecExpr AsBitVector(ISolver solver)
    {
        solver.Assert(_name, _assertions);

        return solver.Context.MkBVConst(_name, (uint) Size);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as Symbol);
    }

    public bool Equals(Symbol? other)
    {
        return other is not null && _name.Equals(other._name) && _assertions.SequenceEqual(other._assertions);
    }

    public override int GetHashCode()
    {
        var assertionsHash = new HashCode();
        foreach (var assertion in _assertions)
            assertionsHash.Add(assertion);

        return HashCode.Combine(assertionsHash.ToHashCode(), _name);
    }

    public static IValue Create(Bits size, string name, IEnumerable<Func<IValue, IValue>> assertions)
    {
        var unconstrained = new Symbol(size, name, Array.Empty<IValue>());

        return new Symbol(size, name, assertions
            .Select(a => a(unconstrained))
            .ToArray());
    }

    public override (HashSet<(IValue, IValue)> subs, bool) IsEquivalentTo(IValue other)
    {
        return other is Symbol s
            ? (_name == s._name ? new() : new(new List<(IValue, IValue)> { (this, s) }), true)
            : (new(), false);
    }

    public override IValue Substitute(IReadOnlyDictionary<IValue, IValue> subs)
    {
        return subs.TryGetValue(this, out var sub)
            ? sub
            : this;
    }
}
