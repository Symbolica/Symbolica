using System.Threading.Tasks;
using Symbolica.Implementation;

namespace Symbolica.Application.Implementation
{
    internal interface IAwaitableProgramPool : IProgramPool
    {
        Task Wait();
    }
}
