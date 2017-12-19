using System;
using BCS.Framework.Models;

namespace BCS.Framework.SecurityServices.Entity
{
    public class RoleInfo : BaseEntity
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public Nullable<byte> DefaultRole { get; set; }
        public Nullable<int> TenantID { get; set; }
    }
}
