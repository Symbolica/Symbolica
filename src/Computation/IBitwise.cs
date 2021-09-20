namespace Symbolica.Computation
{
    internal interface IBitwise : IValue
    {
        IBitwise And(IBitwise value);
        IBool Equal(IBitwise value);
        IBool NotEqual(IBitwise value);
        IBitwise Or(IBitwise value);
        IBitwise Xor(IBitwise value);
    }
}
