using System.Collections.Generic;

namespace Symbolica.Implementation
{
    public interface IExecutable
    {
        (ulong, IEnumerable<IExecutable>) Run();
    }
}
