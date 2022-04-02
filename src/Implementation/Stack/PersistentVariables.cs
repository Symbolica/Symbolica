using Symbolica.Abstraction;
using Symbolica.Collection;
using Symbolica.Expression;
using Symbolica.Implementation.Exceptions;

namespace Symbolica.Implementation.Stack;

internal sealed class PersistentVariables : IPersistentVariables
{
    private readonly IPersistentDictionary<InstructionId, IExpression<IType>> _incomingVariables;
    private readonly IPersistentDictionary<InstructionId, IExpression<IType>> _variables;

    private PersistentVariables(IPersistentDictionary<InstructionId, IExpression<IType>> incomingVariables,
        IPersistentDictionary<InstructionId, IExpression<IType>> variables)
    {
        _incomingVariables = incomingVariables;
        _variables = variables;
    }

    public IExpression<IType> Get(InstructionId id, bool useIncomingValue)
    {
        var variables = useIncomingValue
            ? _incomingVariables
            : _variables;

        return variables.TryGetValue(id, out var variable)
            ? variable
            : throw new UndefinedVariableException(id);
    }

    public IPersistentVariables Set(InstructionId instructionId, IExpression<IType> variable)
    {
        return new PersistentVariables(_incomingVariables,
            _variables.SetItem(instructionId, variable));
    }

    public IPersistentVariables TransferBasicBlock()
    {
        return new PersistentVariables(_variables,
            _variables);
    }

    public static IPersistentVariables Create(ICollectionFactory collectionFactory)
    {
        var variables = collectionFactory.CreatePersistentDictionary<InstructionId, IExpression<IType>>();

        return new PersistentVariables(variables,
            variables);
    }
}
