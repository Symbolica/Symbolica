﻿using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Functions;

internal sealed class Absolute : IFunction
{
    public Absolute(FunctionId id, IParameters parameters)
    {
        Id = id;
        Parameters = parameters;
    }

    public FunctionId Id { get; }
    public IParameters Parameters { get; }

    public void Call(IExpressionFactory exprFactory, IState state, ICaller caller, IArguments arguments)
    {
        var value = arguments.Get(0);

        var mask = value.ArithmeticShiftRight(exprFactory.CreateConstant(value.Size, (uint) value.Size - 1U));
        var result = value.Add(mask).Xor(mask);

        state.Stack.SetVariable(caller.Id, result);
    }
}
