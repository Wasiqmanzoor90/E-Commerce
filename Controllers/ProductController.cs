using CloudinaryDotNet;
using E_Commerce.Data;
using E_Commerce.Interface;
using E_Commerce.Models.DomainModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace E_Commerce.Controllers
{
    public class ProductController(SqldbContext dbcontext,IJasonToken jtoken, ICloudinaryInterface cloudinary) : Controller
    {
        private readonly SqldbContext _dbcontext=dbcontext;
        private readonly IJasonToken _jasonToken=jtoken;
        private readonly ICloudinaryInterface _cloudinary = cloudinary;
        public string errorMessage = "";
        public string successMessage = "";

        [HttpGet]
        public async Task <IActionResult> SellerUi()
        {
            var token = Request.Cookies["TestToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login","User");
            }
            var userid= _jasonToken.VerifyToken(token);
            if(userid ==Guid.Empty)
            {
                return RedirectToAction("Login","User");
            }
            var Seller = await _dbcontext.Users.FirstOrDefaultAsync(u => u.UserId == userid);
            if (Seller == null || Seller.Role != Types.Role.Seller)
            {
                return RedirectToAction("Index", "Home");
            }

        var products = await _dbcontext.Products.Where(p=> p.SellerId == userid).ToListAsync();
            return View(products);
        }


        [HttpGet]
        public async Task <IActionResult> Register()
        {
            var token = Request.Cookies["TestToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "User");
            }
            var userid = _jasonToken.VerifyToken(token);
            if(userid ==Guid.Empty)
            {
                return RedirectToAction("Login", "User");
            }
            var Seller = await _dbcontext.Users.FirstOrDefaultAsync(u => u.UserId == userid);
            if (Seller == null || Seller.Role != Types.Role.Seller)
            {
                return RedirectToAction("Index", "Home");
            }

            else
            {
                return View();
            }
        }


        [HttpPost]
        public async Task<IActionResult> Register([FromForm] Product product, IFormFile file)
        {
            var token = Request.Cookies["TestToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "User");
            }
            var userid = _jasonToken.VerifyToken(token);
            if (userid == Guid.Empty)
            {
                return View();
            }
          string  imgurl = null;
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
                SellerId = userid

            };
            try { 
            
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

        // POST: Perform the deletion directly when the button is clicked
        [HttpPost]
        [Route("Product/Delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var token = Request.Cookies["TestToken"];
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(); // Better UX than a blank page
            }

            var Userid = _jasonToken.VerifyToken(token);
            var product = await _dbcontext.Products.FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound(); // If product does not exist
            }

            _dbcontext.Products.Remove(product);
            await _dbcontext.SaveChangesAsync();

            // Redirect to a list of products (or another relevant page)
            return RedirectToAction("SellerUi"); // Or whatever your listing page is
        }




    }

}
