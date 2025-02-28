using System.ComponentModel.DataAnnotations;

namespace QuizManagement.Models
{
    public class QuestionModel
    {
        [Key]
        public int QuestionID {  get; set; }

        [Required(ErrorMessage ="QuestionText is Required")]
        public string QuestionText {  get; set; }

        [Required(ErrorMessage ="QuestionLevel is Required")]
        public int QuestionLevelID {  get; set; }

        [Required(ErrorMessage ="OptionA is Required")]
        public string OptionA {  get; set; }

        [Required(ErrorMessage = "OptionB is Required")]
        public string OptionB { get; set; }

        //[Required(ErrorMessage = "OptionC is Required")]
        public string? OptionC { get; set; }

        //[Required(ErrorMessage = "OptionD is Required")]
        public string? OptionD { get; set; }

        [Required]
        public string CorrectOption {  get; set; }

        [Required(ErrorMessage ="QuestionMark is Required")]
        public int QuestionMark {  get; set; }

        public bool IsActive {  get; set; }

        [Required]
        public int UserID {  get; set; }
        public string? Created {  get; set; }
        public string? Modified {  get; set; }
    }
}
