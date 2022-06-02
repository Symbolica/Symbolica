using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed record Exists : Bool
{
    private readonly IValue[] _symbols;
    private readonly IValue _value;

    private Exists(IValue[] symbols, IValue value)
    {
        _symbols = symbols;
        _value = value;
    }

    public override ISet<IValue> Symbols => _symbols.SelectMany(s => s.Symbols).Union(_value.Symbols).ToHashSet();

    public override BoolExpr AsBool(ISolver solver)
    {
        using var value = _value.AsBool(solver);
        var boundConstants = _symbols.Select(s => s.AsBitVector(solver)).ToArray();
        try
        {
            return solver.Context.MkExists(boundConstants, value);
        }
        finally
        {
            foreach (var constant in boundConstants)
                constant.Dispose();
        }
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as Equal);
    }

    public override bool TryMerge(IValue value, out IValue? merged)
    {
        merged = null;
        return false;
    }

    public static IValue Create(IEnumerable<IValue> symbols, IValue value)
    {
        return new Exists(symbols.ToArray(), value);
    }

    public override (HashSet<(IValue, IValue)> subs, bool) IsEquivalentTo(IValue other)
    {
        return (new(), false);
    }

    public override IValue Substitute(IReadOnlyDictionary<IValue, IValue> subs)
    {
        return subs.TryGetValue(this, out var sub)
            ? sub
            : Create(_symbols.Select(s => s.Substitute(subs)), _value.Substitute(subs));
    }

    public override object ToJson()
    {
        return new
        {
            Type = GetType().Name,
            Size = (uint) Size,
            Symbols = _symbols.Select(s => s.ToJson()).ToArray(),
            Value = _value.ToJson()
        };
    }

    public override int GetEquivalencyHash()
    {
        var symbolsHash = new HashCode();
        foreach (var symbol in _symbols)
            symbolsHash.Add(symbol.GetEquivalencyHash());

        return HashCode.Combine(
            GetType().Name,
            symbolsHash.ToHashCode(),
            _value.GetEquivalencyHash());
    }

    public override IValue RenameSymbols(Func<string, string> renamer)
    {
        return Create(_symbols.Select(s => s.RenameSymbols(renamer)), _value.RenameSymbols(renamer));
    }
}
