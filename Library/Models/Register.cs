using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class Register
    {
        [Display(Name = "First name")]
        [Required(ErrorMessage = "Enter First name!")]
        public string FirstName { get; set; }
        [Display(Name = "Last name")]
        [Required(ErrorMessage = "Enter Last name!")]
        public string LastName { get; set; }
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Enter Email!")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Bad Email!")]
        public string Email { get; set; }
        [Display(Name = "Password")]
        [Required(ErrorMessage = "Enter password!")]
        [DataType(DataType.Password)]
        [RegularExpression(@"[A-Za-z0-9]+", ErrorMessage = "Password use only English books and numbers!")]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "Password must have more than 8 symvol!")]
        public string Password { get; set; }
        [Display(Name = "Confirm password")]
        [Required(ErrorMessage = "Repeat password!")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Different passwords!")]
        public string ConfirmPassword { get; set; }
        [Display(Name = "Phone number")]
        [DataType(DataType.PhoneNumber)]
        [Required(ErrorMessage = "Enter phone number!")]
        [RegularExpression(@"[0-9]{10}", ErrorMessage = "Enter number like \"0999999999\"!")]
        public string PhoneNumber { get; set; }
    }
}