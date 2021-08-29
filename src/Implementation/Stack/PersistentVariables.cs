using Symbolica.Abstraction;
using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Implementation.Stack
{
    internal sealed class PersistentVariables : IPersistentVariables
    {
        private readonly IPersistentDictionary<InstructionId, IExpression> _incomingVariables;
        private readonly IPersistentDictionary<InstructionId, IExpression> _variables;

        private PersistentVariables(IPersistentDictionary<InstructionId, IExpression> incomingVariables,
            IPersistentDictionary<InstructionId, IExpression> variables)
        {
            _incomingVariables = incomingVariables;
            _variables = variables;
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
            return new PersistentVariables(_variables,
                _variables);
        }

        public static IPersistentVariables Create(ICollectionFactory collectionFactory)
        {
            var variables = collectionFactory.CreatePersistentDictionary<InstructionId, IExpression>();

            return new PersistentVariables(variables,
                variables);
        }
    }
}
