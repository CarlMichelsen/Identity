using Database.Util;

namespace Database.Entity.Id;

public class LoginEntityId(Guid value, bool allowWrongVersion = false)
    : TypedGuid<LoginEntity>(value, allowWrongVersion);