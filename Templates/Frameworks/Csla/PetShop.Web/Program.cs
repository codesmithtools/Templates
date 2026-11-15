// The web app: the business objects' pages (Generated/), interactive on the server, on the EF Core DAL. Csla.cst wrote this
// file once: it's yours.
using Csla.Configuration;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddCascadingAuthenticationState();

// CSLA: an ApplicationContext for each circuit, and its user (AddServerSideBlazor), ViewModel<T>, the data portal here
builder.Services.AddCsla(options => options.AddAspNetCore().AddServerSideBlazor());

// the database: a copy of PetShop.db3, made at each start (the one beside the project stays as it is)
string database = Path.Combine(AppContext.BaseDirectory, "PetShop.web.db3");
File.Copy(Path.Combine(AppContext.BaseDirectory, "PetShop.db3"), database, overwrite: true);
builder.Services.AddPetShopDal(options => options.UseSqlite($"Data Source={database};Pooling=False"));

// the shell's: toasts, questions, the palette, the roots' counts, the editors' view model (Components/Ui)
builder.Services.AddUi();

// the sample's: orders for the demo, as it starts (DemoData.cs)
builder.Services.AddHostedService<PetShop.Web.DemoData>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseRequestLocalization("en-US");
app.UseDemoUsers(); // who you are: the demo's sign-in (Components/Layout/DemoUsers.cs)
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
