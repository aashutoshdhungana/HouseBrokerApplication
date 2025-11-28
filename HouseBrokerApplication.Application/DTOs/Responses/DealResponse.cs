namespace HouseBrokerApplication.Application.DTOs.Responses
{
    public class DealResponse
    {
        public int Id { get; set; }
        public int OfferId { get; set; }
        public string DealDate { get; set; }
        public string BuyerFullName { get; set; }
        public string BuyerEmail { get; set; }
        public string BuyerPhone { get; set; }
        public decimal Commission { get; set; }
        public decimal DealPrice { get; set; }
    }
}
