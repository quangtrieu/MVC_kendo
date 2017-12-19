using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCS.Framework.SecurityServices.DataProviders;
using BCS.Framework.Singleton;

namespace BCS.Framework.Models
{
    public class ChangePasswordModel : IValidatableObject
    {
        public long UserId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }

        [Required]
        [Display(Name = "New Password")]
        [DataType(DataType.Password)]
        [StringLength(20, ErrorMessage = " Password length must be between 6 to 20 characters.", MinimumLength = 6)] 
        public string PassWord { get; set; }

        [Required]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [StringLength(20, ErrorMessage = "Password length must be between 6 to 20 characters.", MinimumLength = 6)] 
        
        public string ConfirmPassword { get; set; }

        public string FullName { get; set; }

        public Guid ForgotCode { get; set; }
        public DateTime ForgotExpired { get; set; }

        public ChangePasswordModel()
        {
            
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (!string.IsNullOrEmpty(PassWord) && !string.IsNullOrEmpty(ConfirmPassword) && PassWord != ConfirmPassword)
            {
                results.Add(new ValidationResult("Password and Confirmed Password do not match.", new[] { "ConfirmPassword" }));
            }
           
            return results;
        }
    }
}
