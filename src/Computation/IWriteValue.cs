using Symbolica.Expression;

namespace Symbolica.Computation;

internal interface IWriteValue : IValue
{
    IValue Read(IValue offset, Bits size);
}
