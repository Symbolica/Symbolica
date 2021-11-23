using System;
using Symbolica.Abstraction;

namespace Symbolica.Representation
{
    [Serializable]
    public sealed class Parameters : IParameters
    {
        private readonly Parameter[] _parameters;

        public Parameters(Parameter[] parameters)
        {
            _parameters = parameters;
        }

        public int Count => _parameters.Length;

        public Parameter Get(int index)
        {
            return _parameters[index];
        }
    }
}
