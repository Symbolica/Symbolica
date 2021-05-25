using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Symbolica.Abstraction;
using Symbolica.Deserialization.DTOs;

namespace Symbolica.Deserialization
{
    public static class Deserializer
    {
        internal static readonly JsonSerializerOptions Options = new()
        {
            PropertyNameCaseInsensitive = true,
            MaxDepth = int.MaxValue
        };

        public static async Task<IModule> DeserializeModule(Stream stream)
        {
            var dto = await JsonSerializer.DeserializeAsync<ModuleDto>(stream, Options);

            return dto?.Convert()
                   ?? throw new Exception("Module cannot be null.");
        }
    }
}