using eBooks.Database;
using eBooks.Database.Models;

namespace eBooks.API
{
    public static class DbSeeder
    {
        public static void SeedRoles(EBooksContext db)
        {
            var set = db.Set<Role>();
            var list = new[] { "User", "Admin", "Moderator" };
            if (!set.Any(x => list.Contains(x.Name)))
            {
                var newItems = list.Select(x => new Role { Name = x }).ToList();
                set.AddRange(newItems);
                db.SaveChanges();
            }
        }

        public static void SeedLanguages(EBooksContext db)
        {
            var set = db.Set<Language>();
            var list = new[] { "English" };
            if (!set.Any(x => list.Contains(x.Name)))
            {
                var newItems = list.Select(x => new Language { Name = x }).ToList();
                set.AddRange(newItems);
                db.SaveChanges();
            }
        }
    }
}
