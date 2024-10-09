using Domain.Abstraction;
using Domain.Dto;

namespace Interface.Service;

public interface IUserRefreshService
{
    Task<Result<ServiceResponse>> Refresh();
}