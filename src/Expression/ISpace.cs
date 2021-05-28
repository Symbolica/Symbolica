using System;
using System.Collections.Generic;
using System.Numerics;

namespace Symbolica.Expression
{
    public interface ISpace
    {
        Bits PointerSize { get; }

        IExample GetExample();
        IExpression CreateConstant(Bits size, BigInteger value);
        IExpression CreateGarbage(Bits size);
        IExpression CreateSymbolic(Bits size, string? name, IEnumerable<Func<IExpression, IExpression>>? constraints);
    }
}
