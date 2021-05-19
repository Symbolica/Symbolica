using System.Threading.Tasks;
using Symbolica.Implementation;

namespace Symbolica.Execution
{
    public interface IAwaitableProgramPool : IProgramPool
    {
        Task Wait();
    }
}