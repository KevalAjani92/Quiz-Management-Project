using System.ComponentModel.DataAnnotations;

namespace QuizManagement.Models
{
    public class QuizwiseQuestionModel
    {
        [Key]
        public int QuizwiseQuestionsID {  get; set; }

        [Required(ErrorMessage ="Quiz Name is Required")]
        public int QuizID {  get; set; }

        [Required(ErrorMessage ="Question Text is Required")]
        public int QuestionID { get; set; }

        [Required]
        public int UserID {  get; set; }
        public string? Created {  get; set; }
        public string? Modified {  get; set; }
    }
}
