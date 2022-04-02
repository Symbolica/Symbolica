using Symbolica.Expression;
using Symbolica.Expression.Values;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Implementation.Memory;

internal sealed class PersistentBlock : IPersistentBlock
{
    private readonly IExpression<IType> _data;
    private readonly Section _section;

    public PersistentBlock(Section section, IExpression<IType> address, IExpression<IType> data)
    {
        _section = section;
        Address = address;
        _data = data;
    }

    public bool IsValid => true;
    public IExpression<IType> Address { get; }
    public Bytes Size => _data.Size.ToBytes();

    public IPersistentBlock Move(IExpression<IType> address, Bits size)
    {
        return new PersistentBlock(_section, address, Expression.Values.Resize.Create(size, _data));
    }

    public bool CanFree(ISpace space, Section section, IExpression<IType> address)
    {
        return _section == section && IsZeroOffset(space, address);
    }

    public Result<IPersistentBlock> TryWrite(ISpace space, IExpression<IType> address, IExpression<IType> value)
    {
        if (value.Size == _data.Size && IsZeroOffset(space, address))
            return Result<IPersistentBlock>.Success(
                new PersistentBlock(_section, Address, value));

        var isFullyInside = IsFullyInside(address, value.Size.ToBytes());
        using var proposition = space.CreateProposition(isFullyInside);

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

    public Result<IExpression<IType>> TryRead(ISpace space, IExpression<IType> address, Bits size)
    {
        if (size == _data.Size && IsZeroOffset(space, address))
            return Result<IExpression<IType>>.Success(
                _data);

        var isFullyInside = IsFullyInside(address, size.ToBytes());
        using var proposition = space.CreateProposition(isFullyInside);

        return proposition.CanBeFalse()
            ? proposition.CanBeTrue()
                ? Result<IExpression<IType>>.Both(
                    proposition.CreateFalseSpace(),
                    Read(space, GetOffset(address, isFullyInside), size))
                : Result<IExpression<IType>>.Failure(
                    proposition.CreateFalseSpace())
            : Result<IExpression<IType>>.Success(
                Read(space, GetOffset(address), size));
    }

    private bool IsZeroOffset(ISpace space, IExpression<IType> address)
    {
        var isEqual = Equal.Create(Address, address);
        using var proposition = space.CreateProposition(isEqual);

        return !proposition.CanBeFalse();
    }

    private IExpression<IType> IsFullyInside(IExpression<IType> address, Bytes size)
    {
        return And.Create(
            UnsignedGreaterOrEqual.Create(address, Address),
            UnsignedLessOrEqual.Create(GetBound(address, size), GetBound(Address, Size)));
    }

    private static IExpression<IType> GetBound(IExpression<IType> address, Bytes size)
    {
        return Add.Create(address, ConstantUnsigned.Create(address.Size, (uint) size));
    }

    private IExpression<IType> GetOffset(IExpression<IType> address)
    {
        return Multiply.Create(
            ConstantUnsigned.Create(address.Size, (uint) Bytes.One.ToBits()),
            Subtract.Create(address, Address));
    }

    private IExpression<IType> GetOffset(IExpression<IType> address, IExpression<IType> isFullyInside)
    {
        return Select.Create(
            isFullyInside,
            GetOffset(address),
            ConstantUnsigned.Create(address.Size, (uint) _data.Size));
    }

    private IPersistentBlock Write(ISpace space, IExpression<IType> offset, IExpression<IType> value)
    {
        return new PersistentBlock(_section, Address, space.Write(_data, offset, value));
    }

    private IExpression<IType> Read(ISpace space, IExpression<IType> offset, Bits size)
    {
        return space.Read(_data, offset, size);
    }
}
