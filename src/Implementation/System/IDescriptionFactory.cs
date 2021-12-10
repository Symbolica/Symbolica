using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Implementation.System;

internal interface IDescriptionFactory
{
    IPersistentDescription? Create(ISpace space, IMemory memory, IExpression path);
    IPersistentDescription CreateInput();
    IPersistentDescription CreateOutput();
    IPersistentDescription CreateInvalid();
}
