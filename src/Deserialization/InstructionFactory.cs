using System;
using System.Collections.Generic;
using System.Linq;
using LLVMSharp.Interop;
using Symbolica.Abstraction;
using Symbolica.Deserialization.Extensions;
using Symbolica.Expression;
using Symbolica.Representation;
using Symbolica.Representation.Instructions;

namespace Symbolica.Deserialization
{
    internal sealed class InstructionFactory : IInstructionFactory
    {
        private readonly IIdFactory _idFactory;
        private readonly IOperandFactory _operandFactory;
        private readonly LLVMTargetDataRef _targetData;
        private readonly IUnsafeContext _unsafeContext;

        public InstructionFactory(LLVMTargetDataRef targetData, IIdFactory idFactory, IUnsafeContext unsafeContext,
            IOperandFactory operandFactory)
        {
            _targetData = targetData;
            _idFactory = idFactory;
            _unsafeContext = unsafeContext;
            _operandFactory = operandFactory;
        }

        public IInstruction Create(LLVMValueRef instruction, LLVMOpcode opcode)
        {
            var id = (InstructionId) _idFactory.GetOrCreate(instruction.Handle);
            var operands = instruction.GetOperands()
                .Select(o => _operandFactory.Create(o, this))
                .ToArray();

            return opcode switch
            {
                LLVMOpcode.LLVMRet => new Return(id, operands),
                LLVMOpcode.LLVMBr => new Branch(id, operands),
                LLVMOpcode.LLVMSwitch => new Switch(id, operands),
                LLVMOpcode.LLVMIndirectBr => new IndirectBranch(id, operands),
                LLVMOpcode.LLVMInvoke => new Invoke(
                    CreateCall(id, operands, instruction),
                    (BasicBlockId) _idFactory.GetOrCreate(instruction.GetSuccessor(0U).Handle)),
                LLVMOpcode.LLVMUnreachable => new Unsupported(id, "unreachable"),
                LLVMOpcode.LLVMCallBr => new Unsupported(id, "callbr"),
                LLVMOpcode.LLVMFNeg => new FloatNegate(id, operands),
                LLVMOpcode.LLVMAdd => new Add(id, operands),
                LLVMOpcode.LLVMFAdd => new FloatAdd(id, operands),
                LLVMOpcode.LLVMSub => new Subtract(id, operands),
                LLVMOpcode.LLVMFSub => new FloatSubtract(id, operands),
                LLVMOpcode.LLVMMul => new Multiply(id, operands),
                LLVMOpcode.LLVMFMul => new FloatMultiply(id, operands),
                LLVMOpcode.LLVMUDiv => new UnsignedDivide(id, operands),
                LLVMOpcode.LLVMSDiv => new SignedDivide(id, operands),
                LLVMOpcode.LLVMFDiv => new FloatDivide(id, operands),
                LLVMOpcode.LLVMURem => new UnsignedRemainder(id, operands),
                LLVMOpcode.LLVMSRem => new SignedRemainder(id, operands),
                LLVMOpcode.LLVMFRem => new FloatRemainder(id, operands),
                LLVMOpcode.LLVMShl => new ShiftLeft(id, operands),
                LLVMOpcode.LLVMLShr => new LogicalShiftRight(id, operands),
                LLVMOpcode.LLVMAShr => new ArithmeticShiftRight(id, operands),
                LLVMOpcode.LLVMAnd => new And(id, operands),
                LLVMOpcode.LLVMOr => new Or(id, operands),
                LLVMOpcode.LLVMXor => new Xor(id, operands),
                LLVMOpcode.LLVMAlloca => new Allocate(
                    id,
                    operands,
                    _unsafeContext.GetAllocatedType(instruction).GetAllocSize(_targetData).ToBits()),
                LLVMOpcode.LLVMLoad => new Load(id, operands, instruction.TypeOf.GetSize(_targetData)),
                LLVMOpcode.LLVMStore => new Store(id, operands),
                LLVMOpcode.LLVMGetElementPtr => CreateGep(id, operands, instruction),
                LLVMOpcode.LLVMTrunc => new Truncate(id, operands, instruction.TypeOf.GetSize(_targetData)),
                LLVMOpcode.LLVMZExt => new ZeroExtend(id, operands, instruction.TypeOf.GetSize(_targetData)),
                LLVMOpcode.LLVMSExt => new SignExtend(id, operands, instruction.TypeOf.GetSize(_targetData)),
                LLVMOpcode.LLVMFPToUI => new FloatToUnsigned(id, operands, instruction.TypeOf.GetSize(_targetData)),
                LLVMOpcode.LLVMFPToSI => new FloatToSigned(id, operands, instruction.TypeOf.GetSize(_targetData)),
                LLVMOpcode.LLVMUIToFP => new UnsignedToFloat(id, operands, instruction.TypeOf.GetSize(_targetData)),
                LLVMOpcode.LLVMSIToFP => new SignedToFloat(id, operands, instruction.TypeOf.GetSize(_targetData)),
                LLVMOpcode.LLVMFPTrunc => new FloatTruncate(id, operands, instruction.TypeOf.GetSize(_targetData)),
                LLVMOpcode.LLVMFPExt => new FloatExtend(id, operands, instruction.TypeOf.GetSize(_targetData)),
                LLVMOpcode.LLVMPtrToInt => new PointerToInteger(id, operands, instruction.TypeOf.GetSize(_targetData)),
                LLVMOpcode.LLVMIntToPtr => new IntegerToPointer(id, operands, instruction.TypeOf.GetSize(_targetData)),
                LLVMOpcode.LLVMBitCast => new BitCast(id, operands),
                LLVMOpcode.LLVMAddrSpaceCast => new Unsupported(id, "addrspacecast"),
                LLVMOpcode.LLVMICmp => instruction.ICmpPredicate switch
                {
                    LLVMIntPredicate.LLVMIntEQ => new Equal(id, operands),
                    LLVMIntPredicate.LLVMIntNE => new NotEqual(id, operands),
                    LLVMIntPredicate.LLVMIntUGT => new UnsignedGreater(id, operands),
                    LLVMIntPredicate.LLVMIntUGE => new UnsignedGreaterOrEqual(id, operands),
                    LLVMIntPredicate.LLVMIntULT => new UnsignedLess(id, operands),
                    LLVMIntPredicate.LLVMIntULE => new UnsignedLessOrEqual(id, operands),
                    LLVMIntPredicate.LLVMIntSGT => new SignedGreater(id, operands),
                    LLVMIntPredicate.LLVMIntSGE => new SignedGreaterOrEqual(id, operands),
                    LLVMIntPredicate.LLVMIntSLT => new SignedLess(id, operands),
                    LLVMIntPredicate.LLVMIntSLE => new SignedLessOrEqual(id, operands),
                    _ => throw new Exception("Comparison type is unknown.")
                },
                LLVMOpcode.LLVMFCmp => instruction.FCmpPredicate switch
                {
                    LLVMRealPredicate.LLVMRealPredicateFalse => new FloatTrue(id),
                    LLVMRealPredicate.LLVMRealOEQ => new FloatOrderedAndEqual(id, operands),
                    LLVMRealPredicate.LLVMRealOGT => new FloatOrderedAndGreater(id, operands),
                    LLVMRealPredicate.LLVMRealOGE => new FloatOrderedAndGreaterOrEqual(id, operands),
                    LLVMRealPredicate.LLVMRealOLT => new FloatOrderedAndLess(id, operands),
                    LLVMRealPredicate.LLVMRealOLE => new FloatOrderedAndLessOrEqual(id, operands),
                    LLVMRealPredicate.LLVMRealONE => new FloatOrderedAndNotEqual(id, operands),
                    LLVMRealPredicate.LLVMRealORD => new FloatOrdered(id, operands),
                    LLVMRealPredicate.LLVMRealUNO => new FloatUnordered(id, operands),
                    LLVMRealPredicate.LLVMRealUEQ => new FloatUnorderedOrEqual(id, operands),
                    LLVMRealPredicate.LLVMRealUGT => new FloatUnorderedOrGreater(id, operands),
                    LLVMRealPredicate.LLVMRealUGE => new FloatUnorderedOrGreaterOrEqual(id, operands),
                    LLVMRealPredicate.LLVMRealULT => new FloatUnorderedOrLess(id, operands),
                    LLVMRealPredicate.LLVMRealULE => new FloatUnorderedOrLessOrEqual(id, operands),
                    LLVMRealPredicate.LLVMRealUNE => new FloatUnorderedOrNotEqual(id, operands),
                    LLVMRealPredicate.LLVMRealPredicateTrue => new FloatFalse(id),
                    _ => throw new Exception("Float comparison type is unknown.")
                },
                LLVMOpcode.LLVMPHI => new Phi(
                    id,
                    operands,
                    instruction.GetIncomingBasicBlocks()
                        .Select(b => (BasicBlockId) _idFactory.GetOrCreate(b.Handle))
                        .ToArray()),
                LLVMOpcode.LLVMCall => CreateCall(id, operands, instruction),
                LLVMOpcode.LLVMSelect => new Select(id, operands),
                LLVMOpcode.LLVMUserOp1 => throw new Exception("User op 1 should only be used internally in passes."),
                LLVMOpcode.LLVMUserOp2 => throw new Exception("User op 2 should only be used internally in passes."),
                LLVMOpcode.LLVMVAArg => throw new Exception("The front-end should lower va_arg."),
                LLVMOpcode.LLVMExtractElement => new Unsupported(id, "extractelement"),
                LLVMOpcode.LLVMInsertElement => new Unsupported(id, "insertelement"),
                LLVMOpcode.LLVMShuffleVector => throw new Exception("The scalarizer pass should lower shufflevector."),
                LLVMOpcode.LLVMExtractValue => new ExtractValue(
                    id,
                    operands,
                    instruction.TypeOf.GetStoreSize(_targetData).ToBits(),
                    GetConstantOffsets(instruction).ToArray()),
                LLVMOpcode.LLVMInsertValue => new InsertValue(
                    id,
                    operands,
                    GetConstantOffsets(instruction).ToArray()),
                LLVMOpcode.LLVMFreeze => new Unsupported(id, "freeze"),
                LLVMOpcode.LLVMFence => throw new Exception("The loweratomic pass should lower fence."),
                LLVMOpcode.LLVMAtomicCmpXchg => throw new Exception("The loweratomic pass should lower cmpxchg."),
                LLVMOpcode.LLVMAtomicRMW => throw new Exception("The loweratomic pass should lower atomicrmw."),
                LLVMOpcode.LLVMResume => new Unsupported(id, "resume"),
                LLVMOpcode.LLVMLandingPad => new Unsupported(id, "landingpad"),
                LLVMOpcode.LLVMCleanupRet => new Unsupported(id, "cleanupret"),
                LLVMOpcode.LLVMCatchRet => new Unsupported(id, "catchret"),
                LLVMOpcode.LLVMCatchPad => new Unsupported(id, "catchpad"),
                LLVMOpcode.LLVMCleanupPad => new Unsupported(id, "cleanuppad"),
                LLVMOpcode.LLVMCatchSwitch => new Unsupported(id, "catchswitch"),
                _ => throw new Exception("Instruction type is unknown.")
            };
        }

        private Call CreateCall(InstructionId id, IOperand[] operands, LLVMValueRef instruction)
        {
            return new(
                id,
                operands,
                instruction.TypeOf.GetStoreSize(_targetData).ToBits(),
                GetAttributes(instruction, LLVMAttributeIndex.LLVMAttributeReturnIndex),
                instruction.GetAttributeIndices()
                    .Select(i => GetAttributes(instruction, i))
                    .ToArray());
        }

        private IAttributes GetAttributes(LLVMValueRef instruction, LLVMAttributeIndex attributeIndex)
        {
            var attributes = instruction.GetCallSiteAttributes(attributeIndex);

            var kind = _unsafeContext.GetEnumAttributeKind("signext");
            var isSignExtended = attributes.Any(a => _unsafeContext.GetEnumAttributeKind(a) == kind);

            return new Attributes(isSignExtended);
        }

        private IInstruction CreateGep(InstructionId id, IOperand[] operands, LLVMValueRef instruction)
        {
            var constantOffsets = new List<Bytes>();
            var offsets = new List<Offset>();

            var indexedType = instruction.GetOperand(0U).TypeOf;

            foreach (var (operand, index) in instruction.GetOperands().Select((o, i) => (o, i)).Skip(1))
                if (indexedType.Kind == LLVMTypeKind.LLVMStructTypeKind)
                {
                    var element = (uint) operand.ConstIntZExt;

                    constantOffsets.Add(indexedType.GetElementOffset(_targetData, element));
                    indexedType = indexedType.StructGetTypeAtIndex(element);
                }
                else
                {
                    indexedType = indexedType.ElementType;
                    offsets.Add(new Offset(index, indexedType.GetStoreSize(_targetData)));
                }

            return new GetElementPointer(id, operands, constantOffsets.ToArray(), offsets.ToArray());
        }

        private IEnumerable<Bits> GetConstantOffsets(LLVMValueRef instruction)
        {
            var indexedType = instruction.GetOperand(0U).TypeOf;

            foreach (var index in _unsafeContext.GetIndices(instruction))
                if (indexedType.Kind == LLVMTypeKind.LLVMStructTypeKind)
                {
                    yield return indexedType.GetElementOffset(_targetData, index).ToBits();
                    indexedType = indexedType.StructGetTypeAtIndex(index);
                }
                else
                {
                    indexedType = indexedType.ElementType;
                    yield return (Bits) (index * (uint) indexedType.GetStoreSize(_targetData).ToBits());
                }
        }
    }
}
