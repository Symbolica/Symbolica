using System.Numerics;
using Symbolica.Abstraction;

namespace Symbolica.Representation.StateActions
{
    public class TransferBasicBlock : IStateAction
    {
        private readonly BasicBlockId _successorId;

        public TransferBasicBlock(BasicBlockId successorId)
        {
            _successorId = successorId;
        }

        public Unit Run(IState state)
        {
            state.Stack.TransferBasicBlock(_successorId);
            return new Unit();
        }
    }

    public class TransferBasicBlockOfId : IFunc<BigInteger, IStateAction>
    {
        public IStateAction Run(BigInteger value) =>
            new StateActions.TransferBasicBlock((BasicBlockId)(ulong)value);
    }
}
