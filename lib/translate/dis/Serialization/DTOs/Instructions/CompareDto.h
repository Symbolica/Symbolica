#ifndef DIS_COMPARE_DTO_H
#define DIS_COMPARE_DTO_H

#include "InstructionDto.h"

namespace {
    struct CompareDto : InstructionDto {
        const char *comparisonType;

        explicit CompareDto(const char *type, CmpInst *instruction, DataLayout *dataLayout)
                : InstructionDto(type, instruction, dataLayout),
                  comparisonType(getComparisonType(instruction)) {}

    protected:
        void SerializeProperties() const override {
            InstructionDto::SerializeProperties();
            SERIALIZE(comparisonType);
        }

    private:
        static const char *getComparisonType(CmpInst *instruction) {
            switch (instruction->getPredicate()) {
                case CmpInst::FCMP_FALSE:
                    return "FloatFalse";
                case CmpInst::FCMP_OEQ:
                    return "FloatOrderedAndEqual";
                case CmpInst::FCMP_OGT:
                    return "FloatOrderedAndGreater";
                case CmpInst::FCMP_OGE:
                    return "FloatOrderedAndGreaterOrEqual";
                case CmpInst::FCMP_OLT:
                    return "FloatOrderedAndLess";
                case CmpInst::FCMP_OLE:
                    return "FloatOrderedAndLessOrEqual";
                case CmpInst::FCMP_ONE:
                    return "FloatOrderedAndNotEqual";
                case CmpInst::FCMP_ORD:
                    return "FloatOrdered";
                case CmpInst::FCMP_UNO:
                    return "FloatUnordered";
                case CmpInst::FCMP_UEQ:
                    return "FloatUnorderedOrEqual";
                case CmpInst::FCMP_UGT:
                    return "FloatUnorderedOrGreater";
                case CmpInst::FCMP_UGE:
                    return "FloatUnorderedOrGreaterOrEqual";
                case CmpInst::FCMP_ULT:
                    return "FloatUnorderedOrLess";
                case CmpInst::FCMP_ULE:
                    return "FloatUnorderedOrLessOrEqual";
                case CmpInst::FCMP_UNE:
                    return "FloatUnorderedOrNotEqual";
                case CmpInst::FCMP_TRUE:
                    return "FloatTrue";
                case CmpInst::BAD_FCMP_PREDICATE:
                    return "BadFloatComparison";
                case CmpInst::ICMP_EQ:
                    return "Equal";
                case CmpInst::ICMP_NE:
                    return "NotEqual";
                case CmpInst::ICMP_UGT:
                    return "UnsignedGreater";
                case CmpInst::ICMP_UGE:
                    return "UnsignedGreaterOrEqual";
                case CmpInst::ICMP_ULT:
                    return "UnsignedLess";
                case CmpInst::ICMP_ULE:
                    return "UnsignedLessOrEqual";
                case CmpInst::ICMP_SGT:
                    return "SignedGreater";
                case CmpInst::ICMP_SGE:
                    return "SignedGreaterOrEqual";
                case CmpInst::ICMP_SLT:
                    return "SignedLess";
                case CmpInst::ICMP_SLE:
                    return "SignedLessOrEqual";
                case CmpInst::BAD_ICMP_PREDICATE:
                    return "BadComparison";
                default:
                    return "Unknown";
            }
        }
    };
}

#endif
