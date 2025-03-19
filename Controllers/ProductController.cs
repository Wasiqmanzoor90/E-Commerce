using CloudinaryDotNet;
using E_Commerce.Data;
using E_Commerce.Interface;
using E_Commerce.Models.DomainModel;
using E_Commerce.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Razorpay.Api;
using static System.Net.Mime.MediaTypeNames;
using Product = E_Commerce.Models.DomainModel.Product;

namespace E_Commerce.Controllers
{
    public class ProductController(SqldbContext dbcontext, IJasonToken jtoken, ICloudinaryInterface cloudinary) : Controller
    {
        private readonly SqldbContext _dbcontext = dbcontext;
        private readonly IJasonToken _jasonToken = jtoken;
        private readonly ICloudinaryInterface _cloudinary = cloudinary;
        public string errorMessage = "";
        public string successMessage = "";

        [HttpGet]
        public async Task<IActionResult> SellerUi()
        {
            var Seller = HttpContext.Items["User"] as User;
            if (Seller == null || Seller.Role != Types.Role.Seller)
            {
                return RedirectToAction("Index", "Home");
            }

            var products = await _dbcontext.Products.Where(p => p.SellerId == Seller.UserId).ToListAsync();
            return View(products);
        }


        [HttpGet]
        public IActionResult Register()
        {
            var Seller = HttpContext.Items["User"] as User;

            if (Seller == null || Seller.Role != Types.Role.Seller)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();

        }


        [HttpPost]
        public async Task<IActionResult> Register([FromForm] Product product, IFormFile file)
        {
            var user = HttpContext.Items["User"] as User;
            if(user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            string imgurl = null;
            if (file != null && file.Length > 0)
            {
                imgurl = await _cloudinary.UploadImageAsync(file);
            }
            var newproduct = new Product
            {
                ProductId = Guid.NewGuid(),
                ProductName = product.ProductName,
                ProductDescription = product.ProductDescription,
                ProductPrice = product.ProductPrice,
                ProductQuantity = product.ProductQuantity,
                Category = product.Category,
                SubCategory = product.SubCategory,
                ProductPicUrl = imgurl,
                SellerId = user.UserId

            };
            try
            {

                _dbcontext.Add(newproduct);
                await _dbcontext.SaveChangesAsync();
                ViewBag.successMessage = "Product registered successfully!";

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving product: {ex.Message}");
                ViewBag.errorMessage = "Failed to save product.";
            }

            return View();
        }

     
        [HttpGet]
        public async Task<IActionResult> SellerProduct()
        {
            var seller = HttpContext.Items["User"] as User;

            if (seller == null || seller.Role != Types.Role.Seller)
            {
                return RedirectToAction("Index", "Home");
            }

            var products = await _dbcontext.Products.Where(p => p.SellerId == seller.UserId && p.IsArchived == false && p.IsDeleted == false).ToListAsync();
            return View(products);

        }


        [HttpGet]
        public async Task<IActionResult> IsArcheive()
        {
            var user = HttpContext.Items["User"] as User;
            if (user == null|| user.Role != Role.Seller)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.UserId = user.UserId;
            var product = await _dbcontext.Products.Where(p => p.SellerId == user.UserId && p.IsArchived == true && p.IsDeleted == false).ToListAsync();
            return View(product);

        }


        [HttpGet]
        public async Task<IActionResult> Archiveproduct(Guid id)
        {
            var product = await _dbcontext.Products.FindAsync(id);
            if (product != null)
            {
                product.IsArchived = true;
                await _dbcontext.SaveChangesAsync();

            }
            return RedirectToAction("SellerProduct");
        }


        [HttpGet]
        public async Task<IActionResult> UnArchive(Guid id)
        {
            var product = await _dbcontext.Products.FindAsync(id);
            if (product != null)
            {
                product.IsArchived = false;
                await _dbcontext.SaveChangesAsync();
            }
            return RedirectToAction("Archiveproduct");

        }


        [HttpGet]
        public async Task<IActionResult> IsDeleted()
        {
            var user = HttpContext.Items["User"] as User;
            if (user == null || user.Role != Role.Seller)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.UserId = user.UserId;
            var product = await _dbcontext.Products.Where(p => p.SellerId == user.UserId && p.IsArchived == true && p.IsDeleted == true).ToListAsync();
            return View(product);

        }

        [HttpGet]
        public async Task<IActionResult> Deleted(Guid id)
        {
            var product = await _dbcontext.Products.FindAsync(id);
            if (product != null)
            {
                product.IsDeleted = true;
                await _dbcontext.SaveChangesAsync();
            }
            return RedirectToAction("IsDeleted");
        }


        [HttpGet]
        public async Task<IActionResult>Restore (Guid id)
        {

            var product = await _dbcontext.Products.FindAsync(id);
            if (product != null)
            {
                product.IsDeleted = false;
                await _dbcontext.SaveChangesAsync();
            }
            return RedirectToAction("IsDeleted");

        }


        [HttpGet]
        public async Task<IActionResult> OrderProduct()
        {
            var user = HttpContext.Items["User"] as User;
            if(user == null || user.Role != Role.Seller)
            {
                return RedirectToAction("Login", "User");

            }

            Console.WriteLine($"Logged-in UserId: {user.UserId}"); // Debugging
            ViewBag.UserId = user.UserId;
            // Fetch orders where payment is completed
            var order = await _dbcontext.Orders
                     .Include(o => o.OrderProducts)
                     .ThenInclude(p => p.product)
                     .Include(o => o.Address)
                     .Where(o => o.UserId == user.UserId && o.Status == Status.PaymentDone).ToListAsync();
            Console.WriteLine($"Orders Count: {order.Count}"); // Debugging
            return View(order);
        }
    }
}
