using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Interfaces;
using eBooks.Models.Exceptions;
using eBooks.Models.Responses;
using MapsterMapper;
using Microsoft.Extensions.Logging;

namespace eBooks.Services.BooksStateMachine
{
    public class ApproveBooksState : BaseBooksState
    {
        public ApproveBooksState(EBooksContext db, IMapper mapper, IServiceProvider serviceProvider, ILogger<BooksService> logger)
            : base(db, mapper, serviceProvider, logger)
        {
        }

        public override async Task<BooksRes> Hide(int id)
        {
            var entity = await _db.Set<Book>().FindAsync(id);
            if (entity == null)
                throw new ExceptionNotFound();
            entity.StateMachine = "Hide";
            await _db.SaveChangesAsync();
            _logger.LogInformation($"Book with title {entity.Title} hidden.");
            return _mapper.Map<BooksRes>(entity);
        }

        public override async Task<List<string>> AllowedActions(Book entity)
        {
            return new List<string>() { nameof(Hide) };
        }
    }
}
