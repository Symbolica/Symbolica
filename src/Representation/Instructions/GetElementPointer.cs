﻿using System;
using System.Collections.Generic;
using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Instructions;

public sealed class GetElementPointer : IInstruction
{
    private readonly IType _indexedType;
    private readonly IOperand[] _operands;

    public GetElementPointer(InstructionId id, IOperand[] operands, IType indexedType)
    {
        Id = id;
        _operands = operands;
        _indexedType = indexedType;
    }

    public InstructionId Id { get; }

    public void Execute(IExpressionFactory exprFactory, IState state)
    {
        IAddress CombineOffsets(IAddress baseAddress, IEnumerable<(IType, IExpression)> offsets)
        {
            if (!baseAddress.Offsets.Any())
                // This filth is to deal with the fact we currently fake BitCasts by creating an address without an offsets
                // If every expression had a type then BitCasting would just change that type.
                // Then when this GEP instruction was called it would just be a regular baseAddress expression of the correct pointer type.
                return Address.Create(exprFactory, baseAddress.IndexedType, baseAddress.BaseAddress, offsets);

            var lastRight = baseAddress.Offsets.Last();
            var firstLeft = offsets.First();
            if ((lastRight.Item1 is IArrayType p ? p.ElementType.Size : lastRight.Item1.Size) != firstLeft.Item1.Size)
                throw new Exception("Can't merge addresses when sizes of adjoining offsets are different.");

            var combinedOffsets = baseAddress.Offsets
                .SkipLast(1)
                .Append((lastRight.Item1, lastRight.Item2.Add(firstLeft.Item2)))
                .Concat(offsets.Skip(1));
            return Address.Create(exprFactory, baseAddress.IndexedType, baseAddress.BaseAddress, combinedOffsets);
        }

        IExpression baseAddress = _operands[0].Evaluate(exprFactory, state);
        IEnumerable<(IType, IExpression)> offsets = GetOffsets(exprFactory, state);
        state.Stack.SetVariable(
            Id,
            baseAddress is IAddress a
                ? CombineOffsets(a, offsets)
                : Address.Create(exprFactory, _indexedType, baseAddress, offsets));
    }

    private IEnumerable<(IType, IExpression)> GetOffsets(IExpressionFactory exprFactory, IState state)
    {
        var indexedType = _indexedType;

        foreach (var operand in _operands.Skip(1))
        {
            var index = operand.Evaluate(exprFactory, state);
            var elementType = indexedType.GetType(state.Space, index);
            yield return (elementType, indexedType.GetOffsetBytes(exprFactory, state.Space, index));
            indexedType = elementType;
        }
    }
}
