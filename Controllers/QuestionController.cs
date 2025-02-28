using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuizManagement.Models;
using QuizManagement.Services;

namespace QuizManagement.Controllers
{
    //[CheckAccess]
    public class QuestionController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly DropDownService _dropDownService;
        public QuestionController(IConfiguration configuration, DropDownService dropDownService)
        {
            _configuration = configuration;
            _dropDownService = dropDownService;
        }

        public IActionResult AddEdit_Question(int questionID)
        {
            ViewBag.QuestionLevelList = _dropDownService.GetQuestionLevelDropDown();
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            DataTable table = new DataTable();
            QuestionModel questionModel = new QuestionModel();
            questionModel.IsActive = true;
            using(SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using(SqlCommand command = new SqlCommand("PR_Question_SelectByPK", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@QuestionID", questionID);
                    using(SqlDataReader reader = command.ExecuteReader())
                    {
                        table.Load(reader);

                        foreach(DataRow row in table.Rows)
                        {
                            questionModel.QuestionID = Convert.ToInt32(@row["QuestionID"]);
                            questionModel.QuestionText = @row["QuestionText"].ToString();
                            questionModel.QuestionLevelID = Convert.ToInt32(@row["QuestionLevelID"]);
                            questionModel.OptionA = @row["OptionA"].ToString();
                            questionModel.OptionB = @row["OptionB"].ToString();
                            questionModel.OptionC = row["OptionC"] != DBNull.Value ? row["OptionC"].ToString() : null;
                            questionModel.OptionD = row["OptionD"] != DBNull.Value ? row["OptionD"].ToString() : null;
                            questionModel.CorrectOption = @row["CorrectOption"].ToString();
                            questionModel.QuestionMark = Convert.ToInt32(@row["QuestionMarks"]);
                            questionModel.IsActive = Convert.ToBoolean(@row["IsActive"]);
                            questionModel.UserID = Convert.ToInt32(@row["UserID"]);
                        }
                    }
                }
            }
            
            return View("AddEdit_Question",questionModel);
        }

        public IActionResult QuestionSave(QuestionModel questionModel)
        {
            if(questionModel.UserID <= 0)
            {
                ModelState.AddModelError("UserID", "A valid User is required.");
            }

            if (!ModelState.IsValid)
            {
                foreach (var modelStateKey in ModelState.Keys)
                {
                    var value = ModelState[modelStateKey];
                    foreach (var error in value.Errors)
                    {
                        Console.WriteLine($"Key: {modelStateKey}, Error: {error.ErrorMessage}");
                    }
                }
            }

            //Console.WriteLine("ModelState : " + ModelState.IsValid);
            //Console.WriteLine(questionModel.QuestionID);
            //Console.WriteLine(questionModel.QuestionText);
            //Console.WriteLine(questionModel.QuestionLevelID);
            //Console.WriteLine(questionModel.OptionA);
            //Console.WriteLine(questionModel.OptionB);
            //Console.WriteLine("=====");
            //Console.WriteLine(questionModel.OptionC == null);
            //Console.WriteLine("=====");
            //Console.WriteLine(questionModel.OptionD);
            Console.WriteLine(questionModel.CorrectOption.GetType());
            //Console.WriteLine(questionModel.QuestionMark);
            //Console.WriteLine(questionModel.IsActive);
            if (ModelState.IsValid)
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                using(SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using(SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        if (questionModel.QuestionID== null || questionModel.QuestionID == 0)
                        {
                            command.CommandText = "PR_Question_Insert";
                        }
                        else
                        {
                            command.CommandText = "PR_Question_UpdateByPK";
                            command.Parameters.Add("@QuestionID", SqlDbType.Int).Value = questionModel.QuestionID;
                        }
                        command.Parameters.Add("@QuestionText",SqlDbType.VarChar).Value = questionModel.QuestionText;
                        command.Parameters.Add("@QuestionLevelID", SqlDbType.Int).Value = questionModel.QuestionLevelID;
                        command.Parameters.Add("@OptionA", SqlDbType.VarChar).Value = questionModel.OptionA.ToString();
                        command.Parameters.Add("@OptionB", SqlDbType.VarChar).Value = questionModel.OptionB.ToString();
                        command.Parameters.Add("@OptionC", SqlDbType.VarChar).Value = questionModel.OptionC ?? (object)DBNull.Value;
                        command.Parameters.Add("@OptionD", SqlDbType.VarChar).Value = questionModel.OptionD ?? (object)DBNull.Value;
                        command.Parameters.Add("@CorrectOption", SqlDbType.VarChar).Value = questionModel.CorrectOption.ToString();
                        command.Parameters.Add("@QuestionMarks", SqlDbType.Int).Value = questionModel.QuestionMark;
                        command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = questionModel.IsActive ? 1 : 0;
                        command.Parameters.Add("@UserID", SqlDbType.Int).Value = questionModel.UserID;

                        foreach (SqlParameter param in command.Parameters)
                        {
                            Console.WriteLine($"{param.ParameterName}: {param.Value} ({param.SqlDbType})");
                        }

                        command.ExecuteNonQuery();
                        return RedirectToAction("QuestionList");
                    }
                }
            }


            return View("AddEdit_Question", questionModel);
        }

        //[Route("Question/QuestionList")]
        public IActionResult QuestionList(string? questionLevel,string? question,int? minMarks,int? maxMarks)
        {
            var tempList = _dropDownService.GetQuestionLevelDropDown();
            var questionLevels = tempList.Select(item => new SelectListItem
            {
                Value = item.QuestionLevel,
                Text = item.QuestionLevel
            }).ToList();
            ViewBag.QuestionLevelList = questionLevels;

            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            DataTable table = new DataTable();
            using(SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("PR_Question_SelectAll",connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@QuestionLevel", (object)questionLevel ?? DBNull.Value);
                    command.Parameters.AddWithValue("@QuestionText", (object)question ?? DBNull.Value);
                    command.Parameters.AddWithValue("@MinMarks", (object)minMarks ?? DBNull.Value);
                    command.Parameters.AddWithValue("@MaxMarks", (object)maxMarks ?? DBNull.Value);
                    using(SqlDataReader reader = command.ExecuteReader())
                    {
                        table.Load(reader);
                    }
                }
            }

            return View(table);
        }
        public IActionResult QuestionDelete(int questionID)
        {
            try
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("PR_Question_DeleteByPK", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.Add("@QuestionID", SqlDbType.Int).Value = questionID;
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(ex.ToString());
            }
            return RedirectToAction("QuestionList");
        }
        public IActionResult QuestionDetail()
        {
            return View();
        }
    }
}
