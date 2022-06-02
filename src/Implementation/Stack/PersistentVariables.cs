using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Collection;
using Symbolica.Expression;
using Symbolica.Implementation.Exceptions;

namespace Symbolica.Implementation.Stack;

internal sealed class PersistentVariables : IPersistentVariables
{
    private readonly ICollectionFactory _collectionFactory;
    private readonly IPersistentDictionary<InstructionId, IExpression> _incomingVariables;
    private readonly IPersistentDictionary<InstructionId, IExpression> _variables;
    private readonly Lazy<int> _equivalencyHash;
    private readonly Lazy<int> _mergeHash;

    private PersistentVariables(
        ICollectionFactory collectionFactory,
        IPersistentDictionary<InstructionId, IExpression> incomingVariables,
        IPersistentDictionary<InstructionId, IExpression> variables)
    {
        _collectionFactory = collectionFactory;
        _incomingVariables = incomingVariables;
        _variables = variables;
        _equivalencyHash = new(() => CreateHash(k => k.GetEquivalencyHash(), v => v.GetEquivalencyHash()));
        _mergeHash = new(() => CreateHash(k => k.GetMergeHash(), v => v.GetMergeHash()));

        int CreateHash(Func<InstructionId, int> createKeyHash, Func<IExpression, int> createValueHash)
        {
            int GetVariablesHash(IEnumerable<KeyValuePair<InstructionId, IExpression>> variables)
            {
                var hashCode = new HashCode();
                foreach (var variable in variables)
                    hashCode.Add(HashCode.Combine(
                        createKeyHash(variable.Key),
                        createValueHash(variable.Value)));
                return hashCode.ToHashCode();
            }

            return HashCode.Combine(
                GetVariablesHash(_incomingVariables),
                GetVariablesHash(_variables));
        }
    }

    public IExpression Get(InstructionId id, bool useIncomingValue)
    {
        var variables = useIncomingValue
            ? _incomingVariables
            : _variables;

        return variables.TryGetValue(id, out var variable)
            ? variable
            : throw new UndefinedVariableException(id);
    }

    public IPersistentVariables Set(InstructionId instructionId, IExpression variable)
    {
        return new PersistentVariables(
            _collectionFactory,
            _incomingVariables,
            _variables.SetItem(instructionId, variable));
    }

    public IPersistentVariables TransferBasicBlock()
    {
        return new PersistentVariables(_collectionFactory, _variables, _variables);
    }

    public static IPersistentVariables Create(ICollectionFactory collectionFactory)
    {
        var variables = collectionFactory.CreatePersistentDictionary<InstructionId, IExpression>();
        return new PersistentVariables(collectionFactory, variables, variables);
    }

    public (HashSet<ExpressionSubs> subs, bool) IsEquivalentTo(
        IPersistentVariables other)
    {
        static (HashSet<ExpressionSubs> subs, bool) IsVariableEquivalent(
            KeyValuePair<InstructionId, IExpression> x,
            KeyValuePair<InstructionId, IExpression> y)
        {
            return x.Key.IsEquivalentTo(y.Key)
                .And(x.Value.IsEquivalentTo(y.Value).ToHashSet());
        }

        return other is PersistentVariables pv
            ? _incomingVariables.IsSequenceEquivalentTo(pv._incomingVariables, IsVariableEquivalent)
                .And(_variables.IsSequenceEquivalentTo(pv._variables, IsVariableEquivalent))
            : (new(), false);
    }

    public object ToJson()
    {
        return new
        {
            IncomingVariables = _incomingVariables.ToDictionary(v => v.Key.ToJson(), v => v.Value.ToJson()),
            Variables = _variables.ToDictionary(v => v.Key.ToJson(), v => v.Value.ToJson())
        };
    }

    public int GetEquivalencyHash()
    {
        return _equivalencyHash.Value;
    }

    public int GetMergeHash()
    {
        return _mergeHash.Value;
    }

    public bool TryMerge(IPersistentVariables other, IExpression predicate, [MaybeNullWhen(false)] out IPersistentVariables merged)
    {
        if (other is PersistentVariables pv
            && _incomingVariables.TryMerge(pv._incomingVariables, predicate, out var mergedIncomingVariables)
            && _variables.TryMerge(pv._variables, predicate, out var mergedVariables))
        {
            merged = new PersistentVariables(
                _collectionFactory,
                _collectionFactory.CreatePersistentDictionary(mergedIncomingVariables),
                _collectionFactory.CreatePersistentDictionary(mergedVariables));
            return true;
        }

        merged = null;
        return false;
    }
}
