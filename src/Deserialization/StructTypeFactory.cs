using System.Linq;
using LLVMSharp.Interop;
using Symbolica.Abstraction;
using Symbolica.Deserialization.Extensions;
using Symbolica.Representation;

namespace Symbolica.Deserialization;

internal sealed class StructTypeFactory : IStructTypeFactory
{
    private readonly LLVMTargetDataRef _targetData;

    public StructTypeFactory(LLVMTargetDataRef targetData)
    {
        _targetData = targetData;
    }

    public IStructType Create(LLVMTypeRef type)
    {
        return new StructType(
            type.GetStoreSize(_targetData).ToBits(),
            type.StructElementTypes
                .Select((_, i) => type.GetElementOffset(_targetData, (uint) i).ToBits())
                .ToArray());
    }
}
