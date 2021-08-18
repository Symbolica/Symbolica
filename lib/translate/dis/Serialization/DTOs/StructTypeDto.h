#ifndef DIS_STRUCT_TYPE_DTO_H
#define DIS_STRUCT_TYPE_DTO_H

namespace {
    struct StructTypeDto : Serializable {
        uint64_t size;
        std::vector<uint64_t> offsets;

        explicit StructTypeDto(StructType *structType, DataLayout *dataLayout)
                : size(dataLayout->getTypeStoreSizeInBits(structType)) {
            auto *structLayout = dataLayout->getStructLayout(structType);
            for (unsigned i = 0; i < structType->getNumElements(); ++i) {
                offsets.push_back(structLayout->getElementOffsetInBits(i));
            }
        }

    protected:
        void SerializeProperties() const override {
            SERIALIZE(size);
            SERIALIZE(offsets);
        }
    };
}

#endif
