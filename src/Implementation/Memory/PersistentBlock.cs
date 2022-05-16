﻿using System.Collections.Generic;
using Symbolica.Abstraction;
using Symbolica.Abstraction.Memory;
using Symbolica.Expression;

namespace Symbolica.Implementation.Memory;

internal sealed class PersistentBlock : IPersistentBlock
{
    private readonly IExpressionFactory _exprFactory;

    public PersistentBlock(IExpressionFactory exprFactory, Section section, IExpression offset, IExpression data)
    {
        _exprFactory = exprFactory;
        Section = section;
        Offset = offset;
        Data = data;
    }

    public bool IsValid => true;
    public IExpression Offset { get; }
    public Bytes Size => Data.Size.ToBytes();
    public Section Section { get; }
    public IExpression Data { get; }

    public IPersistentBlock Move(IExpression address, Bits size)
    {
        return new PersistentBlock(_exprFactory, Section, address, Data.ZeroExtend(size).Truncate(size));
    }

    public bool CanFree(ISpace space, Section section, IExpression address)
    {
        return Section == section && IsZeroOffset(space, address);
    }

    public Result<IPersistentBlock> TryWrite(ISpace space, IAddress address, IExpression value)
    {
        if (value.Size == Data.Size && IsZeroOffset(space, address))
            return Result<IPersistentBlock>.Success(
                new PersistentBlock(_exprFactory, Section, Offset, value));

        var isFullyInside = IsFullyInside(address, value.Size.ToBytes());
        using var proposition = isFullyInside.GetProposition(space);

        return proposition.CanBeFalse()
            ? proposition.CanBeTrue()
                ? Result<IPersistentBlock>.Both(
                    proposition.CreateFalseSpace(),
                    Write(space, GetOffset(address, isFullyInside), value))
                : Result<IPersistentBlock>.Failure(
                    proposition.CreateFalseSpace())
            : Result<IPersistentBlock>.Success(
                Write(space, GetOffset(address), value));
    }

    public Result<IExpression> TryRead(ISpace space, IAddress address, Bits size)
    {
        if (size == Data.Size && IsZeroOffset(space, address))
            return Result<IExpression>.Success(
                Data);

        var isFullyInside = IsFullyInside(address, size.ToBytes());
        using var proposition = isFullyInside.GetProposition(space);

        return proposition.CanBeFalse()
            ? proposition.CanBeTrue()
                ? Result<IExpression>.Both(
                    proposition.CreateFalseSpace(),
                    Read(space, GetOffset(address, isFullyInside), size))
                : Result<IExpression>.Failure(
                    proposition.CreateFalseSpace())
            : Result<IExpression>.Success(
                Read(space, GetOffset(address), size));
    }

    public PersistentBlock Read(ISpace space, Bytes address, Bytes size)
    {
        return new PersistentBlock(
            _exprFactory,
            Section,
            _exprFactory.CreateConstant(Offset.Size, (uint) address).Subtract(Offset),
            TryRead(
                space,
                Address.Create(_exprFactory, _exprFactory.CreateConstant(Offset.Size, (uint) address)),
                size.ToBits()).Value);
    }

    private bool IsZeroOffset(ISpace space, IExpression address)
    {
        var isEqual = Offset.Equal(address);
        using var proposition = isEqual.GetProposition(space);

        return !proposition.CanBeFalse();
    }

    private IExpression IsFullyInside(IExpression address, Bytes size)
    {
        return address.UnsignedGreaterOrEqual(Offset)
            .And(GetBound(address, size).UnsignedLessOrEqual(GetBound(Offset, Size)));
    }

    private IExpression GetBound(IExpression address, Bytes size)
    {
        return address.Add(_exprFactory.CreateConstant(address.Size, (uint) size));
    }

    private IExpression GetOffset(IExpression address)
    {
        var offset = _exprFactory.CreateConstant(address.Size, (uint) Bytes.One.ToBits())
            .Multiply(address.Subtract(Offset));

        return offset.ZeroExtend(Data.Size).Truncate(Data.Size);
    }

    private IExpression GetOffset(IExpression address, IExpression isFullyInside)
    {
        return isFullyInside
            .Select(
                GetOffset(address),
                _exprFactory.CreateConstant(Data.Size, (uint) Data.Size));
    }

    private IPersistentBlock Write(ISpace space, IExpression offset, IExpression value)
    {
        return new PersistentBlock(_exprFactory, Section, Offset, Data.Write(space, offset, value));
    }

    private IExpression Read(ISpace space, IExpression offset, Bits size)
    {
        return Data.Read(space, offset, size);
    }

    public (HashSet<(IExpression, IExpression)> subs, bool) IsDataEquivalentTo(IPersistentBlock other)
    {
        return other is PersistentBlock pb
            ? Data.IsEquivalentTo(pb.Data)
            : (new(), false);
    }
}
