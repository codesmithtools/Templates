// The demo's sign-in: two users to be, picked in the top bar (UserMenu). A cookie says who you are when a page loads,
// and switching tells the open circuit at once, through CSLA's ApplicationContext.User (which sets Blazor's
// AuthenticationState). Use your real sign-in instead (cookies, OpenID Connect, ...): the pages only ask CSLA what the
// user may do. Csla.cst wrote this file once: it's yours.
using System.Security.Claims;

namespace PetShop.Web.Components.Layout;

public sealed record DemoUser(string Name, string Role, string Description, string Color)
{
    public ClaimsPrincipal Principal => new(new ClaimsIdentity([new Claim(ClaimTypes.Name, Name), new Claim(ClaimTypes.Role, Role)], "Demo"));

    public string Initial => Name[..1];
}

public static class DemoUsers
{
    public const string Cookie = "demo-user";

    public static readonly DemoUser Admin = new("Admin", "Admin", "Has the Admin role: may do what the rules keep for Admins.", "#4f46e5");
    public static readonly DemoUser Clerk = new("Clerk", "Clerk", "Hasn't: what the rules keep for Admins is read-only, or not offered.", "#0d9488");

    public static IReadOnlyList<DemoUser> All { get; } = [Admin, Clerk];

    public static DemoUser Find(string? name) => All.FirstOrDefault(u => string.Equals(u.Name, name, StringComparison.OrdinalIgnoreCase)) ?? Clerk;

    /// <summary>Who a request is: the cookie's user (Clerk at first).</summary>
    public static IApplicationBuilder UseDemoUsers(this IApplicationBuilder app) =>
        app.Use((context, next) =>
        {
            context.User = Find(context.Request.Cookies[Cookie]).Principal;
            return next(context);
        });
}
