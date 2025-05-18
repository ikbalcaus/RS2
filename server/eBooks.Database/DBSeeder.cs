using eBooks.Database;
using eBooks.Database.Models;

namespace eBooks.API
{
    public static class DbSeeder
    {
        public static void SeedRoles(EBooksContext db)
        {
            var roles = new[] { "User", "Admin", "Moderator" };
            foreach (var role in roles)
            {
                if (!db.Roles.Any(x => x.Name == role))
                {
                    db.Roles.Add(new Role { Name = role });
                }
            }
            db.SaveChanges();
        }
    }
}
