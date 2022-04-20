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
        return type.Kind == LLVMTypeKind.LLVMPointerTypeKind
            ? new PointerType(type.GetSize(_targetData), CreateNonPointer(type.ElementType))
            : CreateNonPointer(type);
    }

    private IType CreateNonPointer(LLVMTypeRef type)
    {
        return type.Kind switch
        {
            LLVMTypeKind.LLVMStructTypeKind => new StructType(
                type.GetStoreSize(_targetData).ToBits(),
                Enumerable.Range(0, (int) type.StructElementTypesCount)
                    .Select(e => type.GetElementOffset(_targetData, (uint) e).ToBits())
                    .ToArray(),
                type.StructElementTypes
                    .Select(CreateNonPointer)
                    .ToArray()),
            LLVMTypeKind.LLVMArrayTypeKind => new ArrayType(type.ArrayLength, CreateNonPointer(type.ElementType)),
            _ => new SingleValueType(type.GetStoreSize(_targetData).ToBits())
        };
    }
}
