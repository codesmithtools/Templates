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
using System.Text.RegularExpressions;
using CodeSmith.CustomProperties;
using CodeSmith.Engine;
using CodeSmith.SchemaHelper;
using Configuration = CodeSmith.SchemaHelper.Configuration;

namespace Generator.CSLA.CodeTemplates {
    /// <summary>
    /// The base class for all of the CSLA templates.
    /// </summary>
    public class CSLABaseTemplate : CodeTemplate {
        private StringCollection _ignoreExpressions = new StringCollection();
        private StringCollection _includeExpressions = new StringCollection();
        private StringCollection _cleanExpressions = new StringCollection();

        public CSLABaseTemplate() {
            ResolveTargetLanguage();

            // Preserve Backwards Compatibility.
            Configuration.Instance.IncludeManyToManyAssociations = false;
            Configuration.Instance.SearchCriteriaProperty.MethodKeySuffix = String.Empty;
        }

        #region 0. Naming

        /// <summary>
        /// List of regular expressions to clean table, view and column names.
        /// </summary>
        [Category("0. Naming")]
        [Description("List of regular expressions to clean table, view and column names.")]
        [Optional]
        [DefaultValue("^(sp|tbl|udf|vw)_")]
        public StringCollection CleanExpressions {
            get {
                return _cleanExpressions;
            }
            set {
                if (value == null || String.Equals(_cleanExpressions.ToString(), value.ToString()))
                    return;

                Configuration.Instance.CleanExpressions = new List<Regex>();
                _cleanExpressions = value ?? new StringCollection();
                foreach (string clean in _cleanExpressions) {
                    if (!String.IsNullOrEmpty(clean)) {
                        Configuration.Instance.CleanExpressions.Add(new Regex(clean, RegexOptions.IgnoreCase));
                    }
                }

                RefreshDataSource();
            }
        }

        /// <summary>
        /// List of regular expressions to ignore tables when generating.
        /// </summary>
        [Category("0. Naming")]
        [Description("List of regular expressions to ignore tables when generating.")]
        [Optional]
        [DefaultValue("sysdiagrams$")]
        public StringCollection IgnoreExpressions {
            get {
                return _ignoreExpressions;
            }
            set {
                if (value == null || String.Equals(_ignoreExpressions.ToString(), value.ToString()))
                    return;

                Configuration.Instance.IgnoreExpressions = new List<Regex>();
                _ignoreExpressions = value ?? new StringCollection();

                foreach (string ignore in _ignoreExpressions) {
                    if (!String.IsNullOrEmpty(ignore)) {
                        Configuration.Instance.IgnoreExpressions.Add(new Regex(ignore, RegexOptions.IgnoreCase));
                    }
                }

                RefreshDataSource();
            }
        }

        /// <summary>
        /// List of regular expressions to include tables and views when generating mapping.
        /// </summary>
        [Category("0. Naming")]
        [Description("List of regular expressions to include tables and views when generating mapping.")]
        [Optional]
        public StringCollection IncludeExpressions {
            get {
                return _includeExpressions;
            }
            set {
                if (value == null || String.Equals(_includeExpressions.ToString(), value.ToString()))
                    return;

                Configuration.Instance.IncludeExpressions = new List<Regex>();
                _includeExpressions = value ?? new StringCollection();

                foreach (string include in _includeExpressions) {
                    if (!String.IsNullOrEmpty(include)) {
                        Configuration.Instance.IncludeExpressions.Add(new Regex(include, RegexOptions.IgnoreCase));
                    }
                }

                RefreshDataSource();
            }
        }

        /// <summary>
        /// Controls the naming conventions.
        /// </summary>
        [Category("0. Naming")]
        [Description("Controls the naming conventions.")]
        [Optional]
        public NamingProperty NamingConventions {
            get {
                return Configuration.Instance.NamingProperty;
            }
            set {
                if (value == null)
                    return;

                Configuration.Instance.NamingProperty = value;
                RefreshDataSource();
            }
        }

        /// <summary>
        /// Refreshes the  data source. This method is called any time a configuration property is changed.
        /// </summary>
        protected virtual void RefreshDataSource() { }

        #endregion

        #region 1. DataSource

        /// <summary>
        /// Includes Entity associations if set to true.
        /// </summary>
        [Optional]
        [Category("1. DataSource")]
        [Description("Includes Entity associations if set to true.")]
        public bool IncludeAssociations {
            get { return Configuration.Instance.IncludeAssociations; }
            set {
                if (Configuration.Instance.IncludeAssociations == value)
                    return;

                Configuration.Instance.IncludeAssociations = value;
                RefreshDataSource();
            }
        }

        #endregion


        [Browsable(false)]
        public string TemplateVersion {
            get {
                var fileName = typeof(CSLABaseTemplate).Assembly.Location;
                if (!String.IsNullOrEmpty(fileName))
                    return FileVersionInfo.GetVersionInfo(fileName).FileVersion;

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
            RegisterReference(IsCSLA38 ? Path.GetFullPath(Path.Combine(CodeTemplateInfo.DirectoryName, @"..\..\Common\Csla\3.8\Client\Csla.dll")) : Path.GetFullPath(Path.Combine(CodeTemplateInfo.DirectoryName, @"..\..\Common\Csla\4.3\Client\Csla.dll")));

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
            "SourceDatabase",
            "SourceTable",
            "SourceView",
            "SourceCommand",
            "NamingConventions",
            "CleanExpressions",
            "IgnoreExpressions",
            "IncludeExpressions",
            "IncludeAssociations",
            "IncludeViews",
            "IncludeFunctions"
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