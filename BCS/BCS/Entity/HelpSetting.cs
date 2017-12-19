using System;

namespace BCS.Entity
{
    public class HelpSetting
    {
        public int HelpSettingId { get; set; }
        public int UserId { get; set; }
        public int HelpSettingDataId { get; set; }
        public Nullable<bool> IsHidden { get; set; }
        public Nullable<bool> DeletedFlg { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }
    }
}
