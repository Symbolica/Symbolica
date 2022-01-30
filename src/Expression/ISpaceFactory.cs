namespace Symbolica.Expression;

public interface ISpaceFactory
{
    ISpace CreateInitial(Bits pointerSize, bool useSymbolicGarbage);
}
