using System.Linq;
using LLVMSharp.Interop;
using Symbolica.Abstraction;
using Symbolica.Deserialization.Exceptions;
using Symbolica.Deserialization.Extensions;
using Symbolica.Representation.Types;

namespace Symbolica.Deserialization;

internal sealed class TypeFactory : ITypeFactory
{
    private readonly LLVMTargetDataRef _targetData;

    public TypeFactory(LLVMTargetDataRef targetData)
    {
        _targetData = targetData;
    }

    public TType Create<TType>(LLVMTypeRef type)
        where TType : class, IType
    {
        return Create(type) as TType ?? throw new IncompatibleTypeException(typeof(TType), type.Kind);
    }

    private IType Create(LLVMTypeRef type)
    {
        return type.Kind switch
        {
            LLVMTypeKind.LLVMStructTypeKind => new StructType(
                type.GetStoreSize(_targetData),
                Enumerable.Range(0, (int) type.StructElementTypesCount)
                    .Select(e => type.GetElementOffset(_targetData, (uint) e))
                    .ToArray(),
                type.StructElementTypes
                    .Select(Create)
                    .ToArray()),
            LLVMTypeKind.LLVMArrayTypeKind => new ArrayType(type.ArrayLength, Create(type.ElementType)),
            _ => new SingleValueType(type.GetStoreSize(_targetData))
        };
    }
}
