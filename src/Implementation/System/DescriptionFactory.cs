using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Implementation.System;

internal sealed class DescriptionFactory : IDescriptionFactory
{
    private readonly IExpressionFactory _exprFactory;
    private readonly IFileSystem _fileSystem;

    public DescriptionFactory(IExpressionFactory exprFactory, IFileSystem fileSystem)
    {
        _exprFactory = exprFactory;
        _fileSystem = fileSystem;
    }

    public IPersistentDescription? Create(string path)
    {
        return CreateFile(path)
               ?? CreateDirectory(path);
    }

    public IPersistentDescription CreateInput()
    {
        return new InputDescription(_exprFactory);
    }

    public IPersistentDescription CreateOutput()
    {
        return new OutputDescription(_exprFactory);
    }

    public IPersistentDescription CreateInvalid()
    {
        return InvalidDescription.Instance(_exprFactory);
    }

    private IPersistentDescription? CreateFile(string path)
    {
        var file = _fileSystem.GetFile(path);

        return file == null
            ? null
            : FileDescription.Create(_exprFactory, file);
    }

    private IPersistentDescription? CreateDirectory(string path)
    {
        var directory = _fileSystem.GetDirectory(path);

        return directory == null
            ? null
            : new DirectoryDescription(_exprFactory, directory);
    }

    private sealed class InvalidDescription : IPersistentDescription
    {
        private readonly IExpressionFactory _exprFactory;

        private InvalidDescription(IExpressionFactory exprFactory)
        {
            _exprFactory = exprFactory;
        }

        public static IPersistentDescription Instance(IExpressionFactory exprFactory) =>
            new Lazy<InvalidDescription>(() => new(exprFactory)).Value;

        public (long, IPersistentDescription) Seek(long offset, uint whence)
        {
            return (-1L, this);
        }

        public int Read(ISpace space, IMemory memory, IExpression address, int count)
        {
            return -1;
        }

        public IExpression ReadDirectory(ISpace space, IMemory memory, IStruct entry, IExpression address, int tell)
        {
            return _exprFactory.CreateZero(_exprFactory.PointerSize);
        }

        public int GetStatus(ISpace space, IMemory memory, IStruct stat, IExpression address)
        {
            return -1;
        }

        public (HashSet<ExpressionSubs> subs, bool) IsEquivalentTo(IPersistentDescription other)
        {
            return (new(), other is InvalidDescription);
        }

        public object ToJson()
        {
            return GetType().Name;
        }

        public int GetEquivalencyHash()
        {
            return GetType().Name.GetHashCode();
        }

        public int GetMergeHash()
        {
            return GetType().Name.GetHashCode();
        }

        public bool TryMerge(IPersistentDescription other, IExpression predicate, [MaybeNullWhen(false)] out IPersistentDescription merged)
        {
            merged = this;
            return other is InvalidDescription;
        }
    }
}
