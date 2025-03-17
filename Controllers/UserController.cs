using CloudinaryDotNet.Actions;
using E_Commerce.Data;
using E_Commerce.Interface;
using E_Commerce.Models.DomainModel;
using E_Commerce.Models.ViewModel;
using E_Commerce.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Razorpay.Api;
using Product = E_Commerce.Models.DomainModel.Product;

namespace E_Commerce.Controllers;

public class UserController(SqldbContext dbcontext, IJasonToken jtoken) : Controller
{
    private readonly SqldbContext _dbcontext = dbcontext;
    private readonly IJasonToken _jtoken = jtoken;

    public string errorMessage = "";
    public string successMessage = "";

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register([Bind("Name, Email, Password, Role,ProfilePictureUrl")] User user)
    {
        try
        {
            var finduser = await _dbcontext.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (finduser != null)
            {
                ViewBag.ErrorMessage = "User Already Exists";

            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            if (ModelState.IsValid)
            {
                await _dbcontext.Users.AddAsync(user);
                await _dbcontext.SaveChangesAsync();
                ViewBag.SuccessMessage = "User Created Successfully";

            }
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ex.Message;
            return View(user);
        }

        return View(user);
    }


    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(Login login)
    {
        try
        {
            var finduser = await _dbcontext.Users.FirstOrDefaultAsync(u => u.Email == login.Email);
            if (finduser == null)
            {
                ViewBag.errorMessage = "User Doesnt Exists";
                return View();
            }
            if (string.IsNullOrEmpty(login.Password) || string.IsNullOrEmpty(login.Email))
            {
                ViewBag.errorMessage = "Please fill all the fields";
                return View();
            }

            bool verify = BCrypt.Net.BCrypt.Verify(login.Password, finduser.Password);

            if (!verify)
            {
                ViewBag.errorMessage = "Invalid Password";
                return View();
            }

            var token = _jtoken.CreateToken(finduser.UserId, finduser.Email, finduser.Name, finduser.Role);
            HttpContext.Response.Cookies.Append("TestToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(10)
            });

            HttpContext.Response.Cookies.Append("UserName", finduser.Name, new CookieOptions
            {
                Secure = false,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(10)
            });

            HttpContext.Response.Cookies.Append("UserEmail", finduser.Email, new CookieOptions
            {
                Secure = false,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(10)
            });

            HttpContext.Response.Cookies.Append("Role", finduser.Role.ToString(), new CookieOptions
            {
                Secure = false,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(10)
            });
            ViewBag.successMessage = "Login successful";
            return finduser.Role switch
            {
                Types.Role.Admin => RedirectToAction("AdminConsole"),
                Types.Role.Seller => RedirectToAction("SellerUi", "Product"),
                _ => RedirectToAction("BuyerUi")
            };

        }

        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return View();
        }

    }





  


    [HttpGet]
    public IActionResult UserProfile()
    {
        var user = HttpContext.Items["User"] as User;
        if(user == null)
        {
            return RedirectToAction("Login", "User");
        }
        ViewBag.UserName = user.Name;
        ViewBag.UserEmail = user.Email;
        ViewBag.Role = user.Role.ToString();

        return View();
    }

    [HttpPost]
    public IActionResult Logout()
    {
        var token = Request.Cookies["TestToken"];
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Index", "Home");
        }
        Response.Cookies.Delete("TestToken");
        Response.Cookies.Delete("UserName");
        Response.Cookies.Delete("UserEmail");
        Response.Cookies.Delete("Role");

        HttpContext.Session.Clear();

        // Redirect to Login page (or Home)
        return RedirectToAction("Index", "Home");

    }


    [HttpGet]
    public async Task<IActionResult> Changerole()
    {
        var token = Request.Cookies["TestToken"];
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Index", "Home");
        }
        var Userid = _jtoken.VerifyToken(token);
        var user = await _dbcontext.Users.FirstOrDefaultAsync(u => u.UserId == Userid);
        if (user != null)
        {
            //  Toggle role: If Buyer → Seller, if Seller → Buyer
            user.Role = user.Role == Types.Role.Buyer ? Types.Role.Seller : Types.Role.Buyer;
            await _dbcontext.SaveChangesAsync(); // Save changes in DB

            //  Generate a new token with updated role
            var Newtoken = _jtoken.CreateToken(user.UserId, user.Email, user.Name, user.Role);

            //  Update cookies with the new token and role
            Response.Cookies.Append("TestToken", Newtoken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            Response.Cookies.Append("Role", user.Role.ToString(), new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });


            if (user.Role == Types.Role.Buyer)
            {
                return RedirectToAction("BuyerUi", "User");
            }

            else
            {
                return RedirectToAction("SellerUi", "Product"); // Redirect Seller to Index1
            }

        }

        return RedirectToAction("Index", "Home");

    }

    
    [HttpGet]
    public async Task<IActionResult> BuyerUi(Product product)
    {
        var user = HttpContext.Items["User"] as User;

        if (user == null || user.Role != Types.Role.Buyer)
        {
            return RedirectToAction("Index", "Home");
        }
        // Retrieve all products with quantity greater than zero
        var products = await _dbcontext.Products.Where(p => p.ProductQuantity > 0 && p.IsArchived == false && p.IsDeleted == false).ToListAsync();
        return View(products);

    }


    [HttpGet]
    public IActionResult Adminconsole()
    {


        var Admin = HttpContext.Items["User"] as User;

        if (Admin == null || Admin.Role != Types.Role.Admin) // Null check and role check
        {
            return RedirectToAction("Index", "Home");
        }
        else
        {
            return View();
        }

    }

    [HttpPost]
    public async Task<IActionResult> Adminconsole(User user, Product product)
    {
        var token = Request.Cookies["TestToken"];
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Index", "Home");
        }
        var userid = _jtoken.VerifyToken(token);
        if (userid == Guid.Empty)
        {
            return View();
        }
        var Admin = await _dbcontext.Users.FirstOrDefaultAsync(u => u.UserId == userid);
        if (Admin == null || Admin.Role != Types.Role.Admin) // Null check and role check
        {
            return RedirectToAction("Index", "Home");
        }
        else
        {
            return RedirectToAction("Adminconsole", "User");
        }

    }


    [HttpGet]
    public async Task<IActionResult> ProductDetail(Guid ProductId)
    {
        var prod = await _dbcontext.Products.FindAsync(ProductId);
        if (prod == null)
        {
            return NotFound();
        }
        return View(prod);
    }



    [HttpGet]
    public IActionResult AddressUi(Guid orderId)
    {
        var user = HttpContext.Items["User"] as User;
        if (user == null)
        {
            return RedirectToAction("Login", "User"); // 🔒 Redirect if user is not authenticated
        }

        var addressList = _dbcontext.Addresses.Where(a => a.UserId == user.UserId).ToList();
        ViewBag.OrderId = orderId; // ✅ FIXED: Pass OrderId correctly
        return View(addressList); // ✅ Ensure addresses are passed to the view
    }


    [HttpPost]
    public async Task<IActionResult> Addaddress(Address address)
    {
        var user = HttpContext.Items["User"] as User; // ✅ Get authenticated user from middleware
        if (user == null)
        {
            return RedirectToAction("Login", "User"); // 🔒 Redirect if user is not authenticated
        }

        if (!ModelState.IsValid)
        {
            return RedirectToAction("AddressUi"); // ✅ Redirect if validation fails
        }

        address.AddressId = Guid.NewGuid();
        address.UserId = user.UserId; // ✅ Assign UserId from authenticated user
        address.DateCreated = DateTime.Now;
        _dbcontext.Addresses.Add(address);
        await _dbcontext.SaveChangesAsync();
        return RedirectToAction("AddressUi"); // ✅ Ensure the user is redirected back to the updated list
    }




    private async Task<IActionResult>Categoryview(string category)
    {
        var buyer = HttpContext.Items["User"] as User; 
        if (buyer == null|| buyer.Role != Types.Role.Buyer)
        {
            return RedirectToAction("Index", "Home");
        }
        var prod = await _dbcontext.Products.Where(p => p.ProductQuantity > 0 && !p.IsArchived && !p.IsDeleted && p.Category.ToLower() == category.ToLower()).ToArrayAsync();
        return View(prod);
    }


    [HttpGet]
    public Task<IActionResult> LadiesUi() => Categoryview("ladies");

    [HttpGet]
    public Task<IActionResult> MenUi() => Categoryview("men");


    [HttpGet]
    public Task<IActionResult> KidUi() => Categoryview("kid");


}


















