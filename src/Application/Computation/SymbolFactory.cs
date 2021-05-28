using System;
using Microsoft.Z3;
using Symbolica.Computation;
using Symbolica.Expression;

namespace Symbolica.Application.Computation
{
    internal sealed class SymbolFactory : ISymbolFactory
    {
        public Func<Context, BitVecExpr> Create(Bits size, string? name)
        {
            var symbol = name ?? Guid.NewGuid().ToString();

            return c => c.MkBVConst(symbol, (uint) size);
        }
    }
}
