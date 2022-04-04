using System.Collections.Generic;
using Symbolica.Expression;
using Symbolica.Expression.Values;

namespace Symbolica.Abstraction;

public interface IStack
{
    bool IsInitialFrame { get; }
    BasicBlockId PredecessorId { get; }
    IEnumerable<string> Trace { get; }

    void Wind(ICaller caller, IInvocation invocation);
    ICaller Unwind();
    void Save(Address address, bool useJumpBuffer);
    InstructionId Restore(Address address, bool useJumpBuffer);
    void TransferBasicBlock(BasicBlockId id);
    IExpression<IType> GetFormal(int index);
    IExpression<IType> GetInitializedVaList();
    IExpression<IType> GetVariable(InstructionId id, bool useIncomingValue);
    void SetVariable(InstructionId id, IExpression<IType> variable);
    Address Allocate(Bits size);
}
