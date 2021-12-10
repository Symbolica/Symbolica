namespace Symbolica.Abstraction;

public enum StateError
{
    Abort,
    DivideByZero,
    FailingAssertion,
    InvalidFileOffset,
    InvalidJump,
    InvalidMemoryFree,
    InvalidMemoryMove,
    InvalidMemoryRead,
    InvalidMemoryWrite,
    NonZeroExitCode,
    OverlappingMemoryCopy,
    UndefinedShift
}
