using System;

namespace BCS.Framework.Commons
{
    public interface IAuditableEntity 
    {
        DateTime? CreatedDate { get; set; }
        int? CreatedUserId { get; set; }
        DateTime? UpdatedDate { get; set; }
        int? UpdatedUserId { get; set; }
    }
}
