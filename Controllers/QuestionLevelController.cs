using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using QuizManagement.Models;

namespace QuizManagement.Controllers
{
    //[CheckAccess]
    public class QuestionLevelController : Controller
    {
        private readonly IConfiguration _configuration;
        public QuestionLevelController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult AddEdit_QuestionLevel(int questionLevelID)
        {
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            DataTable table = new DataTable();
            QuestionLevelModel questionLevelModel = new QuestionLevelModel();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using(SqlCommand command = new SqlCommand("PR_QuestionLevel_SelectByPK", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@QuestionLevelID", questionLevelID);

                    using(SqlDataReader reader = command.ExecuteReader())
                    {
                        table.Load(reader);
                        
                        foreach(DataRow row in table.Rows)
                        {
                            questionLevelModel.QuestionLevelID = Convert.ToInt32(@row["QuestionLevelID"]);
                            questionLevelModel.QuestionLevel = @row["QuestionLevel"].ToString();
                            questionLevelModel.UserID = Convert.ToInt32(@row["UserID"]);
                        }
                    }
                }
            }
            return View(questionLevelModel);
        }
        public IActionResult QuestionLevelSave(QuestionLevelModel questionLevelModel)
        {
            if (questionLevelModel.UserID <= 0)
            {
                ModelState.AddModelError("UserID", "A valid User is required.");
            }
            

            if (ModelState.IsValid)
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                using(SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using(SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        if(questionLevelModel.QuestionLevelID == null || questionLevelModel.QuestionLevelID == 0)
                        {
                            command.CommandText = "PR_QuestionLevel_Insert";
                        }
                        else
                        {
                            command.CommandText = "PR_QuestionLevel_UpdateByPK";
                            command.Parameters.Add("@QuestionLevelID", SqlDbType.Int).Value = questionLevelModel.QuestionLevelID;
                        }
                        command.Parameters.Add("@QuestionLevel", SqlDbType.VarChar).Value = questionLevelModel.QuestionLevel;
                        command.Parameters.Add("@UserID", SqlDbType.VarChar).Value = questionLevelModel.UserID;
                        command.ExecuteNonQuery();
                        return RedirectToAction("QuestionLevelList");
                    }
                }
            }
            return View("AddEdit_QuestionLevel", questionLevelModel);
        }
        public IActionResult QuestionLevelList()
        {
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            DataTable table = new DataTable();

            using(SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("PR_QuestionLevel_SelectAll", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    using(SqlDataReader reader = command.ExecuteReader())
                    {
                        table.Load(reader);
                    }
                }
            }
            return View(table);
        }
        public IActionResult QuestionLevelDelete(int questionLevelID)
        {
            try
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("PR_QuestionLevel_DeleteByPK", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.Add("@QuestionLevelID", SqlDbType.Int).Value = questionLevelID;
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(ex.ToString());
            }
            return RedirectToAction("QuestionLevelList");
        }
    }
}
