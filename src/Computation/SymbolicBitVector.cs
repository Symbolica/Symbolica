using System.Numerics;
using Microsoft.Z3;
using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal sealed class SymbolicBitVector : IBitVector
    {
        private readonly IUnsigned _buffer;
        private readonly IUnsigned _data;
        private readonly IUnsigned _mask;

        private SymbolicBitVector(IUnsigned buffer, IUnsigned mask, IUnsigned data)
        {
            _buffer = buffer;
            _mask = mask;
            _data = data;
        }

        public Bits Size => _buffer.Size;

        public BigInteger AsConstant(Context context)
        {
            return AsUnsigned().AsConstant(context);
        }

        public IValue GetValue(IPersistentSpace space, IBool[] constraints)
        {
            return AsUnsigned().GetValue(space, constraints);
        }

        public IBitwise AsBitwise()
        {
            return AsUnsigned();
        }

        public IBitVector AsBitVector(ICollectionFactory collectionFactory)
        {
            return this;
        }

        public IUnsigned AsUnsigned()
        {
            return Write(_buffer, _mask, _data).AsUnsigned();
        }

        public ISigned AsSigned()
        {
            return AsUnsigned().AsSigned();
        }

        public IBool AsBool()
        {
            return AsUnsigned().AsBool();
        }

        public IFloat AsFloat()
        {
            return AsUnsigned().AsFloat();
        }

        public IValue IfElse(IBool predicate, IValue falseValue)
        {
            return AsUnsigned().IfElse(predicate, falseValue);
        }

        public IValue Read(IUnsigned offset, Bits size)
        {
            return Write(Read(_buffer, offset, size), Read(_mask, offset, size), Read(_data, offset, size));
        }

        public IValue Write(IUnsigned offset, IBitVector value)
        {
            var mask = Extend(ConstantUnsigned.Create(value.Size, BigInteger.Zero).Not(), offset).Not();
            var data = Extend(value.AsUnsigned(), offset);

            return new SymbolicBitVector(_buffer, _mask.And(mask).AsUnsigned(), Write(_data, mask, data).AsUnsigned());
        }

        public static SymbolicBitVector Create(IUnsigned value)
        {
            var data = ConstantUnsigned.Create(value.Size, BigInteger.Zero);

            return new SymbolicBitVector(value, data.Not(), data);
        }

        private IUnsigned Extend(IUnsigned value, IUnsigned offset)
        {
            return value.ZeroExtend(Size).ShiftLeft(offset);
        }

        private static IBitwise Read(IUnsigned buffer, IUnsigned offset, Bits size)
        {
            return buffer.LogicalShiftRight(offset).Truncate(size);
        }

        private static IBitwise Write(IBitwise buffer, IBitwise mask, IBitwise data)
        {
            return mask.And(buffer).Or(data);
        }
    }
}
