using System;
using Symbolica.Expression;

namespace Symbolica.Computation
{
    [Serializable]
    internal sealed class ConstantProposition : IProposition
    {
        public ConstantProposition(ISpace space, bool isTrue)
        {
            TrueSpace = space;
            CanBeTrue = isTrue;
        }

        public ISpace FalseSpace => TrueSpace;
        public ISpace TrueSpace { get; }
        public bool CanBeFalse => !CanBeTrue;
        public bool CanBeTrue { get; }

        public void Dispose()
        {
        }
    }
}
