namespace Symbolica.Abstraction
{
    public enum StateError
    {
        DivideByZero,
        FailingAssertion,
        InvalidJump,
        InvalidMemoryFree,
        InvalidMemoryMove,
        InvalidMemoryRead,
        InvalidMemoryWrite,
        NonZeroExitCode,
        OverlappingMemoryCopy,
        UndefinedShift
    }
}
