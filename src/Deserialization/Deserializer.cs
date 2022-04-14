using LLVMSharp.Interop;
using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Representation;

namespace Symbolica.Deserialization;

internal sealed class Deserializer : IDeserializer
{
    private readonly IDeclarationFactory _declarationFactory;
    private readonly IUnsafeContext _unsafeContext;

    public Deserializer(IUnsafeContext unsafeContext, IDeclarationFactory declarationFactory)
    {
        _unsafeContext = unsafeContext;
        _declarationFactory = declarationFactory;
    }

    public IModule DeserializeModule(byte[] bytes)
    {
        var memoryBuffer = _unsafeContext.GetMemoryBuffer(bytes);
        using var module = LLVMContextRef.Create().ParseBitcode(memoryBuffer);
        var targetData = _unsafeContext.GetTargetData(module);

        var idFactory = new IdFactory();
        var structTypeFactory = new StructTypeFactory(targetData);

        var operandFactory = new OperandFactory(targetData, idFactory, _unsafeContext, structTypeFactory);
        var instructionFactory = new InstructionFactory(targetData, idFactory, _unsafeContext, operandFactory);

        var functionFactory = new FunctionFactory(targetData, idFactory, instructionFactory, _declarationFactory);
        var globalFactory = new GlobalFactory(targetData, idFactory, instructionFactory, operandFactory);

        var moduleFactory = new ModuleFactory(structTypeFactory, functionFactory, globalFactory);

        return moduleFactory.Create(module, (Bytes) _unsafeContext.GetPointerSize(targetData));
    }
}
