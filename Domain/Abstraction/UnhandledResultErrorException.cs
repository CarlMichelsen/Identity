namespace Domain.Abstraction;

public class UnhandledResultErrorException(string message, ResultError error)
    : System.Exception(message)
{
    public ResultError Error => error;
}