using System.Linq;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Collection;
using Symbolica.Computation.Exceptions;
using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal sealed class ConstantBitVector : IBitVector, IConstantBitVector
    {
        private ConstantBitVector(Bits size, IPersistentList<byte> constant)
        {
            Size = size;
            Constant = constant;
        }

        public Bits Size { get; }

        public BigInteger AsConstant(Context context)
        {
            return AsUnsigned().AsConstant(context);
        }

        public IValue GetValue(IPersistentSpace space, IBool[] constraints)
        {
            return this;
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
            return ConstantUnsigned.Create(Size, new BigInteger(Constant.ToArray(), true));
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
            return AsSigned().AsFloat();
        }

        public IValue IfElse(IBool predicate, IValue falseValue)
        {
            return new SymbolicBitVector(Size, AsUnsigned().Symbolic).IfElse(predicate, falseValue);
        }

        public IBitVector Read(IUnsigned offset, Bits size)
        {
            return Size == offset.Size
                ? offset is IConstantInteger co
                    ? new ConstantBitVector(size, Constant.GetRange(GetIndex(co), GetCount(size)))
                    : new SymbolicBitVector(Size, AsUnsigned().Symbolic).Read(offset, size)
                : throw new InconsistentExpressionSizesException(Size, offset.Size);
        }

        public IBitVector Write(IUnsigned offset, IBitVector value)
        {
            return Size == offset.Size
                ? offset is IConstantInteger co && value is IConstantBitVector cv
                    ? new ConstantBitVector(Size, Constant.SetRange(GetIndex(co), cv.Constant))
                    : new SymbolicBitVector(Size, AsUnsigned().Symbolic).Write(offset, value)
                : throw new InconsistentExpressionSizesException(Size, offset.Size);
        }

        public IPersistentList<byte> Constant { get; }

        public static ConstantBitVector Create(ICollectionFactory collectionFactory, Bits size, BigInteger value)
        {
            var bytes = new byte[GetCount(size)];
            value.TryWriteBytes(bytes, out _, true);

            return new ConstantBitVector(size, collectionFactory.CreatePersistentList<byte>().AddRange(bytes));
        }

        private static int GetIndex(IConstantInteger offset)
        {
            return (int) (uint) ((Bits) (uint) offset.Constant).ToBytes();
        }

        private static int GetCount(Bits size)
        {
            return (int) (uint) size.ToBytes();
        }
    }
}
