using Symbolica.Collection;

namespace Symbolica.Computation
{
    internal interface IConstantValue : IValue
    {
        ConstantBitVector ToConstantBitVector(ICollectionFactory collectionFactory);
        ConstantUnsigned ToConstantUnsigned();
        ConstantSigned ToConstantSigned();
        ConstantBool ToConstantBool();
        IValue ToConstantFloat();
    }
}
