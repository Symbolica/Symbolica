using System;
using Symbolica.Abstraction;

namespace Symbolica.Representation.Functions
{
    internal sealed class Unsupported : IFunction
    {
        private readonly string _name;

        public Unsupported(FunctionId id, string name, IParameters parameters)
        {
            Id = id;
            _name = name;
            Parameters = parameters;
        }

        public FunctionId Id { get; }
        public IParameters Parameters { get; }

        public void Call(IState state, ICaller caller, IArguments arguments)
        {
            throw new Exception($"Function {_name} is unsupported.");
        }
    }
}
