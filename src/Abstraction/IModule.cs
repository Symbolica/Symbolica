using System.Collections.Generic;
using Symbolica.Expression;

namespace Symbolica.Abstraction
{
    public interface IModule
    {
        string Target { get; }
        Bits PointerSize { get; }
        IEnumerable<IStructType> StructTypes { get; }
        IEnumerable<IFunction> Functions { get; }
        IEnumerable<IGlobal> Globals { get; }
    }
}