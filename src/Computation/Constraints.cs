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
    // private static readonly TextWriterTraceListener _solverTracer = new(File.CreateText("/Users/Choc/code/symbolica/symbolica/.traces/solves.txt"));
    // private static readonly TextWriterTraceListener _bigAssertionTracer = new(File.CreateText("/Users/Choc/code/symbolica/symbolica/.traces/big-assertions.txt"));

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
        _context.Assert(assertions.Select(a => a.AsBool(_context)).ToList());
    }

    public bool IsSatisfiable(IValue assertion)
    {
        string PrintAssertion(IValue assertion, int depth) => $"{Environment.NewLine}{string.Join("", Enumerable.Repeat(" ", depth * 2))}{assertion.GetType().Name}{(assertion.PrintedValue is null ? string.Empty : $" ({assertion.PrintedValue})")}{string.Join("", assertion.Children.Select(v => PrintAssertion(v, depth + 1)))}";

        int AssertionCount(IValue assertion) => 1 + assertion.Children.Sum(AssertionCount);

        var id = Guid.NewGuid();

        // if (AssertionCount(assertion) > 50)
        // {
        //     var printedExpr = PrintAssertion(assertion, 0);
        //     _bigAssertionTracer.WriteLine($"{id}, {printedExpr}");
        //     _bigAssertionTracer.Flush();
        // }

        // _solverTracer.WriteLine($"{DateTimeOffset.Now}, Thread {Environment.CurrentManagedThreadId}, {id}, Starting satisfiability check.");
        // _solverTracer.Flush();
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        using var expr = assertion.AsBool(_context);
        stopwatch.Stop();
        stopwatch.Restart();
        var status = _context.Check(expr);
        stopwatch.Stop();
        // _solverTracer.WriteLine($"{DateTimeOffset.Now}, Thread {Environment.CurrentManagedThreadId}, {id}, Finished satisfiability check in {stopwatch.Elapsed}.");
        // _solverTracer.Flush();

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
        bool IsFPNaN()
        {
            if (value is not Float)
                return false;

            using var flt = value.AsFloat(_context);
            using var simplified = flt.Simplify();
            return simplified.IsFPNaN;
        }
        return IsFPNaN()
            ? value.Size.GetNan(_context)
            : AsConstant(value);
    }

    public BigInteger GetValue(IValue value)
    {
        using var expr = value.AsBitVector(_context);
        var id = Guid.NewGuid();
        // _solverTracer.WriteLine($"{DateTimeOffset.Now}, Thread {Environment.CurrentManagedThreadId}, {id}, Starting evaluation.");
        // _solverTracer.Flush();
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        using var result = _context.Evaluate(expr);
        stopwatch.Stop();
        // _solverTracer.WriteLine($"{DateTimeOffset.Now}, Thread {Environment.CurrentManagedThreadId}, {id}, Finished evaluation in {stopwatch.Elapsed}.");
        // _solverTracer.Flush();
        return result.BigInteger;
    }

    public IEnumerable<KeyValuePair<string, string>> GetValues()
    {
        return _context.Evaluate().Select(p =>
        {
            using var func = p.Key;
            using var expr = p.Value;
            return new KeyValuePair<string, string>(func.Name.ToString(), expr.ToString());
        });
    }

    private BigInteger AsConstant(IValue value)
    {
        using var bitVector = value.AsBitVector(_context);
        using var expr = bitVector.Simplify();

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
