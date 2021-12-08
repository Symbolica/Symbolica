using System.Collections.Generic;

namespace Symbolica.Implementation
{
    public interface IExecutable
    {
        IEnumerable<IEnumerable<IExecutable>> Run();
    }
}
