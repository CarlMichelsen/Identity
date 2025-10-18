using Database.Util;

namespace Database.Entity.Id;

public class AttachmentEntityId(Guid value, bool allowWrongVersion = false)
    : TypedGuid<AttachmentEntity>(value, allowWrongVersion);