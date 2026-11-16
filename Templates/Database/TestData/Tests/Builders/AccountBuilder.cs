// Yours: a change to the generated AccountBuilder (Generated/Builders/AccountBuilder.cs). Generating again keeps it.
using Bogus;
using PetShop.Data;

namespace PetShop.Tests;

public sealed partial class AccountBuilder
{
    // email addresses at example.com, which no one receives
    partial void Configure(Faker<Account> faker) =>
        faker.RuleFor(a => a.Email, f => f.Internet.Email(provider: "example.com"));
}
