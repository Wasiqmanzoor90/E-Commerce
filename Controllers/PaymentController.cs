using E_Commerce.Data;
using E_Commerce.Interface;
using E_Commerce.Models.ViewModel;
using E_Commerce.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController(SqldbContext dbcontext, IJasonToken jasonToken, ILogger<OrderController> logger) : ControllerBase
    {
        private readonly ILogger<OrderController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly SqldbContext dbContext = dbcontext;
        private readonly IJasonToken tokenService = jasonToken;
        private readonly RazorPayService razorpayService = new RazorPayService();

        [HttpPost("create-intent")]
        public IActionResult CreateIntent([FromBody] PaymentModel model)
        {
            if (model == null || model.Amount <= 0 || string.IsNullOrEmpty(model.Currency))
            {
                return BadRequest("Invalid payment details.");
            }

            try
            {
                var order = razorpayService.CreateOrder(model.Amount, model.Currency, model.OrderId);
                if (order == null)
                {
                    return StatusCode(500, "Failed to create payment order.");
                }

                return Ok(new
                {
                    orderId = order["id"].ToString(),
                    entity = order["entity"].ToString(),
                    amount = order["amount"],
                    amountPaid = order["amount_paid"],
                    amountDue = order["amount_due"],
                    currency = order["currency"].ToString(),
                    receipt = order["receipt"].ToString(),
                    status = order["status"].ToString(),
                    attempts = order["attempts"],
                    createdAt = order["created_at"]
                });



            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating payment intent.");
                return StatusCode(500, "Internal Server Error");
            }

        }
    }







}