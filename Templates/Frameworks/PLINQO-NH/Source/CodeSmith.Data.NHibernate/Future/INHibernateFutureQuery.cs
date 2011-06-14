namespace CodeSmith.Data.NHibernate
{
    public interface INHibernateFutureQuery
    {
        bool IsLoaded { get; set; }

        void Load();
    }
}