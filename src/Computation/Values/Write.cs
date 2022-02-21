using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Collection;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed class Write : BitVector
{
    private static readonly bool _traceOn = false;
    private static readonly TextWriterTraceListener _tracer = new(File.CreateText("/Users/Choc/code/symbolica/symbolica/.traces/writes.txt"));

    private static void Trace(string message)
    {
        if (_traceOn)
        {
            _tracer.WriteLine($"{DateTimeOffset.Now}, Thread {Environment.CurrentManagedThreadId}, {message}");
            _tracer.Flush();
        }
    }

    private static T Time<T>(Func<T> f, string name)
    {
        Trace($"Started {name}");

        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var result = f();
        stopwatch.Stop();

        // if (stopwatch.Elapsed > TimeSpan.FromSeconds(1))
        //     Debugger.Break();

        Trace($"Completed {name} in {stopwatch.ElapsedMilliseconds}");
        _tracer.Flush();
        return result;
    }

    private readonly record struct Bounds<T>(T Lower, T Upper);

    private interface IOffset : IComparable<IOffset>
    {
        Bounds<BigInteger> Bounds { get; }
        BigInteger Rank { get; }
        IValue Value { get; }

        int IComparable<IOffset>.CompareTo(IOffset? other) => Rank.CompareTo(other?.Rank);
    }

    private readonly record struct ConstantOffset(ConstantUnsigned Value) : IOffset
    {
        public Bounds<BigInteger> Bounds => new(Value, Value);

        public BigInteger Rank => Value;

        IValue IOffset.Value => Value;
    }

    private readonly record struct SymbolicOffset(IValue Value, BigInteger Rank, Bounds<BigInteger> Bounds) : IOffset
    {
        public static SymbolicOffset Create(IValue value, IAssertions assertions, BigInteger bufferSize)
        {
            BigInteger FindLowerBound(Bounds<BigInteger> bounds)
            {
                return Time(() =>
                {
                    if (bounds.Lower == bounds.Upper)
                    {
                        return bounds.Lower;
                    }

                    var mid = bounds.Lower + ((bounds.Upper - bounds.Lower) / 2);

                    var isLessOrEqualMidpoint = UnsignedLessOrEqual.Create(value, ConstantUnsigned.Create(value.Size, mid));
                    using var proposition = assertions.GetProposition(isLessOrEqualMidpoint);
                    return proposition.CanBeTrue
                        ? FindLowerBound(new(bounds.Lower, mid))
                        : FindLowerBound(new(mid + 1, bounds.Upper));
                }, $"{nameof(FindLowerBound)} ({bounds.Lower}-{bounds.Upper})");
            }

            BigInteger FindUpperBound(Bounds<BigInteger> bounds)
            {
                return Time(() =>
                {
                    if (bounds.Lower == bounds.Upper)
                    {
                        return bounds.Lower;
                    }

                    var mid = bounds.Lower + ((bounds.Upper - bounds.Lower) / 2);

                    var isGreaterMidpoint = UnsignedGreater.Create(value, ConstantUnsigned.Create(value.Size, mid));
                    using var proposition = assertions.GetProposition(isGreaterMidpoint);
                    return proposition.CanBeTrue
                        ? FindUpperBound(new(mid + 1, bounds.Upper))
                        : FindUpperBound(new(bounds.Lower, mid));
                }, $"{nameof(FindUpperBound)} ({bounds.Lower}-{bounds.Upper})");
            }

            var rank = assertions.GetConstant(value).AsUnsigned();
            var lowerBound = FindLowerBound(new(0, bufferSize));
            return new SymbolicOffset(value, rank, new(lowerBound, FindUpperBound(new(lowerBound, bufferSize))));
        }
    }

    private readonly record struct WriteData(IOffset Offset, IValue Value, IValue Mask) : IComparable<WriteData>
    {
        public Bounds<BigInteger> Bounds => new(Offset.Bounds.Lower, Offset.Bounds.Upper + (uint) Value.Size);

        public int CompareTo(WriteData other) => Offset.CompareTo(other.Offset);

        public IValue Flatten(IValue buffer)
        {
            var writeData = ShiftLeft.Create(ZeroExtend.Create(buffer.Size, Value), Offset.Value);

            return Or.Create(And.Create(buffer, Not.Create(Mask)), writeData);
        }

        private bool BoundsCanOverlap(WriteData other)
        {
            return other.Bounds.Lower < Bounds.Lower
                ? other.BoundsCanOverlap(this)
                : Bounds.Upper > other.Bounds.Lower;
        }

        public bool IsOverlappingAny(WriteData? lower, WriteData? upper)
        {
            return new[] { lower, upper }.Where(x => x is not null).Cast<WriteData>().Any(BoundsCanOverlap);
        }

        public bool IsAlignedWith(WriteData other)
        {
            return Bounds == other.Bounds;
        }

        private static IValue CreateMask(IValue buffer, IValue offset, Bits size)
        {
            return ShiftLeft.Create(ConstantUnsigned.Create(size, BigInteger.Zero).Not().Extend(buffer.Size), offset);
        }

        public static WriteData Create(IValue buffer, IValue offset, IValue value, IAssertions assertions)
        {
            return new WriteData(
                offset is IConstantValue co
                    ? new ConstantOffset(co.AsUnsigned())
                    : SymbolicOffset.Create(offset, assertions, (uint) buffer.Size),
                value,
                CreateMask(buffer, offset, value.Size));
        }
    }

    private readonly IValue _buffer;

    private readonly ImmutableList<WriteData> _disjointWrites;

    private Write(IValue writeBuffer, ImmutableList<WriteData> disjointWrites)
            : base(writeBuffer.Size)
    {
        _buffer = writeBuffer;
        _disjointWrites = disjointWrites;
    }

    // TODO: Add all the write data values in here
    public override IEnumerable<IValue> Children => new[] { _buffer };

    public override string? PrintedValue => null;

    public override BitVecExpr AsBitVector(IContext context)
    {
        Trace("As bit vector");
        return Flatten().AsBitVector(context);
    }

    private IValue CumulativeMask() =>
        _disjointWrites.Aggregate(
            ConstantUnsigned.Create(_buffer.Size, BigInteger.Zero) as IValue,
            (acc, write) => Or.Create(acc, write.Mask));

    private bool IsDisjointFromThisLayer(IAssertions assertions, WriteData writeData)
    {
        return !assertions.GetProposition(And.Create(writeData.Mask, CumulativeMask())).CanBeTrue;
    }

    public IValue LayerRead(ICollectionFactory collectionFactory, IAssertions assertions,
        IValue offset, Bits size)
    {
        return Time(() =>
        {
            var lookup = WriteData.Create(_buffer, offset, ConstantUnsigned.Create(size, BigInteger.Zero), assertions);

            IValue ReadLayerBelow()
            {
                Trace("Read layer below");
                var boundsOffset = ConstantUnsigned.Create(offset.Size, lookup.Bounds.Lower);
                var boundedSize = (Bits) (uint) (lookup.Bounds.Upper - lookup.Bounds.Lower);

                var alignedBuffer = Read.Create(collectionFactory, assertions, _buffer, boundsOffset, boundedSize);
                // Debug.Assert(IsDisjointFromThisLayer(assertions, lookup));
                return Read.Create(
                    collectionFactory,
                    assertions,
                    alignedBuffer,
                    Truncate.Create(boundedSize, Subtract.Create(offset, boundsOffset)),
                    size);
            }

            IValue OverlappingRead()
            {
                Trace("Overlapping read");
                return Read.Create(collectionFactory, assertions, Flatten(), offset, size);
            }

            var result = _disjointWrites.BinarySearch(lookup);
            if (result < 0)
            {
                var index = ~result;
                WriteData? lower = index > 0 ? _disjointWrites[index - 1] : null;
                WriteData? upper = index < _disjointWrites.Count ? _disjointWrites[index] : null;
                return !lookup.IsOverlappingAny(lower, upper)
                    ? ReadLayerBelow()
                    : lower is not null && lower.Value.IsAlignedWith(lookup)
                        ? lower.Value.Value
                        : upper is not null && upper.Value.IsAlignedWith(lookup)
                            ? upper.Value.Value
                            : OverlappingRead();
            }
            else
            {
                WriteData candidate = _disjointWrites[result];
                return candidate.IsAlignedWith(lookup)
                    ? candidate.Value
                    : OverlappingRead();
            }
        },
        "Layer read");
    }

    private IValue LayerWrite(ICollectionFactory collectionFactory, IAssertions assertions, WriteData writeData)
    {
        return Time(() =>
        {
            IValue Overwrite(int index)
            {
                Trace("Overwrite");
                return new Write(_buffer, _disjointWrites.SetItem(index, writeData));
            }

            IValue NewLayer()
            {
                Trace("New layer");
                return new Write(this, ImmutableList.Create(writeData));
            }

            var result = _disjointWrites.BinarySearch(writeData);
            if (result < 0)
            {
                IValue WriteConstant(IConstantValue buffer, IConstantValue offset, IConstantValue value, int index, WriteData? lower, WriteData? upper)
                {
                    Trace("Write constant");
                    return new Write(ConstantWrite(collectionFactory, buffer, offset, value), _disjointWrites);
                }

                IValue WriteDisjoint(int index, WriteData? lower, WriteData? upper)
                {
                    Trace("Write disjoint");
                    // Debug.Assert(IsDisjointFromThisLayer(assertions, writeData));
                    var disjointWrites = _disjointWrites.Insert(index, writeData);
                    return new Write(_buffer, ImmutableList.CreateRange(disjointWrites));
                }

                // if (writeData.Offset is SymbolicOffset)
                //     Debugger.Break();

                var index = ~result;
                WriteData? lower = index > 0 ? _disjointWrites[index - 1] : null;
                WriteData? upper = index < _disjointWrites.Count ? _disjointWrites[index] : null;
                return !writeData.IsOverlappingAny(lower, upper)
                    // TODO: In the non-overlapping case we should always recurse but need to refactor this
                    // so that it can backtrack if this write can't 'fit' in the layer below.
                    // Otherwise we could end up naively clobbering whole layers or introducing very thin intermediate layers
                    ? _buffer is IConstantValue b && writeData.Offset.Value is IConstantValue o && writeData.Value is IConstantValue v
                        ? WriteConstant(b, o, v, index, lower, upper)
                        : WriteDisjoint(index, lower, upper)
                    : lower is not null && lower.Value.IsAlignedWith(writeData)
                        ? Overwrite(index - 1)
                        : upper is not null && upper.Value.IsAlignedWith(writeData)
                            ? Overwrite(index)
                            : NewLayer();
            }
            else
            {
                WriteData candidate = _disjointWrites[result];
                return candidate.IsAlignedWith(writeData)
                    ? Overwrite(result)
                    : NewLayer();
            }
        }, "Layer write");
    }

    private IValue Flatten()
    {
        return _disjointWrites.Aggregate(_buffer, (b, write) => write.Flatten(b));
    }

    private static IValue ConstantWrite(ICollectionFactory collectionFactory,
        IConstantValue buffer, IConstantValue offset, IConstantValue value)
    {
        return buffer.AsBitVector(collectionFactory).Write(offset.AsUnsigned(), value.AsBitVector(collectionFactory));
    }

    public static IValue Create(ICollectionFactory collectionFactory, IAssertions assertions,
        IValue buffer, IValue offset, IValue value)
    {
        return buffer is IConstantValue b && offset is IConstantValue o && value is IConstantValue v
            ? ConstantWrite(collectionFactory, b, o, v)
            : buffer is Write w
                ? w.LayerWrite(collectionFactory, assertions, WriteData.Create(buffer, offset, value, assertions))
                : new Write(buffer, ImmutableList.Create(WriteData.Create(buffer, offset, value, assertions)));
    }
}
