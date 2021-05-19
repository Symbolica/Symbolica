namespace Symbolica.Abstraction
{
    public interface IParameters
    {
        int Count { get; }

        Parameter Get(int index);
    }
}