namespace Symbolica.Expression;

public interface ISpaceFactory
{
    ISpace CreateInitial(Size pointerSize, bool useSymbolicGarbage);
}
