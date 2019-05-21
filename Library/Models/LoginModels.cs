using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class LoginModel
    {
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Enter Email!")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Некоректна адреса")]
        public string Email { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Enter password!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}