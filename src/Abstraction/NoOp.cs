namespace Symbolica.Abstraction
{
    public class NoOp<TIn, TOut> : IFunc<TIn, TOut>
    {
        private readonly TOut _out;

        public NoOp(TOut @out)
        {
            _out = @out;
        }

        public TOut Run(TIn _) => _out;
    }

    public class NoOp<TIn> : IFunc<TIn, Unit>
    {
        public Unit Run(TIn _) => new Unit();
    }
}
