using E_Commerce.Data;
using E_Commerce.Interface;
using E_Commerce.Models.DomainModel;
using E_Commerce.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Controllers
{
    public class OrderController(SqldbContext dbcontext, IJasonToken jasonToken) : Controller
    {
        private readonly SqldbContext dbcontext = dbcontext;
        private readonly IJasonToken _jasonToken = jasonToken;




        [HttpPost]
        public async Task<IActionResult> Create(Guid orderId, Guid selectedAddressId)
        {
            var user = HttpContext.Items["User"] as User;
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            var cart = await dbcontext.Carts
                .Include(c => c.CartItems)
                .ThenInclude(cp => cp.Product)
                .FirstOrDefaultAsync(c => c.UserId == user.UserId);

            if (cart == null || cart.CartItems.Count == 0)
            {
                ViewBag.errorMessage = "Your Cart is empty! Proceed Directly to the orders section.";
                return View();
            }

            // 🔹 Ensure Address Exists
            var addressExists = await dbcontext.Addresses.AnyAsync(a => a.AddressId == selectedAddressId && a.UserId == user.UserId);
            if (!addressExists)
            {
                return BadRequest("Selected address does not exist.");
            }

            // Convert CartItems to OrderProducts
            var orderProducts = cart.CartItems.Select(cp => new OrderProduct
            {
                ProductId = cp.ProductId,
                Quantity = cp.Quantity
            }).ToList();

            var order = new Order
            {
                Status = Status.Pending,
                OrderPrice = (int)cart.Totalprice,
                UserId = user.UserId,
                CartId = cart.CartId,
                AddressId = selectedAddressId, // ✅ Assign valid AddressId
                OrderProducts = orderProducts
            };

            await dbcontext.Orders.AddAsync(order);
            dbcontext.CartItems.RemoveRange(cart.CartItems);
            cart.Totalprice = 0;
            await dbcontext.SaveChangesAsync();

            return RedirectToAction("Payment", new { orderId = order.OrderId });
        }














        [HttpGet]
        public async Task<IActionResult> Payment(Guid orderId)
        {
            var user = HttpContext.Items["User"] as User;
            if (user == null || user.Role != Role.Buyer)
            {
                return RedirectToAction("Login", "User");
            }

            var order = await dbcontext.Orders
                .Include(o => o.OrderProducts)
                .ThenInclude(op => op.product) // ✅ Ensure Product details are included
                .Include(o => o.Address) // ✅ FIXED: Ensure Address is included
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null || order.OrderProducts.Count == 0)
            {
                return RedirectToAction("Login", "User");
            }

            ViewBag.OrderProducts = order.OrderProducts;
            ViewBag.Order = order;
            ViewBag.Address = order.Address;

            return View(order); // ✅ Pass order model to the view
        }
    }

    }
