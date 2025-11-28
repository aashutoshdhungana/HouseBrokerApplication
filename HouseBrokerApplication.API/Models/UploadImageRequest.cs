using Microsoft.AspNetCore.Http;

namespace HouseBrokerApplication.API.Models
{
    public class UploadImageRequest
    {
        public bool IsPrimary { get; set; }
        public IFormFile File { get; set; }
    }
}
