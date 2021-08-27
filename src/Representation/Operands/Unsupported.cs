using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Operands
{
    public sealed class Unsupported : IOperand
    {
        private readonly string _type;

        public Unsupported(string type)
        {
            _type = type;
        }

        public IExpression Evaluate(IState state)
        {
            throw new RepresentationException($"Operand '{_type}' is unsupported.");
        }
    }
}
