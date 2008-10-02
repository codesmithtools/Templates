using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LinqToSqlShared.Generator
{
    public class GeneratorSettings
    {
        private string mappingFile;

        public string MappingFile
        {
            get { return mappingFile; }
            set { mappingFile = value; }
        }

        private string _contextNamespace;

        public string ContextNamespace
        {
            get { return _contextNamespace; }
            set { _contextNamespace = value; }
        }

        private string _entityBase;

        public string EntityBase
        {
            get { return _entityBase; }
            set { _entityBase = value; }
        }

        private string _entityNamespace;

        public string EntityNamespace
        {
            get { return _entityNamespace; }
            set { _entityNamespace = value; }
        }

        private bool includeViews;

        public bool IncludeViews
        {
            get { return includeViews; }
            set { includeViews = value; }
        }

        private bool includeFunctions;

        public bool IncludeFunctions
        {
            get { return includeFunctions; }
            set { includeFunctions = value; }
        }

        private List<Regex> _ignoreExpressions = new List<Regex>();

        public List<Regex> IgnoreExpressions
        {
            get { return _ignoreExpressions; }
        }

        private List<Regex> _cleanExpressions = new List<Regex>();

        public List<Regex> CleanExpressions
        {
            get { return _cleanExpressions; }
        }

        private bool _disableRenaming = false;

        public bool DisableRenaming
        {
            get { return _disableRenaming; }
            set { _disableRenaming = value; }
        }

        public bool IsIgnored(string name)
        {
            if (IgnoreExpressions.Count == 0)
                return false;

            foreach (Regex regex in IgnoreExpressions)
            {
                if (regex.IsMatch(name))
                    return true;
            }

            return false;
        }

        public string CleanName(string name)
        {
            if (CleanExpressions.Count == 0)
                return name;

            foreach (Regex regex in CleanExpressions)
            {
                if (regex.IsMatch(name))
                {
                    return regex.Replace(name, "");
                }
            }

            return name;
        }

        public enum SerializationModeEnum
        {
            None = 0,
            Net_3_0 = 1,
            Net_3_5_SP1 = 2
        }

        private SerializationModeEnum _serializationMode = SerializationModeEnum.Net_3_5_SP1;

        public SerializationModeEnum SerializationMode
        {
            get { return _serializationMode; }
            set { _serializationMode = value; }
        }
    }
}