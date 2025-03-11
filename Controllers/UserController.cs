using CloudinaryDotNet.Actions;
using E_Commerce.Data;
using E_Commerce.Interface;
using E_Commerce.Models.DomainModel;
using E_Commerce.Models.ViewModel;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    public IActionResult Edit()
    {
        var token = Request.Cookies["TestToken"];
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login");
        }

        return View();

    }


    [HttpPost]
    public async Task<IActionResult> Edit(User model)
    {
        try
        {
            var token = Request.Cookies["TestToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login");
            }

            var userId = _jtoken.VerifyToken(token);
            var user = await _dbcontext.Users.FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return NotFound();
            }

            user.Name = model.Name;

            user.Role = model.Role; // Update role

            // Hash new password only if it is changed
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
            }

            _dbcontext.Users.Update(user);
            await _dbcontext.SaveChangesAsync();

            ViewBag.successMessage = "Updated sucessfull";
            return View();


        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return View();
        }
    }


    [HttpGet]
    public IActionResult Delete()
    {
        var token = Request.Cookies["TestToken"];
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login");
        }

        return View();
    }


    [HttpPost]
    public async Task<IActionResult> Delete(string password)
    {
        try
        {
            var token = Request.Cookies["TestToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login");
            }

            var userId = _jtoken.VerifyToken(token);
            var user = await _dbcontext.Users.FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return NotFound();
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                ViewBag.errorMessage = "Incorrect password";
                return View("Delete"); // Reload delete confirmation page
            }

            _dbcontext.Users.Remove(user);
            await _dbcontext.SaveChangesAsync();
            Response.Cookies.Delete("TestToken");
            return RedirectToAction("Login");

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
        var token = Request.Cookies["TestToken"];
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login");
        }

        var UserName = HttpContext.Request.Cookies["UserName"];
        var UserEmail = HttpContext.Request.Cookies["UserEmail"];
        var Role = HttpContext.Request.Cookies["Role"];

        if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(UserEmail) || string.IsNullOrEmpty(Role))
        {
            return RedirectToAction("Login", "User");
        }
        ViewBag.UserName = UserName;
        ViewBag.UserEmail = UserEmail;
        ViewBag.Role = Role;

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
            { HttpOnly = true, 
                Secure = true,
                SameSite = SameSiteMode.Strict });

            Response.Cookies.Append("Role", user.Role.ToString(), new CookieOptions
              { HttpOnly = true,
               Secure = true,
              SameSite = SameSiteMode.Strict });


            if (user.Role == Types.Role.Buyer)
            {
                return RedirectToAction("BuyerUi","User");
            }
        
            else
            {
                return RedirectToAction("SellerUi","Product"); // Redirect Seller to Index1
            }

              }

        return RedirectToAction("Index", "Home");
    



    }


    [HttpGet]
    public async Task <IActionResult> BuyerUi(Product product)
    {
        var token = Request.Cookies["TestToken"];
      if(string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Index", "Home");
        }
      var userid= _jtoken.VerifyToken(token);
        if(userid == Guid.Empty)
        {
            return View();
        }
        var Buyer = await _dbcontext.Users.FirstOrDefaultAsync(u =>  u.UserId == userid);
        if(Buyer == null|| Buyer.Role != Types.Role.Buyer)
        {
            return RedirectToAction("Index", "Home");   
        }

        // Retrieve all products with quantity greater than zero
        var products = await _dbcontext.Products.Where(p => p.ProductQuantity > 0).ToListAsync();
        return View(products);

    }


    [HttpGet]
    public async Task <IActionResult> Adminconsole()
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
            return View();
        }
       
    }

    [HttpPost]   
    public async Task <IActionResult> Adminconsole(User user, Product product)
    {
        var token = Request.Cookies["TestToken"];
       if(string.IsNullOrEmpty(token))
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

        var prod = await _dbcontext.Products.FindAsync(ProductId);
        if (prod == null)
        {
            return NotFound();
        }
        return View(prod);
    }


    [HttpGet]
    public IActionResult AddressUi()
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

        var address = _dbcontext.Addresses.Where(a => a.UserId == userid).ToList();
        return View(address);
    }


    [HttpPost]
    public async Task<IActionResult>Addaddress(Address address)
    {
        var token = Request.Cookies["TestToken"];
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Index", "Home");
        }

        var userId = _jtoken.VerifyToken(token);
        if (userId == Guid.Empty)
        {
            return RedirectToAction("Index", "Home");
        }

        if (!ModelState.IsValid)
        {
            return RedirectToAction("AddressUi");
        }

        address.AddressId = Guid.NewGuid();
        address.UserId = userId;
        address.DateCreated= DateTime.Now;
        _dbcontext.Addresses.Add(address);
        await _dbcontext.SaveChangesAsync();
        return RedirectToAction("AddressUi");
    }
}

 















