using System;

namespace LinqToSqlShared.Generator
{
    [Serializable]
    public class SchemaItemProcessedEventArgs : EventArgs
    {
        public SchemaItemProcessedEventArgs(string name)
        {
            _name = name;
        }

        private string _name;

        public string Name
        {
            get { return _name; }
        }
    }
}