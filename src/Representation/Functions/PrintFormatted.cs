using System;
using Symbolica.Abstraction;

namespace Symbolica.Representation.Functions
{
    [Serializable]
    internal sealed class PrintFormatted : IFunction
    {
        public PrintFormatted(FunctionId id, IParameters parameters)
        {
            Id = id;
            Parameters = parameters;
        }

        public FunctionId Id { get; }
        public IParameters Parameters { get; }

        public void Call(IState state, ICaller caller, IArguments arguments)
        {
        }
    }
}
