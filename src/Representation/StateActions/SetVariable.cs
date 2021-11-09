using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.StateActions
{
    public class SetVariable : IStateAction
    {
        private readonly InstructionId _id;
        private readonly IFunc<IState, IExpression> _createVariable;

        public SetVariable(InstructionId id, IFunc<IState, IExpression> createVariable)
        {
            _id = id;
            _createVariable = createVariable;
        }

        public SetVariable(InstructionId id, IExpression variable)
            : this(id, new NoOp<IState, IExpression>(variable))
        {
        }

        public Unit Run(IState state)
        {
            state.Stack.SetVariable(_id, _createVariable.Run(state));
            return new Unit();
        }
    }

    public class SetVariableFromFunc : IFunc<IFunc<IState, IExpression>, IStateAction>
    {
        private readonly InstructionId _id;

        public SetVariableFromFunc(InstructionId id)
        {
            _id = id;
        }

        public IStateAction Run(IFunc<IState, IExpression> createVar)
        {
            return new SetVariable(_id, createVar);
        }
    }
}
