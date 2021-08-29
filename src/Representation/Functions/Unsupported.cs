using Symbolica.Abstraction;

namespace Symbolica.Representation.Functions
{
    internal sealed class Unsupported : IFunction
    {
        private readonly string _name;

        public Unsupported(string name, FunctionId id, IParameters parameters)
        {
            _name = name;
            Id = id;
            Parameters = parameters;
        }

        public FunctionId Id { get; }
        public IParameters Parameters { get; }

        public void Call(IState state, ICaller caller, IArguments arguments)
        {
            throw new UnsupportedFunctionException(_name);
        }
    }
}
