using System;

namespace Symbolica.Implementation;

public sealed record Options
{
    public Options(
        int? maxParallelism,
        bool useSymbolicAddresses,
        bool useSymbolicContinuations,
        bool useSymbolicGarbage)
    {
        MaxParallelism = maxParallelism ?? Environment.ProcessorCount * 8;
        UseSymbolicAddresses = useSymbolicAddresses;
        UseSymbolicContinuations = useSymbolicContinuations;
        UseSymbolicGarbage = useSymbolicGarbage;
    }

    public int MaxParallelism { get; init; }
    public bool UseSymbolicAddresses { get; init; }
    public bool UseSymbolicContinuations { get; init; }
    public bool UseSymbolicGarbage { get; init; }
}
