using System;

namespace BCS.Framework.Commons
{
    public abstract class AuditableEntity : Entity, IAuditableEntity    
    {
        public DateTime? CreatedDate { get; set; }

        public int? CreatedUserId { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedUserId { get; set; }
    }
}
