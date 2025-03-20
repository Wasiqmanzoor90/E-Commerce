using System.Diagnostics;
using E_Commerce.Data;
using E_Commerce.Models;
using E_Commerce.Models.DomainModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Controllers
{
    public class HomeController(SqldbContext dbcontext) : Controller
    {
     
        private readonly SqldbContext dbcontext = dbcontext;
     



        [HttpGet]
        public IActionResult Index()
        {
           return View();
        }


        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            var user = HttpContext.Items["User"] as User;
            if (user == null || user.Role != Types.Role.Buyer)
            {
                return RedirectToAction("Index", "Home");
            }
            if (string.IsNullOrEmpty(query))
            {
                return RedirectToAction("Index", "Home");
            }
            var que = await dbcontext.Products
                .Where(p => p.ProductName.ToLower().Contains(query) || p.Category.ToLower().Contains(query)
                || p.SubCategory.ToLower().Contains(query) || p.ProductDescription.ToLower().Contains(query)
                && p.IsDeleted == false && p.IsArchived == false)
                .ToListAsync();
            return View("Search", que);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
