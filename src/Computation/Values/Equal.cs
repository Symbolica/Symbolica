using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Microsoft.Z3;

namespace Symbolica.Computation.Values;

internal sealed record Equal : Bool
{
    private Equal(IValue left, IValue right)
    {
        Left = left;
        Right = right;
    }

    internal IValue Left { get; }

    internal IValue Right { get; }

    public override BoolExpr AsBool(ISolver solver)
    {
        return Left is Bool || Right is Bool
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
            ? new InRange(Left, new Range(Right, Right))
            : new InRange(Right, new Range(Left, Left));
    }

    public override bool TryMerge(IValue value, [MaybeNullWhen(false)] out IValue merged)
    {
        merged = null;
        return value switch
        {
            Equal equal => TryMergeEqual(equal, out merged),
            InRange inRange =>
                inRange.TryMerge(new InRange(Left, new Range(Right, Right)), out merged)
                || inRange.TryMerge(new InRange(Right, new Range(Left, Left)), out merged),
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

        var (result, value) = (Left, Right, equal.Left, equal.Right) switch
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
        using var left = Left.AsBool(solver);
        using var right = Right.AsBool(solver);
        return solver.Context.MkEq(left, right);
    }

    private BoolExpr Bitwise(ISolver solver)
    {
        using var left = Left.AsBitVector(solver);
        using var right = Right.AsBitVector(solver);
        return solver.Context.MkEq(left, right);
    }

    public static IValue Create(IValue left, IValue right)
    {
        return left is IConstantValue l && right is IConstantValue r
            ? l.AsUnsigned().Equal(r.AsUnsigned())
            : new Equal(left, right);
    }
}
