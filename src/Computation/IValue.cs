using System.Numerics;
using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal interface IValue
    {
        Bits Size { get; }

        BigInteger AsConstant(IContextFactory contextFactory);
        IValue GetValue(IPersistentSpace space, IBool[] constraints);
        IBitwise AsBitwise();
        IBitVector AsBitVector(ICollectionFactory collectionFactory);
        IUnsigned AsUnsigned();
        ISigned AsSigned();
        IBool AsBool();
        IFloat AsFloat();
        IValue IfElse(IBool predicate, IValue falseValue);
    }
}
