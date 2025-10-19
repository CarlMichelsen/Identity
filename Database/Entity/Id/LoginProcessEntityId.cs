using Database.Util;

namespace Database.Entity.Id;

public class LoginProcessEntityId(Guid value, bool allowWrongVersion = false)
    : TypedGuid<OAuthProcessEntity>(value, allowWrongVersion);