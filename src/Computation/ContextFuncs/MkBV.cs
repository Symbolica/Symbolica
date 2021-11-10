using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.ContextFuncs
{
    public class MkBVConst : IFunc<Context, BitVecExpr>
    {
        private readonly string _name;
        private readonly uint _size;

        public MkBVConst(string name, uint size)
        {
            _name = name;
            _size = size;
        }

        public BitVecExpr Run(Context ctx) => ctx.MkBVConst(_name, _size);
    }

    internal class MkBVOfBits : IFunc<Context, BitVecNum>
    {
        private readonly bool[] _bits;

        public MkBVOfBits(bool[] bits)
        {
            _bits = bits;
        }

        public BitVecNum Run(Context ctx) => ctx.MkBV(_bits);
    }

    internal class MkBVOfUnsigned : IFunc<Context, BitVecNum>
    {
        private readonly uint _value;
        private readonly uint _size;

        public MkBVOfUnsigned(uint value, uint size)
        {
            _value = value;
            _size = size;
        }

        public BitVecNum Run(Context ctx) => ctx.MkBV(_value, _size);
    }

    internal class MkBVOfString : IFunc<Context, BitVecNum>
    {
        private readonly string _value;
        private readonly uint _size;

        public MkBVOfString(string value, uint size)
        {
            _value = value;
            _size = size;
        }

        public BitVecNum Run(Context ctx) => ctx.MkBV(_value, _size);
    }

    internal class MkBVASHR : IFunc<Context, BitVecExpr>
    {
        private readonly IFunc<Context, BitVecExpr> _t1;
        private readonly IFunc<Context, BitVecExpr> _t2;

        public MkBVASHR(IFunc<Context, BitVecExpr> t1, IFunc<Context, BitVecExpr> t2)
        {
            _t1 = t1;
            _t2 = t2;
        }

        public BitVecExpr Run(Context ctx) => ctx.MkBVASHR(_t1.Run(ctx), _t2.Run(ctx));
    }

    internal class MkBVSHL : IFunc<Context, BitVecExpr>
    {
        private readonly IFunc<Context, BitVecExpr> _t1;
        private readonly IFunc<Context, BitVecExpr> _t2;

        public MkBVSHL(IFunc<Context, BitVecExpr> t1, IFunc<Context, BitVecExpr> t2)
        {
            _t1 = t1;
            _t2 = t2;
        }

        public BitVecExpr Run(Context ctx) => ctx.MkBVSHL(_t1.Run(ctx), _t2.Run(ctx));
    }

    internal class MkBVLSHR : IFunc<Context, BitVecExpr>
    {
        private readonly IFunc<Context, BitVecExpr> _t1;
        private readonly IFunc<Context, BitVecExpr> _t2;

        public MkBVLSHR(IFunc<Context, BitVecExpr> t1, IFunc<Context, BitVecExpr> t2)
        {
            _t1 = t1;
            _t2 = t2;
        }

        public BitVecExpr Run(Context ctx) => ctx.MkBVLSHR(_t1.Run(ctx), _t2.Run(ctx));
    }

    internal class MkBVSDiv : IFunc<Context, BitVecExpr>
    {
        private readonly IFunc<Context, BitVecExpr> _t1;
        private readonly IFunc<Context, BitVecExpr> _t2;

        public MkBVSDiv(IFunc<Context, BitVecExpr> t1, IFunc<Context, BitVecExpr> t2)
        {
            _t1 = t1;
            _t2 = t2;
        }

        public BitVecExpr Run(Context ctx) => ctx.MkBVSDiv(_t1.Run(ctx), _t2.Run(ctx));
    }

    internal class MkBVSGT : IFunc<Context, BoolExpr>
    {
        private readonly IFunc<Context, BitVecExpr> _t1;
        private readonly IFunc<Context, BitVecExpr> _t2;

        public MkBVSGT(IFunc<Context, BitVecExpr> t1, IFunc<Context, BitVecExpr> t2)
        {
            _t1 = t1;
            _t2 = t2;
        }

        public BoolExpr Run(Context ctx) => ctx.MkBVSGT(_t1.Run(ctx), _t2.Run(ctx));
    }

    internal class MkBVSGE : IFunc<Context, BoolExpr>
    {
        private readonly IFunc<Context, BitVecExpr> _t1;
        private readonly IFunc<Context, BitVecExpr> _t2;

        public MkBVSGE(IFunc<Context, BitVecExpr> t1, IFunc<Context, BitVecExpr> t2)
        {
            _t1 = t1;
            _t2 = t2;
        }

        public BoolExpr Run(Context ctx) => ctx.MkBVSGE(_t1.Run(ctx), _t2.Run(ctx));
    }

    internal class MkBVSLT : IFunc<Context, BoolExpr>
    {
        private readonly IFunc<Context, BitVecExpr> _t1;
        private readonly IFunc<Context, BitVecExpr> _t2;

        public MkBVSLT(IFunc<Context, BitVecExpr> t1, IFunc<Context, BitVecExpr> t2)
        {
            _t1 = t1;
            _t2 = t2;
        }

        public BoolExpr Run(Context ctx) => ctx.MkBVSLT(_t1.Run(ctx), _t2.Run(ctx));
    }

    internal class MkBVSLE : IFunc<Context, BoolExpr>
    {
        private readonly IFunc<Context, BitVecExpr> _t1;
        private readonly IFunc<Context, BitVecExpr> _t2;

        public MkBVSLE(IFunc<Context, BitVecExpr> t1, IFunc<Context, BitVecExpr> t2)
        {
            _t1 = t1;
            _t2 = t2;
        }

        public BoolExpr Run(Context ctx) => ctx.MkBVSLE(_t1.Run(ctx), _t2.Run(ctx));
    }

    internal class MkBVSRem : IFunc<Context, BitVecExpr>
    {
        private readonly IFunc<Context, BitVecExpr> _t1;
        private readonly IFunc<Context, BitVecExpr> _t2;

        public MkBVSRem(IFunc<Context, BitVecExpr> t1, IFunc<Context, BitVecExpr> t2)
        {
            _t1 = t1;
            _t2 = t2;
        }

        public BitVecExpr Run(Context ctx) => ctx.MkBVSRem(_t1.Run(ctx), _t2.Run(ctx));
    }

    internal class MkBVAdd : IFunc<Context, BitVecExpr>
    {
        private readonly IFunc<Context, BitVecExpr> _t1;
        private readonly IFunc<Context, BitVecExpr> _t2;

        public MkBVAdd(IFunc<Context, BitVecExpr> t1, IFunc<Context, BitVecExpr> t2)
        {
            _t1 = t1;
            _t2 = t2;
        }

        public BitVecExpr Run(Context ctx) => ctx.MkBVAdd(_t1.Run(ctx), _t2.Run(ctx));
    }

    internal class MkBVSub : IFunc<Context, BitVecExpr>
    {
        private readonly IFunc<Context, BitVecExpr> _t1;
        private readonly IFunc<Context, BitVecExpr> _t2;

        public MkBVSub(IFunc<Context, BitVecExpr> t1, IFunc<Context, BitVecExpr> t2)
        {
            _t1 = t1;
            _t2 = t2;
        }

        public BitVecExpr Run(Context ctx) => ctx.MkBVSub(_t1.Run(ctx), _t2.Run(ctx));
    }

    internal class MkBVMul : IFunc<Context, BitVecExpr>
    {
        private readonly IFunc<Context, BitVecExpr> _t1;
        private readonly IFunc<Context, BitVecExpr> _t2;

        public MkBVMul(IFunc<Context, BitVecExpr> t1, IFunc<Context, BitVecExpr> t2)
        {
            _t1 = t1;
            _t2 = t2;
        }

        public BitVecExpr Run(Context ctx) => ctx.MkBVMul(_t1.Run(ctx), _t2.Run(ctx));
    }

    internal class MkBVNot : IFunc<Context, BitVecExpr>
    {
        private readonly IFunc<Context, BitVecExpr> _t;

        public MkBVNot(IFunc<Context, BitVecExpr> t)
        {
            _t = t;
        }

        public BitVecExpr Run(Context ctx) => ctx.MkBVNot(_t.Run(ctx));
    }

    internal class MkBVAND : IFunc<Context, BitVecExpr>
    {
        private readonly IFunc<Context, BitVecExpr> _t1;
        private readonly IFunc<Context, BitVecExpr> _t2;

        public MkBVAND(IFunc<Context, BitVecExpr> t1, IFunc<Context, BitVecExpr> t2)
        {
            _t1 = t1;
            _t2 = t2;
        }

        public BitVecExpr Run(Context ctx) => ctx.MkBVAND(_t1.Run(ctx), _t2.Run(ctx));
    }

    internal class MkBVOR : IFunc<Context, BitVecExpr>
    {
        private readonly IFunc<Context, BitVecExpr> _t1;
        private readonly IFunc<Context, BitVecExpr> _t2;

        public MkBVOR(IFunc<Context, BitVecExpr> t1, IFunc<Context, BitVecExpr> t2)
        {
            _t1 = t1;
            _t2 = t2;
        }

        public BitVecExpr Run(Context ctx) => ctx.MkBVOR(_t1.Run(ctx), _t2.Run(ctx));
    }

    internal class MkBVXOR : IFunc<Context, BitVecExpr>
    {
        private readonly IFunc<Context, BitVecExpr> _t1;
        private readonly IFunc<Context, BitVecExpr> _t2;

        public MkBVXOR(IFunc<Context, BitVecExpr> t1, IFunc<Context, BitVecExpr> t2)
        {
            _t1 = t1;
            _t2 = t2;
        }

        public BitVecExpr Run(Context ctx) => ctx.MkBVXOR(_t1.Run(ctx), _t2.Run(ctx));
    }

    internal class MkBVUDiv : IFunc<Context, BitVecExpr>
    {
        private readonly IFunc<Context, BitVecExpr> _t1;
        private readonly IFunc<Context, BitVecExpr> _t2;

        public MkBVUDiv(IFunc<Context, BitVecExpr> t1, IFunc<Context, BitVecExpr> t2)
        {
            _t1 = t1;
            _t2 = t2;
        }

        public BitVecExpr Run(Context ctx) => ctx.MkBVUDiv(_t1.Run(ctx), _t2.Run(ctx));
    }

    internal class MkBVURem : IFunc<Context, BitVecExpr>
    {
        private readonly IFunc<Context, BitVecExpr> _t1;
        private readonly IFunc<Context, BitVecExpr> _t2;

        public MkBVURem(IFunc<Context, BitVecExpr> t1, IFunc<Context, BitVecExpr> t2)
        {
            _t1 = t1;
            _t2 = t2;
        }

        public BitVecExpr Run(Context ctx) => ctx.MkBVURem(_t1.Run(ctx), _t2.Run(ctx));
    }

    internal class MkBVUGT : IFunc<Context, BoolExpr>
    {
        private readonly IFunc<Context, BitVecExpr> _t1;
        private readonly IFunc<Context, BitVecExpr> _t2;

        public MkBVUGT(IFunc<Context, BitVecExpr> t1, IFunc<Context, BitVecExpr> t2)
        {
            _t1 = t1;
            _t2 = t2;
        }

        public BoolExpr Run(Context ctx) => ctx.MkBVUGT(_t1.Run(ctx), _t2.Run(ctx));
    }

    internal class MkBVUGE : IFunc<Context, BoolExpr>
    {
        private readonly IFunc<Context, BitVecExpr> _t1;
        private readonly IFunc<Context, BitVecExpr> _t2;

        public MkBVUGE(IFunc<Context, BitVecExpr> t1, IFunc<Context, BitVecExpr> t2)
        {
            _t1 = t1;
            _t2 = t2;
        }

        public BoolExpr Run(Context ctx) => ctx.MkBVUGE(_t1.Run(ctx), _t2.Run(ctx));
    }

    internal class MkBVULT : IFunc<Context, BoolExpr>
    {
        private readonly IFunc<Context, BitVecExpr> _t1;
        private readonly IFunc<Context, BitVecExpr> _t2;

        public MkBVULT(IFunc<Context, BitVecExpr> t1, IFunc<Context, BitVecExpr> t2)
        {
            _t1 = t1;
            _t2 = t2;
        }

        public BoolExpr Run(Context ctx) => ctx.MkBVULT(_t1.Run(ctx), _t2.Run(ctx));
    }

    internal class MkBVULE : IFunc<Context, BoolExpr>
    {
        private readonly IFunc<Context, BitVecExpr> _t1;
        private readonly IFunc<Context, BitVecExpr> _t2;

        public MkBVULE(IFunc<Context, BitVecExpr> t1, IFunc<Context, BitVecExpr> t2)
        {
            _t1 = t1;
            _t2 = t2;
        }

        public BoolExpr Run(Context ctx) => ctx.MkBVULE(_t1.Run(ctx), _t2.Run(ctx));
    }
}
