using E_Commerce.Data;
using E_Commerce.Interface;
using E_Commerce.Models.DomainModel;
using E_Commerce.Models.JunctionModel;
using E_Commerce.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Controllers
{
    public class OrderController(SqldbContext dbcontext, IJasonToken jasonToken) : Controller
    {
        private readonly SqldbContext dbcontext = dbcontext;
        private readonly IJasonToken _jasonToken = jasonToken;


        [HttpGet, HttpPost]
        public IActionResult PaymentSuccess()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(Guid orderId, Guid GetAddressId)
        {
            var user = HttpContext.Items["User"] as User;
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            var cart = await dbcontext.Carts
                .Include(ci => ci.CartItems)
                .ThenInclude(p => p.Product)
                .FirstOrDefaultAsync(u => u.UserId == user.UserId);
            if (cart == null || cart.CartItems.Count == 0)
            {
                ViewBag.errorMessage = "Your Cart is empty! Proceed Directly to the orders section.";
                return View();
            }


            var addressExists = await dbcontext.Addresses.AnyAsync(a => a.AddressId == GetAddressId && a.UserId == user.UserId);
               if (!addressExists)
                {
                    return BadRequest("Selected address does not exist.");
              }


            //In short, this links the order with the items that were in the cart
            var orderproduct = cart.CartItems.Select(cp => new OrderProduct
            {
                ProductId = cp.ProductId, //Extract ProductId from the cart item
                Quantity = cp.Quantity,   //Extract Quantity from the cart item
            }).ToList(); //Convert the result into a List of OrderProduct);


            var order = new Order
            {
                Status = Status.Pending,    //Set initial order status to Pending (waiting for payment)
                OrderPrice = (int)cart.Totalprice,//Assign total cart price as the order price
                UserId = user.UserId,       //Link the order to the current user
                CartId = cart.CartId,       //Store the cart ID for reference
                AddressId = GetAddressId,      // Assign selected shipping address
                OrderProducts = orderproduct //Link the order with the converted cart items
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
                .ThenInclude(op => op.product) //Ensure Product details are included
                .Include(o => o.Address) //FIXED: Ensure Address is included
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
