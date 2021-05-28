#ifndef DIS_INSTRUCTION_MAPPER_H
#define DIS_INSTRUCTION_MAPPER_H

#include "DTOs/Instructions/AggregateDto.h"
#include "DTOs/Instructions/AllocateDto.h"
#include "DTOs/Instructions/CastDto.h"
#include "DTOs/Instructions/CompareDto.h"
#include "DTOs/Instructions/GetElementPointerDto.h"
#include "DTOs/Instructions/InvokeDto.h"
#include "DTOs/Instructions/LoadDto.h"
#include "DTOs/Instructions/PhiDto.h"
#include "DTOs/Instructions/VectorDto.h"

namespace {
    static InstructionDto *map(Instruction *instruction, DataLayout *dataLayout) {
        switch (instruction->getOpcode()) {
            case Instruction::Ret:
                return new InstructionDto("Return", instruction, dataLayout);
            case Instruction::Br:
                return new InstructionDto("Branch", instruction, dataLayout);
            case Instruction::Switch:
                return new InstructionDto("Switch", instruction, dataLayout);
            case Instruction::IndirectBr:
                return new InstructionDto("IndirectBranch", instruction, dataLayout);
            case Instruction::Invoke:
                return new InvokeDto("Invoke", cast<InvokeInst>(instruction), dataLayout);
            case Instruction::Resume:
                return new InstructionDto("Resume", instruction, dataLayout);
            case Instruction::Unreachable:
                return new InstructionDto("Unreachable", instruction, dataLayout);
            case Instruction::CleanupRet:
                return new InstructionDto("CleanupReturn", instruction, dataLayout);
            case Instruction::CatchRet:
                return new InstructionDto("CatchReturn", instruction, dataLayout);
            case Instruction::CatchSwitch:
                return new InstructionDto("CatchSwitch", instruction, dataLayout);
            case Instruction::FNeg:
                return new InstructionDto("FloatNegate", instruction, dataLayout);
            case Instruction::Add:
                return new InstructionDto("Add", instruction, dataLayout);
            case Instruction::FAdd:
                return new InstructionDto("FloatAdd", instruction, dataLayout);
            case Instruction::Sub:
                return new InstructionDto("Subtract", instruction, dataLayout);
            case Instruction::FSub:
                return new InstructionDto("FloatSubtract", instruction, dataLayout);
            case Instruction::Mul:
                return new InstructionDto("Multiply", instruction, dataLayout);
            case Instruction::FMul:
                return new InstructionDto("FloatMultiply", instruction, dataLayout);
            case Instruction::UDiv:
                return new InstructionDto("UnsignedDivide", instruction, dataLayout);
            case Instruction::SDiv:
                return new InstructionDto("SignedDivide", instruction, dataLayout);
            case Instruction::FDiv:
                return new InstructionDto("FloatDivide", instruction, dataLayout);
            case Instruction::URem:
                return new InstructionDto("UnsignedRemainder", instruction, dataLayout);
            case Instruction::SRem:
                return new InstructionDto("SignedRemainder", instruction, dataLayout);
            case Instruction::FRem:
                return new InstructionDto("FloatRemainder", instruction, dataLayout);
            case Instruction::Shl:
                return new InstructionDto("ShiftLeft", instruction, dataLayout);
            case Instruction::LShr:
                return new InstructionDto("LogicalShiftRight", instruction, dataLayout);
            case Instruction::AShr:
                return new InstructionDto("ArithmeticShiftRight", instruction, dataLayout);
            case Instruction::And:
                return new InstructionDto("And", instruction, dataLayout);
            case Instruction::Or:
                return new InstructionDto("Or", instruction, dataLayout);
            case Instruction::Xor:
                return new InstructionDto("Xor", instruction, dataLayout);
            case Instruction::Alloca:
                return new AllocateDto("Allocate", cast<AllocaInst>(instruction), dataLayout);
            case Instruction::Load:
                return new LoadDto("Load", cast<LoadInst>(instruction), dataLayout);
            case Instruction::Store:
                return new InstructionDto("Store", instruction, dataLayout);
            case Instruction::GetElementPtr:
                return new GetElementPointerDto("GetElementPointer", cast<GetElementPtrInst>(instruction), dataLayout);
            case Instruction::Fence:
                return new InstructionDto("Fence", instruction, dataLayout);
            case Instruction::AtomicCmpXchg:
                return new InstructionDto("AtomicCompareExchange", instruction, dataLayout);
            case Instruction::AtomicRMW:
                return new InstructionDto("AtomicReadModifyWrite", instruction, dataLayout);
            case Instruction::Trunc:
                return new CastDto("Truncate", instruction, dataLayout);
            case Instruction::ZExt:
                return new CastDto("ZeroExtend", instruction, dataLayout);
            case Instruction::SExt:
                return new CastDto("SignExtend", instruction, dataLayout);
            case Instruction::FPToUI:
                return new CastDto("FloatToUnsigned", instruction, dataLayout);
            case Instruction::FPToSI:
                return new CastDto("FloatToSigned", instruction, dataLayout);
            case Instruction::UIToFP:
                return new CastDto("UnsignedToFloat", instruction, dataLayout);
            case Instruction::SIToFP:
                return new CastDto("SignedToFloat", instruction, dataLayout);
            case Instruction::FPTrunc:
                return new CastDto("FloatTruncate", instruction, dataLayout);
            case Instruction::FPExt:
                return new CastDto("FloatExtend", instruction, dataLayout);
            case Instruction::PtrToInt:
                return new CastDto("PointerToInteger", instruction, dataLayout);
            case Instruction::IntToPtr:
                return new CastDto("IntegerToPointer", instruction, dataLayout);
            case Instruction::BitCast:
                return new InstructionDto("BitCast", instruction, dataLayout);
            case Instruction::AddrSpaceCast:
                return new InstructionDto("AddressSpaceCast", instruction, dataLayout);
            case Instruction::CleanupPad:
                return new InstructionDto("CleanupPad", instruction, dataLayout);
            case Instruction::CatchPad:
                return new InstructionDto("CatchPad", instruction, dataLayout);
            case Instruction::ICmp:
                return new CompareDto("Compare", cast<ICmpInst>(instruction), dataLayout);
            case Instruction::FCmp:
                return new CompareDto("FloatCompare", cast<FCmpInst>(instruction), dataLayout);
            case Instruction::PHI:
                return new PhiDto("Phi", cast<PHINode>(instruction), dataLayout);
            case Instruction::Call:
                return new CallDto("Call", cast<CallInst>(instruction), dataLayout);
            case Instruction::Select:
                return new InstructionDto("Select", instruction, dataLayout);
            case Instruction::UserOp1:
                return new InstructionDto("UserOp1", instruction, dataLayout);
            case Instruction::UserOp2:
                return new InstructionDto("UserOp2", instruction, dataLayout);
            case Instruction::VAArg:
                return new InstructionDto("VariableArgument", instruction, dataLayout);
            case Instruction::ExtractElement:
                return new VectorDto("ExtractElement", cast<ExtractElementInst>(instruction), dataLayout);
            case Instruction::InsertElement:
                return new VectorDto("InsertElement", cast<InsertElementInst>(instruction), dataLayout);
            case Instruction::ShuffleVector:
                return new InstructionDto("ShuffleVector", instruction, dataLayout);
            case Instruction::ExtractValue:
                return new AggregateDto("ExtractValue", cast<ExtractValueInst>(instruction), dataLayout);
            case Instruction::InsertValue:
                return new AggregateDto("InsertValue", cast<InsertValueInst>(instruction), dataLayout);
            case Instruction::LandingPad:
                return new InstructionDto("LandingPad", instruction, dataLayout);
            default:
                return new InstructionDto("Unknown", instruction, dataLayout);
        }
    }
}

#endif
