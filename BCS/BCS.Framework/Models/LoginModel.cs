using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using BCS.Framework.SecurityServices.DataProviders;
using BCS.Framework.Singleton;

namespace BCS.Framework.Models
{
    public class LoginModel:IValidatableObject
    {
        [Required]
        [Display(Name = "Username or email")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
        
        [Display(Name = "Email")]
        public string Email { get; set; }

        public LoginModel()
        {
            
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (string.IsNullOrEmpty(UserName))
                results.Add(new ValidationResult("The Username field is required.", new[] { "Username" }));

            if (string.IsNullOrEmpty(Password))
                results.Add(new ValidationResult("The Password field is required.", new[] { "Password" }));


            // Check User does't exitst of User
            var ctx = SingletonIpl.GetInstance<DataProvider>();
            var exist = ctx.IsAuthenticated(UserName, Password);
            if (!exist)
            {
                results.Add(new ValidationResult("The Username, Password is incorrect."));
            }

            return results;
        }
    }
}
