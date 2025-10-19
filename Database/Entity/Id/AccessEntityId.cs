using Database.Util;

namespace Database.Entity.Id;

public class AccessEntityId(Guid value, bool allowWrongVersion = false)
    : TypedGuid<AccessEntity>(value, allowWrongVersion);