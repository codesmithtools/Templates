using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

using CodeSmith.Engine;
using CodeSmith.SchemaHelper;
using Configuration = CodeSmith.SchemaHelper.Configuration;

namespace Generator.CSLA
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
                var fileName = GetType().Assembly.Location;
                if (fileName != null)
                {
                    return FileVersionInfo.GetVersionInfo(fileName).FileVersion;
                }

                return "3.0.0.0";
            }
        }

        [Browsable(false)]
        public string CSLAVersion
        {
            get
            {
                return IsCSLA40 ? "4.0.0" : "3.8.4";
            }
        }

        [Browsable(false)]
        public virtual bool IsCSLA40
        {
            get
            {
                return Configuration.Instance.FrameworkVersion == FrameworkVersion.v40;
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
                return String.Format("CodeSmith: v{0}, CSLA Templates: v{1}, CSLA Framework: v{2}", CodeSmithVersion, TemplateVersion, CSLAVersion);
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
                    Configuration.Instance.TargetLanguage = Language.VB;
                }
                else
                {
                    Configuration.Instance.TargetLanguage = Language.CSharp;
                }
            }
        }

        #endregion

        #region Public Method(s)

        public virtual void RegisterReferences()
        {
            RegisterReference(!IsCSLA40
                                  ? Path.GetFullPath(Path.Combine(CodeTemplateInfo.DirectoryName, @"..\..\Common\Csla\3.8\Client\Csla.dll"))
                                  : Path.GetFullPath(Path.Combine(CodeTemplateInfo.DirectoryName, @"..\..\Common\Csla\4.0\Client\Csla.dll")));

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

        public void RenderHelper<T>(T template, IEntity table) where T : EntityCodeTemplate
        {
            RenderHelper(template, table, false);
        }

        public void RenderHelper<T>(T template, IEntity table, bool renderOptionalContent) where T : EntityCodeTemplate
        {
            this.CopyPropertiesTo(template);
            template.Entity = table;
            template.RenderOptionalContent = renderOptionalContent;
            template.Render(this.Response);
        }

        #endregion
    }
}
