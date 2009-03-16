using System;
using System.IO;

using CodeSmith.Engine;
using CodeSmith.SchemaHelper;

using Configuration=CodeSmith.SchemaHelper.Configuration;

namespace QuickStart
{
    public class CSLABaseTemplate : CodeTemplate
    {
        #region Constructor(s)

        public CSLABaseTemplate()
        {
            ResolveTargetLanguage();
        }

        #endregion

        #region Public Properties

        #endregion

        #region Private Method(s)

        private void ResolveTargetLanguage()
        {
            if (CodeTemplateInfo != null)
            {
                if (CodeTemplateInfo.TargetLanguage.ToUpper() == "VB")
                {
                    Configuration.Instance.TargetLanguage = LanguageEnum.VB;
                }
            }
        }

        #endregion

        #region Public Method(s)

        public virtual void RegisterReferences()
        {
            RegisterReference(Path.Combine(CodeTemplateInfo.DirectoryName, @"..\..\Common\Csla\Csla.dll"));
        }

        #endregion
    }
}
