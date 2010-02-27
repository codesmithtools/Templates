using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

using CodeSmith.Engine;
using CodeSmith.SchemaHelper;
using SchemaExplorer;
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

        [Browsable(false)]
        public string TemplateVersion
        {
            get
            {
                var fileName = typeof(CodeSmith.SchemaHelper.Entity).Assembly.Location;
                if (fileName != null)
                {
                    return FileVersionInfo.GetVersionInfo(fileName).FileVersion;
                }

                return "0.0.0.0";
            }
        }

        [Browsable(false)]
        public string CSLAVersion
        {
            get
            {
                return "3.8.2";
            }
        }

        [Browsable(false)]
        public string CodeSmithVersion
        {
            get
            {
                return CodeSmith.Engine.Configuration.Instance.ConfigurationVersion;
            }
        }

        [Browsable(false)]
        public string VersionInfo
        {
            get
            {
                return string.Format("CodeSmith: v{0}, CSLA Templates: v{1}, CSLA Framework: v{2}", CodeSmithVersion, TemplateVersion, CSLAVersion);
            }
        }

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
                else
                {
                    Configuration.Instance.TargetLanguage = LanguageEnum.CSharp;
                }
            }
        }

        #endregion

        #region Public Method(s)

        public virtual void RegisterReferences()
        {
            RegisterReference(Path.GetFullPath(Path.Combine(CodeTemplateInfo.DirectoryName, @"..\..\Common\Csla\Csla.dll")));
            RegisterReference("System.Configuration");
        }

        #endregion

        #region RenderHelper

        public void RenderHelper<T>(T template) where T : EntityCodeTemplate
        {
            RenderHelper(template, false);
        }

        public void RenderHelper<T>(T template, bool renderOptionalContent) where T : EntityCodeTemplate
        {
            this.CopyPropertiesTo(template);
            template.RenderOptionalContent = renderOptionalContent;
            template.Render(this.Response);
        }

        public void RenderHelper<T>(T template, TableSchema table) where T : EntityCodeTemplate
        {
            RenderHelper(template, table, false);
        }

        public void RenderHelper<T>(T template, TableSchema table, bool renderOptionalContent) where T : EntityCodeTemplate
        {
            this.CopyPropertiesTo(template);
            template.SourceTable = table;
            template.RenderOptionalContent = renderOptionalContent;
            template.Render(this.Response);
        }

        #endregion
    }
}
