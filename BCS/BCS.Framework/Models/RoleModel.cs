using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCS.Framework.Models
{
    public class RoleModel : BaseEntity
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public Nullable<byte> DefaultRole { get; set; }
        public Nullable<int> TenantID { get; set; }
    }
}
