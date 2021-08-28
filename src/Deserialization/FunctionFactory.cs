using System.Linq;
using LLVMSharp.Interop;
using Symbolica.Abstraction;
using Symbolica.Deserialization.Extensions;
using Symbolica.Representation;
using Symbolica.Representation.Functions;

namespace Symbolica.Deserialization
{
    internal sealed class FunctionFactory : IFunctionFactory
    {
        private readonly IIdFactory _idFactory;
        private readonly IInstructionFactory _instructionFactory;
        private readonly LLVMTargetDataRef _targetData;

        public FunctionFactory(LLVMTargetDataRef targetData, IIdFactory idFactory,
            IInstructionFactory instructionFactory)
        {
            _targetData = targetData;
            _idFactory = idFactory;
            _instructionFactory = instructionFactory;
        }

        public IFunction Create(LLVMValueRef function)
        {
            var id = (FunctionId) _idFactory.GetOrCreate(function.Handle);
            var parameters = new Parameters(function.Params
                .Select(p => new Parameter(p.TypeOf.GetStoreSize(_targetData).ToBits()))
                .ToArray());

            return function.IsDeclaration
                ? DeclarationFactory.Create(function.Name, id, parameters)
                : CreateDefinition(id, parameters, function);
        }

        private IFunction CreateDefinition(FunctionId id, IParameters parameters, LLVMValueRef function)
        {
            return Definition.Create(
                id,
                function.Name,
                parameters,
                function.TypeOf.ElementType.IsFunctionVarArg,
                (BasicBlockId) _idFactory.GetOrCreate(function.EntryBasicBlock.Handle),
                function.BasicBlocks.Select(CreateBasicBlock));
        }

        private IBasicBlock CreateBasicBlock(LLVMBasicBlockRef basicBlock)
        {
            return new BasicBlock(
                (BasicBlockId) _idFactory.GetOrCreate(basicBlock.Handle),
                basicBlock.GetInstructions()
                    .Select(i => _instructionFactory.Create(i, i.InstructionOpcode))
                    .ToArray());
        }
    }
}
