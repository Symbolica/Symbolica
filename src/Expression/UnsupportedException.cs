using System;

namespace Symbolica.Expression
{
    /// <summary>
    /// Used as a base class for exceptions that indicate that the user's code
    /// has invoked something that is currently unsupported by us.
    /// This type of exception is unfixable by the user.
    /// </summary>
    /// <remarks>
    /// This base exception makes it possible for users to specify that they wish to ignore all unsupported behaviours.
    /// They might do this if they want to keep analyzing other code paths, even though some code paths hit this exception.
    /// <remarks>
    [Serializable]
    public abstract class UnsupportedException : SymbolicaException
    {
        protected UnsupportedException(string message)
            : base(message)
        {
        }
    }
}
