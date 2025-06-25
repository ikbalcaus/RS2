using System.Security.Claims;
using EasyNetQ;
using EasyNetQ.Internals;
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

namespace eBooks.Services
{
    public class UsersService : BaseCRUDService<User, UsersSearch, UsersPostReq, UsersPutReq, UsersRes>, IUsersService
    {
        protected IBus _bus;
        protected ILogger<UsersService> _logger;

        public UsersService(EBooksContext db, IMapper mapper, IHttpContextAccessor httpContextAccessor, IBus bus, ILogger<UsersService> logger)
            : base(db, mapper, httpContextAccessor)
        {
            _bus = bus;
            _logger = logger;
        }
        
        public override IQueryable<User> AddFilters(IQueryable<User> query, UsersSearch search)
        {
            if (!string.IsNullOrWhiteSpace(search.FirstName))
                query = query.Where(x => x.FirstName.ToLower().Contains(search.FirstName.ToLower()));
            if (!string.IsNullOrWhiteSpace(search.LastName))
                query = query.Where(x => x.LastName.ToLower().Contains(search.LastName.ToLower()));
            if (!string.IsNullOrWhiteSpace(search.UserName))
                query = query.Where(x => x.UserName.ToLower().Contains(search.UserName.ToLower()));
            if (!string.IsNullOrWhiteSpace(search.Email))
                query = query.Where(x => x.Email.ToLower().Contains(search.Email.ToLower()));
            if (search.IsDeleted == "Not deleted")
                query = query.Where(x => x.DeletionReason == null);
            else if (search.IsDeleted == "Deleted")
                query = query.Where(x => x.DeletionReason != null);
            if (search.OrderBy == "Username (Z-A)")
                query = query.OrderByDescending(x => x.UserName);
            else
                query = query.OrderBy(x => x.UserName);
            return query;
        }

        public virtual async Task<UsersRes> Post(UsersPostReq req)
        {
            var errors = new Dictionary<string, List<string>>();
            req.Email = req.Email.Trim().ToLower();
            if (string.IsNullOrEmpty(req.FirstName))
                errors.AddError("FirstName", "This field is required");
            if (string.IsNullOrEmpty(req.LastName))
                errors.AddError("LastName", "This field is required");
            if (string.IsNullOrEmpty(req.UserName))
                errors.AddError("UserName", "This field is required");
            if (!Helpers.IsEmailValid(req.Email))
                errors.AddError("Email", "Email must be in following format: example@example.com");
            if (await _db.Users.AnyAsync(x => x.Email == req.Email))
                errors.AddError("Email", "Email already exists");
            if (await _db.Users.AnyAsync(x => x.UserName.Trim().ToLower() == req.UserName.Trim().ToLower()))
                errors.AddError("UserName", "Username already exists");
            if (!Helpers.IsPasswordValid(req.Password))
                errors.AddError("Password", "Password must include at least 8 characters, one uppercase letter, one lowercase letter and one digit");
            if (errors.Count > 0)
                throw new ExceptionBadRequest(errors);
            var entity = _mapper.Map<User>(req);
            entity.PasswordSalt = Helpers.GenerateSalt();
            entity.PasswordHash = Helpers.GenerateHash(entity.PasswordSalt, req.Password);
            entity.RoleId = (await _db.Set<Role>().FirstOrDefaultAsync(x => x.Name == "User")).RoleId;
            entity.FilePath = $"{Guid.NewGuid():N}";
            var accountOptions = new AccountCreateOptions
            {
                Type = "express",
                Email = req.Email,
                Capabilities = new AccountCapabilitiesOptions
                {
                    CardPayments = new AccountCapabilitiesCardPaymentsOptions { Requested = true },
                    Transfers = new AccountCapabilitiesTransfersOptions { Requested = true }
                }
            };
            var stripeService = new AccountService();
            var stripeAccount = await stripeService.CreateAsync(accountOptions);
            entity.StripeAccountId = stripeAccount.Id;
            entity.VerificationToken = $"{Guid.NewGuid():N}".Substring(0, 6);
            entity.TokenExpiry = DateTime.UtcNow.AddHours(24);
            _db.Add(entity);
            await _db.SaveChangesAsync();
            _bus.PubSub.Publish(new EmailVerification { Token = _mapper.Map<TokenRes>(entity) });
            return _mapper.Map<UsersRes>(entity);
        }

        public override async Task<UsersRes> Put(int id, UsersPutReq req)
        {
            var errors = new Dictionary<string, List<string>>();
            if (string.IsNullOrEmpty(req.FirstName))
                errors.AddError("FirstName", "This field is required");
            if (string.IsNullOrEmpty(req.LastName))
                errors.AddError("LastName", "This field is required");
            var entity = await _db.Set<User>().FindAsync(id);
            if (entity == null)
                throw new ExceptionNotFound();
            if (!string.IsNullOrWhiteSpace(req.OldPassword) && !string.IsNullOrWhiteSpace(req.NewPassword))
            {
                if (Helpers.GenerateHash(entity.PasswordSalt, req.OldPassword) != entity.PasswordHash)
                    errors.AddError("OldPassword", "Old password is incorrect");
                if (!Helpers.IsPasswordValid(req.NewPassword))
                    errors.AddError("NewPassword", "Password must include at least 8 characters, one uppercase letter, one lowercase letter and one digit");
                entity.PasswordSalt = Helpers.GenerateSalt();
                entity.PasswordHash = Helpers.GenerateHash(entity.PasswordSalt, req.NewPassword);
            }
            if (errors.Count > 0)
                throw new ExceptionBadRequest(errors);
            if (req.ImageFile != null)
                await Helpers.UploadImageFile(entity.FilePath, req.ImageFile, false);
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
            entity.DeletionReason = "Deleted by user";
            await _db.SaveChangesAsync();
            _logger.LogInformation($"User with email {entity.Email} deleted.");
            var result = _mapper.Map<UsersRes>(entity);
            _bus.PubSub.Publish(new AccountDeactivated { User = result });
            return result;
        }

        public async Task<UsersRes> DeleteByAdmin(int id, string? reason)
        {
            var set = _db.Set<User>();
            var entity = await set.FirstOrDefaultAsync(x => x.UserId == id);
            var currentUser = await set.FirstOrDefaultAsync(x => x.UserId == GetUserId());
            if (entity == null)
                throw new ExceptionNotFound();
            if (entity.DeletionReason == null && reason == null)
                throw new ExceptionBadRequest("Not deleted");
            if (entity.DeletionReason != null && reason != null)
                throw new ExceptionBadRequest("Already deleted");
            if (id == currentUser.UserId)
                throw new ExceptionBadRequest("You cannot delete yourself");
            entity.DeletionReason = reason;
            await _db.SaveChangesAsync();
            _logger.LogInformation($"User with email {entity.Email} deleted.");
            var result = _mapper.Map<UsersRes>(entity);
            _bus.PubSub.Publish(new AccountDeactivated { User = result });
            return result;
        }

        public async Task<LoginRes> Login(LoginReq req)
        {
            var errors = new Dictionary<string, List<string>>();
            if (!Helpers.IsEmailValid(req.Email))
                errors.AddError("Email", "Email must be in following format: example@example.com");
            if (!Helpers.IsPasswordValid(req.Password))
                errors.AddError("Password", "Password must include at least 8 characters, one uppercase letter, one lowercase letter and one digit");
            var entity = await _db.Set<User>().Include(x => x.Role).FirstOrDefaultAsync(x => x.Email == req.Email);
            if (entity == null)
            {
                _logger.LogInformation($"User with email {req.Email} failed to log in. Wrong email.");
                errors.AddError("Email", "Email not found");
                throw new ExceptionBadRequest(errors);
            }
            if (Helpers.GenerateHash(entity.PasswordSalt, req.Password) != entity.PasswordHash)
            {
                _logger.LogInformation($"User with email {req.Email} failed to log in. Wrong password.");
                errors.AddError("Password", "Password is not correct");
            }
            if (errors.Count > 0)
                throw new ExceptionBadRequest(errors);
            _logger.LogInformation($"User with email {req.Email} logged in.");
            return _mapper.Map<LoginRes>(entity);
        }

        public async Task<UsersRes> VerifyEmail(int id)
        {
            var entity = await _db.Set<User>().FindAsync(id);
            if (entity == null)
                throw new ExceptionNotFound();
            entity.VerificationToken = $"{Guid.NewGuid():N}".Substring(0, 6);
            entity.TokenExpiry = DateTime.UtcNow.AddHours(24);
            await _db.SaveChangesAsync();
            _bus.PubSub.Publish(new EmailVerification { Token = _mapper.Map<TokenRes>(entity) });
            return _mapper.Map<UsersRes>(entity);
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
            if (entity.PublisherVerifiedById == null)
                entity.PublisherVerifiedById = GetUserId();
            else
                entity.PublisherVerifiedById = null;
            await _db.SaveChangesAsync();
            _logger.LogInformation($"Publisher with email {entity.Email} is verified.");
            var result = _mapper.Map<UsersRes>(entity);
            _bus.PubSub.Publish(new PublisherVerified { User = result });
            return result;
        }
    }
}
