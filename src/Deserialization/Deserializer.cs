using LLVMSharp.Interop;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Deserialization
{
    public static class Deserializer
    {
        public static IModule DeserializeModule(byte[] bytes)
        {
            return DeserializeModule(new UnsafeContext(), bytes);
        }

        private static IModule DeserializeModule(IUnsafeContext unsafeContext, byte[] bytes)
        {
            var memoryBuffer = unsafeContext.GetMemoryBuffer(bytes);
            using var module = LLVMContextRef.Create().ParseBitcode(memoryBuffer);
            var targetData = unsafeContext.GetTargetData(module);

            var idFactory = new IdFactory();

            var operandFactory = new OperandFactory(targetData, idFactory, unsafeContext);
            var instructionFactory = new InstructionFactory(targetData, idFactory, unsafeContext, operandFactory);

            var structTypeFactory = new StructTypeFactory(targetData);
            var functionFactory = new FunctionFactory(targetData, idFactory, instructionFactory);
            var globalFactory = new GlobalFactory(targetData, idFactory, instructionFactory, operandFactory);

            var moduleFactory = new ModuleFactory(structTypeFactory, functionFactory, globalFactory);

            return moduleFactory.Create(module, (Bytes) unsafeContext.GetPointerSize(targetData));
        }
    }
}
