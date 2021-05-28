using System.Collections.Generic;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation
{
    public sealed class Module : IModule
    {
        private readonly IFunction[] _functions;
        private readonly IGlobal[] _globals;
        private readonly IStructType[] _structTypes;

        public Module(string target, Bits pointerSize,
            IStructType[] structTypes, IFunction[] functions, IGlobal[] globals)
        {
            Target = target;
            PointerSize = pointerSize;
            _structTypes = structTypes;
            _functions = functions;
            _globals = globals;
        }

        public string Target { get; }
        public Bits PointerSize { get; }
        public IEnumerable<IStructType> StructTypes => _structTypes;
        public IEnumerable<IFunction> Functions => _functions;
        public IEnumerable<IGlobal> Globals => _globals;
    }
}
