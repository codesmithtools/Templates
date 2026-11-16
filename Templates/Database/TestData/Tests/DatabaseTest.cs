// What every test derives from (TestData.csp's TestBase): a database of its own, a copy of PetShop.db3 in memory, so
// each test starts from the same rows and changes only its copy. Tests run side by side.
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Xunit;

namespace PetShop.Tests;

public abstract class DatabaseTest : IAsyncLifetime
{
    private readonly SqliteConnection connection = new("Data Source=:memory:;Foreign Keys=True");

    static DatabaseTest() => SqliteTypes.Register();

    /// <summary>The test's database.</summary>
    protected DbConnection Connection => connection;

    /// <summary>The builders, on <see cref="Connection"/>.</summary>
    protected TestData Data { get; private set; } = null!;

    /// <summary>The test's own: cancelled when the test run is.</summary>
    protected static CancellationToken CancellationToken => TestContext.Current.CancellationToken;

    public async ValueTask InitializeAsync()
    {
        await connection.OpenAsync(CancellationToken);
        await using var petShop = new SqliteConnection($"Data Source={Path.Combine(AppContext.BaseDirectory, "PetShop.db3")};Mode=ReadOnly");
        await petShop.OpenAsync(CancellationToken);
        petShop.BackupDatabase(connection);
        Data = new TestData(connection);
    }

    public async ValueTask DisposeAsync()
    {
        await connection.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
