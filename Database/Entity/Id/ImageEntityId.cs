using Database.Util;

namespace Database.Entity.Id;

public class ImageEntityId(Guid value, bool allowWrongVersion = false)
    : TypedGuid<ImageEntity>(value, allowWrongVersion);