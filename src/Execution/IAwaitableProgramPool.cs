using System.Threading.Tasks;

namespace Symbolica.Execution
{
    public interface IAwaitableProgramPool : IProgramPool
    {
        Task Wait();
    }
}