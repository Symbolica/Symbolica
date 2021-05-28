namespace Symbolica.Implementation.Stack
{
    internal interface ISavedFrame
    {
        IPersistentFrame Restore(bool useJumpBuffer,
            IPersistentJumps jumps, IPersistentProgramCounter programCounter, IPersistentVariables variables);
    }
}
