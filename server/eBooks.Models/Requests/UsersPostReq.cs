using System.ComponentModel.DataAnnotations;

namespace eBooks.Models.Requests
{
    public class UsersPostReq
    {
        private string _firstName;
        private string _lastName;
        private string _userName;
        private string _email;
        private string _password;

        [Required]
        public string FirstName
        {
            get => _firstName;
            set => _firstName = value.Trim();
        }

        [Required]
        public string LastName
        {
            get => _lastName;
            set => _lastName = value.Trim();
        }

        [Required]
        public string UserName
        {
            get => _userName;
            set => _userName = value.Trim();
        }

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
