﻿using Symbolica.Abstraction;

namespace Symbolica.Representation.Functions;

internal sealed class FloatSubtract : IFunction
{
    public FloatSubtract(FunctionId id, IParameters parameters)
    {
        Id = id;
        Parameters = parameters;
    }

    public FunctionId Id { get; }
    public IParameters Parameters { get; }

    public void Call(IState state, ICaller caller, IArguments arguments)
    {
        var left = arguments.Get(0);
        var right = arguments.Get(1);
        var result = Expression.Values.FloatSubtract.Create(left, right);

        state.Stack.SetVariable(caller.Id, result);
    }
}
