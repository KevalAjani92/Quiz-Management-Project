using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using QuizManagement.Services;

namespace QuizManagement.Controllers
{
    [CheckAccess]
    public class QuizwiseQuestionController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly DropDownService _dropDownService;
        public QuizwiseQuestionController(IConfiguration configuration,DropDownService dropDownService)
        {
            _configuration = configuration;
            _dropDownService = dropDownService;
        }
        public IActionResult AddEdit_QuizwiseQuestion(int quizID)
        {
            ViewBag.QuizList = _dropDownService.GetQuizDropDown();
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            DataTable table = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("PR_QuizWiseQuestion_SelectAll", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        table.Load(reader);
                    }
                }
            }
            ViewBag.quizID = quizID;
            return View(table);
        }
        public IActionResult QuizwiseQuestionList()
        {
            DataTable table = new DataTable();
            try
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("PR_QuizwiseQuestions_QuizList", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            table.Load(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(ex.ToString());
            }

            return View(table);
        }
        public IActionResult QuizwiseQuestionDetails(int quizID)
        {
            DataTable table = new DataTable();
            try
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("PR_QuizwiseQuestions_QuestionList_From_QuizID", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.Add("@QuizID",SqlDbType.Int).Value = quizID;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            table.Load(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(ex.ToString());
            }

            return View(table);
        }
        public IActionResult DeleteQuestionFromQuiz(int quizWiseQuestionID)
        {
            try
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("PR_QuizWiseQuestions_DeleteByPK", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.Add("@QuizWiseQuestionsID", SqlDbType.Int).Value = quizWiseQuestionID;
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch(Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(ex.ToString());
            }
            return RedirectToAction("QuizwiseQuestionList");
        }

        public IActionResult DeleteQuiz(int quizID)
        {
            try
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("PR_QuizwiseQuestions_QuizDelete", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.Add("@QuizID", SqlDbType.Int).Value = quizID;
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(ex.ToString());
            }
            return RedirectToAction("QuizwiseQuestionList");
        }

        [HttpPost]
        public JsonResult MultipleQuestionRemoveFromQuiz([FromBody] List<string> data)
        {
            try
            {
                foreach(string item in data)
                {
                    int id = Convert.ToInt32(item);
                    DeleteQuestionFromQuiz(id);
                    Console.WriteLine(id);
                }
                return Json(new { message = "Success" });
            }catch(Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(ex.ToString());
                return Json(new { message = "Failed" });
            }
        }

        
        public IActionResult GetQuestionID_From_QuizID([FromBody] int quizID)
        {
            DataTable table = new DataTable();
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            using(SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using(SqlCommand command = new SqlCommand("PR_QuizwiseQuestions_QuestionIDs_From_QuizID",connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add("@QuizID", SqlDbType.Int).Value = quizID;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        table.Load(reader);
                    }
                }
            }
            List<string> data = new List<string>();

            foreach (DataRow row in table.Rows)
            {
                string rowData = @row["QuestionID"].ToString(); // Convert row to a string
                data.Add(rowData);
            }
            return Json(new {message = "Success",dataList = data});
        }
        public IActionResult GetQuizDetails([FromBody] int quizID)
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
            DataRow row = table.Rows[0];

            return Json(new { message = "Success", quizName = row["QuizName"], totalQuestion = row["TotalQuestions"] });
        }

        [HttpPost]
        [Route("QuizwiseQuestion/AddQuestions_Into_Quiz")]
        public IActionResult AddQuestions_Into_Quiz([FromBody] QuizQuestionRequest request)
        {
            try
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_QuizwiseQuestions_AddMultipleQuestion_Into_Quiz";
                command.Parameters.Add("@QuizID", SqlDbType.Int).Value = request.QuizID;
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = request.UserID;
                command.Parameters.Add("@QuestionIDs", SqlDbType.VarChar).Value = request.Values;
                command.ExecuteNonQuery();
                return Json(new { message = "Success" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return Json(new { message = "Unsuccess" });
            }
        }
    }
    public class QuizQuestionRequest
    {
        public string Values { get; set; }  // Comma-separated question IDs
        public int QuizID { get; set; }
        public int UserID { get; set; }
    }
}
