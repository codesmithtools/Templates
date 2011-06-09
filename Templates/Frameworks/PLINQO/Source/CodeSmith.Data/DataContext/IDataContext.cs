namespace CodeSmith.Data
{
    public interface IDataContext
    {
        string ConnectionString { get; }

        void Detach(params object[] enities);
    }
}