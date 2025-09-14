using System;

namespace StackoverflowService.Application.Common.Results
{
    public sealed class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public Error[] Errors { get; }

        private Result(bool ok, Error[] errors)
        {
            IsSuccess = ok;
            Errors = errors ?? Array.Empty<Error>();
        }

        public static Result Ok() => new Result(true, Array.Empty<Error>());
        public static Result Fail(params Error[] errors) => new Result(false, errors ?? Array.Empty<Error>());
        public static Result<T> Ok<T>(T value) => Result<T>.Ok(value);
        public static Result<T> Fail<T>(params Error[] errors) => Result<T>.Fail(errors);
    }

    public sealed class Result<T>
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public T Value { get; }
        public Error[] Errors { get; }

        private Result(bool ok, T value, Error[] errors)
        {
            IsSuccess = ok;
            Value = value;
            Errors = errors ?? Array.Empty<Error>();
        }

        public static Result<T> Ok(T value) => new Result<T>(true, value, Array.Empty<Error>());
        public static Result<T> Fail(params Error[] errors) => new Result<T>(false, default(T)!, errors);
    }
}