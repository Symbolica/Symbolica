using System;
using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Implementation.System
{
    [Serializable]
    internal sealed class DescriptionFactory : IDescriptionFactory
    {
        private readonly IFileSystem _fileSystem;

        public DescriptionFactory(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public IPersistentDescription? Create(string path)
        {
            return CreateFile(path)
                   ?? CreateDirectory(path);
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
}
