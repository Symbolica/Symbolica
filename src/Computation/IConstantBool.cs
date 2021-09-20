namespace Symbolica.Computation
{
    internal interface IConstantBool : IValue
    {
        bool Constant { get; }
    }
}
