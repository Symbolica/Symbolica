using System;
using Symbolica.Expression;

namespace Symbolica.Application
{
    internal sealed class Result
    {
        private readonly SymbolicaException? _exception;

        private Result(SymbolicaException? exception)
        {
            _exception = exception;
        }

        public bool IsSuccess => _exception == null;
        public SymbolicaException Exception => _exception ?? throw new Exception("Success has no exception.");

        public static Result Failure(SymbolicaException exception)
        {
            return new(exception);
        }

        public static Result Success()
        {
            return new(null);
        }
    }
}
