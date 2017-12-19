using AutoMapper;
using BCS.DataAccess;
using BCS.Entity;
using BCS.Framework.SecurityServices.DataProviders;
using BCS.Framework.Singleton;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace BCS.Web.Models
{
    public class RegisterModel : IValidatableObject
    {
        [HiddenInput()]
        public int UserId { get; set; }

        [Required]
        [Display(Name = "Email")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "The {0} field is not a valid e-mail address.")]
        [StringLength(255, ErrorMessage = "Email length must be between 3 to 255 characters.", MinimumLength = 3)]
        public string Email { get; set; }

        [Required]
        [Display(Name = "UserName")]
        [StringLength(255, ErrorMessage = "UserName length must be between 3 to 255 characters.", MinimumLength = 3)]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [StringLength(20, ErrorMessage = "Password length must be between 6 to 20 characters.", MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Display(Name = "System")]
        public int SystemId { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        [StringLength(255, ErrorMessage = "Full Name length must be between 3 to 255 characters.", MinimumLength = 3)]
        public string FullName { get; set; }

        [Display(Name = "Phone Number")]
        [StringLength(30, ErrorMessage = "Phone length must be between 6 to 30 characters.", MinimumLength = 6)]
        public string Phone { get; set; }

        public Nullable<System.DateTime> LastedDateLogin { get; set; }
        public Nullable<bool> DeletedFlg { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedUserId { get; set; }
        public Nullable<System.Guid> ForgotCode { get; set; }
        public Nullable<System.DateTime> ForgotExpired { get; set; }

        public string TokenId { get; set; }
        public string RestCode { get; set; }
        public string RestName { get; set; }

        public RegisterModel()
        {

        }

        public RegisterModel(User obj, bool encodeHtml = false)
        {
            Mapper.CreateMap<User, UserModel>();
            Mapper.Map(obj, this);
        }

        public User GetUser()
        {
            Mapper.CreateMap<RegisterModel, User>();
            User entity = Mapper.Map<RegisterModel, User>(this);

            return entity;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (!string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(ConfirmPassword) && Password != ConfirmPassword)
            {
                results.Add(new ValidationResult("Password and Confirmed Password do not match.", new[] { "ConfirmPassword" }));
            }

            var exitsToken = SingletonIpl.GetInstance<RestActiveCodeDal>().GetByToken(TokenId);
            if (exitsToken != null)
            {
                results.Add(new ValidationResult("Invalid Token error on Register."));
            }

            var ctx = SingletonIpl.GetInstance<DataProvider>();

            // Check Email does't exitst of User
            var existEmail = ctx.GetUserByEmail(Email);
            if (existEmail != null)
            {
                results.Add(new ValidationResult("This e-mail address already exists in this Budget Creator.", new[] { "Email" }));
            }

            var existUserName = ctx.GetUserByUserName(UserName);
            if (existUserName != null)
            {
                results.Add(new ValidationResult("The UserName already exitst in this Budget Creator.", new[] { "UserName" }));
            }

            return results;
        }

        
    }
}