﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal sealed class PersistentSpace : IPersistentSpace
    {
        private readonly IPersistentStack<IBool> _assertions;
        private readonly ICollectionFactory _collectionFactory;
        private readonly IContextFactory _contextFactory;
        private readonly IModelFactory _modelFactory;
        private readonly ISymbolFactory _symbolFactory;
        private readonly bool _useSymbolicGarbage;

        private PersistentSpace(Bits pointerSize, bool useSymbolicGarbage,
            ISymbolFactory symbolFactory, IModelFactory modelFactory,
            IContextFactory contextFactory, ICollectionFactory collectionFactory,
            IPersistentStack<IBool> assertions)
        {
            PointerSize = pointerSize;
            _useSymbolicGarbage = useSymbolicGarbage;
            _symbolFactory = symbolFactory;
            _modelFactory = modelFactory;
            _contextFactory = contextFactory;
            _collectionFactory = collectionFactory;
            _assertions = assertions;
        }

        public Bits PointerSize { get; }

        public IPersistentSpace Assert(IBool assertion)
        {
            return new PersistentSpace(PointerSize, _useSymbolicGarbage,
                _symbolFactory, _modelFactory,
                _contextFactory, _collectionFactory,
                _assertions.Push(assertion));
        }

        public IModel GetModel(IBool[] constraints)
        {
            return _modelFactory.Create(_contextFactory, constraints.Concat(_assertions).Select(a => a.Symbolic));
        }

        public IExample GetExample()
        {
            return Example.Create(this);
        }

        public IExpression CreateConstant(Bits size, BigInteger value)
        {
            return Expression.Create(_contextFactory, _collectionFactory,
                ConstantUnsigned.Create(size, value), null);
        }

        public IExpression CreateConstantFloat(Bits size, string value)
        {
            return Expression.Create(_contextFactory, _collectionFactory,
                size.ParseFloat(value), null);
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

            return Expression.Create(_contextFactory, _collectionFactory,
                new SymbolicInteger(size, func), constraints);
        }

        public static ISpace Create(Bits pointerSize, bool useSymbolicGarbage,
            ISymbolFactory symbolFactory, IModelFactory modelFactory,
            IContextFactory contextFactory, ICollectionFactory collectionFactory)
        {
            return new PersistentSpace(pointerSize, useSymbolicGarbage,
                symbolFactory, modelFactory,
                contextFactory, collectionFactory,
                collectionFactory.CreatePersistentStack<IBool>());
        }
    }
}
