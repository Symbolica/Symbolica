using System.Net.Sockets;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Implementation.Memory;

internal sealed class PersistentBlock : IPersistentBlock
{
    private readonly IExpression _data;

    public PersistentBlock(Section section, IExpression address, IExpression data)
    {
        Section = section;
        Offset = address;
        _data = data;
    }

    public bool IsValid => true;
    public IExpression Offset { get; }
    public Bytes Size => _data.Size.ToBytes();

    internal Section Section { get; }

    public IPersistentBlock Move(IExpression address, Bits size)
    {
        return new PersistentBlock(Section, address, _data.ZeroExtend(size).Truncate(size));
    }

    public bool CanFree(ISpace space, Section section, IExpression address)
    {
        return Section == section && IsZeroOffset(space, address);
    }

    public Result<IPersistentBlock> TryWrite(ISpace space, IAddress address, IExpression value)
    {
        if (value.Size == _data.Size && IsZeroOffset(space, address))
            return Result<IPersistentBlock>.Success(
                new PersistentBlock(Section, Offset, value));

        var isFullyInside = IsFullyInside(space, address, value.Size.ToBytes());
        using var proposition = isFullyInside.GetProposition(space);

        return proposition.CanBeFalse()
            ? proposition.CanBeTrue()
                ? Result<IPersistentBlock>.Both(
                    proposition.CreateFalseSpace(),
                    Write(space, GetOffset(space, address, isFullyInside), value))
                : Result<IPersistentBlock>.Failure(
                    proposition.CreateFalseSpace())
            : Result<IPersistentBlock>.Success(
                Write(space, GetOffset(space, address), value));
    }

    public Result<IExpression> TryRead(ISpace space, IAddress address, Bits size)
    {
        if (size == _data.Size && IsZeroOffset(space, address))
            return Result<IExpression>.Success(
                _data);

        var isFullyInside = IsFullyInside(space, address, size.ToBytes());
        using var proposition = isFullyInside.GetProposition(space);

        return proposition.CanBeFalse()
            ? proposition.CanBeTrue()
                ? Result<IExpression>.Both(
                    proposition.CreateFalseSpace(),
                    Read(space, GetOffset(space, address, isFullyInside), size))
                : Result<IExpression>.Failure(
                    proposition.CreateFalseSpace())
            : Result<IExpression>.Success(
                Read(space, GetOffset(space, address), size));
    }

    public PersistentBlock Read(ISpace space, Bytes address, Bytes size)
    {
        return new PersistentBlock(
            Section,
            space.CreateConstant(Offset.Size, (uint) address).Subtract(Offset),
            TryRead(
                space,
                Address.Create(space.CreateConstant(Offset.Size, (uint) address)),
                size.ToBits()).Value);
    }

    private bool IsZeroOffset(ISpace space, IExpression address)
    {
        var isEqual = Offset.Equal(address);
        using var proposition = isEqual.GetProposition(space);

        return !proposition.CanBeFalse();
    }

    private IExpression IsFullyInside(ISpace space, IExpression address, Bytes size)
    {
        return address.UnsignedGreaterOrEqual(Offset)
            .And(GetBound(space, address, size).UnsignedLessOrEqual(GetBound(space, Offset, Size)));
    }

    private static IExpression GetBound(ISpace space, IExpression address, Bytes size)
    {
        return address.Add(space.CreateConstant(address.Size, (uint) size));
    }

    private IExpression GetOffset(ISpace space, IExpression address)
    {
        var offset = space.CreateConstant(address.Size, (uint) Bytes.One.ToBits())
            .Multiply(address.Subtract(Offset));

        return offset.ZeroExtend(_data.Size).Truncate(_data.Size);
    }

    private IExpression GetOffset(ISpace space, IExpression address, IExpression isFullyInside)
    {
        return isFullyInside
            .Select(
                GetOffset(space, address),
                space.CreateConstant(_data.Size, (uint) _data.Size));
    }

    private IPersistentBlock Write(ISpace space, IExpression offset, IExpression value)
    {
        return new PersistentBlock(Section, Offset, _data.Write(space, offset, value));
    }

    private IExpression Read(ISpace space, IExpression offset, Bits size)
    {
        return _data.Read(space, offset, size);
    }

    public IExpression Data(ISpace space)
    {
        return _data;
    }
}
