using System;
using System.Collections.Generic;
using System.Numerics;

namespace Symbolica.Expression;

public interface ISpace
{
    Size PointerSize { get; }

    IExample GetExample();
    IExpression CreateZero(Size size);
    IExpression CreateConstant(Size size, BigInteger value);
    IExpression CreateConstantFloat(Size size, string value);
    IExpression CreateGarbage(Size size);
    IExpression CreateSymbolic(Size size, string? name);
    IExpression CreateSymbolic(Size size, string? name, IEnumerable<Func<IExpression, IExpression>> assertions);
}
