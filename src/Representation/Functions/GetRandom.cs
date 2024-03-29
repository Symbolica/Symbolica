﻿using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Functions;

internal sealed class GetRandom : IFunction
{
    public GetRandom(FunctionId id, IParameters parameters)
    {
        Id = id;
        Parameters = parameters;
    }

    public FunctionId Id { get; }
    public IParameters Parameters { get; }

    public void Call(IState state, ICaller caller, IArguments arguments)
    {
        var address = arguments.Get(0);
        var length = arguments.Get(1).GetSingleValue(state.Space);
        var size = (Bytes) (uint) length;

        state.Memory.Write(address, state.Space.CreateGarbage(size.ToBits()));
        state.Stack.SetVariable(caller.Id, state.Space.CreateConstant(caller.Size, length));
    }
}
