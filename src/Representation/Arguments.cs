using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation
{
    internal sealed class Arguments : IArguments
    {
        private readonly IExpression[] _expressions;

        public Arguments(IExpression[] expressions)
        {
            _expressions = expressions;
        }

        public IExpression Get(int index)
        {
            return _expressions[index];
        }

        public IEnumerator<IExpression> GetEnumerator()
        {
            return _expressions.AsEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
