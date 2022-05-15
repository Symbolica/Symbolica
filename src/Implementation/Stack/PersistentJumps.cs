using System.Collections.Generic;
using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Implementation.Stack;

internal sealed class PersistentJumps : IPersistentJumps
{
    private readonly IPersistentStack<Point> _points;

    private PersistentJumps(IPersistentStack<Point> points)
    {
        _points = points;
    }

    public IPersistentJumps Add(IExpression continuation, bool useJumpBuffer, ISavedFrame frame)
    {
        return new PersistentJumps(_points.Push(new Point(continuation, useJumpBuffer, frame)));
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
        return new PersistentJumps(collectionFactory.CreatePersistentStack<Point>());
    }

    public (HashSet<(IExpression, IExpression)> subs, bool) IsEquivalentTo(IPersistentJumps other)
    {
        return other is PersistentJumps pj
            ? _points.IsSequenceEquivalentTo<IExpression, Point>(pj._points)
            : (new(), false);
    }

    private readonly struct Point : IMergeable<IExpression, Point>
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

        public (HashSet<(IExpression, IExpression)> subs, bool) IsEquivalentTo(Point other)
        {
            return other is Point p && _useJumpBuffer == other._useJumpBuffer
                ? _continuation.IsEquivalentTo(other._continuation)
                    .And(Frame.IsEquivalentTo(other.Frame))
                : (new(), false);
        }

        public bool IsMatch(ISpace space, IExpression continuation, bool useJumpBuffer)
        {
            return _useJumpBuffer == useJumpBuffer && IsMatch(space, continuation);
        }

        private bool IsMatch(ISpace space, IExpression continuation)
        {
            var isEqual = _continuation.Equal(continuation);
            using var proposition = isEqual.GetProposition(space);

            return !proposition.CanBeFalse();
        }
    }
}
