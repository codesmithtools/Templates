namespace LinqToSqlShared.DbmlObjectModel
{
    public abstract class Node : IProcessed
    {
        protected static string KeyValue<T>(string key, T value)
        {
            return !Equals(value, default(T)) ? key + "=" + value + " " : string.Empty;
        }

        protected static string SingleValue<T>(T value)
        {
            return !Equals(value, default(T)) ? value + " " : string.Empty;
        }

        public bool IsProcessed { get; set; }
        
    }
}