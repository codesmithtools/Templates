//------------------------------------------------------------------------------
//
// Copyright (c) 2002-2012 CodeSmith Tools, LLC.  All rights reserved.
// 
// The terms of use for this software are contained in the file
// named sourcelicense.txt, which can be found in the root of this distribution.
// By using this software in any fashion, you are agreeing to be bound by the
// terms of this license.
// 
// You must not remove this notice, or any other, from this software.
//
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using CodeSmith.Engine;
using CodeSmith.SchemaHelper;
using Configuration = CodeSmith.SchemaHelper.Configuration;

namespace Generator.CSLA.CodeTemplates {
    public class CSLABaseTemplate : CodeTemplate {
        public CSLABaseTemplate() {
            ResolveTargetLanguage();

            // Preserve Backwards Compat.
            Configuration.Instance.IncludeManyToManyAssociations = false;
            Configuration.Instance.SearchCriteriaProperty.MethodKeySuffix = String.Empty;
            Configuration.Instance.NamingProperty.ColumnNaming = ColumnNaming.Preserve;
            Configuration.Instance.NamingProperty.AssociationTypeNameSuffix = AssociationSuffix.Singular;
            Configuration.Instance.NamingProperty.AssociationSuffix = AssociationSuffix.Plural;
        }

        [Browsable(false)]
        public string TemplateVersion {
            get {
                var fileName = typeof(CSLABaseTemplate).Assembly.Location;
                if (fileName != null) {
                    return FileVersionInfo.GetVersionInfo(fileName).FileVersion;
                }

                return "4.5.0.0";
            }
        }

        private string GetCSLAVersionString() {
            switch (Configuration.Instance.FrameworkVersion) {
                case FrameworkVersion.v45:
                    return "4.5.x";

                case FrameworkVersion.v40:
                    return "4.3.x";

                default:
                    return "3.8.x";
            }
        }

        [Browsable(false)]
        private string CodeSmithVersion { get { return CodeSmith.Engine.Configuration.Instance.ConfigurationVersion; } }

        [Browsable(false)]
        public bool IsCSLA38 { get { return Configuration.Instance.FrameworkVersion == FrameworkVersion.v35; } }

        [Browsable(false)]
        public bool IsCSLA43 { get { return Configuration.Instance.FrameworkVersion == FrameworkVersion.v40; } }

        [Browsable(false)]
        public bool IsCSLA45 { get { return Configuration.Instance.FrameworkVersion == FrameworkVersion.v45; } }

        [Browsable(false)]
        public string VersionInfo { get { return String.Format("CodeSmith: v{0}, CSLA Templates: v{1}, CSLA Framework: v{2}", CodeSmithVersion, TemplateVersion, GetCSLAVersionString()); } }

        private void ResolveTargetLanguage() {
            if (CodeTemplateInfo == null)
                return;

            if (CodeTemplateInfo.TargetLanguage.ToUpper() == "VB")
                Configuration.Instance.TargetLanguage = Language.VB;
            else
                Configuration.Instance.TargetLanguage = Language.CSharp;
        }

        public virtual void RegisterReferences() {
            // TODO: Add NuGet Support.
            RegisterReference(IsCSLA38
                ? Path.GetFullPath(Path.Combine(CodeTemplateInfo.DirectoryName, @"..\..\Common\Csla\3.8\Client\Csla.dll"))
                : Path.GetFullPath(Path.Combine(CodeTemplateInfo.DirectoryName, @"..\..\Common\Csla\4.3\Client\Csla.dll")));

            RegisterReference("System.Configuration");
        }

        #region Render Helpers

        protected readonly List<string> PropertyIgnoreList = new List<string> {
                                                                         "CommandObject",
                                                                         "Criteria",
                                                                         "DynamicRoot",
                                                                         "EditableChild",
                                                                         "EditableRoot",
                                                                         "ReadOnlyChild",
                                                                         "ReadOnlyRoot",
                                                                         "SwitchableObject",
                                                                         "DynamicListBase",
                                                                         "DynamicRootList",
                                                                         "EditableRootList",
                                                                         "EditableChildList",
                                                                         "ReadOnlyList",
                                                                         "ReadOnlyChildList",
                                                                         "NameValueList",
                                                                         "SourceTable",
                                                                         "SourceView",
                                                                         "SourceCommand"
                                                                     };

        public void RenderHelper<T>(T template) where T : EntityCodeTemplate {
            RenderHelper(template, false);
        }

        public void RenderHelper<T>(T template, bool renderOptionalContent) where T : EntityCodeTemplate {
            CopyPropertiesTo(template, true, PropertyIgnoreList);
            template.RenderOptionalContent = renderOptionalContent;
            template.Render(this.Response);
        }

        public void RenderHelper<T>(T template, IEntity entity) where T : EntityCodeTemplate {
            RenderHelper(template, entity, false);
        }

        public void RenderHelper<T>(T template, IEntity entity, bool renderOptionalContent) where T : EntityCodeTemplate {
            this.CopyPropertiesTo(template, true, PropertyIgnoreList);
            template.Entity = entity;
            template.RenderOptionalContent = renderOptionalContent;
            template.Render(this.Response);
        }

        #endregion
    }
}