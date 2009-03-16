using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

using CodeSmith.Engine;
using CodeSmith.SchemaHelper;

using SchemaExplorer;

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
