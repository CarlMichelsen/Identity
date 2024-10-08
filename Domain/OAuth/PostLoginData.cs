namespace Domain.OAuth;

public record PostLoginData(
    UserEntity User,
    long LoginId,
    long RefreshId,
    long AccessId,
    bool FirstLogin,
    string RefreshJwtId,
    string AccessJwtId);