using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal sealed class SymbolicProposition : IProposition
    {
        private readonly SymbolicBool _assertion;
        private readonly IModel _model;
        private readonly SymbolicBool _negation;
        private readonly IPersistentSpace _space;

        private SymbolicProposition(IPersistentSpace space, IModel model, SymbolicBool assertion, SymbolicBool negation)
        {
            _space = space;
            _model = model;
            _assertion = assertion;
            _negation = negation;
        }

        public ISpace FalseSpace => _space.Assert(_negation);
        public ISpace TrueSpace => _space.Assert(_assertion);
        public bool CanBeFalse => _negation.IsSatisfiable(_model);
        public bool CanBeTrue => _assertion.IsSatisfiable(_model);

        public void Dispose()
        {
            _model.Dispose();
        }

        public static IProposition Create(IPersistentSpace space, SymbolicBool assertion, SymbolicBool[] constraints)
        {
            var model = space.GetModel(constraints);

            return new SymbolicProposition(space, model, assertion, assertion.Not());
        }
    }
}