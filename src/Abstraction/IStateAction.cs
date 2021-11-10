namespace Symbolica.Abstraction
{
    public interface IStateAction
    {
        void Invoke(IState state);
    }
}
