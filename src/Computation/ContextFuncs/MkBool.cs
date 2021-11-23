using System;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.ContextFuncs
{
    [Serializable]
    internal class MkBool : IFunc<Context, BoolExpr>
    {
        private readonly bool _constant;

        public MkBool(bool constant)
        {
            _constant = constant;
        }

        public BoolExpr Run(Context ctx) => ctx.MkBool(_constant);
    }
}
