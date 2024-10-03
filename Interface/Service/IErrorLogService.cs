using Domain.Abstraction;
using Domain.Dto;

namespace Interface.Service;

public interface IErrorLogService
{
    string Log(ResultError error);
}