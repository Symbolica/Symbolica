namespace Symbolica.Abstraction
{
    // TODO: In C#10 this can be a struct
    public record Unit();

    public interface IFunc<TIn, TOut>
    {
        TOut Run(TIn arg);
    }

    public class MapFunc<A, B, C> : IFunc<A, C>
    {
        private readonly IFunc<A, B> _f;
        private readonly IFunc<B, C> _g;

        public MapFunc(IFunc<A, B> f, IFunc<B, C> g)
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
