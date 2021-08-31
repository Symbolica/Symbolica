﻿using System;
using Symbolica.Expression;

namespace Symbolica.Implementation.Exceptions
{
    [Serializable]
    public class UnsupportedArchitectureException : ErrorException
    {
        public UnsupportedArchitectureException(string target)
            : base($"Architecture for target {target} is unsupported.")
        {
            Target = target;
        }

        public string Target { get; }
    }
}
