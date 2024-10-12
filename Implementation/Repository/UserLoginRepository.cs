using System.Text.Json;
using Database;
using Database.Entity;
using Domain.Abstraction;
using Domain.Configuration;
using Domain.OAuth;
using Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Implementation.Repository;

public class UserLoginRepository(
    ApplicationContext applicationContext) : IUserLoginRepository
{
    public async Task<Result<PostLoginData>> LoginUser(
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
                ExpiresUtc = DateTime.UtcNow + ApplicationConstants.RefreshTokenLifeTime,
                AccessRecords = [],
                Ip = clientInfo.Ip,
                UserAgent = clientInfo.UserAgent,
            };
            loginRecord.RefreshRecords.Add(refreshRecord);
            
            await applicationContext.SaveChangesAsync();
            var accessRecord = new AccessRecordEntity
            {
                RefreshRecordId = refreshRecord.Id,
                RefreshRecord = refreshRecord,
                CreatedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow + ApplicationConstants.AccessTokenLifeTime,
                Ip = clientInfo.Ip,
                UserAgent = clientInfo.UserAgent,
            };
            refreshRecord.AccessRecords.Add(accessRecord);
            
            await applicationContext.SaveChangesAsync();
            await transaction.CommitAsync();
            return new PostLoginData(
                User: user,
                LoginId: loginRecord.Id,
                RefreshId: refreshRecord.Id,
                AccessId: accessRecord.Id,
                FirstLogin: isFirstLogin);
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

    private async Task<Result<UserEntity>> GetAndUpdateUser(OAuthUser oAuthUser)
    {
        var maybeUser = await applicationContext.User
            .Include(u => u.OldInformationRecords)
            .Include(u => u.LoginRecords)
            .ThenInclude(l => l.RefreshRecords)
            .ThenInclude(r => r.AccessRecords)
            .AsSplitQuery()
            .FirstOrDefaultAsync(u => u.AuthenticationProvider == oAuthUser.AuthenticationProvider && u.ProviderId == oAuthUser.ProviderId);
        
        if (maybeUser is null)
        {
            return new ResultError(ResultErrorType.NotFound, "Did not find user");
        }
        
        if (maybeUser.Username != oAuthUser.Username)
        {
            maybeUser.OldInformationRecords.Add(new OldInformationRecordEntity
            {
                UserId = maybeUser.Id,
                User = maybeUser,
                Type = OldInformation.Username,
                Information = maybeUser.Username,
                ReplacedUtc = DateTime.UtcNow,
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
                ReplacedUtc = DateTime.UtcNow,
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
                ReplacedUtc = DateTime.UtcNow,
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