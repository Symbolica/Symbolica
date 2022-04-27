using System.Collections.Generic;
using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Representation.Exceptions;

namespace Symbolica.Representation.Types;

public sealed class SingleValueType : IType
{
    public SingleValueType(Bytes size)
    {
        Size = size;
    }

    public Bytes Size { get; }
    public IEnumerable<IType> Types => Enumerable.Empty<IType>();
    public IEnumerable<Bytes> Offsets => Enumerable.Empty<Bytes>();

    public IType GetType(ISpace space, IExpression index)
    {
        throw new InvalidIndexException();
    }

    public IExpression GetOffsetBits(ISpace space, IExpression index)
    {
        throw new InvalidIndexException();
    }

    public IExpression GetOffsetBytes(ISpace space, IExpression index)
    {
        throw new InvalidIndexException();
    }
}
