using Database.Entity;
using Domain.Abstraction;
using Domain.Dto;

namespace Implementation.Map;

public static class LoginMapper
{
    public static Result<List<LoginDto>> Map(List<LoginRecordEntity> logins)
    {
        try
        {
            return logins.Select(Map).ToList();
        }
        catch (Exception e)
        {
            return new ResultError(
                ResultErrorType.Exception,
                "Exception thrown while mapping session",
                e);
        }
    }

    private static LoginDto Map(LoginRecordEntity loginRecordEntity)
    {
        return new LoginDto(
            LoginId: loginRecordEntity.Id,
            RefreshRecords: loginRecordEntity.RefreshRecords.Select(Map).ToList(),
            CreatedUtc: loginRecordEntity.CreatedUtc,
            Ip: loginRecordEntity.Ip,
            UserAgent: loginRecordEntity.UserAgent,
            InvalidatedUtc: loginRecordEntity.InvalidatedUtc);
    }
    
    private static RefreshDto Map(RefreshRecordEntity refreshRecordEntity)
    {
        return new RefreshDto(
            RefreshId: refreshRecordEntity.Id,
            AccessRecords: refreshRecordEntity.AccessRecords.Select(Map).ToList(),
            ExpiresUtc: refreshRecordEntity.ExpiresUtc,
            CreatedUtc: refreshRecordEntity.CreatedUtc,
            Ip: refreshRecordEntity.Ip,
            UserAgent: refreshRecordEntity.UserAgent,
            InvalidatedUtc: refreshRecordEntity.InvalidatedUtc);
    }
    
    private static AccessDto Map(AccessRecordEntity accessRecordEntity)
    {
        return new AccessDto(
            AccessId: accessRecordEntity.Id,
            ExpiresUtc: accessRecordEntity.ExpiresUtc,
            CreatedUtc: accessRecordEntity.CreatedUtc,
            Ip: accessRecordEntity.Ip,
            UserAgent: accessRecordEntity.UserAgent);
    }
}