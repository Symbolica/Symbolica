﻿using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Collection;
using Symbolica.Expression;
using Symbolica.Implementation.Memory;

namespace Symbolica.Implementation.System
{
    internal sealed class PersistentSystem : IPersistentSystem
    {
        private readonly IDescriptionFactory _descriptionFactory;
        private readonly IPersistentList<Handle> _handles;
        private readonly IPersistentList<int> _indices;
        private readonly IStructTypes _structTypes;
        private readonly IExpression? _threadAddress;

        private PersistentSystem(IStructTypes structTypes, IDescriptionFactory descriptionFactory,
            IExpression? threadAddress, IPersistentList<int> indices, IPersistentList<Handle> handles)
        {
            _structTypes = structTypes;
            _descriptionFactory = descriptionFactory;
            _threadAddress = threadAddress;
            _indices = indices;
            _handles = handles;
        }

        public (IExpression, IPersistentSystem) GetThreadAddress(ISpace space, IMemoryProxy memory)
        {
            return _threadAddress == null
                ? AllocateThread(space, memory)
                : (_threadAddress, this);
        }

        public (int, IPersistentSystem) Open(string path)
        {
            var description = _descriptionFactory.Create(path);

            return description == null
                ? (-1, this)
                : Open(description);
        }

        public (int, IPersistentSystem) Duplicate(int descriptor)
        {
            var (index, handle) = Get(descriptor);

            return handle.References == 0U
                ? (-1, this)
                : Duplicate(index, handle.References, handle.Description);
        }

        public (int, IPersistentSystem) Close(int descriptor)
        {
            var (index, handle) = Get(descriptor);

            return handle.References == 0U
                ? (-1, this)
                : (0, Close(descriptor, index, handle.References, handle.Description));
        }

        public (long, IPersistentSystem) Seek(int descriptor, long offset, uint whence)
        {
            var (index, handle) = Get(descriptor);

            var (result, description) = handle.Description.Seek(offset, whence);

            return (result, new PersistentSystem(_structTypes, _descriptionFactory,
                _threadAddress, _indices, _handles.SetItem(index, new Handle(handle.References, description))));
        }

        public int Read(ISpace space, IMemory memory, int descriptor, IExpression address, int count)
        {
            var (_, handle) = Get(descriptor);

            return handle.Description.Read(space, memory, address, count);
        }

        public IExpression ReadDirectory(ISpace space, IMemory memory, IExpression address)
        {
            var directoryType = _structTypes.Get("struct.__dirstream");
            var directory = directoryType.Create(memory.Read(address, directoryType.Size));

            var tell = (int) directory.Read(space, 0).Integer;
            var descriptor = (int) directory.Read(space, 1).Integer;
            var buffer = address.Add(space.CreateConstant(address.Size, (uint) directoryType.GetOffset(5).ToBytes()));

            memory.Write(address, directory
                .Write(space, 0, tell + 1)
                .Expression);

            var entryType = _structTypes.Get("struct.dirent");
            var entry = entryType.Create(space.CreateGarbage(entryType.Size));

            var (_, handle) = Get(descriptor);

            return handle.Description.ReadDirectory(space, memory, entry, buffer, tell);
        }

        public int GetStatus(ISpace space, IMemory memory, int descriptor, IExpression address)
        {
            var statType = _structTypes.Get("struct.stat");
            var stat = statType.Create(space.CreateGarbage(statType.Size));

            var (_, handle) = Get(descriptor);

            return handle.Description.GetStatus(space, memory, stat, address);
        }

        private (IExpression, IPersistentSystem) AllocateThread(ISpace space, IMemoryProxy memory)
        {
            var localeType = _structTypes.Get("struct.__locale_struct");
            var locale = localeType.Create(space.CreateGarbage(localeType.Size));

            var localeAddress = memory.Allocate(Section.Global, locale.Expression.Size);
            memory.Write(localeAddress, locale.Expression);

            var threadType = _structTypes.Get("struct.__pthread");
            var thread = threadType.Create(space.CreateGarbage(threadType.Size))
                .Write(space, 24, localeAddress);

            var threadAddress = memory.Allocate(Section.Global, thread.Expression.Size);
            memory.Write(threadAddress, thread.Expression);

            return (threadAddress, new PersistentSystem(_structTypes, _descriptionFactory,
                threadAddress, _indices, _handles));
        }

        private (int, Handle) Get(int descriptor)
        {
            var index = descriptor >= 0 && descriptor < _indices.Count
                ? _indices.Get(descriptor)
                : 0;

            return (index, _handles.Get(index));
        }

        private (int, PersistentSystem) Open(IPersistentDescription description)
        {
            var handle = new Handle(1U, description);

            foreach (var (value, descriptor) in _indices.Select((v, d) => (v, d)))
                if (value == 0)
                    return (descriptor, new PersistentSystem(_structTypes, _descriptionFactory,
                        _threadAddress, _indices.SetItem(descriptor, _handles.Count), _handles.Add(handle)));

            return (_indices.Count, new PersistentSystem(_structTypes, _descriptionFactory,
                _threadAddress, _indices.Add(_handles.Count), _handles.Add(handle)));
        }

        private (int, IPersistentSystem) Duplicate(int index, uint references, IPersistentDescription description)
        {
            var handle = new Handle(references + 1U, description);

            foreach (var (value, descriptor) in _indices.Select((v, d) => (v, d)))
                if (value == 0)
                    return (descriptor, new PersistentSystem(_structTypes, _descriptionFactory,
                        _threadAddress, _indices.SetItem(descriptor, index), _handles.SetItem(index, handle)));

            return (_indices.Count, new PersistentSystem(_structTypes, _descriptionFactory,
                _threadAddress, _indices.Add(index), _handles.SetItem(index, handle)));
        }

        private IPersistentSystem Close(int descriptor, int index, uint references, IPersistentDescription description)
        {
            var handle = new Handle(references - 1U, references == 1U
                ? _descriptionFactory.CreateInvalid()
                : description);

            return new PersistentSystem(_structTypes, _descriptionFactory,
                _threadAddress, _indices.SetItem(descriptor, 0), _handles.SetItem(index, handle));
        }

        private PersistentSystem Add(IPersistentDescription description)
        {
            var (_, system) = Open(description);

            return system;
        }

        public static IPersistentSystem Create(IStructTypes structTypes, IDescriptionFactory descriptionFactory,
            ICollectionFactory collectionFactory)
        {
            var invalidHandle = new Handle(0U, descriptionFactory.CreateInvalid());

            var system = new PersistentSystem(structTypes, descriptionFactory,
                null,
                collectionFactory.CreatePersistentList<int>(),
                collectionFactory.CreatePersistentList<Handle>().Add(invalidHandle));

            return system
                .Add(descriptionFactory.CreateInput())
                .Add(descriptionFactory.CreateOutput())
                .Add(descriptionFactory.CreateOutput());
        }

        private readonly struct Handle
        {
            public Handle(uint references, IPersistentDescription description)
            {
                References = references;
                Description = description;
            }

            public uint References { get; }
            public IPersistentDescription Description { get; }
        }
    }
}