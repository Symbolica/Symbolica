namespace Symbolica.Abstraction
{
    public interface IAction
    {
        void Invoke(IState state);
    }
}
