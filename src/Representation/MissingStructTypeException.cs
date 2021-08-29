using System;

namespace Symbolica.Representation
{
    [Serializable]
    public class MissingStructTypeException : Exception
    {
        public MissingStructTypeException(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
