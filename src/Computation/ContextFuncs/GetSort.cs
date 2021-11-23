using System;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.ContextFuncs
{
    [Serializable]
    internal class GetSort : IFunc<Context, FPSort>
    {
        private readonly Bits _bits;

        public GetSort(Bits bits)
        {
            _bits = bits;
        }

        public FPSort Run(Context ctx) => _bits.GetSort(ctx);
    }
}
