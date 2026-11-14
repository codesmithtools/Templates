// SQLite has four storage classes (INTEGER, REAL, TEXT, BLOB), and a column's values can mix them: PetShop's
// decimal(10,2) columns hold 16.5 as REAL and 10 as INTEGER, which Dapper can't read as a decimal; its datetime columns
// hold text ("2006-09-01 09:01:48Z"), which Dapper would read as local time. These handlers read them; Dapper and
// Microsoft.Data.Sqlite do the rest (long to int, 0 and 1 to bool, text to DateOnly).
using System.Data;
using System.Globalization;
using Dapper;

static class SqliteTypes
{
    public static void Register()
    {
        SqlMapper.AddTypeHandler(new Handler<decimal>(value => Convert.ToDecimal(value, CultureInfo.InvariantCulture)));
        SqlMapper.AddTypeHandler(new Handler<DateTime>(value => value is string text
            ? DateTime.Parse(text, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind)
            : Convert.ToDateTime(value, CultureInfo.InvariantCulture)));
    }

    // Reads with parse; writes the value as it is (Microsoft.Data.Sqlite writes decimals and dates as text).
    private sealed class Handler<T>(Func<object, T> parse) : SqlMapper.TypeHandler<T>
    {
        public override T Parse(object value) => parse(value);

        public override void SetValue(IDbDataParameter parameter, T? value) => parameter.Value = value;
    }
}
