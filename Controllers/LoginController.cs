using Dapper;
using GelirGider.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Net.Mail;
using System.Net;

namespace GelirGider.Controllers
{
    public class LoginController : Controller
    {
        
        public IActionResult Login()
        {
            ViewData["email"] = HttpContext.Session.GetString("email");
            ViewData["Id"] = HttpContext.Session.GetInt32("Id");
            return View(new Login());
        }
        [HttpPost]
        public IActionResult Login(Login model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Error"] = "Form eksik veya hatalı!";
                return View("Login", model);
            }
            model.Password = Helper.Hash(model.Password);
            using var connection = new SqlConnection(connectionString);
            var login = connection.Query<Login>("SELECT * FROM Users").ToList();

            foreach (var user in login)
            {
                if (user.Email == model.Email && user.Password == model.Password)
                {
                    ViewData["Msg"] = "Giriş Başarılı";
                    HttpContext.Session.SetString("email", user.Email);
                    HttpContext.Session.SetInt32("Id", user.Id);


                    ViewBag.IdUser = user.Id;
                    return RedirectToAction("Index", "Home");

                }

            }
            ViewData["Error"] = "E-posta adresi veya şifre yanlış";
            return View("Login", model);
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(Register model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.MessageCssClass = "alert-danger";
                ViewBag.Message = "Eksik veya hatalı işlem yaptın";
                return View("Message");
            }
            using var connection = new SqlConnection(connectionString);
            var login = connection.QueryFirstOrDefault<Login>("SELECT * FROM Users WHERE Email = @Email ", new { model.Email });

            if (model.Password != model.ConfirmPassword)
            {
                ViewData["Message"] = "Sifreler uyusmuyor";
                return View("Register", model);
            }
            else if (login?.Email == model.Email)
            {
                ViewData["Message"] = "Bu mail kayıtlı";
                return View("Register", model);
            }
            else
            {
                model.Password = Helper.Hash(model.Password);

                var client = new SmtpClient("smtp.eu.mailgun.org", 587)
                {
                    Credentials = new NetworkCredential("postmaster@bildirim.mkadirgulgun.com.tr", "cb5edda1ad0913ef5144e9fc0f8484a2-fe9cf0a8-3d53c1ae"),
                    EnableSsl = true
                };
                var Key = Guid.NewGuid().ToString();

                var signup = "INSERT INTO users (Name, Password, Email) VALUES (@Name, @Password, @Email)";

                var data = new
                {
                    model.Name,
                    model.Password,
                    model.Email,
                };

                var rowsAffected = connection.Execute(signup, data);

                ViewBag.Subject = "Hoş Geldiniz! Kayıt İşleminiz Başarıyla Tamamlandı";
                ViewBag.Body = $"<p>Merhaba ,</p>\r\n\r\n    <p>Aramıza katıldığınız için teşekkür ederiz!\r\n    <p>Kayıt işleminiz başarıyla tamamlandı. Artık platformumuzun sunduğu tüm özellikleri kullanmaya başlayabilirsiniz.\r\n    <p>Hesabınıza giriş yapmak için aşağıdaki linke tıklayabilirsiniz:</p>\r\n\r\n    <p><a href=\"https://gelirgider.mkadirgulgun.com.tr\" target=\"_blank\">Giriş yapmak için tıklayın</a></p>\r\n\r\n    <p>Herhangi bir sorunuz veya yardıma ihtiyacınız olursa, lütfen bize ulaşmaktan çekinmeyin. Size yardımcı olmaktan memnuniyet duyarız.</p>\r\n\r\n    <p>En kısa sürede sizinle tekrar buluşmak dileğiyle!</p>\r\n\r\n    <p>Saygılarımızla,<br>";
                ViewBag.MessageCssClass = "alert-success";
                ViewBag.Message = "Başarıyla kayıt olundu. Onaylamak için mail kutunuza gidin";
                ViewBag.Return = "Message";
                SendMail(model);
                return View("Message");
            }
        }

        public IActionResult SendMail(Register model)
        {
            
            var mailMessage = new MailMessage
            {
                From = new MailAddress("bildirim@gelirgider.com.tr", "GelirGider.com"),
                //ReplyTo = new MailAddress("info@mkadirgulgun.com.tr", "Mehmet Kadir Gülgün"),
                Subject = ViewBag.Subject,
                Body = ViewBag.Body,
                IsBodyHtml = true,
            };

            mailMessage.ReplyToList.Add(model.Email);
            //mailMessage.To.Add("mkadirgulgun@gmail.com");
            mailMessage.To.Add(new MailAddress($"{model.Email}"));

            client.Send(mailMessage);
            return RedirectToAction(ViewBag.Return);

        }
    }
}
