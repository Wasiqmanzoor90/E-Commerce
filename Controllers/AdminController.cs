using E_Commerce.Data;
using E_Commerce.Models.DomainModel;
using E_Commerce.Models.JunctionModel;
using E_Commerce.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Razorpay.Api;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController(SqldbContext dbcontext) : ControllerBase
    {
        private readonly SqldbContext _dbcontext = dbcontext;

        [HttpGet("Dashboard")]
        public async Task<IActionResult> Admin()
        {
            var user = HttpContext.Items["User"] as User;
            if (user==null|| user.Role!= Types.Role.Admin)
            {
                return Forbid();    
            }

            var cart = await _dbcontext.Carts
                .Include(c => c.CartItems)
                .ThenInclude(p => p.Product)
                .Select(c => new 
                {
                
                c.CartId,
                c.UserId,
                
                CartItem = c.CartItems.Select(p => new {

                    p.CartItemId,  //Corrected: Fetching CartItemId from CartItems
                    p.Quantity,


                }).ToList()
                
                
                }).ToListAsync();






            var orders = await _dbcontext.Orders.Include(o => o.OrderProducts)
                .ThenInclude(op => op.product).Include(o=>o.Address).Select(o => new {

                    o.OrderId,
                    o.OrderPrice,
                    o.Status,
                    OrderProduct = o.OrderProducts.Select(op => new { 
                    
                    op.ProductId,
                    
                    op.product.ProductName,
                    op.Quantity,
                    
                    
                    }).ToList(),
                    Address = o.Address != null ? new
                    {
                        o.Address.FirstName,
                        o.Address.Street1,
                        o.Address.DateCreated,
                        o.Address.Pincode
                    } : null



                }).ToArrayAsync();







            var Buyer = await _dbcontext.Users.Where(b => b.Role == Role.Buyer).Select(b => new
            {
                b.Email,
                b.Name,
                b.Role,
                Address = b.Addresses.Select(adr => new
                {
                    adr.Phone,
                    adr.State

                }).ToList()

            }).ToListAsync();






            var Seller = await _dbcontext.Users.
                Where(a => a.Role == Role.Seller).Select(a => new

                {
                    a.Name,
                    a.Email,
                    a.Role,
                    Address = a.Addresses.Select(ad => new
                    {
                        ad.Pincode,
                        ad.State,
                    }).ToList()


                }).ToListAsync();




            var prod = await _dbcontext.Users.Where(us=> us.Role == Role.Seller  || us.Role ==Role.Buyer).Select(us => new
            {
                us.Name,
                Product = us.Products.Select(pa=> new 
                {
                    pa.ProductName,
                    pa.ProductQuantity,
                    pa.Category,
                    pa.ProductPrice
                }).ToList()
            }).ToListAsync();



           

            return Ok(new { Cart = cart, Orders = orders , User = Seller, Buyers =Buyer, Prod=prod});
        }

    }
}
