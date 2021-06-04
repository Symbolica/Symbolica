#ifndef DIS_SERIALIZABLE_H
#define DIS_SERIALIZABLE_H

#define SERIALIZE(x) Serialize(#x, x)

namespace {
    struct Serializable {
        void Serialize() const {
            printf("{");
            firstProperty = true;
            SerializeProperties();
            printf("}");
        }

    protected:
        virtual void SerializeProperties() const = 0;

        template<typename T>
        void Serialize(const char *name, T value) const {
            if (!firstProperty) {
                printf(",");
            }
            Serialize(name);
            printf(":");
            firstProperty = false;

            Serialize(value);
        }

    private:
        mutable bool firstProperty = true;

        void Serialize(const bool value) const { printf("%s", value ? "true" : "false"); }

        void Serialize(const uint64_t value) const { printf("%lu", value); }

        void Serialize(const unsigned value) const { printf("%u", value); }

        void Serialize(const char *value) const { printf("\"%s\"", value); }

        void Serialize(const std::string &value) const { Serialize(value.c_str()); }

        void Serialize(const Serializable *value) const { if (value) value->Serialize(); else printf("null"); }

        template<typename T>
        void Serialize(const std::vector<T> &values) const {
            printf("[");
            auto firstValue = true;
            for (auto &value : values) {
                if (!firstValue) {
                    printf(",");
                }
                Serialize(value);
                firstValue = false;
            }
            printf("]");
        }
    };
}

#endif
