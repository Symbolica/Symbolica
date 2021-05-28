using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal sealed class PersistentSpace : IPersistentSpace
    {
        private readonly IPersistentStack<SymbolicBool> _assertions;
        private readonly ICollectionFactory _collectionFactory;
        private readonly IModelFactory _modelFactory;
        private readonly ISymbolFactory _symbolFactory;
        private readonly bool _useSymbolicGarbage;

        private PersistentSpace(Bits pointerSize, bool useSymbolicGarbage,
            ISymbolFactory symbolFactory, IModelFactory modelFactory, ICollectionFactory collectionFactory,
            IPersistentStack<SymbolicBool> assertions)
        {
            PointerSize = pointerSize;
            _useSymbolicGarbage = useSymbolicGarbage;
            _symbolFactory = symbolFactory;
            _modelFactory = modelFactory;
            _collectionFactory = collectionFactory;
            _assertions = assertions;
        }

        public Bits PointerSize { get; }

        public IPersistentSpace Assert(SymbolicBool assertion)
        {
            return new PersistentSpace(PointerSize, _useSymbolicGarbage,
                _symbolFactory, _modelFactory, _collectionFactory,
                _assertions.Push(assertion));
        }

        public IModel GetModel(SymbolicBool[] constraints)
        {
            return SymbolicBool.GetModel(_modelFactory, constraints.Concat(_assertions));
        }

        public IExample GetExample()
        {
            return Example.Create(this);
        }

        public IExpression CreateConstant(Bits size, BigInteger value)
        {
            return Expression.Create(_collectionFactory,
                ConstantUnsigned.Create(size, value), null);
        }

        public IExpression CreateGarbage(Bits size)
        {
            return _useSymbolicGarbage
                ? CreateSymbolic(size, null, null)
                : CreateConstant(size, BigInteger.Zero);
        }

        public IExpression CreateSymbolic(Bits size,
            string? name, IEnumerable<Func<IExpression, IExpression>>? constraints)
        {
            var func = _symbolFactory.Create(size, name);

            return Expression.Create(_collectionFactory,
                new SymbolicBitVector(size, func), constraints);
        }

        public static ISpace Create(Bits pointerSize, bool useSymbolicGarbage,
            ISymbolFactory symbolFactory, IModelFactory modelFactory, ICollectionFactory collectionFactory)
        {
            return new PersistentSpace(pointerSize, useSymbolicGarbage,
                symbolFactory, modelFactory, collectionFactory,
                collectionFactory.CreatePersistentStack<SymbolicBool>());
        }
    }
}
