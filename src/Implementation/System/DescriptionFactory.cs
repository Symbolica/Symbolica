using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Implementation.System;

internal sealed class DescriptionFactory : IDescriptionFactory
{
    private readonly IFileSystem _fileSystem;
    private readonly ulong _symbolicFileSize;

    public DescriptionFactory(IFileSystem fileSystem, ulong symbolicFileSize)
    {
        _fileSystem = fileSystem;
        _symbolicFileSize = symbolicFileSize;
    }

    public IPersistentDescription? Create(ISpace space, IMemory memory, IExpression path)
    {
        IEnumerable<char> ReadCharacters()
        {
            while (true)
            {
                var character = (char) memory.Read(path, Bytes.One.ToBits()).Constant;
                if (character == default)
                    yield break;

                yield return character;
                path = path.Add(space.CreateConstant(path.Size, (uint) Bytes.One));
            }
        }
        try
        {
            var concretePath = new string(ReadCharacters().ToArray());
            return CreateFile(concretePath)
                ?? CreateDirectory(concretePath);
        }
        catch (SymbolicaException)
        {
            return SymbolicDescription.Create(_symbolicFileSize);
        }
    }

    public IPersistentDescription CreateInput()
    {
        return new InputDescription();
    }

    public IPersistentDescription CreateOutput()
    {
        return new OutputDescription();
    }

    public IPersistentDescription CreateInvalid()
    {
        return InvalidDescription.Instance;
    }

    private IPersistentDescription? CreateFile(string path)
    {
        var file = _fileSystem.GetFile(path);

        return file == null
            ? null
            : FileDescription.Create(file);
    }

    private IPersistentDescription? CreateDirectory(string path)
    {
        var directory = _fileSystem.GetDirectory(path);

        return directory == null
            ? null
            : new DirectoryDescription(directory);
    }

    private sealed class InvalidDescription : IPersistentDescription
    {
        private InvalidDescription()
        {
        }

        public static IPersistentDescription Instance => new InvalidDescription();

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
            return space.CreateConstant(space.PointerSize, BigInteger.Zero);
        }

        public int GetStatus(ISpace space, IMemory memory, IStruct stat, IExpression address)
        {
            return -1;
        }
    }
}
