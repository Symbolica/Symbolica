using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Symbolica.Deserialization
{
    internal abstract record PolymorphicSerializable<T>(
            string Type)
        : Serializable<T>
    {
        [JsonExtensionData]
        // ReSharper disable once UnusedMember.Global
        public IDictionary<string, JsonElement>? PolymorphicData { get; set; }

        public TPolymorphic As<TPolymorphic>()
            where TPolymorphic : PolymorphicSerializable<T>
        {
            var json = JsonSerializer.Serialize(this, GetType(), Deserializer.Options);
            return JsonSerializer.Deserialize<TPolymorphic>(json, Deserializer.Options)
                   ?? throw new Exception("DTO cannot be null.");
        }
    }
}
