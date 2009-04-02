using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;

using CodeSmith.CustomProperties;
using CodeSmith.Engine;
using CodeSmith.SchemaHelper;

using SchemaExplorer;

using Configuration=CodeSmith.SchemaHelper.Configuration;

namespace QuickStart
{
    public class CSLAMasterTemplate : CSLABaseTemplate
    {  
        #region Private Member(s)

        private TableSchemaCollection _tables;

        #endregion

        #region Constructor(s)

        public CSLAMasterTemplate()
        {
            CleanExpressions = new StringCollection();
            IgnoreExpressions = new StringCollection();
        }

        #endregion

        #region Public Properties

        [Category("1. DataSource")]
        [Description("Source Tables")]
        public TableSchemaCollection SourceTables
        {
            get
            {
                return _tables;
            }
            set
            {
                if (value != null && _tables != value)
                {
                    _tables = value;
                    OnTablesChanged();
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
        public List<Entity> Entities { get; internal set; }

        [Editor(typeof(System.Windows.Forms.Design.FolderNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Optional, NotChecked]
        [Category("2. Solution")]
        [Description("The path to the Solution location.")]
        [DefaultValue("")]
        public string Location { get; set; }

        #endregion
        
        #region Public Virtual Method(s)

        public virtual void OnTablesChanged()
        {
            if (CleanExpressions.Count == 0)
                CleanExpressions.Add("^\\w+_");

            if (IgnoreExpressions.Count == 0)
                IgnoreExpressions.Add("sysdiagrams$");

            Configuration.Instance.CleanExpressions = new List<Regex>();
            foreach (string clean in CleanExpressions)
            {
                if (!string.IsNullOrEmpty(clean))
                {
                    Configuration.Instance.CleanExpressions.Add(new Regex(clean));
                }
            }

            Configuration.Instance.IgnoreExpressions = new List<Regex>();
            foreach (string ignore in IgnoreExpressions)
            {
                if (!string.IsNullOrEmpty(ignore))
                {
                    Configuration.Instance.IgnoreExpressions.Add(new Regex(ignore));
                }
            }

            Entities = new EntityManager(SourceTables).Entities;

            if (string.IsNullOrEmpty(Location) && SourceTables.Count > 0)
                Location = Path.Combine(
                    CodeSmith.Engine.Configuration.Instance.CodeSmithTemplatesDirectory, 
                    Path.Combine("CSLA", SourceTables[0].Database.Name));
        }

        public virtual void Generate()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
