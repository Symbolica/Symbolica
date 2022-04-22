using System;
using Symbolica.Expression;

namespace Symbolica.Abstraction;

public interface IStructType : IType
{
    IStruct CreateStruct(Func<Bits, IExpression> initializer);
}
