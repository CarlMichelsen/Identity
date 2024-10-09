namespace Database;

public interface IClientInfo
{
    string Ip { get; init; }
    
    string UserAgent { get; init; }
}