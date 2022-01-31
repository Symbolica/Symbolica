using System.Numerics;
using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation.Values.Symbolics;

internal sealed class Write : BitVector, IWriteValue
{
    private readonly IValue _writeBuffer;
    private readonly IValue _writeOffset;
    private readonly IValue _writeValue;

    public Write(IValue writeBuffer, IValue writeOffset, IValue writeValue)
        : base(writeBuffer.Size)
    {
        _writeBuffer = writeBuffer;
        _writeOffset = writeOffset;
        _writeValue = writeValue;
    }

    public override BitVecExpr AsBitVector(Context context)
    {
        return Flatten().AsBitVector(context);
    }

    public IValue Read(IValue offset, Bits size)
    {
        var readMask = Mask(offset, size);
        var writeMask = Mask(_writeOffset, _writeValue.Size);

        return readMask is IConstantValue cr && writeMask is IConstantValue cw
            ? cr.AsUnsigned().And(cw.AsUnsigned()).IsZero
                ? _writeBuffer is IWriteValue w
                    ? w.Read(offset, size)
                    : new Read(_writeBuffer, offset, size)
                : cr.AsUnsigned().Xor(cw.AsUnsigned()).IsZero
                    ? _writeValue
                    : new Read(Flatten(), offset, size)
            : new Read(Flatten(), offset, size);
    }

    private IValue Flatten()
    {
        var writeMask = Mask(_writeOffset, _writeValue.Size);
        var writeData = new ShiftLeft(new ZeroExtend(Size, _writeValue), _writeOffset);

        return new Or(new And(_writeBuffer, new Not(writeMask)), writeData);
    }

    private IValue Mask(IValue offset, Bits size)
    {
        var low = ConstantUnsigned.Create(size, BigInteger.Zero).Not().Extend(Size);

        return offset is IConstantValue co
            ? low.ShiftLeft(co.AsUnsigned())
            : new ShiftLeft(low, offset);
    }
}
