namespace Domain.Abstraction;

public sealed class ResultError(
    ResultErrorType type,
    string description,
    System.Exception? innerException = default)
{
    public ResultErrorType Type => type;

    public string Description => description;
    
    public System.Exception? InnerException => innerException;

    public List<string> FriendlyErrors { get; init; } = [];
}