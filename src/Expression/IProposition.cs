using System;

namespace Symbolica.Expression
{
    public interface IProposition : IDisposable
    {
        ISpace FalseSpace { get; }
        ISpace TrueSpace { get; }
        bool CanBeFalse { get; }
        bool CanBeTrue { get; }
    }
}
