using System.Linq;
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

    public ISavedFrame? TryGet(ISpace space, IExpression continuation, bool useJumpBuffer)
    {
        return _points
            .Where(p => p.IsMatch(space, continuation, useJumpBuffer))
            .Select(p => p.Frame)
            .FirstOrDefault();
    }

    public static IPersistentJumps Create(ICollectionFactory collectionFactory)
    {
        return new PersistentJumps(collectionFactory.CreatePersistentStack<Point>());
    }

    private readonly struct Point
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
