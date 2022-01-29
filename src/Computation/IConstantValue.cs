using Symbolica.Collection;
using Symbolica.Computation.Values.Constants;

namespace Symbolica.Computation;

internal interface IConstantValue : IValue
{
    ConstantBitVector AsBitVector(ICollectionFactory collectionFactory);
    ConstantUnsigned AsUnsigned();
    ConstantSigned AsSigned();
    ConstantBool AsBool();
    ConstantSingle AsSingle();
    ConstantDouble AsDouble();
}
