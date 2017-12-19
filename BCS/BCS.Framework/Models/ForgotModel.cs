using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BCS.Framework.SecurityServices.DataProviders;
using BCS.Framework.Singleton;

namespace BCS.Framework.Models
{
    public class ForgotModel : IValidatableObject
    {
        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress, ErrorMessage = "The {0} field is not a valid e-mail address.")]
        public string Email { get; set; }

        [Display(Name = "Captcha")]
        [Required]
        public int? Sum { get; set; }

        public ForgotModel()
        {

        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (string.IsNullOrEmpty(Email))
                results.Add(new ValidationResult("The Email field is required.", new[] { "Email" }));

            if (!Sum.HasValue)
                results.Add(new ValidationResult("The Captcha field is required.", new[] { "Sum" }));

            // Check Email & Restaurant does't exitst of User
            var ctx = SingletonIpl.GetInstance<DataProvider>();
            var exist = ctx.IsExistEmailOfUser(Email);
            if (!exist)
            {
                results.Add(new ValidationResult("The email address does not exist in system.", new[] { "Email" }));
            }

            return results;
        }
    }
}
