using FluentValidation.Results;

public class Result<T>
{
    public bool IsSuccess { get; set; }
    public ResultType ResultType { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public Dictionary<string, List<string>> FieldErrors { get; set; } = new();

    // Make this a static method, not an extension method inside the class.
    public static Result<T> ValidationError(List<ValidationFailure> validationErrors)
    {
        var result = new Result<T>
        {
            IsSuccess = false,
            Message = "Validation failed",
            ResultType = ResultType.ValidationError
        };

        foreach (var error in validationErrors)
        {

            if (!result.FieldErrors.ContainsKey(error.PropertyName))
            {
                result.FieldErrors[error.PropertyName] = new List<string>();
            }

            result.FieldErrors[error.PropertyName].Add(error.ErrorMessage);
        }

        return result;
    }

    public static Result<T> Failure(string message)
    {
        var result = new Result<T>
        {
            IsSuccess = false,
            Message = message,
            ResultType = ResultType.Error
        };
        return result;
    }

    public static Result<T> Success(string message, T data)
    {
        return new Result<T>
        {
            IsSuccess = true,
            Message = message,
            ResultType = ResultType.Success,
            Data = data
        };
    }
}


public enum ResultType
{
    Success,
    ValidationError,
    Error,
    Unathorized,
}

public static class ResultExtensions
{
    public static Result<T> ValidationError<T>(this List<ValidationFailure> validationErrors)
    {
        var result = new Result<T>
        {
            IsSuccess = false,
            Message = "Validation failed"
        };

        foreach (var error in validationErrors)
        {

            if (!result.FieldErrors.ContainsKey(error.PropertyName))
            {
                result.FieldErrors[error.PropertyName] = new List<string>();
            }

            result.FieldErrors[error.PropertyName].Add(error.ErrorMessage);
        }

        return result;
    }
}
