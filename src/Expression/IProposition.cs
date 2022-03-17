using System;

namespace Symbolica.Expression;

public interface IProposition : IDisposable
{
    ISpace FalseSpace();
    ISpace TrueSpace();
    bool CanBeFalse();
    bool CanBeTrue();
}
