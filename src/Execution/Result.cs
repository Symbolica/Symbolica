using System;
using Symbolica.Expression;

namespace Symbolica.Execution
{
    public sealed class Result
    {
        private readonly IExample? _example;
        private readonly string? _message;

        private Result(bool isSuccess, string? message, IExample? example)
        {
            IsSuccess = isSuccess;
            _message = message;
            _example = example;
        }

        public bool IsSuccess { get; }
        public string Message => _message ?? throw new Exception("Success result has no message.");
        public IExample Example => _example ?? throw new Exception("Success result has no example.");

        public static Result Failure(string message, IExample example)
        {
            return new(false, message, example);
        }

        public static Result Success()
        {
            return new(true, null, null);
        }
    }
}