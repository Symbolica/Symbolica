using System;
using Symbolica.Expression;

namespace Symbolica.Abstraction
{
    [Serializable]
    public readonly struct Parameter
    {
        public Parameter(Bits size)
        {
            Size = size;
        }

        public Bits Size { get; }
    }
}
