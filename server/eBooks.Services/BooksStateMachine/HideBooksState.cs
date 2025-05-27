using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Models.Exceptions;
using eBooks.Models.Requests;
using eBooks.Models.Responses;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace eBooks.Services.BooksStateMachine
{
    public class HideBooksState : BaseBooksState
    {
        public HideBooksState(EBooksContext db, IMapper mapper, IServiceProvider serviceProvider, ILogger<BooksService> logger)
            : base(db, mapper, serviceProvider, logger)
        {
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
            _mapper.Map(req, entity);
            entity.StateMachine = "draft";
            var filePath = entity.FilePath;
            if (req.BookPdfFile != null)
                await Helpers.UploadPdfFile(filePath, req.BookPdfFile, true);
            if (req.PreviewPdfFile != null)
                await Helpers.UploadPdfFile(filePath, req.PreviewPdfFile, false);
            if (req.ImageFile != null)
                await Helpers.UploadImageFile(filePath, req.ImageFile);
            await _db.SaveChangesAsync();
            _logger.LogInformation($"Book with title {entity.Title} updated.");
            return _mapper.Map<BooksRes>(entity);
        }

        public override async Task<BooksRes> Hide(int id)
        {
            var entity = await _db.Set<Book>().FindAsync(id);
            if (entity == null)
                throw new ExceptionNotFound();
            entity.StateMachine = "approve";
            await _db.SaveChangesAsync();
            _logger.LogInformation($"Book with title {entity.Title} unhidden.");
            return _mapper.Map<BooksRes>(entity);
        }

        public override async Task<List<string>> AllowedActions(Book entity)
        {
            return new List<string>() { nameof(Update), nameof(Hide) };
        }
    }
}
