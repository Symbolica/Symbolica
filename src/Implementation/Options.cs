using System;

namespace Symbolica.Implementation
{
    [Serializable]
    public sealed record Options(
        bool UseSymbolicGarbage,
        bool UseSymbolicAddresses,
        bool UseSymbolicContinuations);
}
