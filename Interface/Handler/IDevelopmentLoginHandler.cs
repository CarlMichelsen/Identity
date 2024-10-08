using Microsoft.AspNetCore.Http;

namespace Interface.Handler;

public interface IDevelopmentLoginHandler
{
    Task<IResult> Login(long developmentUserId, string state);
    
    Task<IResult> GetTestUsers();
}