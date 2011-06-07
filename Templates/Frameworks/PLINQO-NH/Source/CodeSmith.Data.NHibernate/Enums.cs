namespace CodeSmith.Data.NHibernate
{
    public enum ComparisonOperator
    {
        Equals,
        NotEquals,
        GreaterThan,
        GreaterThanOrEquals,
        LessThan,
        LessThanOrEquals
    }

    public enum ContainmentOperator
    {
        Equals,
        NotEquals,
        Contains,
        StartsWith,
        EndsWith,
        NotContains
    }
}
