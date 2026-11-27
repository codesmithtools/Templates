// The PetShop API: the generated endpoints over the generated EF Core model, with request validation, problem details
// and an OpenAPI document at /openapi/v1.json. It serves the web UI too (client/, built into wwwroot by npm run build):
// a path that isn't the API's or a file is the UI's, which routes it (one with a dot in it too: /settings/max.items).
using Microsoft.EntityFrameworkCore;
using PetShop.Api;
using PetShop.Data;

var builder = WebApplication.CreateBuilder(args);
string database = Path.Combine(AppContext.BaseDirectory, "PetShop.db3");
builder.Services.AddDbContext<PetShopContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("PetShop") ?? $"Data Source={database}"));
builder.Services.AddValidation();
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();

var app = builder.Build();
app.UseStatusCodePages();
app.UseStaticFiles();
app.UseRouting(); // after the files: the UI's fallback below takes every other path, a dot in it or not
app.MapOpenApi();
app.MapPetShopApi();
app.MapFallback("/api/{**path}", () => TypedResults.NotFound()); // not the UI's
if (File.Exists(Path.Combine(app.Environment.WebRootPath ?? "wwwroot", "index.html")))
    app.MapFallbackToFile("{*path}", "index.html");
else
    app.MapGet("/", () => "The API is at /api and /openapi/v1.json. For the UI too, build it: cd client && npm run build.");
app.Run();
