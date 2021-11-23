using System;

namespace Symbolica.Abstraction
{
    [Serializable]
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
