using Symbolica.Representation;

namespace Symbolica.Deserialization
{
    public static class DeserializerFactory
    {
        public static IDeserializer Create(IDeclarationFactory declarationFactory)
        {
            return new Deserializer(new UnsafeContext(), declarationFactory);
        }
    }
}
