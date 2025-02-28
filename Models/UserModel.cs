using System.ComponentModel.DataAnnotations;

namespace QuizManagement.Models
{
    public class UserModel
    {
        [Key]
        public int UserID { get; set; }

        [Required(ErrorMessage = "UserName is Required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is Required")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Email is Required ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mobile No. is Required")]
        public string Mobile { get; set; }

        public int IsActive { get; set; }
        public int IsAdmin { get; set; }
        public string Created { get; set; }
        public string Modified { get; set; }

    }
    public class UserRegisterModel
    {
        public int? UserID { get; set; }

        [Required(ErrorMessage = "UserName is Required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mobile No. is Required")]
        public string Mobile { get; set; }

        [Required(ErrorMessage = "Password is Required")]
        public string Password { get; set; }
    }
    public class UserLoginModel
    {
        [Required(ErrorMessage = "Username / Email / MobileNo is required.")]
        public string Credential { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
    }
}
