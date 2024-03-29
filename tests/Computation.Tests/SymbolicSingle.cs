﻿using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal sealed record SymbolicSingle : Float
{
    private readonly float _value;

    public SymbolicSingle(float value)
        : base((Bits) 32U)
    {
        _value = value;
    }

    public override FPExpr AsFloat(ISolver solver)
    {
        using var sort = Size.GetSort(solver);
        return solver.Context.MkFP(_value, sort);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as SymbolicSingle);
    }
}
