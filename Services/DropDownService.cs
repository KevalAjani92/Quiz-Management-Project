using System.Data;
using System.Data.SqlClient;
using QuizManagement.Models;

namespace QuizManagement.Services
{
    public class DropDownService
    {
        private readonly IConfiguration _configuration;

        public DropDownService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public List<QuestionLevelDropDownModel> GetQuestionLevelDropDown()
        {
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            DataTable table = new DataTable();
            List<QuestionLevelDropDownModel> questionLevelList = new List<QuestionLevelDropDownModel>();
            using(SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using(SqlCommand command = new SqlCommand("PR_QuestionLevel_Dropdown",connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    using(SqlDataReader reader = command.ExecuteReader())
                    {
                        table.Load(reader);
                        foreach(DataRow row in table.Rows)
                        {
                            QuestionLevelDropDownModel questionLevelDropDown = new QuestionLevelDropDownModel();
                            questionLevelDropDown.QuestionLevelID = Convert.ToInt32(row["QuestionLevelID"]);
                            questionLevelDropDown.QuestionLevel = row["QuestionLevel"].ToString();
                            questionLevelList.Add(questionLevelDropDown);
                        }
                    }
                }
            }
            return questionLevelList;
        }
    }
}
