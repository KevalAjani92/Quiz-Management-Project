using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using QuizManagement.Models;
using OfficeOpenXml;

namespace QuizManagement.Controllers
{
    [CheckAccess]
    public class QuizController : Controller
    {
        private readonly IConfiguration _configuration;

        public QuizController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult ExportToExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            string connectionString = _configuration.GetConnectionString("ConnectionString");
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sqlCommand.CommandText = "PR_Quiz_SelectAll";
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            DataTable data = new DataTable();
            data.Load(sqlDataReader);
            int i = 1;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("DataSheet");

                // Add headers
                worksheet.Cells[1, 1].Value = "Sr No.";
                worksheet.Cells[1, 2].Value = "QuizName";
                worksheet.Cells[1, 3].Value = "QuizDate";
                worksheet.Cells[1, 4].Value = "TotalQuestions";
                worksheet.Cells[1, 5].Value = "UserName";
                worksheet.Cells[1, 6].Value = "Creation Date";

                // Apply styling to the header row
                var headerRange = worksheet.Cells["A1:F1"];
                headerRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkGray); // Dark header row
                headerRange.Style.Font.Color.SetColor(System.Drawing.Color.White); // White text
                headerRange.Style.Font.Bold = true;
                headerRange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                // Add data
                int row = 2;
                foreach (DataRow item in data.Rows)
                {
                    worksheet.Cells[row, 1].Value = i;
                    worksheet.Cells[row, 2].Value = item["QuizName"];
                    worksheet.Cells[row, 3].Value = item["QuizDate"];
                    worksheet.Cells[row, 4].Value = item["TotalQuestions"];
                    worksheet.Cells[row, 5].Value = item["UserName"];
                    worksheet.Cells[row, 6].Value = item["Created"];
                    row++;
                    i++;
                }
                // Auto-fit columns
                worksheet.Cells.AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string excelName = $"Data-{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
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
