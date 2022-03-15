using System;
using System.Collections.Generic;
using System.Numerics;

namespace Symbolica.Expression;

public interface ISpace : IDisposable
{
    Bits PointerSize { get; }

    IExample GetExample();
    IExpression CreateConstant(Bits size, BigInteger value);
    IExpression CreateConstantFloat(Bits size, string value);
    IExpression CreateGarbage(Bits size);
    IExpression CreateSymbolic(Bits size, string? name);
    IExpression CreateSymbolic(Bits size, string? name, IEnumerable<Func<IExpression, IExpression>> assertions);
}
