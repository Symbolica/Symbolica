﻿using System;
using Symbolica.Abstraction;

namespace Symbolica.Representation.Functions
{
    [Serializable]
    internal sealed class FloatPower : IFunction
    {
        public FloatPower(FunctionId id, IParameters parameters)
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
            var result = left.FloatPower(right);

            state.Stack.SetVariable(caller.Id, result);
        }
    }
}
