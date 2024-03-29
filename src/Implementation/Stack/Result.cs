﻿using Symbolica.Implementation.Exceptions;

namespace Symbolica.Implementation.Stack;

internal sealed class Result<TValue>
    where TValue : class
{
    private readonly TValue? _value;

    private Result(TValue? value)
    {
        _value = value;
    }

    public bool IsSuccess => _value != null;
    public TValue Value => _value ?? throw new ImplementationException("Failure has no value.");

    public static Result<TValue> Failure()
    {
        return new Result<TValue>(null);
    }

    public static Result<TValue> Success(TValue value)
    {
        return new Result<TValue>(value);
    }
}
