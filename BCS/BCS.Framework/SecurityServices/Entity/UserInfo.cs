//******************************************************************************
//Description: Contains Properties for CurrentUser
//Remarks: User Info
//Author : HiepNV
//Copyright(C) 2015 Seta International VietNam. All right reserved.
//******************************************************************************

using BCS.Framework.Commons;
using System;

namespace BCS.Framework.SecurityServices.Entity
{
    [Serializable]
    public class UserInfo : AuditableEntity
    {
        /*** USER INFO ***/
        #region Current User Info...

        public int UserId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int SystemId { get; set; }
        public int RoleId { get; set; }
        public bool Active { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public Nullable<System.DateTime> LastedDateLogin { get; set; }
        public Nullable<bool> DeletedFlg { get; set; }
        public Nullable<System.Guid> ForgotCode { get; set; }
        public Nullable<System.DateTime> ForgotExpired { get; set; }

        #endregion

        public UserInfo SupportUser { get; set; }
    }
}