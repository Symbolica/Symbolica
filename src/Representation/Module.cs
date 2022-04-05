using System.Collections.Generic;
using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Representation.Exceptions;

namespace Symbolica.Representation;

public sealed class Module : IModule
{
    private readonly (string, IStructType?) _directoryEntryType;
    private readonly (string, IStructType?) _directoryStreamType;
    private readonly IReadOnlyDictionary<FunctionId, IFunction> _functions;
    private readonly IReadOnlyDictionary<GlobalId, IGlobal> _globals;
    private readonly (string, IStructType?) _jumpBufferType;
    private readonly (string, IStructType?) _localeType;
    private readonly (string, IStructType?) _statType;
    private readonly (string, IStructType?) _threadType;
    private readonly (string, IStructType?) _vaListType;

    private Module(
        string target,
        Size pointerSize,
        (string, IStructType?) directoryStreamType,
        (string, IStructType?) directoryEntryType,
        (string, IStructType?) jumpBufferType,
        (string, IStructType?) localeType,
        (string, IStructType?) statType,
        (string, IStructType?) threadType,
        (string, IStructType?) vaListType,
        IReadOnlyDictionary<FunctionId, IFunction> functions,
        IReadOnlyDictionary<GlobalId, IGlobal> globals)
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
    public Size PointerSize { get; }
    public IStructType DirectoryStreamType => GetStructType(_directoryStreamType);
    public IStructType DirectoryEntryType => GetStructType(_directoryEntryType);
    public IStructType JumpBufferType => GetStructType(_jumpBufferType);
    public IStructType LocaleType => GetStructType(_localeType);
    public IStructType StatType => GetStructType(_statType);
    public IStructType ThreadType => GetStructType(_threadType);
    public IStructType VaListType => GetStructType(_vaListType);

    public IDefinition GetMain()
    {
        return _functions.Values.OfType<IDefinition>().SingleOrDefault(d => d.Name == "main")
               ?? throw new MissingMainFunctionException();
    }

    public IFunction GetFunction(FunctionId id)
    {
        return _functions.TryGetValue(id, out var function)
            ? function
            : throw new MissingFunctionException(id);
    }

    public IGlobal GetGlobal(GlobalId id)
    {
        return _globals.TryGetValue(id, out var global)
            ? global
            : throw new MissingGlobalException(id);
    }

    private static IStructType GetStructType((string, IStructType?) namedStructType)
    {
        var (name, structType) = namedStructType;

        return structType ?? throw new MissingStructTypeException(name);
    }

    public static IModule Create(
        string target,
        Size pointerSize,
        (string, IStructType?) directoryStreamType,
        (string, IStructType?) directoryEntryType,
        (string, IStructType?) jumpBufferType,
        (string, IStructType?) localeType,
        (string, IStructType?) statType,
        (string, IStructType?) threadType,
        (string, IStructType?) vaListType,
        IEnumerable<IFunction> functions,
        IEnumerable<IGlobal> globals)
    {
        return new Module(
            target,
            pointerSize,
            directoryStreamType,
            directoryEntryType,
            jumpBufferType,
            localeType,
            statType,
            threadType,
            vaListType,
            functions.ToDictionary(f => f.Id),
            globals.ToDictionary(g => g.Id));
    }
}
