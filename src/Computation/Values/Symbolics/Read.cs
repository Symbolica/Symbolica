using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.Values.Symbolics;

internal sealed class Read : BitVector
{
    private readonly IValue _buffer;
    private readonly IValue _offset;

    public Read(IValue buffer, IValue offset, Bits size)
        : base(size)
    {
        _buffer = buffer;
        _offset = offset;
    }

    public override BitVecExpr AsBitVector(Context context)
    {
        var value = new Truncate(Size, new LogicalShiftRight(_buffer, _offset));

        return value.AsBitVector(context);
    }
}
