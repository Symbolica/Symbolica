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

    public void Assert(IEnumerable<IValue> assertions)
    {
        _context.Assert(assertions.Select(a => a.AsBool(_context)));
    }

    public bool IsSatisfiable(IValue assertion)
    {
        var id = Guid.NewGuid();

        _assertionTracer.WriteLine($"{DateTimeOffset.Now}, Thread {Environment.CurrentManagedThreadId}, {id}, Starting assertion construction.");
        _assertionTracer.Flush();
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var boolExpr = assertion.AsBool(_context);
        stopwatch.Stop();
        _assertionTracer.WriteLine($"{DateTimeOffset.Now}, Thread {Environment.CurrentManagedThreadId}, {id}, Finished assertion construction in {stopwatch.Elapsed}.");
        _assertionTracer.Flush();

        bool writtenAssertion = false;
        if (stopwatch.Elapsed > TimeSpan.FromMinutes(1))
        {
            writtenAssertion = true;
            var sExpr = boolExpr.SExpr();
            _bigAssertionTracer.WriteLine($"{id}, {sExpr}");
            _bigAssertionTracer.Flush();
        }

        _solverTracer.WriteLine($"{DateTimeOffset.Now}, Thread {Environment.CurrentManagedThreadId}, {id}, Starting satisfiability check.");
        _solverTracer.Flush();
        stopwatch.Restart();
        var status = _context.Check(boolExpr);
        stopwatch.Stop();
        _solverTracer.WriteLine($"{DateTimeOffset.Now}, Thread {Environment.CurrentManagedThreadId}, {id}, Finished satisfiability check in {stopwatch.Elapsed}.");
        _solverTracer.Flush();

        if (!writtenAssertion && stopwatch.Elapsed > TimeSpan.FromMinutes(1))
        {
            var sExpr = boolExpr.SExpr();
            _bigAssertionTracer.WriteLine($"{id}, {sExpr}");
            _bigAssertionTracer.Flush();
        }

        return status switch
        {
            Status.UNSATISFIABLE => false,
            Status.SATISFIABLE => true,
            Status.UNKNOWN => throw new UnexpectedSatisfiabilityException(status),
            _ => throw new UnexpectedSatisfiabilityException(status)
        };
    }

    public BigInteger GetConstant(IValue value)
    {
        return value is Float && value.AsFloat(_context).Simplify().IsFPNaN
            ? value.Size.GetNan(_context)
            : AsConstant(value);
    }

    public BigInteger GetValue(IValue value)
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

    public IEnumerable<KeyValuePair<string, string>> GetValues()
    {
        return _context.Evaluate().Select(p =>
            new KeyValuePair<string, string>(p.Key.Name.ToString(), p.Value.ToString()));
    }

    private BigInteger AsConstant(IValue value)
    {
        var expr = value.AsBitVector(_context).Simplify();

        var constant = expr.IsNumeral
            ? ((BitVecNum) expr).BigInteger
            : throw new IrreducibleSymbolicExpressionException();

        return constant == GetValue(value)
            ? constant
            : throw new IrreducibleSymbolicExpressionException();
    }

    public static IConstraints Create<TContext>()
        where TContext : IContext, new()
    {
        return new Constraints(new TContext());
    }
}
