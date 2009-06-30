using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using CodeSmith.Engine;
using CodeSmith.SchemaHelper;

using SchemaExplorer;

using Configuration=CodeSmith.SchemaHelper.Configuration;
using StringCollection=CodeSmith.CustomProperties.StringCollection;

namespace QuickStart
{
    public class QuickStartCodeTemplate : CSLABaseTemplate
    {
        #region Private Member(s)

        private DatabaseSchema _database;

        #endregion

        #region Constructor(s)

        public QuickStartCodeTemplate()
        {
            DataAccessImplementation = DataAccessMethod.ParameterizedSQL;
            LaunchVisualStudio = true;
            UseLazyLoading = true;

            CleanExpressions = new StringCollection();
            IgnoreExpressions = new StringCollection();
        }

        #endregion

        #region 1. DataSource

        [Category("1. DataSource")]
        [Description("Source Database")]
        public DatabaseSchema SourceDatabase
        {
            get
            {
                return _database;
            }
            set
            {
                if (value != null)
                {
                    _database = value;
                    _database.DeepLoad = true;
                    OnDatabaseChanged();
                }
            }
        }

        [Category("1. DataSource")]
        [Description("List of regular expressions to clean table, view and column names.")]
        [Optional]
        [DefaultValue("^\\w+_")]
        public CodeSmith.CustomProperties.StringCollection CleanExpressions { get; set; }
        
        [Category("1. DataSource")]
        [Description("List of regular expressions to ignore tables when generating.")]
        [Optional]
        [DefaultValue("sysdiagrams$")]
        public CodeSmith.CustomProperties.StringCollection IgnoreExpressions { get; set; }

        [Browsable(false)]
        public List<Entity> Entities { get; set; }

        #endregion

        #region 2. Solution

        [Category("2. Solution")]
        [Description("Launch Visual Studio after generation.")]
        [DefaultValue(true)]
        public bool LaunchVisualStudio { get; set; }

        [Category("2. Solution")]
        [Description("The language the project will be built in.")]
        [DefaultValue("")]
        public LanguageEnum Language { get; set; }

        [Editor(typeof(System.Windows.Forms.Design.FolderNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Optional, NotChecked]
        [Category("2. Solution")]
        [Description("The path to the Solution location.")]
        [DefaultValue("")]
        public string Location { get; set; }

        [Category("2. Solution")]
        [Description("Name of the project to be generated.")]
        [DefaultValue("")]
        public string SolutionName { get; set; }

        #endregion

        #region 3. Business Project

        [Category("3. Business Project")]
        [Description("The namespace for the business project.")]
        public string BusinessProjectName { get; set; }

        [Category("3. Business Project")]
        [Description("Uses private member backing variables for properties.")]
        [DefaultValue(false)]
        public bool UseMemberVariables { get; set; }

        #endregion

        #region 4. Data Project

        [Category("4. Data Project")]
        [Description("Changes how the Business Data Access Methods and Data Access Layer are implemented.")]
        public DataAccessMethod DataAccessImplementation { get; set; }

        [Category("4. Data Project")]
        [Description("The Name Space for the Data Project.")]
        public string DataProjectName { get; set; }

        [Category("4. Data Project")]
        [Description("The name of the Data Access class.")]
        [DefaultValue("DataAccessLayer")]
        public string DataClassName { get; set; }

        [Category("4. Data Project")]
        [Description("The value all sql parameters should be prefixed with.")]
        [DefaultValue("@p_")]
        public string ParameterPrefix
        {
            get
            {
                return Configuration.Instance.ParameterPrefix;
            }
            set
            {
                Configuration.Instance.ParameterPrefix = value;
            }
        }

        [Category("4. Data Project")]
        [Description("Changes how the business layer and data acces layer is implemented.")]
        [DefaultValue(false)]
        public bool UseLazyLoading { get; set; }

        #endregion

        #region 5. Interface Project

        [Category("5. Interface Project")]
        [Description("The namespace for the interface project.")]
        public string InterfaceProjectName { get; set; }

        #endregion

        #region Public Methods

        public virtual void Generate()
        {
        }

        public List<Entity> GetChildEntities()
        {
            List<Entity> entities = new List<Entity>(Entities.Count);

            foreach (var entity in Entities)
            {
                foreach (AssociationMember associationMember in entity.ManyToOne)
                {
                    if (!entities.Contains(associationMember.Entity))
                    {
                        entities.Add(associationMember.Entity);
                    }
                }

                foreach (AssociationMember associationMember in entity.OneToZeroOrOne)
                {
                    if (!entities.Contains(associationMember.Entity))
                    {
                        entities.Add(associationMember.Entity);
                    }
                }
            }

            return entities;
        }

        public List<Entity> GetListEntities()
        {
            List<Entity> entities = new List<Entity>(Entities.Count);

            foreach (var entity in Entities)
            {
                foreach (AssociationMember associationMember in entity.OneToMany)
                {
                    if(!entities.Contains(associationMember.Entity))
                    {
                        entities.Add(associationMember.Entity);
                    }
                }
            }

            return entities;
        }

        public IEnumerable<Entity> GetEntities()
        {
            IEnumerable<Entity> excludedEntities = GetChildEntities();

            if(excludedEntities == null)
                return Entities;

            return from entity in Entities
                    where !excludedEntities.Contains(entity)
                    select entity;
        }

        #endregion

        #region Private Method(s)

        public static void LaunchVisualStudioWithSolution(string solutionLink)
        {
            const string args = "/build debug";
            using (Process.Start(solutionLink, args))
            { }
        }

        public virtual void OnDatabaseChanged()
        {
            if (CleanExpressions.Count == 0)
                CleanExpressions.Add("^\\w+_");

            if (IgnoreExpressions.Count == 0)
                IgnoreExpressions.Add("sysdiagrams$");

            Configuration.Instance.CleanExpressions.Clear();
            foreach (string clean in CleanExpressions)
            {
                if (!string.IsNullOrEmpty(clean))
                {
                    Configuration.Instance.CleanExpressions.Add(new Regex(clean));
                }
            }

            Configuration.Instance.IgnoreExpressions.Clear();
            foreach (string ignore in IgnoreExpressions)
            {
                if (!string.IsNullOrEmpty(ignore))
                {
                    Configuration.Instance.IgnoreExpressions.Add(new Regex(ignore));
                }
            }

            Entities = new EntityManager(SourceDatabase).Entities;

            if (string.IsNullOrEmpty(DataClassName))
                DataClassName = "DataAccessLayer";

            if (string.IsNullOrEmpty(SolutionName))
                SolutionName = SourceDatabase.Namespace();

            if (string.IsNullOrEmpty(BusinessProjectName))
                BusinessProjectName = string.Format("{0}.Business", SourceDatabase.Namespace());

            if (string.IsNullOrEmpty(DataProjectName))
                DataProjectName = string.Format("{0}.Data", SourceDatabase.Namespace());

            if (string.IsNullOrEmpty(InterfaceProjectName))
                InterfaceProjectName = string.Format("{0}.UI", SourceDatabase.Namespace());

            //if (string.IsNullOrEmpty(TestProjectName))
            //    TestProjectName = string.Format("{0}.Test", SourceDatabase.Namespace());

            if (string.IsNullOrEmpty(Location))
                Location = Path.Combine(
                    CodeSmith.Engine.Configuration.Instance.CodeSmithTemplatesDirectory,
                    Path.Combine("CSLA", SourceDatabase.Name));
        }

        #endregion
    }
}
