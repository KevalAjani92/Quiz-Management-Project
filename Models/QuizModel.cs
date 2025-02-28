using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace QuizManagement.Models
{
    public class QuizModel
    {
        [Key]
        public int QuizID {  get; set; }

        [Required(ErrorMessage ="QuizName is Required")]
        [StringLength(100)]
        public string QuizName {  get; set; }

        [Required(ErrorMessage ="TotalQuestions is Required")]
        [Range(1,100)]
        public int TotalQuestions {  get; set; }

        [Required(ErrorMessage ="QuizDate is Required")]
        public DateTime QuizDate { get; set; }

        [Required]
        public int UserID { get; set; }

        [AllowNull]
        public string? Created {  get; set; }
        [AllowNull]
        public string? Modified {  get; set; }
    }
}
