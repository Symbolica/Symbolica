using Symbolica.Expression;

namespace Symbolica.Implementation.Memory;

internal sealed class PersistentBlock : IPersistentBlock
{
    private readonly Section _section;

    public PersistentBlock(Section section, IExpression address, IExpression data)
    {
        _section = section;
        Address = address;
        Data = data;
    }

    public bool IsValid => true;
    public IExpression Address { get; }
    public IExpression Data { get; }

    public IPersistentBlock Move(IExpression address, Bits size)
    {
        return new PersistentBlock(_section, address, Data.ZeroExtend(size).Truncate(size));
    }

    public bool CanFree(ISpace space, Section section, IExpression address)
    {
        return _section == section && IsZeroOffset(space, address);
    }

    public Result<IPersistentBlock> TryWrite(ISpace space, IExpression address, IExpression value)
    {
        if (value.Size == Data.Size && IsZeroOffset(space, address))
            return Result<IPersistentBlock>.Success(
                new PersistentBlock(_section, Address, value));

        var isFullyInside = IsFullyInside(space, address, value.Size.ToBytes());
        using var proposition = isFullyInside.GetProposition(space);

        return proposition.CanBeFalse()
            ? proposition.CanBeTrue()
                ? Result<IPersistentBlock>.Both(
                    proposition.CreateFalseSpace(),
                    new PersistentBlock(_section, Address, Data.Write(GetOffset(space, address, isFullyInside), value)))
                : Result<IPersistentBlock>.Failure(
                    proposition.CreateFalseSpace())
            : Result<IPersistentBlock>.Success(
                new PersistentBlock(_section, Address, Data.Write(GetOffset(space, address), value)));
    }

    public Result<IPersistentBlock> TryRead(ISpace space, IExpression address, Bits size)
    {
        if (size == Data.Size && IsZeroOffset(space, address))
            return Result<IPersistentBlock>.Success(
                this);

        var isFullyInside = IsFullyInside(space, address, size.ToBytes());
        using var proposition = isFullyInside.GetProposition(space);

        return proposition.CanBeFalse()
            ? proposition.CanBeTrue()
                ? Result<IPersistentBlock>.Both(
                    proposition.CreateFalseSpace(),
                    new PersistentBlock(_section, address, Data.Read(GetOffset(space, address, isFullyInside), size)))
                : Result<IPersistentBlock>.Failure(
                    proposition.CreateFalseSpace())
            : Result<IPersistentBlock>.Success(
                new PersistentBlock(_section, address, Data.Read(GetOffset(space, address), size)));
    }

    private bool IsZeroOffset(ISpace space, IExpression address)
    {
        var isEqual = Address.Equal(address);
        using var proposition = isEqual.GetProposition(space);

        return !proposition.CanBeFalse();
    }

    private IExpression IsFullyInside(ISpace space, IExpression address, Bytes size)
    {
        return address.UnsignedGreaterOrEqual(Address)
            .And(GetBound(space, address, size).UnsignedLessOrEqual(GetBound(space, Address, Data.Size.ToBytes())));
    }

    private static IExpression GetBound(ISpace space, IExpression address, Bytes size)
    {
        return address.Add(space.CreateConstant(address.Size, (uint) size));
    }

    private IExpression GetOffset(ISpace space, IExpression address)
    {
        var offset = space.CreateConstant(address.Size, (uint) Bytes.One.ToBits())
            .Multiply(address.Subtract(Address));

        return offset.ZeroExtend(Data.Size).Truncate(Data.Size);
    }

    private IExpression GetOffset(ISpace space, IExpression address, IExpression isFullyInside)
    {
        return isFullyInside
            .Select(
                GetOffset(space, address),
                space.CreateConstant(Data.Size, (uint) Data.Size));
    }
}
