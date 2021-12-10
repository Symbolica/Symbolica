namespace Symbolica.Implementation;

public sealed record Options(
    ulong SymbolicFileSize,
    bool UseSymbolicGarbage,
    bool UseSymbolicAddresses,
    bool UseSymbolicContinuations);
