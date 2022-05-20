using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal abstract record Integer : IValue
{
    protected Integer(Bits size)
    {
        Size = size;
    }

    public Bits Size { get; }

    public abstract BitVecExpr AsBitVector(ISolver solver);
    public abstract BoolExpr AsBool(ISolver solver);
    public abstract bool Equals(IValue? other);

    public virtual bool TryMerge(IValue value, [MaybeNullWhen(false)] out IValue merged)
    {
        merged = value;
        return Equals(value);
    }

    public FPExpr AsFloat(ISolver solver)
    {
        using var bitVector = AsBitVector(solver);
        using var sort = Size.GetSort(solver);
        return solver.Context.MkFPToFP(bitVector, sort);
    }

    public abstract (HashSet<(IValue, IValue)> subs, bool) IsEquivalentTo(IValue other);
    public abstract IValue Substitute(IReadOnlyDictionary<IValue, IValue> subs);
    public abstract object ToJson();
    public abstract int GetEquivalencyHash();
}
