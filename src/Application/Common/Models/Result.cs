namespace NigerianPrimarySchool.Application.Common.Models;

/// <summary>
/// A discriminated-union style result type.
/// Avoids throwing exceptions for expected business failures.
/// </summary>
public class Result
{
    protected Result(bool succeeded, IEnumerable<string> errors)
    {
        Succeeded = succeeded;
        Errors = [.. errors];
    }

    public bool Succeeded { get; }
    public bool Failed => !Succeeded;
    public string[] Errors { get; }
    public string? FirstError => Errors.Length > 0 ? Errors[0] : null;

    public static Result Success() => new(true, []);
    public static Result Failure(params string[] errors) => new(false, errors);
    public static Result Failure(IEnumerable<string> errors) => new(false, errors);
}

/// <summary>
/// A result that carries a payload on success.
/// </summary>
public class Result<T> : Result
{
    private Result(bool succeeded, T? value, IEnumerable<string> errors)
        : base(succeeded, errors)
    {
        Value = value;
    }

    public T? Value { get; }

    public static Result<T> Success(T value) => new(true, value, []);
    public static new Result<T> Failure(params string[] errors) => new(false, default, errors);
    public static new Result<T> Failure(IEnumerable<string> errors) => new(false, default, errors);
}
