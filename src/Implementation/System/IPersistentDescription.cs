using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Implementation.System;

internal interface IPersistentDescription
{
    (long, IPersistentDescription) Seek(long offset, uint whence);
    int Read(ISpace space, IMemory memory, IExpression<IType> address, int count);
    IExpression<IType> ReadDirectory(ISpace space, IMemory memory, IStruct entry, IExpression<IType> address, int tell);
    int GetStatus(ISpace space, IMemory memory, IStruct stat, IExpression<IType> address);
}
