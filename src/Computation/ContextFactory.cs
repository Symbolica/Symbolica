using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Z3;
using Symbolica.Computation.Exceptions;

namespace Symbolica.Computation;

internal static class ContextFactory
{
    public static IContext Create()
    {
        return new ContextProxy();
    }

    private sealed class ContextProxy : IContext
    {
        private static readonly ConcurrentBag<Context> Contexts = new();
        private readonly Context _context;
        private readonly ISet<string> _names;
        private readonly Lazy<Solver> _solver;

        public ContextProxy()
        {
            _names = new HashSet<string>();
            _context = CreateContext();
            _solver = new Lazy<Solver>(CreateSolver);
        }

        public void Dispose()
        {
            Contexts.Add(_context);
        }

        public void Assert(IEnumerable<BoolExpr> assertions)
        {
            _solver.Value.Add(assertions);
        }

        public void Assert(string name, IEnumerable<BoolExpr> assertions)
        {
            if (_names.Contains(name))
                return;

            _names.Add(name);
            Assert(assertions);
        }

        public Status Check(BoolExpr assertion)
        {
            return _solver.Value.Check(assertion);
        }

        public BitVecNum Evaluate(BitVecExpr variable)
        {
            return (BitVecNum) CreateModel(_solver.Value).Eval(variable, true);
        }

        public IEnumerable<KeyValuePair<FuncDecl, Expr>> Evaluate()
        {
            return CreateModel(_solver.Value).Consts;
        }

        public TSort CreateSort<TSort>(Func<Context, TSort> func)
            where TSort : Sort
        {
            return Create(func);
        }

        public TExpr CreateExpr<TExpr>(Func<Context, TExpr> func)
            where TExpr : Expr
        {
            return Create(func);
        }

        private Solver CreateSolver()
        {
            return Create(c => c.MkSolver(c.MkTactic("smt")));
        }

        private TResult Create<TResult>(Func<Context, TResult> func)
        {
            return func(_context);
        }

        private static Model CreateModel(Solver solver)
        {
            var status = solver.Check();

            return status == Status.SATISFIABLE
                ? solver.Model
                : throw new InvalidModelException(status);
        }

        private static Context CreateContext()
        {
            return Contexts.TryTake(out var context)
                ? context
                : new Context();
        }
    }
}
