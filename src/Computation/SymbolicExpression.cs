using System;
using System.Collections.Generic;
using System.Linq;
using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal sealed class SymbolicExpression : Expression
    {
        private SymbolicExpression(IContextFactory contextFactory, ICollectionFactory collectionFactory,
            IValue value, IValue[] constraints)
            : base(contextFactory, collectionFactory)
        {
            Value = value;
            Constraints = constraints;
        }

        public override Bits Size => Value.Size;
        public override IValue Value { get; }
        public override IValue[] Constraints { get; }

        public override IExpression Read(IExpression offset, Bits size)
        {
            return LogicalShiftRight(offset).Truncate(size);
        }

        public static IExpression Create(IContextFactory contextFactory, ICollectionFactory collectionFactory,
            IValueExpression x, Func<IValue, IValue> func)
        {
            return new SymbolicExpression(contextFactory, collectionFactory,
                func(x.Value), x.Constraints);
        }

        public static IExpression Create(IContextFactory contextFactory, ICollectionFactory collectionFactory,
            IValueExpression x, IValueExpression y, Func<IValue, IValue, IValue> func)
        {
            return new SymbolicExpression(contextFactory, collectionFactory,
                func(x.Value, y.Value), x.Constraints.Concat(y.Constraints).ToArray());
        }

        public static IExpression Create(IContextFactory contextFactory, ICollectionFactory collectionFactory,
            IValueExpression x, IValueExpression y, IValueExpression z, Func<IValue, IValue, IValue, IValue> func)
        {
            return new SymbolicExpression(contextFactory, collectionFactory,
                func(x.Value, y.Value, z.Value), x.Constraints.Concat(y.Constraints.Concat(z.Constraints)).ToArray());
        }

        public static SymbolicExpression Create(IContextFactory contextFactory, ICollectionFactory collectionFactory,
            IValue value, IEnumerable<Func<IExpression, IExpression>> constraints)
        {
            var unconstrained = new SymbolicExpression(contextFactory, collectionFactory,
                value, Array.Empty<IValue>());

            return new SymbolicExpression(contextFactory, collectionFactory,
                value, constraints
                    .Select(c => ((IValueExpression) c(unconstrained)).Value)
                    .ToArray());
        }
    }
}
