using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using QuizManagement.Models;

namespace QuizManagement.Controllers
{
    //[CheckAccess]
    public class QuizController : Controller
    {
        private readonly IConfiguration _configuration;

        public QuizController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult AddEdit_Quiz(int quizID)
        {
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Quiz_SelectByPK";
            command.Parameters.AddWithValue("@QuizID", quizID);
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            QuizModel quizModel = new QuizModel();

            foreach (DataRow dataRow in table.Rows)
            {
                quizModel.QuizID = Convert.ToInt32(@dataRow["QuizID"]);
                quizModel.QuizName = @dataRow["QuizName"].ToString();
                quizModel.TotalQuestions = Convert.ToInt32(@dataRow["TotalQuestions"]);
                quizModel.QuizDate = Convert.ToDateTime(dataRow["QuizDate"]);
                quizModel.UserID = Convert.ToInt32(@dataRow["UserID"]);
            }
            return View("AddEdit_Quiz",quizModel);
        }
        public IActionResult QuizSave(QuizModel quizModel)
        {
            if(quizModel.UserID <= 0)
            {
                ModelState.AddModelError("UserID", "A valid User is required.");
            }

            //if (!ModelState.IsValid)
            //{
            //    foreach (var modelStateKey in ModelState.Keys)
            //    {
            //        var value = ModelState[modelStateKey];
            //        foreach (var error in value.Errors)
            //        {
            //            Console.WriteLine($"Key: {modelStateKey}, Error: {error.ErrorMessage}");
            //        }
            //    }
            //}

            if (ModelState.IsValid)
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                
                if (quizModel.QuizID == null || quizModel.QuizID == 0)
                {
                    command.CommandText = "PR_Quiz_Insert";
                }
                else
                {
                    command.CommandText = "PR_Quiz_UpdateByPK";
                    command.Parameters.Add("@QuizID", SqlDbType.Int).Value = quizModel.QuizID;
                }
                command.Parameters.Add("@QuizName", SqlDbType.VarChar).Value = quizModel.QuizName;
                command.Parameters.Add("@TotalQuestions", SqlDbType.Int).Value = quizModel.TotalQuestions;
                command.Parameters.Add("@QuizDate", SqlDbType.DateTime).Value = quizModel.QuizDate.ToString("yyyy-MM-dd");
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = quizModel.UserID;
                command.ExecuteNonQuery();
                return RedirectToAction("QuizList");
            }

            return View("AddEdit_Quiz", quizModel);
        }
        public IActionResult QuizList(string? quizName,int? minQuestions,int? maxQuestions,DateTime? fromDate,DateTime? toDate)
        {
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Quiz_SelectAll";
            command.Parameters.AddWithValue("@QuizName", (object)quizName ?? DBNull.Value);
            command.Parameters.AddWithValue("@MinQuestions", (object)minQuestions ?? DBNull.Value);
            command.Parameters.AddWithValue("@MaxQuestions", (object)maxQuestions ?? DBNull.Value);
            command.Parameters.AddWithValue("@FromDate", (object)fromDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@ToDate", (object)toDate ?? DBNull.Value);

            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);

            return View(table);
        }
        public IActionResult QuizDelete(int quizID)
        {
            try
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                using(SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using(SqlCommand command = new SqlCommand("PR_Quiz_DeleteByPK", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.Add("@QuizID", SqlDbType.Int).Value = quizID;
                        command.ExecuteNonQuery();
                    }
                }
            }catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(ex.ToString());
            }
            return RedirectToAction("QuizList");
        }
    }
}
