using eBooks.Database.Models;
using eBooks.Database;
using eBooks.Interfaces;
using eBooks.Models.Responses;
using eBooks.Models.SearchObjects;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace eBooks.Services
{
    public class PurchasesService : BaseReadOnlyService<Purchase, BaseSearch, PurchasesRes>, IPurchasesService
    {
        public PurchasesService(EBooksContext db, IMapper mapper)
            : base(db, mapper)
        {
        }

        public override async Task<IQueryable<Purchase>> AddIncludes(IQueryable<Purchase> query)
        {
            return query.Include(x => x.User).Include(x => x.Publisher).Include(x => x.Book).ThenInclude(x => x.Language);
        }
    }
}
