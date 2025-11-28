namespace HouseBrokerApplication.Application.DTOs.Responses
{
    public class OfferResponse
    {
        public int Id { get; set; }
        public int BuyerId { get; set; }
        public string BuyerName { get; set; }
        public decimal OfferAmount { get; set; }
        public decimal EstimatedCommission { get; set; }
    }
}
