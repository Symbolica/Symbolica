using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Collection;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed record Write : BitVector
{
    private readonly IValue _writeBuffer;
    private readonly IValue _writeMask;
    private readonly IValue _writeOffset;
    private readonly IValue _writeValue;

    private Write(IValue writeBuffer, IValue writeOffset, IValue writeValue)
        : base(writeBuffer.Size)
    {
        _writeBuffer = writeBuffer;
        _writeOffset = writeOffset;
        _writeValue = writeValue;
        _writeMask = Mask(writeBuffer, writeOffset, writeValue.Size);
    }

    public override ISet<IValue> Symbols => _writeBuffer.Symbols.Union(_writeOffset.Symbols).Union(_writeValue.Symbols).ToHashSet();

    public override BitVecExpr AsBitVector(ISolver solver)
    {
        return Flatten().AsBitVector(solver);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as Write);
    }

    public IValue LayerRead(ICollectionFactory collectionFactory, IPersistentSpace space, IValue offset, Bits size)
    {
        var mask = Mask(this, offset, size);

        return IsNotOverlapping(space, mask)
            ? Read.Create(collectionFactory, space, _writeBuffer, offset, size)
            : IsAligned(space, mask)
                ? _writeValue
                : Read.Create(collectionFactory, space, Flatten(), offset, size);
    }

    private IValue LayerWrite(ICollectionFactory collectionFactory, IPersistentSpace space, IValue offset, IValue value)
    {
        var mask = Mask(this, offset, value.Size);

        return IsNotOverlapping(space, mask)
            ? new Write(Create(collectionFactory, space, _writeBuffer, offset, value), _writeOffset, _writeValue)
            : IsAligned(space, mask)
                ? new Write(_writeBuffer, offset, value)
                : new Write(this, offset, value);
    }

    private bool IsNotOverlapping(IPersistentSpace space, IValue mask)
    {
        var isOverlapping = And.Create(mask, _writeMask);
        using var solver = space.CreateSolver();

        return !solver.IsSatisfiable(isOverlapping);
    }

    private bool IsAligned(IPersistentSpace space, IValue mask)
    {
        var isNotAligned = Xor.Create(mask, _writeMask);
        using var solver = space.CreateSolver();

        return !solver.IsSatisfiable(isNotAligned);
    }

    private IValue Flatten()
    {
        var writeData = ShiftLeft.Create(ZeroExtend.Create(Size, _writeValue), _writeOffset);

        return Or.Create(And.Create(_writeBuffer, Not.Create(_writeMask)), writeData);
    }

    private static IValue Mask(IValue buffer, IValue offset, Bits size)
    {
        return ShiftLeft.Create(ConstantUnsigned.Create(size, BigInteger.Zero).Not().Extend(buffer.Size), offset);
    }

    public static IValue Create(ICollectionFactory collectionFactory, IPersistentSpace space,
        IValue buffer, IValue offset, IValue value)
    {
        return buffer is IConstantValue b && offset is IConstantValue o && value is IConstantValue v
            ? b.AsBitVector(collectionFactory).Write(o.AsUnsigned(), v.AsBitVector(collectionFactory))
            : buffer is Write w
                ? w.LayerWrite(collectionFactory, space, offset, value)
                : new Write(buffer, offset, value);
    }

    public override (HashSet<(IValue, IValue)> subs, bool) IsEquivalentTo(IValue other)
    {
        return other is Write v
            ? _writeBuffer.IsEquivalentTo(v._writeBuffer)
                .And(_writeMask.IsEquivalentTo(v._writeMask))
                .And(_writeOffset.IsEquivalentTo(v._writeOffset))
            : (new(), false);
    }

    public override IValue Substitute(IReadOnlyDictionary<IValue, IValue> subs)
    {
        return subs.TryGetValue(this, out var sub)
            ? sub
            : new Write(
                _writeBuffer.Substitute(subs),
                _writeOffset.Substitute(subs),
                _writeValue.Substitute(subs));
    }

    public override object ToJson()
    {
        return new
        {
            Type = GetType().Name,
            Size = (uint) Size,
            Buffer = _writeBuffer.ToJson(),
            Offset = _writeOffset.ToJson(),
            Value = _writeValue.ToJson()
        };
    }

    public override int GetEquivalencyHash(bool includeSubs)
    {
        return HashCode.Combine(
            GetType().Name,
            _writeBuffer.GetEquivalencyHash(includeSubs),
            _writeOffset.GetEquivalencyHash(includeSubs),
            _writeValue.GetEquivalencyHash(includeSubs));
    }

    public override IValue RenameSymbols(Func<string, string> renamer)
    {
        return new Write(
            _writeBuffer.RenameSymbols(renamer),
            _writeOffset.RenameSymbols(renamer),
            _writeValue.RenameSymbols(renamer));
    }
}
