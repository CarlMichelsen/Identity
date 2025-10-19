using Database.Util;

namespace Database.Entity.Id;

public class UserEntityId(Guid value, bool allowWrongVersion = false)
    : TypedGuid<UserEntity>(value, allowWrongVersion);