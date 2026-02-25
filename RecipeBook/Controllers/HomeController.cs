using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RecipeBook.Models;

namespace RecipeBook.Controllers
{
    public class HomeController : Controller
    {
        // this is our http get and post for the home page
        public IActionResult Index()
        {   // reading cookies on line 12-13
            string username = Request.Cookies["username"];
            ViewBag.WelcomeUser = username ?? "Guest";
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Contact()
        {
            ViewBag.successMessage = TempData["successMessage"];
            return View();
        }

        [HttpPost] // we are making a contact form for customers to fill out and submit, but i also want to keep a database of their messages in there, but idk how to do that yet. (not important)
        public IActionResult Contact(CustomerDatabase model)
        {
            if (model.Email.Contains("@") == false || string.IsNullOrEmpty(model.Email))
            {
                ViewBag.errorMessage = "Email invalid.";
                return View(model);
            }

            // my attempt of developing cookies
            Response.Cookies.Append("username", model.Name);
            TempData["successMessage"] = $"{model.Name}, thank you for your submission. We will contact later bc i hate u";
            return RedirectToAction("Contact");
        }


            [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
