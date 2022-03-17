using System;

namespace Symbolica.Implementation;

public readonly record struct Options
{
    public Options(
        bool useSymbolicGarbage,
        bool useSymbolicAddresses,
        bool useSymbolicContinuations,
        int? maxParallelism = null)
    {
        MaxParallelism = maxParallelism ?? 8 * Environment.ProcessorCount;
        UseSymbolicAddresses = useSymbolicAddresses;
        UseSymbolicContinuations = useSymbolicContinuations;
        UseSymbolicGarbage = useSymbolicGarbage;
    }

    public int MaxParallelism { get; }
    public bool UseSymbolicAddresses { get; }
    public bool UseSymbolicContinuations { get; }
    public bool UseSymbolicGarbage { get; }
}
