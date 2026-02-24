using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RecipeBook.Models;

namespace RecipeBook.Controllers
{
    public class HomeController : Controller
    {
        // this is our http get and post for the home page
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Contact(CustomerDatabase model)
        {
            if (model.Email.Contains("@") == false)
            {
                ViewBag.errorMessage = "Email invalid.";
                return View("Contact", model);
            }

            
            ViewBag.successMessage = model.Name + ", thank you, we will contact later bc i hate u";
            ModelState.Clear();
            return View(model);
        }


            [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
