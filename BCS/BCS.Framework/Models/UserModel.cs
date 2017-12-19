using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCS.Framework.Models
{
    public class UserModel : BaseEntity
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int SystemId { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public Nullable<System.DateTime> LastedDateLogin { get; set; }
        public Nullable<bool> DeletedFlg { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }
        public Nullable<System.Guid> ForgotCode { get; set; }
        public Nullable<System.DateTime> ForgotExpired { get; set; }
    }
}
