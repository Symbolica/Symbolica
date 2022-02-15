using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Collection;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed class Write : BitVector
{
    private readonly record struct Bounds<T>(T Lower, T Upper);

    private interface IOffset : IComparable<IOffset>
    {
        Bounds<BigInteger?> Bounds { get; }
        BigInteger Rank { get; }
        IValue Value { get; }

        IOffset RefineBounds(Bounds<BigInteger?> bounds);

        int IComparable<IOffset>.CompareTo(IOffset? other) => Rank.CompareTo(other?.Rank);
    }

    private readonly record struct ConstantOffset(ConstantUnsigned Value) : IOffset
    {
        public Bounds<BigInteger?> Bounds => new(Value, Value);

        public BigInteger Rank => Value;

        IValue IOffset.Value => Value;

        public IOffset RefineBounds(Bounds<BigInteger?> bounds) => this;
    }

    private readonly record struct SymbolicOffset(IValue Value, BigInteger Rank, Bounds<BigInteger?> Bounds) : IOffset
    {
        public static SymbolicOffset Create(IValue value, IAssertions assertions)
        {
            var order = assertions.GetConstant(value).AsUnsigned();
            return new SymbolicOffset(value, order, new(null, null));
        }

        public IOffset RefineBounds(Bounds<BigInteger?> bounds)
        {
            static BigInteger? Binary(Func<BigInteger, BigInteger, BigInteger> f, BigInteger? a, BigInteger? b) => (a, b) switch
            {
                (null, null) => null,
                (BigInteger x, null) => x,
                (null, BigInteger y) => y,
                (BigInteger x, BigInteger y) => f(x, y)
            };

            static BigInteger? Max(BigInteger? a, BigInteger? b) => Binary(BigInteger.Max, a, b);

            static BigInteger? Min(BigInteger? a, BigInteger? b) => Binary(BigInteger.Min, a, b);

            return this with { Bounds = new(Max(bounds.Lower, Bounds.Lower), Min(bounds.Upper, Bounds.Upper)) };
        }
    }

    private readonly record struct WriteData(IOffset Offset, IValue Value, IValue Mask) : IComparable<WriteData>
    {
        public Bounds<BigInteger?> Bounds => new(Offset.Bounds.Lower, Offset.Bounds.Upper + (uint) Value.Size);

        public int CompareTo(WriteData other) => Offset.CompareTo(other.Offset);

        public IValue Flatten(IValue buffer)
        {
            var writeData = ShiftLeft.Create(ZeroExtend.Create(buffer.Size, Value), Offset.Value);

            return Or.Create(And.Create(buffer, Not.Create(Mask)), writeData);
        }

        public bool IsOverlappingWith(WriteData other, IAssertions assertions)
        {
            if (other.Bounds.Lower < Bounds.Lower)
            {
                return other.IsOverlappingWith(this, assertions);
            }
            if (Bounds.Upper <= other.Bounds.Lower)
            {
                return false;
            }
            var isOverlapping = And.Create(Mask, other.Mask);
            using var proposition = assertions.GetProposition(isOverlapping);

            return proposition.CanBeTrue;
        }

        public bool IsAlignedWith(WriteData other, IAssertions assertions)
        {
            var isNotAligned = Xor.Create(Mask, other.Mask);
            using var proposition = assertions.GetProposition(isNotAligned);

            return !proposition.CanBeTrue;
        }

        public WriteData RefineBounds(Bounds<BigInteger?> bounds) =>
            this with { Offset = Offset.RefineBounds(new(bounds.Lower, bounds.Upper - (uint) Value.Size)) };

        private static IValue CreateMask(IValue buffer, IValue offset, Bits size)
        {
            return ShiftLeft.Create(ConstantUnsigned.Create(size, BigInteger.Zero).Not().Extend(buffer.Size), offset);
        }

        public static WriteData Create(IValue buffer, IValue offset, IValue value, IAssertions assertions)
        {
            return new WriteData(
                offset is IConstantValue co
                    ? new ConstantOffset(co.AsUnsigned())
                    : SymbolicOffset.Create(offset, assertions),
                value,
                CreateMask(buffer, offset, value.Size));
        }
    }

    private readonly IValue _writeBuffer;

    private readonly ImmutableList<WriteData> _disjointWrites;

    private Write(IValue writeBuffer, ImmutableList<WriteData> disjointWrites)
            : base(writeBuffer.Size)
    {
        _writeBuffer = writeBuffer;
        _disjointWrites = disjointWrites;
    }

    // TODO: Add all the write data values in here
    public override IEnumerable<IValue> Children => new[] { _writeBuffer };

    public override string? PrintedValue => null;

    public override BitVecExpr AsBitVector(IContext context)
    {
        return Flatten().AsBitVector(context);
    }

    public IValue LayerRead(ICollectionFactory collectionFactory, IAssertions assertions,
        IValue offset, Bits size)
    {
        var lookup = WriteData.Create(_writeBuffer, offset, ConstantUnsigned.Create(size, BigInteger.Zero), assertions);
        var result = _disjointWrites.BinarySearch(lookup);
        if (result < 0)
        {
            // If not found then we have to consider the ones either side.
            // If overlaps with neither then read from the layer below.
            // If it overlaps with one of them then check for alignment and do an exact read.
            // Otherwise flatten and read (be careful when trying to smart about which Writes to include as in general we'd need to perform a search to determine which writes are potentially in range of this one if it's not disjoint).
            var index = ~result;
            WriteData? lower = index > 0 ? _disjointWrites[index - 1] : null;
            WriteData? upper = index < _disjointWrites.Count ? _disjointWrites[index] : null;
            return lower is not null && lower.Value.IsOverlappingWith(lookup, assertions)
                ? lower.Value.IsAlignedWith(lookup, assertions)
                    ? lower.Value.Value
                    : Read.Create(collectionFactory, assertions, Flatten(), offset, size)
                : upper is not null && upper.Value.IsOverlappingWith(lookup, assertions)
                    ? upper.Value.IsAlignedWith(lookup, assertions)
                        ? upper.Value.Value
                        : Read.Create(collectionFactory, assertions, Flatten(), offset, size)
                    : Read.Create(collectionFactory, assertions, _writeBuffer, offset, size);
        }
        else
        {
            // If found a value then we know it's overlapping and we need to check for alignment
            WriteData candidate = _disjointWrites[result];
            return candidate.IsAlignedWith(lookup, assertions)
                ? candidate.Value
                : Read.Create(collectionFactory, assertions, Flatten(), offset, size);
        }
    }

    private IValue LayerWrite(IAssertions assertions, WriteData writeData)
    {
        var result = _disjointWrites.BinarySearch(writeData);
        if (result < 0)
        {
            // If not found then we have to consider the ones either side.
            // If overlaps with neither then slot in this new disjoint write.
            // If it overlaps with one of them then check for alignment again.
            // Otherwise write it as a whole new Write layer.
            var index = ~result;
            WriteData? lower = index > 0 ? _disjointWrites[index - 1] : null;
            WriteData? upper = index < _disjointWrites.Count ? _disjointWrites[index] : null;
            return lower is not null && lower.Value.IsOverlappingWith(writeData, assertions)
                ? lower.Value.IsAlignedWith(writeData, assertions)
                    ? new Write(_writeBuffer, _disjointWrites.Insert(index - 1, writeData))
                    : new Write(this, ImmutableList.Create(writeData))
                : upper is not null && upper.Value.IsOverlappingWith(writeData, assertions)
                    ? upper.Value.IsAlignedWith(writeData, assertions)
                        ? new Write(_writeBuffer, _disjointWrites.Insert(index, writeData))
                        : new Write(this, ImmutableList.Create(writeData))
                    : new Write(
                        _writeBuffer,
                        ImmutableList.CreateRange(
                            _disjointWrites
                                .Take(index - 1)
                                .Concat(
                                    new[]
                                    {
                                        lower?.RefineBounds(new(null, writeData.Bounds.Lower)),
                                        writeData.RefineBounds(new(lower?.Bounds.Upper, upper?.Bounds.Lower)),
                                        upper?.RefineBounds(new(writeData.Bounds.Upper, null))
                                    }.Where(x => x is not null).Cast<WriteData>())
                                .Concat(_disjointWrites.Skip(index + 1))));
        }
        else
        {
            // If found a value then we know it's overlapping and we need to check for alignment
            WriteData candidate = _disjointWrites[result];
            return candidate.IsAlignedWith(writeData, assertions)
                ? new Write(_writeBuffer, _disjointWrites.Insert(result, writeData))
                : new Write(this, ImmutableList.Create(writeData));
        }
    }

    private IValue Flatten()
    {
        return _disjointWrites.Aggregate(_writeBuffer, (b, write) => write.Flatten(b));
    }

    public static IValue Create(ICollectionFactory collectionFactory, IAssertions assertions,
        IValue buffer, IValue offset, IValue value)
    {
        return buffer is IConstantValue b && offset is IConstantValue o && value is IConstantValue v
            ? b.AsBitVector(collectionFactory).Write(o.AsUnsigned(), v.AsBitVector(collectionFactory))
            : buffer is Write w
                ? w.LayerWrite(assertions, WriteData.Create(buffer, offset, value, assertions))
                : new Write(buffer, ImmutableList.Create(WriteData.Create(buffer, offset, value, assertions)));
    }
}
