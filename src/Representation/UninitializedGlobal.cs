using System;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation
{
    [Serializable]
    public sealed class UninitializedGlobal : IGlobal
    {
        public UninitializedGlobal(GlobalId id, Bits size)
        {
            Id = id;
            Size = size;
        }

        public GlobalId Id { get; }
        public Bits Size { get; }

        public void Initialize(IState state, IExpression address)
        {
        }
    }
}
