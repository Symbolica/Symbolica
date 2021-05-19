using System;
using System.Collections.Generic;
using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Collection;
using Symbolica.Expression;
using Symbolica.Implementation.Memory;

namespace Symbolica.Implementation
{
    internal sealed class PersistentGlobals : IPersistentGlobals
    {
        private readonly IPersistentDictionary<GlobalId, IExpression> _addresses;
        private readonly IReadOnlyDictionary<GlobalId, IGlobal> _globals;

        private PersistentGlobals(IReadOnlyDictionary<GlobalId, IGlobal> globals,
            IPersistentDictionary<GlobalId, IExpression> addresses)
        {
            _globals = globals;
            _addresses = addresses;
        }

        public (IExpression, Action<IState>, IPersistentGlobals) GetAddress(IMemoryProxy memory, GlobalId globalId)
        {
            return _addresses.TryGetValue(globalId, out var address)
                ? (address, _ => { }, this)
                : Allocate(memory, _globals.TryGetValue(globalId, out var global)
                    ? global
                    : throw new Exception("Global was not found."));
        }

        private (IExpression, Action<IState>, IPersistentGlobals) Allocate(IMemoryProxy memory, IGlobal global)
        {
            var address = memory.Allocate(Section.Global, global.Size);

            return (address, s => Initialize(s, address, global), new PersistentGlobals(_globals,
                _addresses.SetItem(global.Id, address)));
        }

        private static void Initialize(IState state, IExpression address, IGlobal global)
        {
            if (global.Initializer != null)
                state.Memory.Write(address, global.Initializer.Evaluate(state));
        }

        public static IPersistentGlobals Create(IEnumerable<IGlobal> globals, ICollectionFactory collectionFactory)
        {
            return new PersistentGlobals(globals.ToDictionary(g => g.Id),
                collectionFactory.CreatePersistentDictionary<GlobalId, IExpression>());
        }
    }
}