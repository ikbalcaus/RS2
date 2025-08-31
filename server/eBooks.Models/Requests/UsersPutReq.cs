using Microsoft.AspNetCore.Http;

namespace eBooks.Models.Requests
{
    public class UsersPutReq
    {
        private string? _firstName;
        private string? _lastName;
        private string? _oldPassword;
        private string? _newPassword;

        public string? FirstName
        {
            get => _firstName;
            set => _firstName = value?.Trim();
        }

        public string? LastName
        {
            get => _lastName;
            set => _lastName = value?.Trim();
        }

        public string? OldPassword
        {
            get => _oldPassword;
            set => _oldPassword = value?.Trim();
        }

        public string? NewPassword
        {
            get => _newPassword;
            set => _newPassword = value?.Trim();
        }

        public IFormFile? ImageFile { get; set; }
    }
}
