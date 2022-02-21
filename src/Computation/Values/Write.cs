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
    private static readonly TextWriterTraceListener _tracer = new(File.CreateText("/Users/Choc/code/symbolica/symbolica/.traces/writes.txt"));

    private static void Trace(string message)
    {
        _tracer.WriteLine($"{DateTimeOffset.Now}, Thread {Environment.CurrentManagedThreadId}, {message}");
        _tracer.Flush();
    }

    private static T Time<T>(Func<T> f, string name)
    {
        Trace($"Started {name}");

        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var result = f();
        stopwatch.Stop();

        // if (stopwatch.Elapsed > TimeSpan.FromSeconds(5))
        //     Debugger.Break();

        Trace($"Completed {name} in {stopwatch.ElapsedMilliseconds}");
        _tracer.Flush();
        return result;
    }

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
            var rank = assertions.GetConstant(value).AsUnsigned();
            return new SymbolicOffset(value, rank, new(null, null));
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

        private bool BoundsCanOverlap(WriteData other)
        {
            return (other.Bounds.Lower ?? 0) < (Bounds.Lower ?? 0)
                ? other.BoundsCanOverlap(this)
                : Bounds.Upper is null || Bounds.Upper > (other.Bounds.Lower ?? 0);
        }

        public bool IsOverlappingAny(IEnumerable<WriteData> others, IAssertions assertions)
        {
            if (!others.Any(BoundsCanOverlap))
            {
                return false;
            }
            var isOverlapping = And.Create(Mask, others.Aggregate(ConstantUnsigned.Create(Mask.Size, BigInteger.Zero) as IValue, (acc, w) => Or.Create(acc, w.Mask)));
            return Time(() =>
            {
                using var proposition = assertions.GetProposition(isOverlapping);

                return proposition.CanBeTrue;
            }, nameof(IsOverlappingAny));
        }

        public bool IsAlignedWith(WriteData other, IAssertions assertions)
        {
            var isNotAligned = Xor.Create(Mask, other.Mask);
            return Time(() =>
            {
                using var proposition = assertions.GetProposition(isNotAligned);

                return !proposition.CanBeTrue;
            }, $"{nameof(IsAlignedWith)}-{Offset.GetType()}-{other.Offset.GetType()}");
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
                // Debug.Assert(IsDisjointFromThisLayer(assertions, lookup));
                return Read.Create(collectionFactory, assertions, _buffer, offset, size);
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
                return !lookup.IsOverlappingAny(new[] { lower, upper }.Where(x => x is not null).Cast<WriteData>(), assertions)
                    ? ReadLayerBelow()
                    : lower is not null && lower?.Offset is SymbolicOffset && lower.Value.IsAlignedWith(lookup, assertions)
                        ? lower.Value.Value
                        : upper is not null && upper?.Offset is SymbolicOffset && upper.Value.IsAlignedWith(lookup, assertions)
                            ? upper.Value.Value
                            : OverlappingRead();
            }
            else
            {
                WriteData candidate = _disjointWrites[result];
                return candidate.IsAlignedWith(lookup, assertions)
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
            IValue Overwrite(int index, WriteData oldData)
            {
                Trace("Overwrite");
                return new Write(_buffer, _disjointWrites.SetItem(index, writeData.RefineBounds(oldData.Bounds)));
            }

            IValue NewLayer()
            {
                Trace("New layer");
                return new Write(this, ImmutableList.Create(writeData));
            }

            var result = _disjointWrites.BinarySearch(writeData);
            if (result < 0)
            {
                ImmutableList<T> TryUpdate<T>(ImmutableList<T> list, int index, T? value)
                    where T : struct
                {
                    return index >= 0 && index < list.Count && value.HasValue
                        ? list.SetItem(index, value.Value)
                        : list;
                }

                ImmutableList<WriteData> RefineBounds(int index, WriteData? lower, WriteData? upper)
                {
                    return
                        TryUpdate(
                            TryUpdate(_disjointWrites, index - 1, lower?.RefineBounds(new(null, writeData.Bounds.Lower ?? writeData.Offset.Rank))),
                            index,
                            upper?.RefineBounds(new(writeData.Bounds.Upper ?? (writeData.Offset.Rank + (uint) writeData.Value.Size), null)));
                }

                IValue WriteConstant(IConstantValue buffer, IConstantValue offset, IConstantValue value, int index, WriteData? lower, WriteData? upper)
                {
                    Trace("Write constant");
                    return new Write(
                        ConstantWrite(collectionFactory, buffer, offset, value),
                        RefineBounds(index, lower, upper));
                }

                IValue WriteDisjoint(int index, WriteData? lower, WriteData? upper)
                {
                    Trace("Write disjoint");
                    // Debug.Assert(IsDisjointFromThisLayer(assertions, writeData));
                    var disjointWrites =
                        RefineBounds(index, lower, upper)
                        .Insert(
                            index,
                            writeData.RefineBounds(
                                new(
                                    lower?.Bounds.Upper ?? (lower?.Offset.Rank + (uint?) lower?.Value.Size),
                                    upper?.Bounds.Lower ?? upper?.Offset.Rank)));

                    return new Write(_buffer, ImmutableList.CreateRange(disjointWrites));
                }

                // if (writeData.Offset is SymbolicOffset)
                //     Debugger.Break();

                var index = ~result;
                WriteData? lower = index > 0 ? _disjointWrites[index - 1] : null;
                WriteData? upper = index < _disjointWrites.Count ? _disjointWrites[index] : null;
                return !writeData.IsOverlappingAny(new[] { lower, upper }.Where(x => x is not null).Cast<WriteData>(), assertions)
                    // TODO: In the non-overlapping case we should always recurse but need to refactor this
                    // so that it can backtrack if this write can't 'fit' in the layer below.
                    // Otherwise we could end up naively clobbering whole layers or introducing very thin intermediate layers
                    ? _buffer is IConstantValue b && writeData.Offset.Value is IConstantValue o && writeData.Value is IConstantValue v
                        ? WriteConstant(b, o, v, index, lower, upper)
                        : WriteDisjoint(index, lower, upper)
                    : lower is not null && lower.Value.IsAlignedWith(writeData, assertions)
                        ? Overwrite(index - 1, lower.Value)
                        : upper is not null && upper.Value.IsAlignedWith(writeData, assertions)
                            ? Overwrite(index, upper.Value)
                            : NewLayer();
            }
            else
            {
                WriteData candidate = _disjointWrites[result];
                return candidate.IsAlignedWith(writeData, assertions)
                    ? Overwrite(result, candidate)
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
