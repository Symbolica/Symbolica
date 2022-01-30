namespace Symbolica.Abstraction;

public enum StateError
{
    Abort,
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
