using Symbolica.Expression;

namespace Symbolica.Implementation.Memory
{
    internal sealed class Result<TValue>
        where TValue : class
    {
        private readonly ISpace? _failureSpace;
        private readonly TValue? _value;

        private Result(ISpace? failureSpace, TValue? value)
        {
            _failureSpace = failureSpace;
            _value = value;
        }

        public ISpace FailureSpace => _failureSpace ?? throw new ImplementationException("Success has no space.");
        public bool CanBeFailure => _failureSpace != null;
        public bool CanBeSuccess => _value != null;
        public TValue Value => _value ?? throw new ImplementationException("Failure has no value.");

        public static Result<TValue> Failure(ISpace failureSpace)
        {
            return new(failureSpace, null);
        }

        public static Result<TValue> Both(ISpace failureSpace, TValue value)
        {
            return new(failureSpace, value);
        }

        public static Result<TValue> Success(TValue value)
        {
            return new(null, value);
        }
    }
}
