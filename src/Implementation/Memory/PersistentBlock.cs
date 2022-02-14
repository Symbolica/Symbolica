using Symbolica.Expression;

namespace Symbolica.Implementation.Memory;

internal sealed class PersistentBlock : IPersistentBlock
{
    private readonly IExpression _data;
    private readonly Section _section;

    public PersistentBlock(Section section, IExpression address, IExpression data)
    {
        _section = section;
        Address = address;
        _data = data;
    }

    public bool IsValid => true;
    public IExpression Address { get; }
    public Bytes Size => _data.Size.ToBytes();

    public IPersistentBlock Move(IExpression address, Bits size)
    {
        return new PersistentBlock(_section, address, _data.ZeroExtend(size).Truncate(size));
    }

    public bool CanFree(ISpace space, Section section, IExpression address)
    {
        return _section == section && IsZeroOffset(space, address);
    }

    public Result<IPersistentBlock> TryWrite(ISpace space, IExpression address, IExpression value)
    {
        if (value.Size == _data.Size && IsZeroOffset(space, address))
            return Result<IPersistentBlock>.Success(
                new PersistentBlock(_section, Address, value));

        var isFullyInside = IsFullyInside(space, address, value.Size.ToBytes());
        using var proposition = isFullyInside.GetProposition(space);

        return proposition.CanBeFalse
            ? proposition.CanBeTrue
                ? Result<IPersistentBlock>.Both(
                    proposition.FalseSpace,
                    Write(space, GetOffset(space, address, isFullyInside), value))
                : Result<IPersistentBlock>.Failure(
                    proposition.FalseSpace)
            : Result<IPersistentBlock>.Success(
                Write(space, GetOffset(space, address), value));
    }

    public Result<IExpression> TryRead(ISpace space, IExpression address, Bits size)
    {
        if (size == _data.Size && IsZeroOffset(space, address))
            return Result<IExpression>.Success(
                _data);

        var isFullyInside = IsFullyInside(space, address, size.ToBytes());
        using var proposition = isFullyInside.GetProposition(space);

        return proposition.CanBeFalse
            ? proposition.CanBeTrue
                ? Result<IExpression>.Both(
                    proposition.FalseSpace,
                    Read(space, GetOffset(space, address, isFullyInside), size))
                : Result<IExpression>.Failure(
                    proposition.FalseSpace)
            : Result<IExpression>.Success(
                Read(space, GetOffset(space, address), size));
    }

    private bool IsZeroOffset(ISpace space, IExpression address)
    {
        var isEqual = Address.Equal(address);
        using var proposition = isEqual.GetProposition(space);

        return !proposition.CanBeFalse;
    }

    private IExpression IsFullyInside(ISpace space, IExpression address, Bytes size)
    {
        return address.UnsignedGreaterOrEqual(Address)
            .And(GetBound(space, address, size).UnsignedLessOrEqual(GetBound(space, Address, Size)));
    }

    private static IExpression GetBound(ISpace space, IExpression address, Bytes size)
    {
        return address.Add(space.CreateConstant(address.Size, (uint) size));
    }

    private IExpression GetOffset(ISpace space, IExpression address)
    {
        var offset = space.CreateConstant(address.Size, (uint) Bytes.One.ToBits())
            .Multiply(address.Subtract(Address));

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
        return new PersistentBlock(_section, Address, _data.Write(space, offset, value));
    }

    private IExpression Read(ISpace space, IExpression offset, Bits size)
    {
        return _data.Read(space, offset, size);
    }
}
