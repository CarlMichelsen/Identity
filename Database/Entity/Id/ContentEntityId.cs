using Database.Util;

namespace Database.Entity.Id;

public class ContentEntityId(Guid value, bool allowWrongVersion = false)
    : TypedGuid<ContentEntity>(value, allowWrongVersion);