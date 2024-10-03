using System.Diagnostics.CodeAnalysis;

namespace Domain.Abstraction;

public class Result
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// This Result is a successful result.
    /// You must include an error to make it a failure result.
    /// </summary>
    public Result()
    {
    }

    private Result(ResultError error)
    {
        this.IsError = true;
        this.Error = error;
    }

    public bool IsError { get; protected init; }

    public bool IsSuccess => !this.IsError;

    public ResultError? Error { get; protected init; }

    public static implicit operator Result(ResultError error) => new(error);

    public TResult Match<TResult>(
        Func<TResult> success,
        Func<ResultError, TResult> failure) =>
        !this.IsError ? success() : failure(this.Error!);
}

[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "It makes sense to have these close to each other.")]
public sealed class Result<T> : Result
{
    private readonly T? value;

    private Result(T value)
        : base()
    {
        this.value = value;
        this.Error = default;
    }

    private Result(ResultError error)
    {
        this.IsError = true;
        this.Error = error;
    }

    public static implicit operator Result<T>(T value) => new(value);

    public static implicit operator Result<T>(ResultError error) => new(error);

    public TResult Match<TResult>(
        Func<T, TResult> success,
        Func<ResultError, TResult> failure) =>
        !this.IsError ? success(this.value!) : failure(this.Error!);

    /// <summary>
    /// Assumes result is successful and returns value.
    /// If result is not successful will throw a UnhandledResultException with the failure exception as InnerException.
    /// </summary>
    /// <exception cref="UnhandledResultErrorException">Thrown if result is failure.</exception>
    /// <returns>T.</returns>
    public T Unwrap()
    {
        return this.Match(
            (T val) => val,
            (ResultError error) => throw new UnhandledResultErrorException(
                "Assumed an error-result was not an error.",
                error));
    }
}