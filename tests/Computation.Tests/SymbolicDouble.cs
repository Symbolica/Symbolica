using System;
using System.Collections.Generic;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal sealed record SymbolicDouble : Float
{
    private readonly double _value;

    public SymbolicDouble(double value)
        : base((Bits) 64U)
    {
        _value = value;
    }

    public override ISet<IValue> Symbols => throw new NotImplementedException();

    public override FPExpr AsFloat(ISolver solver)
    {
        using var sort = Size.GetSort(solver);
        return solver.Context.MkFP(_value, sort);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as SymbolicDouble);
    }

    public override int GetEquivalencyHash()
    {
        throw new NotImplementedException();
    }

    public override (HashSet<(IValue, IValue)> subs, bool) IsEquivalentTo(IValue other)
    {
        throw new NotImplementedException();
    }

    public override IValue RenameSymbols(Func<string, string> renamer)
    {
        throw new NotImplementedException();
    }

    public override IValue Substitute(IReadOnlyDictionary<IValue, IValue> subs)
    {
        throw new NotImplementedException();
    }

    public override object ToJson()
    {
        throw new NotImplementedException();
    }
}
