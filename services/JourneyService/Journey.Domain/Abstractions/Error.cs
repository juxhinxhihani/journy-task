namespace Journey.Domain.Abstractions;

public record Error
{
    public static Error None = new(string.Empty, string.Empty, ErrorType.Failure);

    public static Error NullValue = new("Error.NullValue", "The value provided is null", ErrorType.Failure);

    public static Error GeneralError = new("Error.Failure", "Something went wrong. Try again later!", ErrorType.Failure);

    private Error(string code, string description, ErrorType type)
    {
        Code = code;
        Description = description;
        Type = type;
    }

    public string Code { get; }
    public string Description { get; set; }
    public ErrorType Type { get; }


    public static Error NotFound(string code, string description) => new Error(code, description, ErrorType.NotFound);
    public static Error Validation(string code, string description) => new Error(code, description, ErrorType.Validation);
    public static Error Conflict(string code, string description) => new Error(code, description, ErrorType.Conflict);
    public static Error Failure(string code, string description) => new Error(code, description, ErrorType.Failure);
    public static Error Forbidden(string code, string description) => new Error(code, description, ErrorType.Forbidden);
}

public enum ErrorType
{
    Failure = 0,
    Validation = 1,
    NotFound = 2,
    Conflict = 3,
    Forbidden = 4

}