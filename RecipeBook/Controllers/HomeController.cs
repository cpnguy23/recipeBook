using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using RecipeBook.Models;

namespace RecipeBook.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _env;
        // 8 GB upload ceiling (matches Kestrel's max request size)
        private const long MaxPhotoBytes = 8L * 1024 * 1024 * 1024;
        private static readonly string[] AllowedPhotoTypes = { "image/jpeg", "image/png", "image/webp" };

        private readonly Data.ApplicationDBContext _db;

        public HomeController(Data.ApplicationDBContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }
        private static readonly Dictionary<string, (string Title, string Blurb, List<Recipe> Recipes)> Categories = new()
        {
            ["breakfast"] = ("Breakfast", "Mornings made easy — pancakes, smoothies and bake-ahead eggs.", new List<Recipe>
            {
                new() { Title = "Fluffy Buttermilk Pancakes", Description = "Tall, diner-style pancakes with a whisper of vanilla. The batter comes together in one bowl.", Time = "25 min", Category = "Breakfast", CategorySlug = "breakfast", Tag = "Popular" },
                new() { Title = "Berry Overnight Oats", Description = "Rolled oats soaked in milk and yogurt, layered with jam and fresh berries.", Time = "10 min", Category = "Breakfast", CategorySlug = "breakfast" },
                new() { Title = "Baked Berry Oatmeal", Description = "A cozy oatmeal bake that reheats all week. Cut it into squares and go.", Time = "40 min", Category = "Breakfast", CategorySlug = "breakfast" },
                new() { Title = "Vanilla Chia Pudding", Description = "Make it the night before and breakfast is already done when you wake up.", Time = "5 min + chill", Category = "Breakfast", CategorySlug = "breakfast" },
            }),
            ["lunch"] = ("Lunch", "Quick midday plates that don't feel like an afterthought.", new List<Recipe>
            {
                new() { Title = "Rosé Pasta Salad", Description = "Cold pasta tossed in a creamy pink dressing with basil and mozzarella pearls.", Time = "20 min", Category = "Lunch", CategorySlug = "lunch", Tag = "Popular" },
                new() { Title = "Strawberry Caprese", Description = "Strawberries standing in for tomatoes — balsamic glaze ties it all together.", Time = "10 min", Category = "Lunch", CategorySlug = "lunch" },
                new() { Title = "Beet Hummus Wrap", Description = "Roasted beet hummus, crunchy cucumber and greens rolled in a soft tortilla.", Time = "15 min", Category = "Lunch", CategorySlug = "lunch" },
                new() { Title = "Tomato Basil Soup", Description = "Creamy pantry soup, best with a grilled cheese for dunking.", Time = "25 min", Category = "Lunch", CategorySlug = "lunch" },
            }),
            ["dinner"] = ("Dinner", "Weeknight dinners the whole table will actually eat.", new List<Recipe>
            {
                new() { Title = "Creamy Rosé Pasta", Description = "Tomatoes and cream meet in the middle. Finish with parmesan and black pepper.", Time = "30 min", Category = "Dinner", CategorySlug = "dinner", Tag = "Popular" },
                new() { Title = "Honey Lavender Salmon", Description = "A quick honey glaze with a tiny pinch of culinary lavender. Bakes in one pan.", Time = "35 min", Category = "Dinner", CategorySlug = "dinner" },
                new() { Title = "Pink Coconut Curry", Description = "Gentle coconut curry with roasted vegetables over jasmine rice.", Time = "40 min", Category = "Dinner", CategorySlug = "dinner" },
                new() { Title = "One-Pan Chicken & Potatoes", Description = "Season everything, slide it in the oven, and the cleanup is one pan.", Time = "45 min", Category = "Dinner", CategorySlug = "dinner", Tag = "New" },
            }),
            ["dessert"] = ("Dessert", "Sweet things, extra pink. Sprinkles encouraged.", new List<Recipe>
            {
                new() { Title = "Strawberry Shortcake", Description = "Tender shortcake layers with macerated berries and softly whipped cream.", Time = "50 min", Category = "Dessert", CategorySlug = "dessert", Tag = "Popular" },
                new() { Title = "Rose Macarons", Description = "Delicate shells with a rose buttercream center. A weekend project worth it.", Time = "1 hr", Category = "Dessert", CategorySlug = "dessert" },
                new() { Title = "Pink Velvet Cupcakes", Description = "Buttermilk cupcakes with a natural pink crumb and cream cheese frosting.", Time = "45 min", Category = "Dessert", CategorySlug = "dessert" },
                new() { Title = "No-Bake Berry Cheesecake", Description = "A graham crust, a fluffy filling, four hours in the fridge. No oven required.", Time = "20 min + chill", Category = "Dessert", CategorySlug = "dessert", Tag = "New" },
            }),
            ["snack"] = ("Snacks", "Little bites for movie nights and afternoon slumps.", new List<Recipe>
            {
                new() { Title = "Pink Party Popcorn", Description = "Popcorn drizzled with white chocolate and topped with freeze-dried strawberry dust.", Time = "10 min", Category = "Snacks", CategorySlug = "snack" },
                new() { Title = "Berry Parfait Cups", Description = "Yogurt, granola and berry compote layered in little glasses.", Time = "5 min", Category = "Snacks", CategorySlug = "snack" },
                new() { Title = "Strawberry Energy Balls", Description = "No-bake oats, dates and dried strawberries rolled into bite-size balls.", Time = "15 min", Category = "Snacks", CategorySlug = "snack" },
                new() { Title = "Raspberry Scones", Description = "Cream scones with raspberries folded in gently so they streak the dough pink.", Time = "30 min", Category = "Snacks", CategorySlug = "snack", Tag = "New" },
            }),
            ["drinks"] = ("Drinks", "Something pretty in the glass.", new List<Recipe>
            {
                new() { Title = "Strawberry Milk", Description = "Real strawberry syrup stirred into cold milk. Better than the packets.", Time = "10 min", Category = "Drinks", CategorySlug = "drinks", Tag = "Popular" },
                new() { Title = "Pink Lemonade", Description = "Fresh-squeezed lemonade with a splash of raspberry for the color.", Time = "10 min", Category = "Drinks", CategorySlug = "drinks" },
                new() { Title = "Lavender Latte", Description = "A homemade lavender syrup, just a little, over espresso and steamed milk.", Time = "10 min", Category = "Drinks", CategorySlug = "drinks" },
                new() { Title = "Berry Iced Tea", Description = "Hibiscus tea brewed strong and sweetened with crushed berries.", Time = "15 min", Category = "Drinks", CategorySlug = "drinks" },
            }),
        };

        // this is our http get and post for the home page
        public IActionResult Index()
        {   // reading cookies on line 12-13
            string username = Request.Cookies["username"];
            ViewBag.WelcomeUser = username ?? "Guest";

            var allRecipes = Categories.Values.SelectMany(c => c.Recipes).ToList();
            ViewBag.Featured = allRecipes.First(r => r.Title == "Creamy Rosé Pasta");
            ViewBag.Popular = allRecipes.Where(r => r.Tag == "Popular").ToList();
            ViewBag.Latest = allRecipes.SkipLast(1).TakeLast(6).ToList();
            ViewBag.Categories = Categories.ToDictionary(
                c => c.Key,
                c => (c.Value.Title, Count: c.Value.Recipes.Count));
            return View();
        }

        public IActionResult Recipes()
        {
            ViewBag.Categories = Categories;
            return View();
        }

        public IActionResult Category(string id)
        {
            if (string.IsNullOrEmpty(id) || !Categories.TryGetValue(id.ToLower(), out var category))
            {
                return RedirectToAction(nameof(Recipes));
            }

            ViewBag.CategoryTitle = category.Title;
            ViewBag.Blurb = category.Blurb;

            // user-submitted recipes from the database, newest first
            var posts = _db.RecipePosts
                .Where(p => p.CategorySlug == id.ToLower())
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            ViewBag.Posts = posts;
            ViewBag.Recipes = category.Recipes;
            return View();
        }

        // ===== Add New > Recipe =====
        [HttpGet]
        public IActionResult NewRecipe()
        {
            return View(new RecipeUploadViewModel());
        }

        [HttpPost]
        [RequestSizeLimit(MaxPhotoBytes)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewRecipe(RecipeUploadViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!Categories.ContainsKey(model.CategorySlug.ToLower()))
            {
                ModelState.AddModelError(nameof(model.CategorySlug), "Pick one of the listed meal types.");
                return View(model);
            }

            var post = new RecipePost
            {
                Title = model.Title.Trim(),
                Instructions = model.Instructions.Trim(),
                CategorySlug = model.CategorySlug.ToLower(),
            };

            if (model.Photo is { Length: > 0 })
            {
                if (model.Photo.Length > MaxPhotoBytes)
                {
                    ModelState.AddModelError(nameof(model.Photo), "That photo is larger than the 8 GB limit.");
                    return View(model);
                }
                if (!AllowedPhotoTypes.Contains(model.Photo.ContentType))
                {
                    ModelState.AddModelError(nameof(model.Photo), "Only JPG, PNG or WebP images please.");
                    return View(model);
                }

                post.PhotoFileName = await SaveCompressedPhotoAsync(model.Photo);
                post.PhotoContentType = "image/jpeg"; // recompressed below as JPEG
            }

            _db.RecipePosts.Add(post);
            await _db.SaveChangesAsync();

            model.ResultMessage = $"\"{post.Title}\" was added to {Categories[post.CategorySlug].Title}. Nice!";
            ModelState.Clear();
            return View(new RecipeUploadViewModel { ResultMessage = model.ResultMessage });
        }

        /// <summary>
        /// Saves the uploaded photo under wwwroot/uploads. Large originals are
        /// recompressed: scaled down to at most 1600px on the long edge and
        /// re-encoded as quality-80 JPEG, so a huge camera photo becomes a
        /// few hundred KB on disk instead of multiple megabytes.
        /// </summary>
        private async Task<string?> SaveCompressedPhotoAsync(IFormFile photo)
        {
            var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsDir);

            var fileName = $"{Guid.NewGuid():N}.jpg";
            var filePath = Path.Combine(uploadsDir, fileName);

            using var stream = photo.OpenReadStream();
            using var original = Image.Load(stream);

            const int maxEdge = 1600;
            if (original.Width > maxEdge || original.Height > maxEdge)
            {
                var scale = (double)maxEdge / Math.Max(original.Width, original.Height);
                original.Mutate(i => i.Resize(
                    (int)Math.Round(original.Width * scale),
                    (int)Math.Round(original.Height * scale)));
            }

            await original.SaveAsJpegAsync(filePath, new JpegEncoder { Quality = 80 });
            return fileName;
        }

        // ===== Add New > Meal Type =====
        [HttpGet]
        public IActionResult NewMealType()
        {
            return View(new MealTypeViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult NewMealType(MealTypeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var slug = model.Name.Trim().ToLower().Replace(' ', '-');
            if (Categories.ContainsKey(slug))
            {
                ModelState.AddModelError(nameof(model.Name), "That meal type already exists.");
                return View(model);
            }

            Categories[slug] = (model.Name.Trim(), $"Community recipes tagged {model.Name.Trim()}.", new List<Recipe>());
            model.ResultMessage = $"Meal type \"{model.Name.Trim()}\" added. It now shows on the recipes page.";
            ModelState.Clear();
            return View(new MealTypeViewModel { ResultMessage = model.ResultMessage });
        }

        [HttpGet]
        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost] // we are making a contact form for customers to fill out and submit, but i also want to keep a database of their messages in there, but idk how to do that yet. (not important)
        public IActionResult Contact(CustomerDatabase model)
        {

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
