using Microsoft.AspNetCore.Http;

namespace Interface.Handler;

public interface IUserReadHandler
{
    IResult GetUser();
}