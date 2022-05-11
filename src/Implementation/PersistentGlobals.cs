using System;
using Symbolica.Abstraction;
using Symbolica.Collection;
using Symbolica.Expression;
using Symbolica.Implementation.Memory;

namespace Symbolica.Implementation;

internal sealed class PersistentGlobals : IPersistentGlobals
{
    private readonly IPersistentDictionary<GlobalId, IExpression> _addresses;
    private readonly IModule _module;
    private readonly IExpressionFactory _exprFactory;

    private PersistentGlobals(IModule module, IExpressionFactory exprFactory,
        IPersistentDictionary<GlobalId, IExpression> addresses)
    {
        _module = module;
        _exprFactory = exprFactory;
        _addresses = addresses;
    }

    public (IExpression, Action<IState>, IPersistentGlobals) GetAddress(IMemoryProxy memory, GlobalId id)
    {
        return _addresses.TryGetValue(id, out var address)
            ? (address, _ => { }, this)
            : Allocate(memory, _module.GetGlobal(id));
    }

    private (IExpression, Action<IState>, IPersistentGlobals) Allocate(IMemoryProxy memory, IGlobal global)
    {
        var address = memory.Allocate(Section.Global, global.Size);

        return (address,
            s => global.Initialize(_exprFactory, s, address),
            new PersistentGlobals(_module, _exprFactory, _addresses.SetItem(global.Id, address)));
    }

    public static IPersistentGlobals Create(IModule module, ICollectionFactory collectionFactory, IExpressionFactory exprFactory)
    {
        return new PersistentGlobals(module, exprFactory,
            collectionFactory.CreatePersistentDictionary<GlobalId, IExpression>());
    }
}
