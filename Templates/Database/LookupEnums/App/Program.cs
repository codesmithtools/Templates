// Uses the enum generated from PetShop's Category table (../Generated/Category.cs).
using System.ComponentModel;
using System.Reflection;
using PetShop.Data;

// Each category: its member, its code in the database, and its description.
foreach (Category category in Enum.GetValues<Category>())
    Console.WriteLine($"{category,-12} {category.ToCode(),-8} {Description(category)}");

// And back, from a code (a row's CategoryId, a query string) to the enum.
string code = Category.Fish.ToCode();
if (CategoryExtensions.TryParseCode(code, out Category parsed))
    Console.WriteLine($"{code} is {parsed}");
if (!CategoryExtensions.TryParseCode("DRAGONS", out _))
    Console.WriteLine("DRAGONS isn't a category");

// The [Description] the template puts on each member.
static string Description(Category category) =>
    typeof(Category).GetField(category.ToString())?.GetCustomAttribute<DescriptionAttribute>()?.Description ?? "";
