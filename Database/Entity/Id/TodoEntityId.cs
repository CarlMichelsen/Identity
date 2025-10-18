using Database.Util;

namespace Database.Entity.Id;

public class IdentityEntityId(Guid value, bool allowWrongVersion = false)
    : TypedGuid<IdentityEntity>(value, allowWrongVersion);