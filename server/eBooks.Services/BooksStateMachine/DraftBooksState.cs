using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Models.Exceptions;
using eBooks.Models.Requests;
using eBooks.Models.Responses;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;

namespace eBooks.Services.BooksStateMachine
{
    public class DraftBooksState : BaseBooksState
    {
        protected AccountService _stripeAccountService;

        public DraftBooksState(EBooksContext db, IMapper mapper, IServiceProvider serviceProvider, ILogger<BooksService> logger, IConfiguration config)
            : base(db, mapper, serviceProvider, logger)
        {
            StripeConfiguration.ApiKey = config["Stripe:SecretKey"];
            _stripeAccountService = new AccountService();
        }

        public override async Task<BooksRes> Update(int id, BooksPutReq req)
        {
            if (req.Price < 0)
                throw new ExceptionBadRequest("Price must be zero or greater");
            if (req.LanguageId != null && !await _db.Set<Language>().AnyAsync(x => x.LanguageId == req.LanguageId))
                throw new ExceptionNotFound();
            var entity = await _db.Set<Book>().FindAsync(id);
            if (entity == null)
                throw new ExceptionNotFound();
            var languageId = entity.LanguageId;
            _mapper.Map(req, entity);
            entity.StateMachine = "draft";
            if (req.PdfFile != null)
                Helpers.UploadPdfFile(entity, req.PdfFile);
            await _db.SaveChangesAsync();
            if (req.Images != null && req.Images.Any())
                Helpers.UploadImages(_db, _mapper, entity.BookId, req.Images);
            _logger.LogInformation($"Book with title {entity.Title} updated.");
            return _mapper.Map<BooksRes>(entity);
        }

        public override async Task<BooksRes> Await(int id)
        {
            var entity = await _db.Set<Book>().FindAsync(id);
            if (entity == null)
                throw new ExceptionNotFound();
            if (string.IsNullOrWhiteSpace(entity.Title) || string.IsNullOrWhiteSpace(entity.Description) || entity.Price == null || entity.NumberOfPages == null || entity.LanguageId == 0 || string.IsNullOrWhiteSpace(entity.PdfPath))
                throw new ExceptionBadRequest("You must fill out all data before awaiting book");
            if (entity.Price > 0)
            {
                var accountService = new Stripe.AccountService();
                var account = await accountService.GetAsync(entity.Publisher.StripeAccountId);
                if (!account.DetailsSubmitted)
                    throw new ExceptionBadRequest("Stripe account is not fully onboarded");
                var transfersCapability = account.Capabilities?.Transfers;
                if (transfersCapability == null || transfersCapability != "active")
                    throw new ExceptionBadRequest("Stripe account does not have transfers capability enabled.");
            }
            entity.StateMachine = "await";
            await _db.SaveChangesAsync();
            return _mapper.Map<BooksRes>(entity);
        }

        public override async Task<List<string>> AllowedActions(Book entity)
        {
            return new List<string>() { nameof(Update), nameof(Await) };
        }
    }
}
