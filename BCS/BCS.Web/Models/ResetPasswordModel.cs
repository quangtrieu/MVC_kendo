using AutoMapper;
using BCS.Framework.SecurityServices.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace BCS.Web.Models
{
    public class ResetPasswordModel
    {
        [HiddenInput()]
        public int UserId { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Current Password")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "New Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [StringLength(20, ErrorMessage = "Password length must be between 6 to 20 characters.", MinimumLength = 6)]
        [System.Web.Mvc.Compare("Password", ErrorMessage = "New Password does not match the Confirm Password.")]
        public string ConfirmPassword { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "The Current Password you have entered is incorrect.")]
        public string CheckOldPassword { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "New Password should not be same as Current Password.")]
        public string CheckPassword { get; set; }

        public ResetPasswordModel()
        {

        }

        public ResetPasswordModel(UserInfo obj, bool encodeHtml = false)
        {
            Mapper.CreateMap<UserInfo, ResetPasswordModel>();
            Mapper.Map(obj, this);
        }

        public UserInfo GetUser()
        {
            Mapper.CreateMap<ResetPasswordModel, UserInfo>();
            UserInfo entity = Mapper.Map<ResetPasswordModel, UserInfo>(this);

            return entity;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            UserInfo user = GetUser();
            var results = new List<ValidationResult>();
            if (!string.IsNullOrEmpty(OldPassword) && OldPassword != user.Password && OldPassword == Password)
            {
                results.Add(new System.ComponentModel.DataAnnotations.ValidationResult("The Current Password you have entered is incorrect.", new[] { "CurrentPassword" }));
            }
            if (!string.IsNullOrEmpty(OldPassword) && OldPassword == Password)
            {
                results.Add(new System.ComponentModel.DataAnnotations.ValidationResult("New Password should not be same as Current Password.", new[] { "Password" }));
            }
            if (!string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(ConfirmPassword) && Password != ConfirmPassword)
            {
                results.Add(new ValidationResult("New Password does not match the Confirm Password.", new[] { "ConfirmPassword" }));
            }

            return results;
        }
    }
}