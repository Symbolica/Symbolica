using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Instructions
{
    [Serializable]
    public sealed class Call : IInstruction, ICaller
    {
        private readonly IOperand[] _operands;
        private readonly IAttributes[] _parameterAttributes;

        public Call(
            InstructionId id,
            IOperand[] operands,
            Bits size,
            IAttributes returnAttributes,
            IAttributes[] parameterAttributes)
        {
            Id = id;
            _operands = operands;
            Size = size;
            ReturnAttributes = returnAttributes;
            _parameterAttributes = parameterAttributes;
        }

        public Bits Size { get; }
        public IAttributes ReturnAttributes { get; }

        public void Return(IState state)
        {
        }

        public InstructionId Id { get; }

        public void Execute(IState state)
        {
            var expressions = _operands.Select(o => o.Evaluate(state)).ToArray();

            var arguments = expressions.SkipLast(1).ToArray();
            var target = expressions.Last();

            state.ForkAll(target, new StateActions.CallTarget(arguments, this, _parameterAttributes));
        }
    }
}
