using System;

namespace BCS.Entity
{

    public class SSPUser
    {
        public long UserID { get; set; }
        public int GroupID { get; set; }
        public Nullable<int> RoleID { get; set; }
        public Nullable<int> PermissionID { get; set; }
        public Nullable<int> PayFreqID { get; set; }
        public Nullable<int> TenantID { get; set; }
        public Nullable<int> AddressID { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string SuffixName { get; set; }
        public string NickName { get; set; }
        public string CellPhone { get; set; }
        public string HomePhone { get; set; }
        public string EmergencyContact { get; set; }
        public string EmergencyContactPhone { get; set; }
        public string SocialSecurityNumber { get; set; }
        public Nullable<System.DateTime> HireDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public Nullable<System.DateTime> NextReviewDates { get; set; }
        public Nullable<byte> TextAllowed { get; set; }
        public Nullable<byte> Status { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<long> CreatedUserID { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<long> UpdatedUserID { get; set; }
        public Nullable<System.DateTime> ForgotExpired { get; set; }
        public Nullable<System.Guid> ForgotCode { get; set; }
        public Nullable<bool> IsBcsUser { get; set; }

        public string TokenId { get; set; }

        public RestaurantEntity Restaurant { get; set; }
    }
}
