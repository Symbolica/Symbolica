using System;

namespace Symbolica.Implementation
{
    public interface IResult<out T, E>
        where T : class
        where E : class
    {
        IResult<U, E> Bind<U>(Func<T, IResult<U, E>> f) where U : class;
        U Fold<U>(Func<T, U> fOk, Func<E, U> fError);
        void Fold(Action<T> fOk, Action<E> fError);
        IResult<U, E> Map<U>(Func<T, U> f) where U : class;
        IResult<T, F> MapError<F>(Func<E, F> f) where F : class;
    }

    public sealed class Ok<T, E> : IResult<T, E>
        where T : class
        where E : class
    {
        private readonly T _value;

        public Ok(T value)
        {
            _value = value;
        }

        public U Fold<U>(Func<T, U> fOk, Func<E, U> fError) =>
            fOk(_value);

        public void Fold(Action<T> fOk, Action<E> fError) =>
            fOk(_value);

        public IResult<U, E> Bind<U>(Func<T, IResult<U, E>> f)
            where U : class =>
                f(_value);

        public IResult<U, E> Map<U>(Func<T, U> f)
            where U : class =>
                new Ok<U, E>(f(_value));

        public IResult<T, F> MapError<F>(Func<E, F> f)
            where F : class =>
                new Ok<T, F>(_value);
    }

    public sealed class Error<T, E> : IResult<T, E>
        where T : class
        where E : class
    {
        private readonly E _error;

        public Error(E error)
        {
            _error = error;
        }

        public U Fold<U>(Func<T, U> fOk, Func<E, U> fError) =>
            fError(_error);

        public void Fold(Action<T> fOk, Action<E> fError) =>
            fError(_error);

        public IResult<U, E> Bind<U>(Func<T, IResult<U, E>> f)
            where U : class =>
                new Error<U, E>(_error);

        public IResult<U, E> Map<U>(Func<T, U> f)
            where U : class =>
                new Error<U, E>(_error);

        public IResult<T, F> MapError<F>(Func<E, F> f)
            where F : class =>
                new Error<T, F>(f(_error));
    }

    public static class Result
    {
        public static IResult<T, Ex> TryCatch<T, Ex>(Func<T> f)
            where T : class
            where Ex : Exception
        {
            try
            {
                return new Ok<T, Ex>(f());
            }
            catch (Ex ex)
            {
                return new Error<T, Ex>(ex);
            }
        }
    }
}
