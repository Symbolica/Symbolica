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

    public IPersistentJumps Add(IExpression<IType> continuation, bool useJumpBuffer, ISavedFrame frame)
    {
        return new PersistentJumps(_points.Push(new Point(continuation, useJumpBuffer, frame)));
    }

    public Result<ISavedFrame> TryGet(ISpace space, IExpression<IType> continuation, bool useJumpBuffer)
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

    private readonly struct Point
    {
        private readonly IExpression<IType> _continuation;
        private readonly bool _useJumpBuffer;

        public Point(IExpression<IType> continuation, bool useJumpBuffer, ISavedFrame frame)
        {
            _continuation = continuation;
            _useJumpBuffer = useJumpBuffer;
            Frame = frame;
        }

        public ISavedFrame Frame { get; }

        public bool IsMatch(ISpace space, IExpression<IType> continuation, bool useJumpBuffer)
        {
            return _useJumpBuffer == useJumpBuffer && IsMatch(space, continuation);
        }

        private bool IsMatch(ISpace space, IExpression<IType> continuation)
        {
            var isEqual = Expression.Values.Equal.Create(_continuation, continuation);
            using var proposition = space.CreateProposition(isEqual);

            return !proposition.CanBeFalse();
        }
    }
}
