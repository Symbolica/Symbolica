using System;
using LLVMSharp.Interop;
using Symbolica.Expression;

namespace Symbolica.Deserialization.Exceptions;

[Serializable]
public class IncompatibleTypeException : SymbolicaException
{
    public IncompatibleTypeException(Type type, LLVMTypeKind kind)
        : base($"Type {type} is incompatible with kind {kind}.")
    {
        Type = type;
        Kind = kind;
    }

    public Type Type { get; }
    public LLVMTypeKind Kind { get; }
}
