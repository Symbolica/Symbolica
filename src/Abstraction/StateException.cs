using System;
using Symbolica.Expression;

namespace Symbolica.Abstraction;

[Serializable]
public class StateException : ErrorException
{
    public StateException(StateError error, IExample example)
        : base(error.ToString())
    {
        Error = error;
        Example = example;
    }

    public StateError Error { get; }
    public IExample Example { get; }
}
