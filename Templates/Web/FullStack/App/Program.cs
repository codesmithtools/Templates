// The PetShop API: the generated endpoints over the generated EF Core model, with request validation and an OpenAPI
// document at /openapi/v1.json.
using Microsoft.EntityFrameworkCore;
using PetShop.Api;
using PetShop.Data;

var builder = WebApplication.CreateBuilder(args);
string database = Path.Combine(AppContext.BaseDirectory, "PetShop.db3");
builder.Services.AddDbContext<PetShopContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("PetShop") ?? $"Data Source={database}"));
builder.Services.AddValidation();
builder.Services.AddOpenApi();

var app = builder.Build();
app.MapOpenApi();
app.MapPetShopApi();
app.Run();
