using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Collection;
using Symbolica.Expression;
using Symbolica.Implementation.Exceptions;
using Symbolica.Implementation.Memory;

namespace Symbolica.Implementation.Stack;

internal sealed class FrameFactory : IFrameFactory
{
    private readonly ICollectionFactory _collectionFactory;
    private readonly IVariadicAbi _variadicAbi;

    public FrameFactory(IVariadicAbi variadicAbi, ICollectionFactory collectionFactory)
    {
        _variadicAbi = variadicAbi;
        _collectionFactory = collectionFactory;
    }

    public IPersistentFrame Create(ISpace space, IMemoryProxy memory, ICaller caller, IInvocation invocation)
    {
        var vaList = invocation.Varargs.Any()
            ? _variadicAbi.PassOnStack(space, memory, invocation.Varargs)
            : _variadicAbi.DefaultVaList;

        return Create(caller, invocation.Definition, invocation.Formals, vaList);
    }

    public IPersistentFrame CreateInitial(IDefinition main)
    {
        return Create(new InitialCaller(), main, new InitialArguments(), _variadicAbi.DefaultVaList);
    }

    private IPersistentFrame Create(ICaller caller, IDefinition definition, IArguments formals, IVaList vaList)
    {
        return new PersistentFrame(caller, formals, vaList,
            PersistentJumps.Create(_collectionFactory), PersistentProgramCounter.Create(definition),
            PersistentVariables.Create(_collectionFactory), PersistentAllocations.Create(_collectionFactory));
    }

    private sealed class InitialCaller : ICaller
    {
        public InstructionId Id => throw new ImplementationException("The initial 'main' call has no caller.");
        public Bits Size => (Bits) 16U;
        public IAttributes ReturnAttributes => new InitialReturnAttributes();

        public void Return(IState state)
        {
        }

        private sealed class InitialReturnAttributes : IAttributes
        {
            public bool IsSignExtended => false;
        }
    }

    private sealed class InitialArguments : IArguments
    {
        public IExpression Get(int index)
        {
            throw new UnboundMainArgumentsException();
        }

        public IEnumerator<IExpression> GetEnumerator()
        {
            return Enumerable.Empty<IExpression>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
