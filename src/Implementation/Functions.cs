using System;
using System.Collections.Generic;
using System.Linq;
using Symbolica.Abstraction;

namespace Symbolica.Implementation
{
    internal sealed class Functions : IFunctions
    {
        private readonly IReadOnlyDictionary<FunctionId, IFunction> _functions;

        public Functions(IEnumerable<IFunction> functions)
        {
            _functions = functions.ToDictionary(f => f.Id);
        }

        public IFunction Get(FunctionId functionId)
        {
            return _functions.TryGetValue(functionId, out var function)
                ? function
                : throw new Exception($"Function {functionId} was not found.");
        }
    }
}
