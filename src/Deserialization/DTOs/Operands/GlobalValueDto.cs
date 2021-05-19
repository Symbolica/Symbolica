using Symbolica.Abstraction;
using Symbolica.Representation.Operands;

namespace Symbolica.Deserialization.DTOs.Operands
{
    internal sealed record GlobalValueDto(
            string Type, ulong Id)
        : OperandDto(Type)
    {
        public IOperand ToFunction()
        {
            return new Function((FunctionId) Id);
        }

        public IOperand ToGlobalVariable()
        {
            return new GlobalVariable((GlobalId) Id);
        }
    }
}