namespace Symbolica.Implementation
{
    public sealed record Options(
        uint MaxErrors,
        bool UseSymbolicGarbage,
        bool UseSymbolicAddresses,
        bool UseSymbolicContinuations);
}
