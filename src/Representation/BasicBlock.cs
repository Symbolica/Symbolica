using System.Collections.Generic;
using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation;

public sealed class BasicBlock : IBasicBlock
{
    private readonly IInstruction[] _instructions;

    public BasicBlock(BasicBlockId id, IInstruction[] instructions)
    {
        Id = id;
        _instructions = instructions;
    }

    public BasicBlockId Id { get; }

    public IInstruction GetInstruction(int index)
    {
        return _instructions[index];
    }

    public (HashSet<(IExpression, IExpression)> subs, bool) IsEquivalentTo(IBasicBlock other)
    {
        return other is BasicBlock b
            ? Id.IsEquivalentTo(b.Id)
                .And(_instructions.IsSequenceEquivalentTo<IExpression, IInstruction>(b._instructions))
            : (new(), false);
    }

    public object ToJson()
    {
        return new
        {
            Id = Id.ToJson(),
            Instructions = _instructions.Select(i => i.ToJson()).ToArray()
        };
    }
}
