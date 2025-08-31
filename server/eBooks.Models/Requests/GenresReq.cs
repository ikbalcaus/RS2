using System.ComponentModel.DataAnnotations;

namespace eBooks.Models.Requests
{
    public class GenresReq
    {
        private string _name;

        [Required]
        public string Name
        {
            get => _name;
            set => _name = value.Trim();
        }
    }
}
