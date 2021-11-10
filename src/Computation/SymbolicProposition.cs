using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal sealed class SymbolicProposition : IProposition
    {
        private readonly IBool _assertion;
        private readonly IModel _model;
        private readonly IBool _negation;
        private readonly IPersistentSpace _space;

        private SymbolicProposition(IPersistentSpace space, IModel model, IBool assertion, IBool negation)
        {
            _space = space;
            _model = model;
            _assertion = assertion;
            _negation = negation;
        }

        public ISpace FalseSpace => _space.Assert(_negation);
        public ISpace TrueSpace => _space.Assert(_assertion);
        public bool CanBeFalse => _model.IsSatisfiable(_negation.Symbolic.Run);
        public bool CanBeTrue => _model.IsSatisfiable(_assertion.Symbolic.Run);

        public void Dispose()
        {
            _model.Dispose();
        }

        public static IProposition Create(IPersistentSpace space, IBool assertion, IBool[] constraints)
        {
            var model = space.GetModel(constraints);

            return new SymbolicProposition(space, model, assertion, assertion.Not());
        }
    }
}
