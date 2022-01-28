using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Computation.Exceptions;

namespace Symbolica.Computation;

internal sealed class Constraints : IConstraints
{
    private static readonly TextWriterTraceListener _solverTracer = new(File.CreateText("/Users/Choc/code/symbolica/symbolica/.traces/solves.txt"));
    private static readonly TextWriterTraceListener _assertionTracer = new(File.CreateText("/Users/Choc/code/symbolica/symbolica/.traces/assertions.txt"));
    private static readonly TextWriterTraceListener _simplifiedAssertionTracer = new(File.CreateText("/Users/Choc/code/symbolica/symbolica/.traces/simplified-assertions.txt"));
    private static readonly TextWriterTraceListener _bigAssertionTracer = new(File.CreateText("/Users/Choc/code/symbolica/symbolica/.traces/big-assertions.txt"));

    private readonly IContext _context;

    private Constraints(IContext context)
    {
        _context = context;
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public bool IsSatisfiable(IValue assertion)
    {
        string PrintAssertion(IValue assertion, int depth) => $"{Environment.NewLine}{string.Join("", Enumerable.Repeat(" ", depth * 2))}{assertion.GetType().Name}{(assertion.PrintedValue is null ? string.Empty : $" ({assertion.PrintedValue})")}{string.Join("", assertion.Children.Select(v => PrintAssertion(v, depth + 1)))}";

        int AssertionCount(IValue assertion) => 1 + assertion.Children.Sum(AssertionCount);

        var id = Guid.NewGuid();

        //  var boolExpr = assertion.AsBool(context);

        // _assertionTracer.WriteLine($"{id}, {boolExpr.SExpr()}");
        // _assertionTracer.Flush();

        // _simplifiedAssertionTracer.WriteLine($"{id}, {boolExpr.Simplify().SExpr()}");
        // _simplifiedAssertionTracer.Flush();

        if (AssertionCount(assertion) > 50)
        {
            var printedExpr = PrintAssertion(assertion, 0);
            _bigAssertionTracer.WriteLine($"{id}, {printedExpr}");
            _bigAssertionTracer.Flush();
        }

        _solverTracer.WriteLine($"{DateTimeOffset.Now}, Thread {Environment.CurrentManagedThreadId}, {id}, Starting satisfiability check.");
        _solverTracer.Flush();
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var status = _context.Check(assertion.AsBool(_context));
        stopwatch.Stop();
        _solverTracer.WriteLine($"{DateTimeOffset.Now}, Thread {Environment.CurrentManagedThreadId}, {id}, Finished satisfiability check in {stopwatch.Elapsed}.");
        _solverTracer.Flush();

        return status switch
        {
            Status.UNSATISFIABLE => false,
            Status.SATISFIABLE => true,
            Status.UNKNOWN => throw new UnexpectedSatisfiabilityException(status),
            _ => throw new UnexpectedSatisfiabilityException(status)
        };
    }

    public BigInteger Evaluate(IValue value)
    {
        var expr = value.AsBitVector(_context);
        var id = Guid.NewGuid();
        _solverTracer.WriteLine($"{DateTimeOffset.Now}, Thread {Environment.CurrentManagedThreadId}, {id}, Starting evaluation.");
        _solverTracer.Flush();
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var result = _context.Evaluate(expr).BigInteger;
        stopwatch.Stop();
        _solverTracer.WriteLine($"{DateTimeOffset.Now}, Thread {Environment.CurrentManagedThreadId}, {id}, Finished evaluation in {stopwatch.Elapsed}.");
        _solverTracer.Flush();
        return result;
    }

    public IEnumerable<KeyValuePair<string, string>> Evaluate()
    {
        return _context.Evaluate().Select(p =>
            new KeyValuePair<string, string>(p.Key.Name.ToString(), p.Value.ToString()));
    }

    public static IConstraints Create<TContext>(IEnumerable<IValue> assertions)
        where TContext : IContext, new()
    {
        var context = new TContext();
        context.Assert(assertions.Select(a => a.AsBool(context)));

        return new Constraints(context);
    }
}
