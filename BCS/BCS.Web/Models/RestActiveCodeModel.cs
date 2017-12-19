using System;

namespace BCS.Web.Models
{
    public partial class RestActiveCodeModel
    {
        public int RestActiveCodeId { get; set; }
        public int UserId { get; set; }
        public string RestCode { get; set; }
        public string RestName { get; set; }
        public string TokenId { get; set; }
        public Nullable<bool> IsDefault { get; set; }
        public Nullable<bool> DeletedFlg { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }
    }
}