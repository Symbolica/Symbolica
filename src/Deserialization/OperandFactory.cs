using System;
using System.Globalization;
using System.Linq;
using System.Numerics;
using LLVMSharp.Interop;
using Symbolica.Abstraction;
using Symbolica.Deserialization.Extensions;
using Symbolica.Expression;
using Symbolica.Representation;
using Symbolica.Representation.Operands;

namespace Symbolica.Deserialization
{
    internal sealed class OperandFactory : IOperandFactory
    {
        private readonly IIdFactory _idFactory;
        private readonly LLVMTargetDataRef _targetData;
        private readonly IUnsafeContext _unsafeContext;

        public OperandFactory(LLVMTargetDataRef targetData, IIdFactory idFactory, IUnsafeContext unsafeContext)
        {
            _targetData = targetData;
            _idFactory = idFactory;
            _unsafeContext = unsafeContext;
        }

        public IOperand Create(LLVMValueRef operand, IInstructionFactory instructionFactory)
        {
            return operand.Kind switch
            {
                LLVMValueKind.LLVMArgumentValueKind =>
                    new Argument(int.Parse(operand.GetValue()[1..])),
                LLVMValueKind.LLVMBasicBlockValueKind =>
                    new BlockLabel((BasicBlockId) _idFactory.GetOrCreate(operand.Handle)),
                LLVMValueKind.LLVMMemoryUseValueKind =>
                    new Unsupported("MemoryUse"),
                LLVMValueKind.LLVMMemoryDefValueKind =>
                    new Unsupported("MemoryDef"),
                LLVMValueKind.LLVMMemoryPhiValueKind =>
                    new Unsupported("MemoryPhi"),
                LLVMValueKind.LLVMFunctionValueKind =>
                    new Function((FunctionId) _idFactory.GetOrCreate(operand.Handle)),
                LLVMValueKind.LLVMGlobalAliasValueKind =>
                    new GlobalAlias(Create(_unsafeContext.GetAlias(operand), instructionFactory)),
                LLVMValueKind.LLVMGlobalIFuncValueKind =>
                    new Unsupported("ifunc"),
                LLVMValueKind.LLVMGlobalVariableValueKind =>
                    new GlobalVariable((GlobalId) _idFactory.GetOrCreate(operand.Handle)),
                LLVMValueKind.LLVMBlockAddressValueKind =>
                    new BlockLabel((BasicBlockId) _idFactory.GetOrCreate(operand.GetOperand(1U).Handle)),
                LLVMValueKind.LLVMConstantExprValueKind =>
                    new ConstantExpression(instructionFactory.Create(operand, operand.ConstOpcode)),
                LLVMValueKind.LLVMConstantArrayValueKind =>
                    CreateConstantAggregate(operand, instructionFactory),
                LLVMValueKind.LLVMConstantStructValueKind =>
                    CreateConstantStruct(operand, instructionFactory),
                LLVMValueKind.LLVMConstantVectorValueKind =>
                    CreateConstantAggregate(operand, instructionFactory),
                LLVMValueKind.LLVMUndefValueValueKind =>
                    new Undefined(operand.TypeOf.GetStoreSize(_targetData).ToBits()),
                LLVMValueKind.LLVMConstantAggregateZeroValueKind =>
                    new ConstantZero(operand.TypeOf.GetStoreSize(_targetData).ToBits()),
                LLVMValueKind.LLVMConstantDataArrayValueKind =>
                    CreateConstantData(operand, instructionFactory),
                LLVMValueKind.LLVMConstantDataVectorValueKind =>
                    CreateConstantData(operand, instructionFactory),
                LLVMValueKind.LLVMConstantIntValueKind =>
                    CreateConstantInteger(operand),
                LLVMValueKind.LLVMConstantFPValueKind =>
                    CreateConstantFloat(operand),
                LLVMValueKind.LLVMConstantPointerNullValueKind =>
                    new ConstantNull(),
                LLVMValueKind.LLVMConstantTokenNoneValueKind =>
                    new Unsupported("none"),
                LLVMValueKind.LLVMMetadataAsValueValueKind =>
                    new Metadata(),
                LLVMValueKind.LLVMInlineAsmValueKind =>
                    new Unsupported("asm"),
                LLVMValueKind.LLVMInstructionValueKind =>
                    new Variable((InstructionId) _idFactory.GetOrCreate(operand.Handle)),
                LLVMValueKind.LLVMPoisonValueValueKind =>
                    new Unsupported("poison"),
                _ =>
                    throw new Exception($"Operand type {operand.Kind} is unknown.")
            };
        }

        private IOperand CreateConstantStruct(LLVMValueRef operand, IInstructionFactory instructionFactory)
        {
            return new ConstantStruct(
                operand.TypeOf.GetStoreSize(_targetData).ToBits(),
                operand.GetOperands()
                    .Select((o, i) => new StructElement(
                        operand.TypeOf.GetElementOffset(_targetData, (uint) i).ToBits(),
                        Create(o, instructionFactory)))
                    .ToArray());
        }

        private IOperand CreateConstantAggregate(LLVMValueRef operand, IInstructionFactory instructionFactory)
        {
            return new ConstantSequence(
                operand.TypeOf.GetStoreSize(_targetData).ToBits(),
                operand.GetOperands()
                    .Select(o => Create(o, instructionFactory))
                    .ToArray());
        }

        private IOperand CreateConstantData(LLVMValueRef operand, IInstructionFactory instructionFactory)
        {
            return new ConstantSequence(
                operand.TypeOf.GetStoreSize(_targetData).ToBits(),
                operand.GetConstants()
                    .Select(o => Create(o, instructionFactory))
                    .ToArray());
        }

        private IOperand CreateConstantInteger(LLVMValueRef operand)
        {
            var size = operand.TypeOf.GetSize(_targetData);
            var value = operand.GetValue();

            return new ConstantInteger(
                size,
                size == Bits.One && bool.TryParse(value, out var boolean)
                    ? boolean
                        ? BigInteger.One
                        : BigInteger.Zero
                    : BigInteger.Parse(value));
        }

        private IOperand CreateConstantFloat(LLVMValueRef operand)
        {
            var size = operand.TypeOf.GetSize(_targetData);
            var value = operand.GetValue();

            return value[..2] == "0x"
                ? value[2] == 'H' || value[2] == 'K' || value[2] == 'L' || value[2] == 'M' || value[2] == 'R'
                    ? new ConstantInteger(size, BigInteger.Parse(value[3..], NumberStyles.HexNumber))
                    : new ConstantInteger(size, BigInteger.Parse(value[2..], NumberStyles.HexNumber))
                : new ConstantFloat(size, value);
        }
    }
}
