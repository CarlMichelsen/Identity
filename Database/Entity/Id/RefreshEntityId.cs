using Database.Util;

namespace Database.Entity.Id;

public class RefreshEntityId(Guid value, bool allowWrongVersion = false)
    : TypedGuid<RefreshEntity>(value, allowWrongVersion);