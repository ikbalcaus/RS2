using eBooks.API;
using eBooks.API.Filters;
using eBooks.Database;
using eBooks.Interfaces;
using eBooks.Services;
using eBooks.Services.BooksStateMachine;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddDbContext<EBooksContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Database")));
builder.Services.AddScoped<IMapper, Mapper>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddTransient<IUsersService, UsersService>();
builder.Services.AddTransient<IBooksService, BooksService>();

builder.Services.AddTransient<BaseBooksState>();
builder.Services.AddTransient<ApproveBooksState>();
builder.Services.AddTransient<HideBooksState>();
builder.Services.AddTransient<AwaitBooksState>();
builder.Services.AddTransient<DraftBooksState>();
builder.Services.AddTransient<RejectBooksState>();

builder.Services.AddControllers(x => x.Filters.Add<ExceptionFilter>());

builder.Services.AddAuthentication("BasicAuthentication").AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("basicAuth", new Microsoft.OpenApi.Models.OpenApiSecurityScheme()
    {
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "basic"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement()
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

/*
dotnet ef dbcontext scaffold "Name=Database" Microsoft.EntityFrameworkCore.SqlServer --output-dir Models --context-dir . --force --project eBooks.Database/eBooks.Database.csproj --startup-project eBooks.API/eBooks.API.csproj
*/
