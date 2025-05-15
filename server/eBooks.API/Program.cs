using eBooks.API;
using eBooks.API.Auth;
using eBooks.API.Filters;
using eBooks.Database;
using eBooks.Interfaces;
using eBooks.Services;
using eBooks.Services.BooksStateMachine;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Stripe;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers(x => x.Filters.Add<ExceptionFilter>());
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

builder.Services.AddDbContext<EBooksContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Database"))
);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("Moderator", policy => policy.RequireRole("Admin", "Moderator"));
    options.AddPolicy("User", policy => policy.RequireRole("Admin", "Moderator", "User"));
});

builder.Services.AddScoped<IMapper, Mapper>();
builder.Services.AddScoped<AccessControlHandler>();

builder.Services.AddTransient<IAccessRightsService, AccessRightsService>();
builder.Services.AddTransient<IAuthorsService, AuthorsService>();
builder.Services.AddTransient<IBooksService, BooksService>();
builder.Services.AddTransient<IFavoritesService, FavoritesService>();
builder.Services.AddTransient<IGenresService, GenresService>();
builder.Services.AddTransient<ILanguagesService, LanguagesService>();
builder.Services.AddTransient<IPaymentService, PaymentService>();
builder.Services.AddTransient<IReadingProgressService, ReadingProgressService>();
builder.Services.AddTransient<IReviewService, ReviewsService>();
builder.Services.AddTransient<IRolesService, RolesService>();
builder.Services.AddTransient<IUsersService, UsersService>();
builder.Services.AddTransient<IWishlistService, WishlistService>();

builder.Services.AddTransient<BaseBooksState>();
builder.Services.AddTransient<ApproveBooksState>();
builder.Services.AddTransient<HideBooksState>();
builder.Services.AddTransient<AwaitBooksState>();
builder.Services.AddTransient<DraftBooksState>();
builder.Services.AddTransient<RejectBooksState>();

builder.Services.AddAuthentication("BasicAuthentication").AddScheme<AuthenticationSchemeOptions, BasicAuthHandler>("BasicAuthentication", null);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(x =>
{
    x.AddSecurityDefinition("basicAuth", new Microsoft.OpenApi.Models.OpenApiSecurityScheme()
    {
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "basic"
    });
    x.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference{Type = ReferenceType.SecurityScheme, Id = "basicAuth"}
            },
            new string[] {}
    } });
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
