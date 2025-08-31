using System.ComponentModel.DataAnnotations;

namespace eBooks.Models.Requests
{
    public class QuestionsReq
    {
        private string _message;

        [Required]
        public string Message
        {
            get => _message;
            set => _message = value.Trim();
        }
    }
}
