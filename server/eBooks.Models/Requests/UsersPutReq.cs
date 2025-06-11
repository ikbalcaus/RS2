using Microsoft.AspNetCore.Http;

namespace eBooks.Models.Requests
{
    public class UsersPutReq
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Password { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
