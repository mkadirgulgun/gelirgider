using Dapper;
using GelirGider.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace GelirGider.Controllers
{
    public class AdminController : Controller
    {
        
        public bool CheckLogin()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("email")))
            {
                return false;
            }

            return true;
        }

        public IActionResult Index()
        {
            if (!CheckLogin())
            {

                return RedirectToAction("Login", "Login");
            }
            var adminModel = new ChartModel();
            ViewData["Id"] = HttpContext.Session.GetInt32("Id");

            using (var connection = new SqlConnection(connectionString))
            {
                var sql = "SELECT * FROM Income WHERE UserId = @Id";
                var income = connection.Query<IncomeModel>(sql, new { Id = ViewData["Id"] }).ToList();

                adminModel.Income = income;

            }
            using (var connection = new SqlConnection(connectionString))
            {
                var sql = "SELECT * FROM expense WHERE UserId = @Id";
                var expense = connection.Query<ExpenseModel>(sql, new { Id = ViewData["Id"] }).ToList();

                adminModel.Expense = expense;

            }
            

            return View(adminModel);
        }
        public IActionResult EditIncome(int id)
        {

            if (!CheckLogin())
            {

                return RedirectToAction("Login", "Login");
            }
            using var connection = new SqlConnection(connectionString);

            var post = connection.QuerySingleOrDefault<IncomeModel>("SELECT * FROM Income WHERE Id = @Id", new { Id = id });

            return View(post);
        }
        [HttpPost]
        public IActionResult EditIncome(IncomeModel model)
        {
            using var connection = new SqlConnection(connectionString);

            var sql = "UPDATE Income SET Income=@Income, IncomePrice=@IncomePrice WHERE Id = @Id";

            var parameters = new
            {
                model.Income,
                model.IncomePrice,
                model.Id,
            };
            var affectedRows = connection.Execute(sql, parameters);

            ViewBag.Message = "Güncellendi.";
            ViewBag.MessageCssClass = "alert-success";
            return View("Message");
        }
        public IActionResult DeleteIncome(int id)
        {
            using var connection = new SqlConnection(connectionString);
            var sql = "DELETE FROM Income WHERE Id = @Id";

            var rowsAffected = connection.Execute(sql, new { Id = id });

            return RedirectToAction("Index");
        }

        public IActionResult EditExpense(int id)
        {

            if (!CheckLogin())
            {

                return RedirectToAction("Login", "Login");
            }
            using var connection = new SqlConnection(connectionString);

            var post = connection.QuerySingleOrDefault<ExpenseModel>("SELECT * FROM expense WHERE Id = @Id", new { Id = id });

            return View(post);
        }
        [HttpPost]
        public IActionResult EditExpense(ExpenseModel model)
        {
            using var connection = new SqlConnection(connectionString);

            var sql = "UPDATE expense SET Expense=@Expense, Price=@Price WHERE Id = @Id";

            var parameters = new
            {
                model.Expense,
                model.Price,
                model.Id,
            };
            var affectedRows = connection.Execute(sql, parameters);

            ViewBag.Message = "Güncellendi.";
            ViewBag.MessageCssClass = "alert-success";
            return View("Message");
        }
        public IActionResult DeleteExpense(int id)
        {
            using var connection = new SqlConnection(connectionString);
            var sql = "DELETE FROM expense WHERE Id = @Id";

            var rowsAffected = connection.Execute(sql, new { Id = id });

            return RedirectToAction("Index");
        }
    }
}
