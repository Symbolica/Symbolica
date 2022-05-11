using Symbolica.Abstraction;

namespace Symbolica.Implementation.System;

internal interface ISystemProxy : ISystem
{
    ISystemProxy Clone();
}
