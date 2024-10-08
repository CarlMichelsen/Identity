using Domain.OAuth;
using Microsoft.AspNetCore.Http;

namespace Interface.Handler;

public interface ICompleteLoginHandler
{
    Task<IResult> CompleteLogin(string authenticationMethod);
}