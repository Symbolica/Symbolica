using System;

namespace Symbolica.Representation
{
    [Serializable]
    public class UnsupportedFunctionException : Exception
    {
        public UnsupportedFunctionException(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
