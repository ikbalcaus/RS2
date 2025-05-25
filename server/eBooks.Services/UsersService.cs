using System.Security.Claims;
using EasyNetQ;
using eBooks.Database;
using eBooks.Database.Models;
using eBooks.Interfaces;
using eBooks.Models.Exceptions;
using eBooks.Models.Messages;
using eBooks.Models.Requests;
using eBooks.Models.Responses;
using eBooks.Models.Search;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stripe;

namespace eBooks.Services;

public class UsersService : BaseCRUDService<User, UsersSearch, UsersPostReq, UsersPutReq, UsersRes>, IUsersService
{
    protected IHttpContextAccessor _httpContextAccessor;
    protected IBus _bus;
    protected ILogger<UsersService> _logger;

    public UsersService(EBooksContext db, IMapper mapper, IHttpContextAccessor httpContextAccessor, IBus bus, ILogger<UsersService> logger)
        : base(db, mapper)
    {
        _httpContextAccessor = httpContextAccessor;
        _bus = bus;
        _logger = logger;
    }

    public virtual async Task<UsersRes> Post(UsersPostReq req)
    {
        req.Email = req.Email.Trim().ToLower();
        if (!Helpers.IsEmailValid(req.Email))
            throw new ExceptionBadRequest("Email is not valid");
        if (await _db.Users.AnyAsync(x => x.Email == req.Email))
            throw new ExceptionBadRequest("Email already exist");
        if (await _db.Users.AnyAsync(x => x.UserName.Trim().ToLower() == req.UserName.Trim().ToLower()))
            throw new ExceptionBadRequest("Username already exists");
        if (!Helpers.IsPasswordValid(req.Password))
            throw new ExceptionBadRequest("Password is not valid");
        var entity = _mapper.Map<User>(req);
        entity.PasswordSalt = Helpers.GenerateSalt();
        entity.PasswordHash = Helpers.GenerateHash(entity.PasswordSalt, req.Password);
        entity.RoleId = (await _db.Set<Role>().FirstOrDefaultAsync(x => x.Name == "User")).RoleId;
        var verificationToken = Guid.NewGuid().ToString();
        entity.VerificationToken = verificationToken;
        entity.TokenExpiry = DateTime.UtcNow.AddHours(24);
        _db.Add(entity);
        await _db.SaveChangesAsync();
        _bus.PubSub.Publish(new EmailVerification { Token = _mapper.Map<TokenRes>(entity) });
        return _mapper.Map<UsersRes>(entity);
    }

    public override async Task<UsersRes> Put(int id, UsersPutReq req)
    {
        var entity = await _db.Set<User>().FindAsync(id);
        if (entity == null)
            throw new ExceptionNotFound();
        if (!string.IsNullOrWhiteSpace(req.UserName) && await _db.Set<User>().AnyAsync(x => x.UserName.Trim().ToLower() == req.UserName.Trim().ToLower()))
            throw new ExceptionBadRequest("Username already exist");
        if (!string.IsNullOrWhiteSpace(req.Password))
        {
            if (!Helpers.IsPasswordValid(req.Password))
                throw new ExceptionBadRequest("Password is not valid");
            entity.PasswordSalt = Helpers.GenerateSalt();
            entity.PasswordHash = Helpers.GenerateHash(entity.PasswordSalt, req.Password);
        }
        _mapper.Map(req, entity);
        await _db.SaveChangesAsync();
        _logger.LogInformation($"User with email {entity.Email} updated.");
        return _mapper.Map<UsersRes>(entity);
    }

    public override async Task<UsersRes> Delete(int id)
    {
        var set = _db.Set<User>();
        var entity = await set.FindAsync(id);
        if (entity == null)
            throw new ExceptionNotFound();
        entity.IsDeleted = true;
        await _db.SaveChangesAsync();
        _logger.LogInformation($"User with email {entity.Email} deleted.");
        return null;
    }

    public async Task<UsersRes> UndoDelete(int id)
    {
        var set = _db.Set<User>();
        var entity = await set.FindAsync(id);
        if (entity == null)
            throw new ExceptionNotFound();
        entity.IsDeleted = false;
        await _db.SaveChangesAsync();
        _logger.LogInformation($"User with email {entity.Email} undo-deleted.");
        return _mapper.Map<UsersRes>(entity);
    }

    public async Task<LoginRes> Login(string email, string password)
    {
        var entity = await _db.Set<User>().Include(x => x.Role).FirstOrDefaultAsync(x => x.Email == email);
        if (entity == null)
        {
            _logger.LogInformation($"User with email {email} failed to log in. Wrong email.");
            return null;
        }
        if (Helpers.GenerateHash(entity.PasswordSalt, password) != entity.PasswordHash)
        {
            _logger.LogInformation($"User with email {email} failed to log in. Wrong password.");
            return null;
        }
        _logger.LogInformation($"User with email {email} logged in.");
        return _mapper.Map<LoginRes>(entity);
    }

    public async Task<UsersRes> VerifyEmail(int id, string token)
    {
        var entity = await _db.Set<User>().FindAsync(id);
        if (entity == null)
            throw new ExceptionNotFound();
        if (entity.VerificationToken != token)
            throw new ExceptionBadRequest("Invalid verification token");
        if (entity.TokenExpiry < DateTime.UtcNow)
            throw new ExceptionBadRequest("Verification token has expired");
        entity.VerificationToken = null;
        entity.TokenExpiry = null;
        entity.IsEmailVerified = true;
        await _db.SaveChangesAsync();
        _logger.LogInformation($"User with email {entity.Email} verified email.");
        return _mapper.Map<UsersRes>(entity);
    }

    public async Task<UsersRes> VerifyPublisher(int id)
    {
        var entity = await _db.Set<User>().FindAsync(id);
        if (entity == null)
            throw new ExceptionNotFound();
        entity.PublisherVerifiedById = int.TryParse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId) ? userId : null;
        await _db.SaveChangesAsync();
        _logger.LogInformation($"Publisher with email {entity.Email} is verified.");
        return _mapper.Map<UsersRes>(entity);
    }

    public async Task<UsersRes> UnVerifyPublisher(int id)
    {
        var entity = await _db.Set<User>().FindAsync(id);
        if (entity == null)
            throw new ExceptionNotFound();
        entity.PublisherVerifiedById = null;
        await _db.SaveChangesAsync();
        _logger.LogInformation($"Publisher with email {entity.Email} is unverified.");
        return _mapper.Map<UsersRes>(entity);
    }
}
