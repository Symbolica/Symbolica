using System;
using System.Collections.Generic;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed record SignedToFloat : Float
{
    private readonly IValue _value;

    private SignedToFloat(Bits size, IValue value)
        : base(size)
    {
        _value = value;
    }

    public override ISet<IValue> Symbols => _value.Symbols;

    public override FPExpr AsFloat(ISolver solver)
    {
        using var rounding = solver.Context.MkFPRNE();
        using var value = _value.AsBitVector(solver);
        using var sort = Size.GetSort(solver);
        return solver.Context.MkFPToFP(rounding, value, sort, true);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as SignedToFloat);
    }

    public static IValue Create(Bits size, IValue value)
    {
        return value is IConstantValue v
            ? (uint) size switch
            {
                32U => v.AsSigned().ToSingle(),
                64U => v.AsSigned().ToDouble(),
                _ => new SignedToFloat(size, v)
            }
            : new SignedToFloat(size, value);
    }

    public override (HashSet<(IValue, IValue)> subs, bool) IsEquivalentTo(IValue other)
    {
        return other is SignedToFloat v
            ? _value.IsEquivalentTo(v._value)
                .And((new(), Size == v.Size))
            : (new(), false);
    }

    public override IValue Substitute(IReadOnlyDictionary<IValue, IValue> subs)
    {
        return subs.TryGetValue(this, out var sub)
            ? sub
            : Create(Size, _value.Substitute(subs));
    }

    public override object ToJson()
    {
        return new
        {
            Type = GetType().Name,
            Size = (uint) Size,
            Value = _value.ToJson()
        };
    }

    public override int GetEquivalencyHash()
    {
        return HashCode.Combine(
            GetType().Name,
            Size,
            _value.GetEquivalencyHash());
    }

    public override IValue RenameSymbols(Func<string, string> renamer)
    {
        return Create(Size, _value.RenameSymbols(renamer));
    }
}
