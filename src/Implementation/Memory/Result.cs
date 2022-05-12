using Symbolica.Expression;
using Symbolica.Implementation.Exceptions;

namespace Symbolica.Implementation.Memory;

internal sealed class Result
{
    private readonly IPersistentBlock? _block;
    private readonly ISpace? _failureSpace;

    private Result(ISpace? failureSpace, IPersistentBlock? block)
    {
        _failureSpace = failureSpace;
        _block = block;
    }

    public ISpace FailureSpace => _failureSpace ?? throw new ImplementationException("Success has no space.");
    public bool CanBeFailure => _failureSpace != null;
    public bool CanBeSuccess => _block != null;
    public IPersistentBlock Block => _block ?? throw new ImplementationException("Failure has no block.");

    public static Result Failure(ISpace failureSpace)
    {
        return new Result(failureSpace, null);
    }

    public static Result Both(ISpace failureSpace, IPersistentBlock block)
    {
        return new Result(failureSpace, block);
    }

    public static Result Success(IPersistentBlock block)
    {
        return new Result(null, block);
    }
}
