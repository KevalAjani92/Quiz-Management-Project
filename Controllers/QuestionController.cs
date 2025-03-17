using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml;
using QuizManagement.Models;
using QuizManagement.Services;

namespace QuizManagement.Controllers
{
    [CheckAccess]
    public class QuestionController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly DropDownService _dropDownService;
        public QuestionController(IConfiguration configuration, DropDownService dropDownService)
        {
            _configuration = configuration;
            _dropDownService = dropDownService;
        }

        public IActionResult ExportToExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            string connectionString = _configuration.GetConnectionString("ConnectionString");
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sqlCommand.CommandText = "PR_Question_SelectAll";
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            DataTable data = new DataTable();
            data.Load(sqlDataReader);
            int i = 1;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("DataSheet");

                // Add headers
                worksheet.Cells[1, 1].Value = "Sr No.";
                worksheet.Cells[1, 2].Value = "QuestionText";
                worksheet.Cells[1, 3].Value = "Option-A";
                worksheet.Cells[1, 4].Value = "Option-B";
                worksheet.Cells[1, 5].Value = "Option-C";
                worksheet.Cells[1, 6].Value = "Option-D";
                worksheet.Cells[1, 7].Value = "QuestionMark";
                worksheet.Cells[1, 8].Value = "Question Level";
                worksheet.Cells[1, 9].Value = "UserName";
                worksheet.Cells[1, 10].Value = "Created";

                // Apply styling to the header row
                var headerRange = worksheet.Cells["A1:J1"];
                headerRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkGray); // Dark header row
                headerRange.Style.Font.Color.SetColor(System.Drawing.Color.White); // White text
                headerRange.Style.Font.Bold = true;
                headerRange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                // Set fixed column widths
                worksheet.Column(1).Width = 8;   // Sr No.
                worksheet.Column(2).Width = 40;  // QuestionText (Longer text, so wider column)
                worksheet.Column(3).Width = 25;  // Option A
                worksheet.Column(4).Width = 25;  // Option B
                worksheet.Column(5).Width = 25;  // Option C
                worksheet.Column(6).Width = 25;  // Option D
                worksheet.Column(7).Width = 12;  // QuestionMarks
                worksheet.Column(8).Width = 15;  // QuestionLevel
                worksheet.Column(9).Width = 20;  // UserName
                worksheet.Column(10).Width = 22; // Created Date


                // Add data
                int row = 2;
                foreach (DataRow item in data.Rows)
                {
                    worksheet.Cells[row, 1].Value = i;
                    worksheet.Cells[row, 2].Value = item["QuestionText"];
                    worksheet.Cells[row, 3].Value = item["OptionA"];
                    worksheet.Cells[row, 4].Value = item["OptionB"];
                    worksheet.Cells[row, 5].Value = item["OptionC"];
                    worksheet.Cells[row, 6].Value = item["OptionD"];
                    worksheet.Cells[row, 7].Value = item["QuestionMarks"];
                    worksheet.Cells[row, 8].Value = item["QuestionLevel"];
                    worksheet.Cells[row, 9].Value = item["UserName"];
                    worksheet.Cells[row, 10].Value = item["Created"];
                    row++;
                    i++;
                }
                // Auto-fit columns
                //worksheet.Cells.AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string excelName = $"Data-{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
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
            //Console.WriteLine(questionModel.CorrectOption.GetType());
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

                        //foreach (SqlParameter param in command.Parameters)
                        //{
                        //    Console.WriteLine($"{param.ParameterName}: {param.Value} ({param.SqlDbType})");
                        //}

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
        public IActionResult QuestionDetail(int questionID)
        {
            try
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                DataTable table = new DataTable();
                QuestionModel questionModel = new QuestionModel();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using(SqlCommand command = new SqlCommand("PR_Question_SelectByPK", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@QuestionID", SqlDbType.Int).Value = questionID;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            table.Load(reader);

                            foreach (DataRow row in table.Rows)
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
                                ViewBag.QuestionLevel = row["QuestionLevel"];
                            }
                        }
                    }
                }
                return View(questionModel);
            }
            catch(Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(ex.ToString());
                return View("QuestionList");
            }
        }
    }
}
