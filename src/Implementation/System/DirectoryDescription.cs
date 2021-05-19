using System;
using System.Numerics;
using System.Text;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Implementation.System
{
    internal sealed class DirectoryDescription : IPersistentDescription
    {
        private readonly IDirectory _directory;
        private readonly Lazy<string[]> _names;

        public DirectoryDescription(IDirectory directory)
        {
            _directory = directory;
            _names = new Lazy<string[]>(_directory.GetNames);
        }

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
            return tell >= 0 && tell < _names.Value.Length
                ? Read(space, memory, entry, address, _names.Value[tell])
                : space.CreateConstant(space.PointerSize, BigInteger.Zero);
        }

        public int GetStatus(ISpace space, IMemory memory, IStruct stat, IExpression address)
        {
            var type = Convert.ToInt32("0040000", 8);
            var mode = Convert.ToInt32("00444", 8);

            memory.Write(address, stat
                .Write(space, 3, type | mode)
                .Write(space, 8, 0L)
                .Write(space, 11, _directory.LastAccessTime)
                .Write(space, 12, _directory.LastModifiedTime)
                .Write(space, 13, _directory.LastModifiedTime)
                .Expression);

            return 0;
        }

        private static IExpression Read(ISpace space, IMemory memory, IStruct entry, IExpression address, string name)
        {
            memory.Write(address, entry
                .Write(space, 4, new BigInteger(Encoding.UTF8.GetBytes(name), true))
                .Expression);

            return address;
        }
    }
}