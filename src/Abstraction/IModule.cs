using System.Collections.Generic;
using Symbolica.Expression;

namespace Symbolica.Abstraction
{
    public interface IModule
    {
        string Target { get; }
        Bits PointerSize { get; }
        IStructType? DirectoryStreamType { get; }
        IStructType? DirectoryEntryType { get; }
        IStructType? JumpBufferType { get; }
        IStructType? LocaleType { get; }
        IStructType? StatType { get; }
        IStructType? ThreadType { get; }
        IStructType? VaListType { get; }
        IEnumerable<IFunction> Functions { get; }
        IEnumerable<IGlobal> Globals { get; }
    }
}
