namespace Symbolica.Implementation;

public sealed record Options(
    bool UseSymbolicGarbage,
    bool UseSymbolicAddresses,
    bool UseSymbolicContinuations);
