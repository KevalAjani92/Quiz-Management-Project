using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace QuizManagement.Models
{
    public class QuestionLevelModel
    {
        [Key]
        public int QuestionLevelID { get; set; }

        [Required(ErrorMessage = "Question Level is Required")]
        [StringLength(100)]
        public string QuestionLevel { get; set; }

        [Required]
        public int UserID { get; set; }

        [AllowNull]
        public string? Created { get; set; }
        [AllowNull]
        public string? Modified { get; set; }
    }
    public class QuestionLevelDropDownModel
    {
        public int QuestionLevelID { get; set; }
        public string QuestionLevel { get; set; }
    }
}
