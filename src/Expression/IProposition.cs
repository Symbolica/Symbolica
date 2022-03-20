using System;

namespace Symbolica.Expression;

public interface IProposition : IDisposable
{
    ISpace CreateFalseSpace();
    ISpace CreateTrueSpace();
    bool CanBeFalse();
    bool CanBeTrue();
}
