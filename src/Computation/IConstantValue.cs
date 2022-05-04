using System;
using Symbolica.Collection;
using Symbolica.Computation.Values.Constants;

namespace Symbolica.Computation;

internal interface IConstantValue : IValue, IEquatable<IConstantValue>
{
    ConstantBitVector AsBitVector(ICollectionFactory collectionFactory);
    ConstantUnsigned AsUnsigned();
    ConstantSigned AsSigned();
    ConstantBool AsBool();
    ConstantSingle AsSingle();
    ConstantDouble AsDouble();
}
