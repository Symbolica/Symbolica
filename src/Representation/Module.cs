using System.Collections.Generic;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation
{
    public sealed class Module : IModule
    {
        private readonly IFunction[] _functions;
        private readonly IGlobal[] _globals;

        public Module(
            string target,
            Bits pointerSize,
            IStructType? directoryStreamType,
            IStructType? directoryEntryType,
            IStructType? jumpBufferType,
            IStructType? localeType,
            IStructType? statType,
            IStructType? threadType,
            IStructType? vaListType,
            IFunction[] functions,
            IGlobal[] globals)
        {
            Target = target;
            PointerSize = pointerSize;
            DirectoryStreamType = directoryStreamType;
            DirectoryEntryType = directoryEntryType;
            JumpBufferType = jumpBufferType;
            LocaleType = localeType;
            StatType = statType;
            ThreadType = threadType;
            VaListType = vaListType;
            _functions = functions;
            _globals = globals;
        }

        public string Target { get; }
        public Bits PointerSize { get; }
        public IStructType? DirectoryStreamType { get; }
        public IStructType? DirectoryEntryType { get; }
        public IStructType? JumpBufferType { get; }
        public IStructType? LocaleType { get; }
        public IStructType? StatType { get; }
        public IStructType? ThreadType { get; }
        public IStructType? VaListType { get; }
        public IEnumerable<IFunction> Functions => _functions;
        public IEnumerable<IGlobal> Globals => _globals;
    }
}
