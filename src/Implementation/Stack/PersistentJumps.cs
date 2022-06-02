using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Implementation.Stack;

internal sealed class PersistentJumps : IPersistentJumps
{
    private readonly ICollectionFactory _collectionFactory;
    private readonly IPersistentStack<Point> _points;
    private readonly Lazy<int> _equivalencyHash;
    private readonly Lazy<int> _mergeHash;

    private PersistentJumps(ICollectionFactory collectionFactory, IPersistentStack<Point> points)
    {
        _collectionFactory = collectionFactory;
        _points = points;
        _equivalencyHash = new(() =>
        {
            var hash = new HashCode();
            foreach (var point in _points)
                hash.Add(point.GetEquivalencyHash());
            return hash.ToHashCode();
        });
        _mergeHash = new(() =>
        {
            var hash = new HashCode();
            foreach (var point in _points)
                hash.Add(point.GetMergeHash());
            return hash.ToHashCode();
        });
    }

    public IPersistentJumps Add(IExpression continuation, bool useJumpBuffer, ISavedFrame frame)
    {
        return new PersistentJumps(
            _collectionFactory,
            _points.Push(new Point(continuation, useJumpBuffer, frame)));
    }

    public Result<ISavedFrame> TryGet(ISpace space, IExpression continuation, bool useJumpBuffer)
    {
        foreach (var point in _points)
            if (point.IsMatch(space, continuation, useJumpBuffer))
                return Result<ISavedFrame>.Success(point.Frame);

        return Result<ISavedFrame>.Failure();
    }

    public static IPersistentJumps Create(ICollectionFactory collectionFactory)
    {
        return new PersistentJumps(
            collectionFactory,
            collectionFactory.CreatePersistentStack<Point>());
    }

    public (HashSet<ExpressionSubs> subs, bool) IsEquivalentTo(IPersistentJumps other)
    {
        return other is PersistentJumps pj
            ? _points.IsSequenceEquivalentTo<ExpressionSubs, Point>(pj._points)
            : (new(), false);
    }

    public object ToJson()
    {
        return _points.Select(p => p.ToJson()).ToArray();
    }

    public int GetEquivalencyHash()
    {
        return _equivalencyHash.Value;
    }

    public int GetMergeHash()
    {
        return _mergeHash.Value;
    }

    public bool TryMerge(IPersistentJumps other, IExpression predicate, [MaybeNullWhen(false)] out IPersistentJumps merged)
    {
        if (other is PersistentJumps pj && _points.TryMerge(pj._points, predicate, out var mergedPoints))
        {
            merged = new PersistentJumps(
                _collectionFactory,
                _collectionFactory.CreatePersistentStack(mergedPoints));
            return true;
        }
        merged = null;
        return false;
    }

    private readonly struct Point : IEquivalent<ExpressionSubs, Point>, IMergeable<Point>
    {
        private readonly IExpression _continuation;
        private readonly bool _useJumpBuffer;

        public Point(IExpression continuation, bool useJumpBuffer, ISavedFrame frame)
        {
            _continuation = continuation;
            _useJumpBuffer = useJumpBuffer;
            Frame = frame;
        }

        public ISavedFrame Frame { get; }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public (HashSet<ExpressionSubs> subs, bool) IsEquivalentTo(Point other)
        {
            return _continuation.IsEquivalentTo(other._continuation).ToHashSet()
                .And(Frame.IsEquivalentTo(other.Frame))
                .And((new(), _useJumpBuffer == other._useJumpBuffer));
        }

        public int GetEquivalencyHash()
        {
            return HashCode.Combine(
                _continuation.GetEquivalencyHash(),
                _useJumpBuffer,
                Frame.GetEquivalencyHash());
        }

        public bool IsMatch(ISpace space, IExpression continuation, bool useJumpBuffer)
        {
            return _useJumpBuffer == useJumpBuffer && IsMatch(space, continuation);
        }

        public object ToJson()
        {
            return new
            {
                Continuation = _continuation.ToJson(),
                Frame = Frame.ToJson(),
                UseJumpBuffer = _useJumpBuffer
            };
        }

        public int GetMergeHash()
        {
            return HashCode.Combine(
                _continuation.GetMergeHash(),
                _useJumpBuffer,
                Frame.GetMergeHash());
        }

        public bool TryMerge(Point other, IExpression predicate, out Point merged)
        {
            if (_continuation.TryMerge(other._continuation, predicate, out var mergedContinuation)
                && _useJumpBuffer == other._useJumpBuffer
                && Frame.TryMerge(other.Frame, predicate, out var mergedFrame))
            {
                merged = new Point(mergedContinuation, _useJumpBuffer, mergedFrame);
                return true;
            }

            merged = new();
            return false;
        }

        public override string? ToString()
        {
            return base.ToString();
        }

        private bool IsMatch(ISpace space, IExpression continuation)
        {
            var isEqual = _continuation.Equal(continuation);
            using var proposition = isEqual.GetProposition(space);

            return !proposition.CanBeFalse();
        }
    }
}
