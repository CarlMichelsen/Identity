using Database.Util;

namespace Database.Entity.Id;

public class OAuthProcessEntityId(Guid value, bool allowWrongVersion = false)
    : TypedGuid<OAuthProcessEntity>(value, allowWrongVersion);