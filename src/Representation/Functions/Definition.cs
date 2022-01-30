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

    public void Call(IState state, ICaller caller, IArguments arguments)
    {
        state.Stack.Wind(caller, _isVariadic
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
}
