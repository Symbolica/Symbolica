﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed record Equal : Bool
{
    private readonly IValue _left;
    private readonly IValue _right;

    private Equal(IValue left, IValue right)
    {
        _left = left;
        _right = right;
    }

    public override BoolExpr AsBool(ISolver solver)
    {
        return _left is Bool || _right is Bool
            ? Logical(solver)
            : Bitwise(solver);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as Equal);
    }

    public InRange AsRange(bool valueIsLeft)
    {
        return valueIsLeft
            ? new InRange(_left, new Range(_right, _right))
            : new InRange(_right, new Range(_left, _left));
    }

    public override bool TryMerge(IValue value, out IValue? merged)
    {
        merged = null;
        return value switch
        {
            LogicalNot not => not.TryMerge(this, out merged),
            Equal equal => TryMergeEqual(equal, out merged),
            InRange inRange =>
                inRange.TryMerge(new InRange(_left, new Range(_right, _right)), out merged)
                || inRange.TryMerge(new InRange(_right, new Range(_left, _left)), out merged),
            _ => false
        };
    }

    private bool TryMergeEqual(Equal equal, [MaybeNullWhen(false)] out IValue merged)
    {
        static IValue CreateRange(IValue value, IConstantValue c1, IConstantValue c2)
        {
            var (min, max) = (BigInteger) c1.AsUnsigned() < (BigInteger) c2.AsUnsigned()
                ? (c1, c2)
                : (c2, c1);
            return new InRange(value, new Range(min, max));
        }

        static bool CanCreateRange(IValue x, IValue y, IConstantValue c1, IConstantValue c2)
        {
            return x.Equals(y) && BigInteger.Abs((BigInteger) c1.AsUnsigned() - (BigInteger) c2.AsUnsigned()) == 1;
        }

        if (Equals(equal))
        {
            merged = this;
            return true;
        }

        var (result, value) = (_left, _right, equal._left, equal._right) switch
        {
            (IConstantValue c1, var x, IConstantValue c2, var y) when CanCreateRange(x, y, c1, c2) => (true, CreateRange(x, c1, c2)),
            (IConstantValue c1, var x, var y, IConstantValue c2) when CanCreateRange(x, y, c1, c2) => (true, CreateRange(x, c1, c2)),
            (var x, IConstantValue c1, IConstantValue c2, var y) when CanCreateRange(x, y, c1, c2) => (true, CreateRange(x, c1, c2)),
            (var x, IConstantValue c1, var y, IConstantValue c2) when CanCreateRange(x, y, c1, c2) => (true, CreateRange(x, c1, c2)),
            _ => (false, null)
        };

        merged = value;
        return result;
    }

    private BoolExpr Logical(ISolver solver)
    {
        using var left = _left.AsBool(solver);
        using var right = _right.AsBool(solver);
        return solver.Context.MkEq(left, right);
    }

    private BoolExpr Bitwise(ISolver solver)
    {
        using var left = _left.AsBitVector(solver);
        using var right = _right.AsBitVector(solver);
        return solver.Context.MkEq(left, right);
    }

    public static IValue Create(IValue left, IValue right)
    {
        return left is IConstantValue l && right is IConstantValue r
            ? l.AsUnsigned().Equal(r.AsUnsigned())
            : new Equal(left, right);
    }

    public override (HashSet<(IValue, IValue)> subs, bool) IsEquivalentTo(IValue other)
    {
        return other is Equal v
            ? _left.IsEquivalentTo(v._left)
                .And(_right.IsEquivalentTo(v._right))
            : (new(), false);
    }

    public override IValue Substitute(IReadOnlyDictionary<IValue, IValue> subs)
    {
        return subs.TryGetValue(this, out var sub)
            ? sub
            : Create(_left.Substitute(subs), _right.Substitute(subs));
    }

    public override object ToJson()
    {
        return new
        {
            Type = GetType().Name,
            Size = (uint) Size,
            Left = _left.ToJson(),
            Right = _right.ToJson()
        };
    }

    public override int GetEquivalencyHash()
    {
        return HashCode.Combine(GetType().Name, _left.GetEquivalencyHash(), _right.GetEquivalencyHash());
    }
}
