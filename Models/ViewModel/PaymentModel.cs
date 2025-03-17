namespace E_Commerce.Models.ViewModel
{
    public class PaymentModel
    {
        public int Amount { get; set; }
        public string? Currency { get; set; }
        public Guid OrderId { get; set; }
    }
}
