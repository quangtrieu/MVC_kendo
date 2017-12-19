using AutoMapper;
using BCS.Entity;
using BCS.Framework.SecurityServices.Entity;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace BCS.Web.Models
{
    public class UserModel
    {
        [HiddenInput()]
        public int UserId { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string OldPassword { get; set; }

        [Display(Name = "System")]
        public int SystemId { get; set; }
        [Required]
        [Display(Name = "Full Name")]
        [StringLength(255, ErrorMessage = "FullName length must be between 3 to 255 characters.", MinimumLength = 3)]
        public string FullName { get; set; }

        [Display(Name = "Phone Number")]
        [StringLength(30, ErrorMessage = "Phone length must be between 6 to 30 characters.", MinimumLength = 6)]
        public string Phone { get; set; }

        public int RoleId { get; set; }
        public bool Active { get; set; }

        public Nullable<System.DateTime> LastedDateLogin { get; set; }
        public Nullable<bool> DeletedFlg { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }
        public Nullable<System.Guid> ForgotCode { get; set; }
        public Nullable<System.DateTime> ForgotExpired { get; set; }

        public UserModel()
        {
            
        }

        public UserModel(User obj, bool encodeHtml = false)
        {
            Mapper.CreateMap<User, UserModel>();
            Mapper.Map(obj, this);
        }


        public UserModel(UserInfo obj, bool encodeHtml = false)
        {
            Mapper.CreateMap<UserInfo, UserModel>();
            Mapper.Map(obj, this);
        }

        public User GetUser()
        {
            Mapper.CreateMap<UserModel, User>();
            User entity = Mapper.Map<UserModel, User>(this);

            return entity;
        }

        public string RestCode { get; set; }
    }
}