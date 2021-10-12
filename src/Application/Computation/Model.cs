using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Z3;
using Symbolica.Computation;

namespace Symbolica.Application.Computation
{
    internal sealed class Model : IModel
    {
        private readonly Context _context;
        private readonly Solver _solver;

        private Model(Context context, Solver solver)
        {
            _context = context;
            _solver = solver;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public bool IsSatisfiable(Func<Context, BoolExpr> assertion)
        {
            // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
            return _solver.Check(assertion(_context)) switch
            {
                Status.UNSATISFIABLE => false,
                Status.SATISFIABLE => true,
                _ => throw new Exception("Satisfiability is unknown.")
            };
        }

        public Expr Evaluate(Func<Context, Expr> func)
        {
            return _solver.Check() == Status.SATISFIABLE
                ? _solver.Model.Eval(func(_context), true)
                : throw new Exception("The model cannot be evaluated.");
        }

        public IEnumerable<KeyValuePair<string, string>> Evaluate()
        {
            return _solver.Check() == Status.SATISFIABLE
                ? _solver.Model.Consts.Select(
                    p => new KeyValuePair<string, string>(p.Key.Name.ToString(), p.Value.ToString()))
                : Enumerable.Empty<KeyValuePair<string, string>>();
        }

        public static IModel Create(IEnumerable<Func<Context, BoolExpr>> assertions)
        {
            var context = new Context();
            var solver = context.MkSolver(context.MkTactic("smt"));
            solver.Assert(assertions.Select(a => a(context)).ToArray());

            return new Model(context, solver);
        }
    }
}
