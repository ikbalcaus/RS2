using System.ComponentModel.DataAnnotations;

namespace eBooks.Models.Requests
{
    public class ReviewsReq
    {
        private string? _comment;

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        public string? Comment
        {
            get => _comment;
            set => _comment = value?.Trim();
        }
    }
}
