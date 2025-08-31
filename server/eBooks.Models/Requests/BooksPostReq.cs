using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace eBooks.Models.Requests
{
    public class BooksPostReq
    {
        private string _title;
        private string? _description;
        private string? _language;
        private string? _authors;
        private string? _genres;

        [Required]
        public string Title
        {
            get => _title;
            set => _title = value.Trim();
        }

        public string? Description
        {
            get => _description;
            set => _description = value?.Trim();
        }

        public decimal? Price { get; set; }

        public int? NumberOfPages { get; set; }

        public string? Language
        {
            get => _language;
            set => _language = value?.Trim();
        }

        public string? Authors
        {
            get => _authors;
            set => _authors = value?.Trim();
        }

        public string? Genres
        {
            get => _genres;
            set => _genres = value?.Trim();
        }

        public IFormFile? ImageFile { get; set; }

        public IFormFile? BookPdfFile { get; set; }

        public IFormFile? SummaryPdfFile { get; set; }
    }
}
