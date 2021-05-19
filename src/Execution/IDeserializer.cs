using System.IO;
using System.Threading.Tasks;
using Symbolica.Abstraction;

namespace Symbolica.Execution
{
    public interface IDeserializer
    {
        Task<IModule> DeserializeModule(Stream stream);
    }
}