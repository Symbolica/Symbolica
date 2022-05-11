﻿using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Functions;

internal sealed class FloatDivide : IFunction
{
    public FloatDivide(FunctionId id, IParameters parameters)
    {
        Id = id;
        Parameters = parameters;
    }

    public FunctionId Id { get; }
    public IParameters Parameters { get; }

    public void Call(IExpressionFactory exprFactory, IState state, ICaller caller, IArguments arguments)
    {
        var left = arguments.Get(0);
        var right = arguments.Get(1);
        var result = left.FloatDivide(right);

        state.Stack.SetVariable(caller.Id, result);
    }
}
