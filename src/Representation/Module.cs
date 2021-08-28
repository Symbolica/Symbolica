using System;
using System.Collections.Generic;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation
{
    public sealed class Module : IModule
    {
        private readonly (string, IStructType?) _directoryEntryType;
        private readonly (string, IStructType?) _directoryStreamType;
        private readonly IFunction[] _functions;
        private readonly IGlobal[] _globals;
        private readonly (string, IStructType?) _jumpBufferType;
        private readonly (string, IStructType?) _localeType;
        private readonly (string, IStructType?) _statType;
        private readonly (string, IStructType?) _threadType;
        private readonly (string, IStructType?) _vaListType;

        public Module(
            string target,
            Bits pointerSize,
            (string, IStructType?) directoryStreamType,
            (string, IStructType?) directoryEntryType,
            (string, IStructType?) jumpBufferType,
            (string, IStructType?) localeType,
            (string, IStructType?) statType,
            (string, IStructType?) threadType,
            (string, IStructType?) vaListType,
            IFunction[] functions,
            IGlobal[] globals)
        {
            Target = target;
            PointerSize = pointerSize;
            _directoryStreamType = directoryStreamType;
            _directoryEntryType = directoryEntryType;
            _jumpBufferType = jumpBufferType;
            _localeType = localeType;
            _statType = statType;
            _threadType = threadType;
            _vaListType = vaListType;
            _functions = functions;
            _globals = globals;
        }

        public string Target { get; }
        public Bits PointerSize { get; }
        public IStructType DirectoryStreamType => GetStructType(_directoryStreamType);
        public IStructType DirectoryEntryType => GetStructType(_directoryEntryType);
        public IStructType JumpBufferType => GetStructType(_jumpBufferType);
        public IStructType LocaleType => GetStructType(_localeType);
        public IStructType StatType => GetStructType(_statType);
        public IStructType ThreadType => GetStructType(_threadType);
        public IStructType VaListType => GetStructType(_vaListType);
        public IEnumerable<IFunction> Functions => _functions;
        public IEnumerable<IGlobal> Globals => _globals;

        private static IStructType GetStructType((string, IStructType?) namedStructType)
        {
            var (name, structType) = namedStructType;

            return structType ?? throw new Exception($"Struct type {name} was not found.");
        }
    }
}
