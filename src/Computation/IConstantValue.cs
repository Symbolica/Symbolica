using System.Numerics;
using Symbolica.Collection;

namespace Symbolica.Computation
{
    internal interface IConstantValue : IValue
    {
        BigInteger Integer { get; }

        ConstantBitVector ToConstantBitVector(ICollectionFactory collectionFactory);
        ConstantUnsigned ToConstantUnsigned();
        ConstantSigned ToConstantSigned();
        ConstantBool ToConstantBool();
        IValue ToConstantFloat();
    }
}
