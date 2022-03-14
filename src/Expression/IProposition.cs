using System;

namespace Symbolica.Expression;

public interface IProposition : IDisposable
{
    long RefCount { get; }
    bool CanBeFalse();
    bool CanBeTrue();
    ISpace FalseSpace();
    ISpace TrueSpace();
}
