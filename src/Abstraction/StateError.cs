namespace Symbolica.Abstraction
{
    public enum StateError
    {
        DivideByZero,
        InvalidJump,
        InvalidMemoryFree,
        InvalidMemoryMove,
        InvalidMemoryRead,
        InvalidMemoryWrite,
        FailingAssertion,
        NonZeroExitCode,
        OverlappingMemoryCopy,
        UndefinedShift
    }
}
