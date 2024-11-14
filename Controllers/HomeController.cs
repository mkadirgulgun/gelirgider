using Dapper;
using GelirGider.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Reflection;

namespace GelirGider.Controllers
{
    public class HomeController : Controller
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
            ViewData["Id"] = HttpContext.Session.GetInt32("Id");
            var userId = ViewData["Id"];
            using var connection = new SqlConnection(connectionString);

            var income = connection.Query<IncomeModel>("SELECT * FROM Income WHERE Income.UserId = @Id", new { Id = ViewData["Id"] }).ToList();

            var expense = connection.Query<ExpenseModel>("SELECT * FROM expense WHERE expense.UserId = @Id", new { Id = ViewData["Id"] }).ToList();

            var Income = "SELECT SUM(IncomePrice) AS IncomePrice FROM Income WHERE UserId = @Id";
            var TotalIncome = connection.QueryFirstOrDefault<IncomeModel>(Income, new { Id = userId });

            var Expense = "SELECT SUM(Price) AS Price FROM expense WHERE UserId = @Id";
            var TotalExpense = connection.QueryFirstOrDefault<ExpenseModel>(Expense, new { Id = userId });
            
            ViewBag.TotalIncome = TotalIncome.IncomePrice;
            ViewBag.TotalExpense = TotalExpense.Price;

            ViewBag.Income = income;
            ViewBag.Expense = expense;
            return View(new ChartModel());
        }
        [HttpPost]
        public IActionResult AddIncome(IncomeModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.MessageCssClass = "alert-danger";
                ViewBag.Message = "Eksik veya hatalý iþlem yaptýn";
                return View("Message");
            }
            ViewData["email"] = HttpContext.Session.GetString("email");
            ViewData["Id"] = HttpContext.Session.GetInt32("Id");
            var userId = ViewData["Id"];

            using var connection = new SqlConnection(connectionString);
            var sql = "INSERT INTO Income (Income, IncomePrice, UserId) VALUES (@Income, @IncomePrice, @userId)";
            var data = new
            {
                model.IncomePrice,
                model.Income,
                userId
            };
            var affectedRows = connection.Execute(sql, data);

            return RedirectToAction("Index");


        }
        [HttpPost]
        public IActionResult AddExpense(ExpenseModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.MessageCssClass = "alert-danger";
                ViewBag.Message = "Eksik veya hatalý iþlem yaptýn";
                return View("Message");
            }
            ViewData["email"] = HttpContext.Session.GetString("email");

            ViewData["Id"] = HttpContext.Session.GetInt32("Id");
            var userId = ViewData["Id"];

            using var connection = new SqlConnection(connectionString);
            var sql = "INSERT INTO expense (Expense, Price, UserId) VALUES (@Expense, @Price, @userId)";

            var data = new
            {
                model.Expense,
                model.Price,
                userId
            };
            var affectedRows = connection.Execute(sql, data);

            return RedirectToAction("Index");

        }
        

    }
}
