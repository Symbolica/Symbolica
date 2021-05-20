#ifndef DIS_SERIALIZABLE_H
#define DIS_SERIALIZABLE_H

#define SERIALIZE(x) Serialize(#x, x)

namespace {
    struct Serializable {
        void Serialize() const {
            out() << "{";
            firstProperty = true;
            SerializeProperties();
            out() << "}";
        }

    protected:
        virtual void SerializeProperties() const = 0;

        template<typename T>
        void Serialize(const char *name, T value) const {
            if (!firstProperty) {
                out() << ",";
            }
            Serialize(name);
            out() << ":";
            firstProperty = false;

            Serialize(value);
        }

    private:
        mutable bool firstProperty = true;

        void Serialize(const bool value) const { out() << (value ? "true" : "false"); }

        void Serialize(const uint64_t value) const { out() << value; }

        void Serialize(const unsigned value) const { out() << value; }

        void Serialize(const char *value) const { out() << "\"" << value << "\""; }

        void Serialize(const std::string &value) const { Serialize(value.c_str()); }

        void Serialize(const Serializable *value) const { if (value) value->Serialize(); else out() << "null"; }

        template<typename T>
        void Serialize(const std::vector<T> &values) const {
            out() << "[";
            auto firstValue = true;
            for (auto &value : values) {
                if (!firstValue) {
                    out() << ",";
                }
                Serialize(value);
                firstValue = false;
            }
            out() << "]";
        }
    };
}

#endif