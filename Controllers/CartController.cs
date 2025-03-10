using E_Commerce.Data;
using E_Commerce.Interface;
using E_Commerce.Models.DomainModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Controllers
{
    public class CartController(SqldbContext dbcontext, IJasonToken jsontoken) : Controller
    {
        private readonly SqldbContext _dbcontext = dbcontext;
        private readonly IJasonToken _jsontoken = jsontoken;

        private const string TokenCookieName = "TestToken"; // Use a constant for cookie name

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CartUi()
        {
            var token = Request.Cookies[TokenCookieName];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account"); // Redirect user if not logged in
            }

            var userId = _jsontoken.VerifyToken(token);
            if (userId == Guid.Empty)
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = await _dbcontext.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            return View(cart); // Pass cart data to the view
        }

        [HttpPost]
        public async Task<IActionResult> CartUi(Guid id, int quantity)
        {
            var token = Request.Cookies["TestToken"];
            if (string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            var userId = _jsontoken.VerifyToken(token);
            if (userId == Guid.Empty)
            {
                return NotFound();
            }

            // Fetch the user's cart
            var cart = await _dbcontext.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    CartId = Guid.NewGuid(),
                    UserId = userId,
                    CartItems = new List<CartItem>(),
                    Totalprice = 0,  // Initialize total price
                    CartQuantity = 0 // Initialize total quantity
                };
                _dbcontext.Carts.Add(cart);
                await _dbcontext.SaveChangesAsync(); // ✅ Save cart before adding items
            }

            // Fetch the product
            var product = await _dbcontext.Products.FindAsync(id);
            if (product == null || product.ProductQuantity < quantity)
            {
                return BadRequest("Invalid product or insufficient stock.");
            }

            // Fetch the existing cart item, even if it was deleted before
            var cartItem = await _dbcontext.CartItems.IgnoreQueryFilters()
                .FirstOrDefaultAsync(ci => ci.CartId == cart.CartId && ci.ProductId == id);

            if (cartItem != null)
            {
                if (_dbcontext.Entry(cartItem).State == EntityState.Deleted)
                {
                    _dbcontext.Entry(cartItem).State = EntityState.Modified; // ✅ Reattach deleted item
                }

                // Update the quantity and total price
                cartItem.Quantity += quantity;
                cartItem.Price = cartItem.Quantity * product.ProductPrice;

                _dbcontext.Entry(cartItem).State = EntityState.Modified; // ✅ Ensure changes are tracked
            }
            else
            {
                // If the item does not exist, create a new one
                cartItem = new CartItem
                {
                    CartItemId = Guid.NewGuid(),
                    CartId = cart.CartId,
                    ProductId = id,
                    Quantity = quantity,
                    Price = quantity * product.ProductPrice
                };

                _dbcontext.CartItems.Add(cartItem);
            }

            // ✅ Update Cart Total Price & Quantity
            cart.Totalprice = cart.CartItems.Sum(ci => ci.Price);
            cart.CartQuantity = cart.CartItems.Sum(ci => ci.Quantity);

            _dbcontext.Carts.Update(cart); // Update cart

            // ✅ Save changes to the database
            await _dbcontext.SaveChangesAsync();

            // ✅ Redirect to Cart UI after saving
            return RedirectToAction("CartUi", "Cart");
        }





        [HttpPost]
        public async Task<IActionResult> RemoveItem(Guid id)
        {
            var token = Request.Cookies["TestToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = _jsontoken.VerifyToken(token);
            if (userId == Guid.Empty)
            {
                return RedirectToAction("Login", "Account");
            }

            var cartItem = await _dbcontext.CartItems
                .FirstOrDefaultAsync(ci => ci.CartItemId == id);

            if (cartItem == null)
            {
                return NotFound("Cart item not found.");
            }

            _dbcontext.CartItems.Remove(cartItem);
            await _dbcontext.SaveChangesAsync();

            return RedirectToAction("CartUi");
        }
}
    }
