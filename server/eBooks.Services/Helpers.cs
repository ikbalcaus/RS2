using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using eBooks.Models.Exceptions;
using SixLabors.ImageSharp.Formats.Webp;

namespace eBooks.Services
{
    public static class Helpers
    {
        public static bool IsEmailValid(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return emailRegex.IsMatch(email);
        }

        public static bool IsPasswordValid(string password)
        {
            if (string.IsNullOrWhiteSpace(password)) return false;
            var hasMinimum8Chars = password.Length >= 8;
            var hasUpperChar = Regex.IsMatch(password, "[A-Z]");
            var hasLowerChar = Regex.IsMatch(password, "[a-z]");
            var hasDigit = Regex.IsMatch(password, "[0-9]");
            return hasMinimum8Chars && hasUpperChar && hasLowerChar && hasDigit;
        }

        public static string GenerateSalt(int size = 32)
        {
            var saltBytes = new byte[size];
            using (var x = RandomNumberGenerator.Create())
            {
                x.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        public static string GenerateHash(string password, string salt)
        {
            var combined = password + salt;
            var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(combined));
            return Convert.ToBase64String(hashBytes);
        }

        public static decimal? CalculateDiscountedPrice(decimal price, int? discountPercentage, DateTime? discountStart, DateTime? discountEnd)
        {
            if (discountPercentage.HasValue && discountStart.HasValue && discountEnd.HasValue)
            {
                var now = DateTime.UtcNow;
                if (now >= discountStart.Value && now <= discountEnd.Value)
                {
                    decimal discountFactor = (100 - discountPercentage.Value) / 100m;
                    return Math.Round(price * discountFactor, 2);
                }
            }
            return price;
        }

        public static void AddError(this Dictionary<string, List<string>> errors, string key, string errorMessage)
        {
            if (!errors.TryGetValue(key, out var errorList))
            {
                errorList = new List<string>();
                errors[key] = errorList;
            }
            errorList.Add(errorMessage);
        }

        public static async Task UploadImageFile(string filePath, IFormFile file)
        {
            var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(extension) || !allowedTypes.Contains(file.ContentType))
                throw new ExceptionBadRequest("Only JPG, PNG or WEBP image formats are allowed.");
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            if (!string.IsNullOrEmpty(filePath))
            {
                var oldFilePath = Path.Combine(folderPath, filePath.TrimStart('/'));
                if (File.Exists(oldFilePath))
                    File.Delete(oldFilePath);
            }
            if (extension == ".webp")
            {
                await using var stream = new FileStream(Path.Combine(folderPath, filePath + extension), FileMode.Create);
                await file.CopyToAsync(stream);
            }
            else
            {
                using var image = await SixLabors.ImageSharp.Image.LoadAsync(file.OpenReadStream());
                var outputPath = Path.Combine(folderPath, filePath + ".webp");
                await using var outputStream = new FileStream(outputPath, FileMode.Create);
                var encoder = new WebpEncoder()
                {
                    Quality = 100
                };
                await image.SaveAsync(outputStream, encoder);
            }
        }

        public static async Task UploadPdfFile(string filePath, IFormFile file, bool isBookPdf)
        {
            if (file.ContentType != "application/pdf" || Path.GetExtension(file.FileName).ToLower() != ".pdf")
                throw new ExceptionBadRequest("Only PDF files are allowed.");
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", isBookPdf ? Path.Combine("pdfs", "books") : Path.Combine("pdfs", "summary"));
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            if (!string.IsNullOrEmpty(filePath))
            {
                var oldFilePath = Path.Combine(folderPath, filePath.TrimStart('/'));
                if (File.Exists(oldFilePath))
                    File.Delete(oldFilePath);
            }
            await using var stream = new FileStream(Path.Combine(folderPath, $"{filePath}.pdf"), FileMode.Create);
            await file.CopyToAsync(stream);
        }
    }
}
