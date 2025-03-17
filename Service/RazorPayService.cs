using Razorpay.Api;
using System;
using System.Collections.Generic;

namespace E_Commerce.Service
{
    public class RazorPayService
    {
        private readonly string key = "rzp_test_RgG9AwfnysizZu"; // Replace with your actual key
        private readonly string secret = "1CaqfrrZdfeKKOIdPypjADfr"; // Replace with your actual secret

        public Order? CreateOrder(int amount, string currency, Guid orderId)
        {
            try
            {
                if (amount <= 0)
                {
                    throw new ArgumentException("Amount must be greater than zero.");
                }

                var razorpay = new RazorpayClient(key, secret);

                var options = new Dictionary<string, object>
                {
                    { "amount", amount * 100 }, // Convert to paise
                    { "currency", currency },
                    { "receipt", orderId.ToString() }, // ✅ FIXED: Convert GUID to string
                    { "payment_capture", 1 } // 1 means auto-capture
                };

                Order order = razorpay.Order.Create(options);
                return order;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in RazorPayService: {ex.ToString()}"); // ✅ Log full error
                return null;
            }
        }
    }
}
