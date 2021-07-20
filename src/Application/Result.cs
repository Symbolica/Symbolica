using System;
using Symbolica.Abstraction;

namespace Symbolica.Application
{
    internal sealed class Result
    {
        private readonly StateException? _exception;

        private Result(StateException? exception)
        {
            _exception = exception;
        }

        public bool IsSuccess => _exception == null;
        public StateException Exception => _exception ?? throw new Exception("Success result has no exception.");

        public static Result Failure(StateException exception)
        {
            return new(exception);
        }

        public static Result Success()
        {
            return new(null);
        }
    }
}
