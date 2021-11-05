namespace Symbolica.Abstraction
{
    // TODO: In C#10 this can be a struct
    public record Unit();

    public interface IFunc<TIn, TOut>
    {
        TOut Run(TIn arg);
    }
}
