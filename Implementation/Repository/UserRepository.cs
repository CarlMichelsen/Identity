using System.Text.Json;
using Database;
using Database.Entity;
using Domain.Abstraction;
using Domain.OAuth;
using Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Implementation.Repository;

public class UserRepository(
    ApplicationContext applicationContext) : IUserRepository
{
    public async Task<Result<UserEntity>> GetUser(long id)
    {
        try
        {
            var user = await applicationContext.User
                .FirstOrDefaultAsync(u => u.Id == id);
            if (user is null)
            {
                return new ResultError(
                    ResultErrorType.NotFound,
                    "User was not found");
            }

            return user;
        }
        catch (Exception e)
        {
            return new ResultError(
                ResultErrorType.Exception,
                "Exception thrown while fetching user",
                e);
        }
    }

    public async Task<Result<UserEntity>> GetUser(OAuthProvider provider, string providerId)
    {
        try
        {
            var user = await applicationContext.User
                .FirstOrDefaultAsync(u => u.AuthenticationProvider == provider && u.ProviderId == providerId);
            if (user is null)
            {
                return new ResultError(
                    ResultErrorType.NotFound,
                    "User was not found");
            }

            return user;
        }
        catch (Exception e)
        {
            return new ResultError(
                ResultErrorType.Exception,
                "Exception thrown while fetching user",
                e);
        }
    }

    public async Task<Result<(UserEntity User, long LoginId, long RefreshId, bool FirstLogin)>> LoginUser(
        IOAuthUserConvertible userConvertible,
        IClientInfo clientInfo)
    {
        await using var transaction = await applicationContext.Database.BeginTransactionAsync();
        try
        {
            var oAuthUserResult = userConvertible.ToUser();
            if (oAuthUserResult.IsError)
            {
                return oAuthUserResult.Error!;
            }

            var oAuthUser = oAuthUserResult.Unwrap();

            var isFirstLogin = false;
            var userResult = await this.GetAndUpdateUser(oAuthUser);
            UserEntity user;
            if (userResult.IsError)
            {
                if (userResult.Error!.Type == ResultErrorType.NotFound)
                {
                    isFirstLogin = true;
                    user = await this.CreateUser(oAuthUser);
                }
                else
                {
                    return userResult.Error!;
                }
            }
            else
            {
                user = userResult.Unwrap();
            }

            var loginRecord = new LoginRecordEntity
            {
                UserId = user.Id,
                User = user,
                OAuthJson = JsonSerializer.Serialize<object>(userConvertible),
                RefreshRecords = [],
                CreatedUtc = DateTime.UtcNow,
                Ip = clientInfo.Ip,
                UserAgent = clientInfo.UserAgent,
            };

            user.LoginRecords.Add(loginRecord);
            
            await applicationContext.SaveChangesAsync();
            
            var refreshRecord = new RefreshRecordEntity
            {
                LoginRecordId = loginRecord.Id,
                LoginRecord = loginRecord,
                CreatedUtc = DateTime.UtcNow,
                Ip = clientInfo.Ip,
                UserAgent = clientInfo.UserAgent,
            };
            loginRecord.RefreshRecords.Add(refreshRecord);
            
            await applicationContext.SaveChangesAsync();
            await transaction.CommitAsync();
            return (User: user, LoginId: loginRecord.Id, RefreshId: refreshRecord.Id, FirstLogin: isFirstLogin);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return new ResultError(
                ResultErrorType.Exception,
                "Exception thrown while logging user in",
                e);
        }
    }

    public async Task<Result<RefreshRecordEntity>> RefreshUser(
        long loginId,
        OAuthUser oAuthUser,
        IClientInfo clientInfo)
    {
        try
        {
            var login = await applicationContext.LoginRecord
                .FindAsync(loginId);
            if (login is null)
            {
                return new ResultError(
                    ResultErrorType.NotFound,
                    "Login was not found");
            }

            var refreshRecord = new RefreshRecordEntity
            {
                LoginRecordId = login.Id,
                LoginRecord = login,
                CreatedUtc = DateTime.UtcNow,
                Ip = clientInfo.Ip,
                UserAgent = clientInfo.UserAgent,
            };

            applicationContext.RefreshRecord.Add(refreshRecord);
            await applicationContext.SaveChangesAsync();
            return refreshRecord;
        }
        catch (Exception e)
        {
            return new ResultError(
                ResultErrorType.Exception,
                "Exception thrown while refreshing user login",
                e);
        }
    }

    private async Task<Result<UserEntity>> GetAndUpdateUser(OAuthUser oAuthUser)
    {
        var maybeUserResult = await this.GetUser(oAuthUser.AuthenticationProvider, oAuthUser.ProviderId);
        if (maybeUserResult.IsError)
        {
            return maybeUserResult.Error!;
        }

        var maybeUser = maybeUserResult.Unwrap();
        if (maybeUser.Username != oAuthUser.Username)
        {
            maybeUser.OldInformationRecords.Add(new OldInformationRecordEntity
            {
                UserId = maybeUser.Id,
                User = maybeUser,
                Type = OldInformation.Username,
                Information = maybeUser.Username,
                CreatedUtc = DateTime.UtcNow,
            });
            maybeUser.Username = oAuthUser.Username;
        }
        
        if (maybeUser.Email != oAuthUser.Email)
        {
            maybeUser.OldInformationRecords.Add(new OldInformationRecordEntity
            {
                UserId = maybeUser.Id,
                User = maybeUser,
                Type = OldInformation.Email,
                Information = maybeUser.Email,
                CreatedUtc = DateTime.UtcNow,
            });
            maybeUser.Email = oAuthUser.Email;
        }
        
        if (maybeUser.AvatarUrl != oAuthUser.AvatarUrl)
        {
            maybeUser.OldInformationRecords.Add(new OldInformationRecordEntity
            {
                UserId = maybeUser.Id,
                User = maybeUser,
                Type = OldInformation.AvatarUrl,
                Information = maybeUser.AvatarUrl,
                CreatedUtc = DateTime.UtcNow,
            });
            maybeUser.AvatarUrl = oAuthUser.AvatarUrl;
        }

        return maybeUser;
    }

    private async Task<UserEntity> CreateUser(OAuthUser oAuthUser)
    {
        var user = new UserEntity
        {
            ProviderId = oAuthUser.ProviderId,
            AuthenticationProvider = oAuthUser.AuthenticationProvider,
            Username = oAuthUser.Username,
            AvatarUrl = oAuthUser.AvatarUrl,
            Email = oAuthUser.Email,
            LoginRecords = [],
            OldInformationRecords = [],
            CreatedUtc = DateTime.UtcNow,
        };

        await applicationContext.User.AddAsync(user);

        return user;
    }
}