using System.Collections.Generic;
using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Implementation.Exceptions;

namespace Symbolica.Implementation.Stack;

internal sealed class PersistentProgramCounter : IPersistentProgramCounter
{
    private readonly IBasicBlock _basicBlock;
    private readonly IDefinition _definition;
    private readonly int _index;
    private readonly BasicBlockId? _predecessorId;

    private PersistentProgramCounter(IDefinition definition,
        IBasicBlock basicBlock, BasicBlockId? predecessorId, int index)
    {
        _definition = definition;
        _basicBlock = basicBlock;
        _predecessorId = predecessorId;
        _index = index;
    }

    public BasicBlockId PredecessorId => _predecessorId ?? throw new UndefinedPredecessorException();

    public IInstruction Instruction => _basicBlock.GetInstruction(_index);

    public IPersistentProgramCounter TransferBasicBlock(BasicBlockId id)
    {
        return new PersistentProgramCounter(_definition,
            _definition.GetBasicBlock(id), _basicBlock.Id, -1);
    }

    public IPersistentProgramCounter MoveNextInstruction()
    {
        return new PersistentProgramCounter(_definition,
            _basicBlock, _predecessorId, _index + 1);
    }

    public static IPersistentProgramCounter Create(IDefinition definition)
    {
        return new PersistentProgramCounter(definition,
            definition.Entry, null, -1);
    }

    public (HashSet<(IExpression, IExpression)> subs, bool) IsEquivalentTo(
            IPersistentProgramCounter other)
    {
        return other is PersistentProgramCounter ppc
            ? _basicBlock.IsEquivalentTo(ppc._basicBlock)
                .And(_definition.IsEquivalentTo(ppc._definition))
                .And((new(), _index == ppc._index))
                .And(Mergeable.IsNullableEquivalentTo<IExpression, BasicBlockId>(_predecessorId, ppc._predecessorId))
            : (new(), false);
    }

    public object ToJson()
    {
        return new
        {
            BasicBlock = _basicBlock.ToJson(),
            Definition = _definition.ToJson(),
            Index = _index,
            PredecessorId = _predecessorId?.ToJson()
        };
    }
}
