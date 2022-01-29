using LLVMSharp.Interop;
using Symbolica.Abstraction;
using Symbolica.Deserialization.Extensions;
using Symbolica.Representation;

namespace Symbolica.Deserialization;

internal sealed class GlobalFactory : IGlobalFactory
{
    private readonly IIdFactory _idFactory;
    private readonly IInstructionFactory _instructionFactory;
    private readonly IOperandFactory _operandFactory;
    private readonly LLVMTargetDataRef _targetData;

    public GlobalFactory(LLVMTargetDataRef targetData, IIdFactory idFactory,
        IInstructionFactory instructionFactory, IOperandFactory operandFactory)
    {
        _targetData = targetData;
        _idFactory = idFactory;
        _instructionFactory = instructionFactory;
        _operandFactory = operandFactory;
    }

    public IGlobal Create(LLVMValueRef global)
    {
        var id = (GlobalId) _idFactory.GetOrCreate(global.Handle);
        var size = global.TypeOf.ElementType.GetStoreSize(_targetData).ToBits();

        return global.Initializer == default
            ? new UninitializedGlobal(id, size)
            : new Global(id, size, _operandFactory.Create(global.Initializer, _instructionFactory));
    }
}
