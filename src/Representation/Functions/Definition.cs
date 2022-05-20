using System;
using System.Collections.Generic;
using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Representation.Exceptions;

namespace Symbolica.Representation.Functions;

public sealed class Definition : IDefinition
{
    private readonly IReadOnlyDictionary<BasicBlockId, IBasicBlock> _basicBlocks;
    private readonly BasicBlockId _entryId;
    private readonly bool _isVariadic;
    private readonly Lazy<int> _equivalencyHash;

    private Definition(
        FunctionId id,
        string name,
        IParameters parameters,
        bool isVariadic,
        BasicBlockId entryId,
        IReadOnlyDictionary<BasicBlockId, IBasicBlock> basicBlocks)
    {
        Id = id;
        Name = name;
        Parameters = parameters;
        _isVariadic = isVariadic;
        _entryId = entryId;
        _basicBlocks = basicBlocks;
        _equivalencyHash = new(() =>
        {
            var blocksHash = new HashCode();
            foreach (var block in _basicBlocks)
                blocksHash.Add(HashCode.Combine(block.Key.GetEquivalencyHash(), block.Value.GetEquivalencyHash()));
            return HashCode.Combine(
                blocksHash.ToHashCode(),
                _entryId.GetEquivalencyHash(),
                _isVariadic,
                Id.GetEquivalencyHash(),
                Name,
                Parameters.GetEquivalencyHash());
        });
    }

    public FunctionId Id { get; }
    public IParameters Parameters { get; }
    public string Name { get; }
    public IBasicBlock Entry => GetBasicBlock(_entryId);

    public IBasicBlock GetBasicBlock(BasicBlockId id)
    {
        return _basicBlocks.TryGetValue(id, out var basicBlock)
            ? basicBlock
            : throw new MissingBasicBlockException(id);
    }

    public void Call(IExpressionFactory exprFactory, IState state, ICaller caller, IArguments arguments)
    {
        state.Stack.Wind(state.Space, state.Memory, caller, _isVariadic
            ? new Invocation(this,
                new Arguments(arguments.Take(Parameters.Count).ToArray()),
                new Arguments(arguments.Skip(Parameters.Count).ToArray()))
            : new Invocation(this,
                arguments,
                new Arguments(Array.Empty<IExpression>())));
    }

    public static IFunction Create(
        FunctionId id,
        string name,
        IParameters parameters,
        bool isVariadic,
        BasicBlockId entryId,
        IEnumerable<IBasicBlock> basicBlocks)
    {
        return new Definition(
            id,
            name,
            parameters,
            isVariadic,
            entryId,
            basicBlocks.ToDictionary(b => b.Id));
    }

    public (HashSet<(IExpression, IExpression)> subs, bool) IsEquivalentTo(IDefinition other)
    {
        return other is Definition d
            ? _basicBlocks.IsSequenceEquivalentTo<IExpression, BasicBlockId, IBasicBlock>(d._basicBlocks)
                .And(_entryId.IsEquivalentTo(d._entryId))
                .And((new(), _isVariadic == d._isVariadic))
                .And(Id.IsEquivalentTo(d.Id))
                .And((new(), Name == d.Name))
                .And(Parameters.IsEquivalentTo(d.Parameters))
            : (new(), false);
    }

    public object ToJson()
    {
        return new
        {
            BasicBlocks = _basicBlocks.ToDictionary(b => b.Key.ToJson(), b => b.Value.ToJson()),
            EntryId = _entryId.ToJson(),
            Id = Id.ToJson(),
            IsVariadic = _isVariadic,
            Name,
            Parameters = Parameters.ToJson()
        };
    }

    public int GetEquivalencyHash()
    {
        return _equivalencyHash.Value;
    }
}
