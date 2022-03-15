namespace Symbolica.Expression;

public interface IProposition
{
    ISpace FalseSpace { get; }
    ISpace TrueSpace { get; }
    bool CanBeFalse { get; }
    bool CanBeTrue { get; }
}
