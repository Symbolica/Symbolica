using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Collection;
using Symbolica.Expression;
using Symbolica.Implementation.Exceptions;
using Symbolica.Implementation.Memory;

namespace Symbolica.Implementation.Stack
{
    [Serializable]
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
            var address = invocation.Varargs.Any()
                ? _variadicAbi.PassOnStack(space, memory, invocation.Varargs)
                : space.CreateConstant(space.PointerSize, BigInteger.Zero);

            return Create(caller, invocation.Definition, invocation.Formals, new CreateVaList(_variadicAbi, address));
        }

        public IPersistentFrame CreateInitial(IDefinition main)
        {
            return Create(new InitialCaller(), main, new InitialArguments(), new CreateConstantAddressVaList(_variadicAbi));
        }

        private IPersistentFrame Create(ICaller caller, IDefinition definition, IArguments formals,
            IFunc<ISpace, IFunc<IStructType, IExpression>> vaList)
        {
            return new PersistentFrame(caller, formals, vaList,
                PersistentJumps.Create(_collectionFactory), PersistentProgramCounter.Create(definition),
                PersistentVariables.Create(_collectionFactory), PersistentAllocations.Create(_collectionFactory));
        }

        [Serializable]
        private class CreateConstantAddressVaList : IFunc<ISpace, IFunc<IStructType, IExpression>>
        {
            private readonly IVariadicAbi _variadicAbi;

            public CreateConstantAddressVaList(IVariadicAbi variadicAbi)
            {
                _variadicAbi = variadicAbi;
            }

            public IFunc<IStructType, IExpression> Run(ISpace space)
            {
                return new CreateOfSpace(_variadicAbi, space);
            }

            private class CreateOfSpace : IFunc<IStructType, IExpression>
            {
                private readonly IVariadicAbi _variadicAbi;
                private readonly ISpace _space;

                public CreateOfSpace(IVariadicAbi variadicAbi, ISpace space)
                {
                    _variadicAbi = variadicAbi;
                    _space = space;
                }

                public IExpression Run(IStructType structType)
                {
                    return _variadicAbi.CreateVaList(_space, structType, _space.CreateConstant(_space.PointerSize, BigInteger.Zero));
                }
            }
        }

        [Serializable]
        private class CreateVaList : IFunc<ISpace, IFunc<IStructType, IExpression>>
        {
            private readonly IVariadicAbi _variadicAbi;
            private readonly IExpression _address;

            public CreateVaList(IVariadicAbi variadicAbi, IExpression address)
            {
                _variadicAbi = variadicAbi;
                _address = address;
            }

            public IFunc<IStructType, IExpression> Run(ISpace space)
            {
                return new CreateOfSpace(_variadicAbi, space, _address);
            }

            private class CreateOfSpace : IFunc<IStructType, IExpression>
            {
                private readonly IVariadicAbi _variadicAbi;
                private readonly ISpace _space;
                private readonly IExpression _address;

                public CreateOfSpace(IVariadicAbi variadicAbi, ISpace space, IExpression address)
                {
                    _variadicAbi = variadicAbi;
                    _space = space;
                    _address = address;
                }

                public IExpression Run(IStructType structType)
                {
                    return _variadicAbi.CreateVaList(_space, structType, _address);
                }
            }
        }

        [Serializable]
        private sealed class InitialCaller : ICaller
        {
            public InstructionId Id => throw new ImplementationException("The initial 'main' call has no caller.");
            public Bits Size => (Bits)16U;
            public IAttributes ReturnAttributes => new InitialReturnAttributes();

            public void Return(IState state)
            {
            }

            private sealed class InitialReturnAttributes : IAttributes
            {
                public bool IsSignExtended => false;
            }
        }

        [Serializable]
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
}
