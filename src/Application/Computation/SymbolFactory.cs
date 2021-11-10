using System;
using Microsoft.Z3;
using Symbolica.Computation;
using Symbolica.Computation.ContextFuncs;
using Symbolica.Expression;

namespace Symbolica.Application.Computation
{
    internal sealed class SymbolFactory : ISymbolFactory
    {
        public IFunc<Context, BitVecExpr> Create(Bits size, string? name)
        {
            var symbol = name ?? Guid.NewGuid().ToString();

            return new MkBVConst(symbol, (uint)size);
        }
    }
}
