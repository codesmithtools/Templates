using System;
using System.ComponentModel;

using CodeSmith.Engine;
using CodeSmith.SchemaHelper;

namespace QuickStart
{
    public class InterfaceCodeTemplate : CSLABaseTemplate
    {
         #region Constructor(s)

        public InterfaceCodeTemplate()
        {
        }

        #endregion

        #region 2. Solution

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

        #endregion

        #region 3. Business Project

        [Category("3. Business Project")]
        [Description("The namespace for the business project.")]
        public string BusinessProjectName { get; set; }

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

        #endregion
    }
}
