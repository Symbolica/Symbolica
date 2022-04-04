using System;
using Symbolica.Abstraction;
using Symbolica.Collection;
using Symbolica.Expression.Values;
using Symbolica.Implementation.Memory;

namespace Symbolica.Implementation;

internal sealed class PersistentGlobals : IPersistentGlobals
{
    private readonly IPersistentDictionary<GlobalId, Address> _addresses;
    private readonly IModule _module;

    private PersistentGlobals(IModule module,
        IPersistentDictionary<GlobalId, Address> addresses)
    {
        _module = module;
        _addresses = addresses;
    }

    public (Address, Action<IState>, IPersistentGlobals) GetAddress(IMemoryProxy memory, GlobalId id)
    {
        return _addresses.TryGetValue(id, out var address)
            ? (address, _ => { }, this)
            : Allocate(memory, _module.GetGlobal(id));
    }

    private (Address, Action<IState>, IPersistentGlobals) Allocate(IMemoryProxy memory, IGlobal global)
    {
        var address = memory.Allocate(Section.Global, global.Size);

        return (address, s => global.Initialize(s, address), new PersistentGlobals(_module,
            _addresses.SetItem(global.Id, address)));
    }

    public static IPersistentGlobals Create(IModule module, ICollectionFactory collectionFactory)
    {
        return new PersistentGlobals(module,
            collectionFactory.CreatePersistentDictionary<GlobalId, Address>());
    }
}
