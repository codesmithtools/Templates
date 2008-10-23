using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SchemaExplorer;

namespace NHibernateHelper
{
    public class SearchCriteria
    {
        #region Static Content

        public static List<SearchCriteria> GetAllSearchCriteria(TableSchema table, string extendedProperty)
        {
            TableSearchCriteria tsc = new TableSearchCriteria(table, extendedProperty);
            return tsc.GetAllSearchCriteria();
        }
        public static List<SearchCriteria> GetAllSearchCriteria(TableSchema table)
        {
            TableSearchCriteria tsc = new TableSearchCriteria(table);
            return tsc.GetAllSearchCriteria();
        }

        public static List<SearchCriteria> GetPrimaryKeySearchCriteria(TableSchema table, string extendedProperty)
        {
            TableSearchCriteria tsc = new TableSearchCriteria(table, extendedProperty);
            return tsc.GetPrimaryKeySearchCriteria();
        }
        public static List<SearchCriteria> GetPrimaryKeySearchCriteria(TableSchema table)
        {
            TableSearchCriteria tsc = new TableSearchCriteria(table);
            return tsc.GetPrimaryKeySearchCriteria();
        }

        public static List<SearchCriteria> GetForeignKeySearchCriteria(TableSchema table, string extendedProperty)
        {
            TableSearchCriteria tsc = new TableSearchCriteria(table, extendedProperty);
            return tsc.GetForeignKeySearchCriteria();
        }
        public static List<SearchCriteria> GetForeignKeySearchCriteria(TableSchema table)
        {
            TableSearchCriteria tsc = new TableSearchCriteria(table);
            return tsc.GetForeignKeySearchCriteria();
        }

        public static List<SearchCriteria> GetIndexSearchCriteria(TableSchema table, string extendedProperty)
        {
            TableSearchCriteria tsc = new TableSearchCriteria(table, extendedProperty);
            return tsc.GetIndexSearchCriteria();
        }
        public static List<SearchCriteria> GetIndexSearchCriteria(TableSchema table)
        {
            TableSearchCriteria tsc = new TableSearchCriteria(table);
            return tsc.GetIndexSearchCriteria();
        }

        #endregion

        #region Declarations

        protected List<MemberColumnSchema> mcsList;
        protected MethodNameGenerationMode methodNameGenerationMode = MethodNameGenerationMode.Default;
        protected string methodName = String.Empty;
        protected string extendedProperty;
        protected bool isPrimaryKey;

        #endregion

        #region Constructors

        internal SearchCriteria(string extendedProperty)
            : this(extendedProperty, new List<MemberColumnSchema>(), false)
        {


        }
        internal SearchCriteria(string extendedProperty, List<MemberColumnSchema> mcsList)
            : this(extendedProperty, mcsList, false)
        {
        }
        internal SearchCriteria(string extendedProperty, List<MemberColumnSchema> mcsList, bool isPrimaryKey)
        {
            this.extendedProperty = extendedProperty;
            this.mcsList = mcsList;
            this.isPrimaryKey = isPrimaryKey;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets MethodName to default generation: "GetBy{0}{1}{n}"
        /// </summary>
        public void SetMethodNameGeneration()
        {
            methodNameGenerationMode = MethodNameGenerationMode.Default;

            GenerateMethodName("GetBy", String.Empty, String.Empty);
        }
        /// <summary>
        /// Sets MethodName to be value of the specified Extended Property from the database.
        /// </summary>
        /// <param name="extendedProperty">Value of the Extended Property.</param>
        public void SetMethodNameGeneration(string extendedProperty)
        {
            methodNameGenerationMode = MethodNameGenerationMode.ExtendedProperty;

            methodName = extendedProperty;
        }
        /// <summary>
        /// Sets MethodName to custom generation: "{prefix}{0}{delimeter}{1}{suffix}"
        /// </summary>
        /// <param name="prefix">Method Prefix</param>
        /// <param name="delimeter">Column Delimeter</param>
        /// <param name="suffix">Method Suffix</param>
        public void SetMethodNameGeneration(string prefix, string delimeter, string suffix)
        {
            methodNameGenerationMode = MethodNameGenerationMode.Custom;

            GenerateMethodName(prefix, delimeter, suffix);
        }

        public override string ToString()
        {
            if (String.IsNullOrEmpty(methodName))
                SetMethodNameGeneration();

            return methodName;
        }

        internal void Add(MemberColumnSchema item)
        {
            mcsList.Add(item);
        }
        protected void GenerateMethodName(string prefix, string delimeter, string suffix)
        {
            StringBuilder sb = new StringBuilder();
            bool isFirst = true;

            sb.Append(prefix);
            foreach (MemberColumnSchema mcs in mcsList)
            {
                if (isFirst)
                    isFirst = false;
                else
                    sb.Append(delimeter);

                if (mcs.ExtendedProperties.Contains(extendedProperty))
                    sb.Append(mcs.ExtendedProperties[extendedProperty].Value.ToString());
                else
                    sb.Append(mcs.Name);
            }
            sb.Append(suffix);

            methodName = sb.ToString();
        }

        #endregion

        #region Properties

        public bool IsAllUnique
        {
            get
            {
                bool result = false;
                foreach (MemberColumnSchema msc in mcsList)
                    if (msc.IsUnique)
                    {
                        result = true;
                        break;
                    }
                return result;
            }
        }
        public List<MemberColumnSchema> Items
        {
            get { return mcsList; }
        }
        public bool IsPrimaryKey
        {
            get { return isPrimaryKey; }
        }
        public string MethodName
        {
            get { return this.ToString(); }
        }
        public MethodNameGenerationMode MethodNameGeneration
        {
            get { return methodNameGenerationMode; }
        }

        internal string Key
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                foreach (MemberColumnSchema mcs in mcsList)
                    sb.Append(mcs.Name);

                return sb.ToString();
            }
        }

        #endregion
    }
}
