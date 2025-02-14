using E_Commerce.Data;
using E_Commerce.Interface;
using E_Commerce.Models.DomainModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Controllers
{
    public class ProductController(SqldbContext dbcontext,IJasonToken jtoken) : Controller
    {
        private readonly SqldbContext _dbcontext=dbcontext;
        private readonly IJasonToken _jasonToken=jtoken;
        public string errorMessage = "";
        public string successMessage = "";

        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public async Task <IActionResult> SellerUi()
        {
            var token = Request.Cookies["TestToken"];
            if (string.IsNullOrEmpty(token))
            {
                return View();
            }
            var userid = _jasonToken.VerifyToken(token);
            if(userid ==Guid.Empty)
            {
                return View();
            }
            var Seller = await _dbcontext.Users.FirstOrDefaultAsync(u => u.UserId == userid);
            if (Seller == null || Seller.Role == Types.Role.Seller)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public async Task <IActionResult> SellerUi(Product product)
        {
            var token = Request.Cookies["TestToken"];
            if(string.IsNullOrEmpty(token))
            {
                return View(product);
            }
         var userid = _jasonToken.VerifyToken(token);
            if (userid == Guid.Empty)
            {
                return View();
            }
            var Seller= await _dbcontext.Users.FirstOrDefaultAsync(u => u.UserId == userid);
            if(Seller == null|| Seller.Role ==Types.Role.Seller)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
          
        }
    }
}
