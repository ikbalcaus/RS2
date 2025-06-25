using Microsoft.AspNetCore.Http;

namespace eBooks.Models.Requests
{
    public class BooksPostReq
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public int? NumberOfPages { get; set; }
        public string? Language { get; set; }
        public string? Authors { get; set; }
        public string? Genres { get; set; }
        public IFormFile? ImageFile { get; set; }
        public IFormFile? BookPdfFile { get; set; }
        public IFormFile? SummaryPdfFile { get; set; }
    }
}
