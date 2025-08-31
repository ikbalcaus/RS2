using System.ComponentModel.DataAnnotations;

namespace eBooks.Models.Requests
{
    public class LoginReq
    {
        private string _email;
        private string _password;

        [Required]
        public string Email
        {
            get => _email;
            set => _email = value.Trim();
        }

        [Required]
        public string Password
        {
            get => _password;
            set => _password = value.Trim();
        }
    }
}
