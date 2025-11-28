namespace HouseBrokerApplication.Application.DTOs.Responses
{
    public class PaginationResponse<T>
    {
        public int PageNo { get; set; }
        public int PageCount { get; set; }
        public IEnumerable<T> Data { get; set; }
    }
}
