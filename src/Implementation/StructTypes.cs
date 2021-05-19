using System;
using System.Collections.Generic;
using System.Linq;
using Symbolica.Abstraction;

namespace Symbolica.Implementation
{
    internal sealed class StructTypes : IStructTypes
    {
        private readonly IReadOnlyDictionary<string, IStructType> _structTypes;

        public StructTypes(IEnumerable<IStructType> structTypes)
        {
            _structTypes = structTypes.ToDictionary(t => t.Name);
        }

        public IStructType Get(string name)
        {
            return _structTypes.TryGetValue(name, out var structType)
                ? structType
                : throw new Exception($"Struct type {name} was not found.");
        }
    }
}