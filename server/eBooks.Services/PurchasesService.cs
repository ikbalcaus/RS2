using eBooks.Database.Models;
using eBooks.Database;
using eBooks.Interfaces;
using eBooks.Models.Responses;
using eBooks.Models.Search;
using MapsterMapper;

namespace eBooks.Services
{
    public class PurchasesService : BaseReadOnlyService<Purchase, BaseSearch, PurchasesRes>, IPurchasesService
    {
        public PurchasesService(EBooksContext db, IMapper mapper)
            : base(db, mapper)
        {
        }
    }
}
