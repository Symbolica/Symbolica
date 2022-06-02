using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Collection;
using Symbolica.Expression;
using Symbolica.Implementation.Exceptions;

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

    public IPersistentFrame Create(ISpace space, IMemory memory, ICaller caller, IInvocation invocation)
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

        public int GetEquivalencyHash()
        {
            return HashCode.Combine(GetType().Name);
        }

        public int GetMergeHash()
        {
            return HashCode.Combine(GetType().Name);
        }

        public (HashSet<ExpressionSubs> subs, bool) IsEquivalentTo(ICaller other)
        {
            return (new(), other is InitialCaller);
        }

        public void Return(IState state)
        {
        }

        public object ToJson()
        {
            return new
            {
                Type = GetType().Name
            };
        }

        public bool TryMerge(ICaller other, IExpression predicate, [MaybeNullWhen(false)] out ICaller merged)
        {
            merged = this;
            return other is InitialCaller;
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

        public int GetEquivalencyHash()
        {
            return HashCode.Combine(GetType().Name);
        }

        public int GetMergeHash()
        {
            return HashCode.Combine(GetType().Name);
        }

        public (HashSet<ExpressionSubs> subs, bool) IsEquivalentTo(IArguments other)
        {
            return (new(), other is InitialArguments);
        }

        public object ToJson()
        {
            return new
            {
                Type = GetType().Name
            };
        }

        public bool TryMerge(IArguments other, IExpression predicate, [MaybeNullWhen(false)] out IArguments merged)
        {
            merged = this;
            return other is InitialArguments;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
