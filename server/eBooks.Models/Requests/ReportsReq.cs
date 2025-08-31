using System.ComponentModel.DataAnnotations;

namespace eBooks.Models.Requests
{
    public class ReportsReq
    {
        private string _reason;

        [Required]
        public string Reason
        {
            get => _reason;
            set => _reason = value.Trim();
        }
    }
}
