namespace Symbolica.Abstraction
{
    public interface IFunction
    {
        FunctionId Id { get; }
        IParameters Parameters { get; }

        void Call(IState state, ICaller caller, IArguments arguments);
    }
}