﻿using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Representation.Instructions;

public sealed class FloatTrue : IInstruction
{
    public FloatTrue(InstructionId id)
    {
        Id = id;
    }

    public InstructionId Id { get; }

    public void Execute(IState state)
    {
        var result = ConstantUnsigned.Create(Bits.One, BigInteger.One);

        state.Stack.SetVariable(Id, result);
    }
}
