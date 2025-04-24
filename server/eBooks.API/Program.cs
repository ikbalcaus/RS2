using eBooks.Database;
using eBooks.Interfaces;
using eBooks.Services;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddDbContext<EBooksContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Database")));
builder.Services.AddScoped<IMapper, Mapper>();

builder.Services.AddTransient<IUsersService, UsersService>();
builder.Services.AddTransient<IBooksService, BooksService>();

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
