using Domain.Abstraction;

namespace Interface.Service;

public interface IErrorLogService
{
    string Log(ResultError error);
}