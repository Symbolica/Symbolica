using Symbolica.Abstraction;

namespace Symbolica.Deserialization;

public interface IDeserializer
{
    IModule DeserializeModule(byte[] bytes);
}
