using System;

namespace Symbolica.Expression;

public interface IProposition : IDisposable
{
    bool CanBeFalse();
    bool CanBeTrue();
    ISpace FalseSpace();
    ISpace TrueSpace();
}
