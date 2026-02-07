namespace Application.Common;

public sealed class Result
{
    public bool IsSuccess { get; }
    public ErrorType ErrorType { get; }
    public string? ErrorMessage { get; }

    private Result(bool success, ErrorType type, string message)
    {
        IsSuccess = success;
        ErrorType = type;
        ErrorMessage = message;
    }

    public static Result Ok() => new Result(true, ErrorType.None, string.Empty);
    public static Result Fail(ErrorType type, string message) => new Result(false, type, message);
}

public sealed class Result<T>
{
    private Result(bool success, T? value, ErrorType errorType, string message)
    {
        if (success == true && errorType != ErrorType.None ||
            success == false && errorType == ErrorType.None)

            throw new ArgumentException("Invalid Result/Error. ", nameof(message));

        IsSuccess = success;
        Value = value;
        ErrorType = errorType;
        ErrorMessage = message;
    }

    public bool IsSuccess { get; }
    public T? Value { get; }
    public ErrorType ErrorType { get; }
    public string ErrorMessage { get; }

    public static Result<T> Ok(T value) => new Result<T>(true, value, ErrorType.None, string.Empty);
    public static Result<T> Fail(ErrorType errorType, string message)
        => new Result<T>(false, default, errorType, message);
}
