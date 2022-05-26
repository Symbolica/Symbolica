﻿using System;
using System.Collections.Generic;
using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Collection;
using Symbolica.Expression;
using Symbolica.Implementation.Exceptions;

namespace Symbolica.Implementation.Stack;

internal sealed class PersistentVariables : IPersistentVariables
{
    private readonly IPersistentDictionary<InstructionId, IExpression> _incomingVariables;
    private readonly IPersistentDictionary<InstructionId, IExpression> _variables;
    private readonly Lazy<int> _equivalencyHash;
    private readonly Lazy<int> _mergeHash;

    private PersistentVariables(IPersistentDictionary<InstructionId, IExpression> incomingVariables,
        IPersistentDictionary<InstructionId, IExpression> variables)
    {
        _incomingVariables = incomingVariables;
        _variables = variables;
        _equivalencyHash = new(() => EquivalencyHash(false));
        _mergeHash = new(() => EquivalencyHash(true));

        int EquivalencyHash(bool includeSubs)
        {
            int GetVariablesHash(IEnumerable<KeyValuePair<InstructionId, IExpression>> variables)
            {
                var hashCode = new HashCode();
                foreach (var variable in variables)
                    hashCode.Add(HashCode.Combine(
                        variable.Key.GetEquivalencyHash(includeSubs),
                        variable.Value.GetEquivalencyHash(includeSubs)));
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
        return new PersistentVariables(_incomingVariables,
            _variables.SetItem(instructionId, variable));
    }

    public IPersistentVariables TransferBasicBlock()
    {
        return new PersistentVariables(_variables, _variables);
    }

    public static IPersistentVariables Create(ICollectionFactory collectionFactory)
    {
        var variables = collectionFactory.CreatePersistentDictionary<InstructionId, IExpression>();
        return new PersistentVariables(variables, variables);
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

    public int GetEquivalencyHash(bool includeSubs)
    {
        return includeSubs
            ? _mergeHash.Value
            : _equivalencyHash.Value;
    }
}
