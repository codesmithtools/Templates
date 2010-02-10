using System;
using System.ComponentModel;

namespace QuickStart
{
    public class DataCodeTemplate : EntityCodeTemplate
    {
        #region Constructor(s)

        public DataCodeTemplate()
        {
        }

        #endregion

        #region Public Properties

        [Category("3. Business Project")]
        [Description("If set to true then the Object Factory Child logic will be implemented.")]
        public bool IsChildBusinessObject { get; set; }

        [Category("3. Business Project")]
        [Description("If set to true then the Object Factory Read-Only logic will be implemented.")]
        public bool IsReadOnlyBusinessObject { get; set; }

        [Category("4. Data Project")]
        [Description("The Name Space for the Data Project.")]
        public string DataProjectName { get; set; }

        #endregion
    }
}
