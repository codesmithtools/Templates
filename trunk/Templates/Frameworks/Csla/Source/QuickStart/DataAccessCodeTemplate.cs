using System;
using System.ComponentModel;
using System.IO;
using Configuration = CodeSmith.SchemaHelper.Configuration;

namespace QuickStart
{
    public class DataAccessCodeTemplate : CSLAMasterTemplate
    {
        #region Constructor(s)

        public DataAccessCodeTemplate()
        {
        }

        #endregion

        #region Public Properties

        [Category("4. Data Project")]
        [Description("Changes how the Business Data Access Methods and Data Access Layer are implemented.")]
        public DataAccessMethod DataAccessImplementation { get; set; }

        [Category("4. Data Project")]
        [Description("The Name Space for the Data Project.")]
        public string DataProjectName { get; set; }

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

        #region Public Overridden Methods(s)

        public override void RegisterReferences()
        {
            RegisterReference("System.Configuration");
            RegisterReference(Path.GetFullPath(Path.Combine(CodeTemplateInfo.DirectoryName, @"..\..\Common\Csla\Csla.dll")));
        }

        #endregion
    }
}
