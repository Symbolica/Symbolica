using System;

namespace Symbolica.Expression
{
    // TODO: In C#10 this can be a struct
    public record Unit();

    public interface IFunc<in TIn, out TOut>
    {
        TOut Run(TIn arg);
    }

    public static class FuncExtensions
    {
        public static Func<TIn, TOut> AsFunc<TIn, TOut>(this IFunc<TIn, TOut> func) => func.Run;
    }

    public static class MapExtensions
    {
        public static IFunc<A, C> Map<A, B, C>(this IFunc<A, B> self, IFunc<B, C> g) =>
            new MapImpl<A, B, C>(self, g);

        private class MapImpl<A, B, C> : IFunc<A, C>
        {
            private readonly IFunc<A, B> _f;
            private readonly IFunc<B, C> _g;

            public MapImpl(IFunc<A, B> f, IFunc<B, C> g)
            {
                _f = f;
                _g = g;
            }

            public C Run(A a)
            {
                return _g.Run(_f.Run(a));
            }
        }
    }

    public static class ApplyExtensions
    {
        public static IFunc<TIn1, TOut> Apply<TIn1, TIn2, TOut>(this IFunc<TIn1, IFunc<TIn2, TOut>> self, IFunc<TIn1, TIn2> f) =>
            new ApplyImpl<TIn1, TIn2, TOut>(f, self);

        private class ApplyImpl<TIn1, TIn2, TOut> : IFunc<TIn1, TOut>
        {
            private readonly IFunc<TIn1, TIn2> _f;
            private readonly IFunc<TIn1, IFunc<TIn2, TOut>> _a;

            public ApplyImpl(IFunc<TIn1, TIn2> f, IFunc<TIn1, IFunc<TIn2, TOut>> a)
            {
                _f = f;
                _a = a;
            }

            public TOut Run(TIn1 arg) => _a.Run(arg).Run(_f.Run(arg));
        }
    }
}
