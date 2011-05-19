//------------------------------------------------------------------------------
//
// Copyright (c) 2002-2011 CodeSmith Tools, LLC.  All rights reserved.
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
using System.Linq;
using System.Text;
using LinqToEdmx.Designer;

namespace Generator.Microsoft.Frameworks
{
    public partial class EdmxGenerator
    {
        private bool _createdMappingDesigner;

        /// <summary>
        /// 2.2: Create Designer.
        /// </summary>
        private void CreateDesigner()
        {
            if (_createdMappingDesigner) return;
            _createdMappingDesigner = true;

            var designer = Designer;

            if (designer.Connection == null)
            {
                designer.Connection = new Connection()
                {
                    DesignerInfoPropertySet = new DesignerInfoPropertySet()
                    {
                        DesignerProperties = new List<DesignerProperty>()
                                                         {
                                                             new DesignerProperty() { Name = "MetadataArtifactProcessing", Value = "EmbedInOutputAssembly" }
                                                         }
                    }
                };
            }

            if (designer.Options == null)
            {
                designer.Options = new Options()
                {
                    DesignerInfoPropertySet = new DesignerInfoPropertySet()
                    {
                        DesignerProperties = new List<DesignerProperty>()
                                                         {
                                                             new DesignerProperty() { Name = "ValidateOnBuild", Value = "True" },
                                                             new DesignerProperty() { Name = "EnablePluralization", Value = "False" },
                                                             new DesignerProperty() { Name = "IncludeForeignKeysInModel", Value = "True" },
                                                             new DesignerProperty() { Name = "CodeGenerationStrategy", Value = "None" },
                                                             new DesignerProperty() { Name = EdmxConstants.ContextNamespace, Value = _settings.ContextNamespace },
                                                             new DesignerProperty() { Name = EdmxConstants.EntityNamespace, Value = _settings.EntityNamespace }
                                                         }
                    }
                };
            }

            if (designer.Diagrams == null)
            {
                designer.Diagrams = new Diagrams() { Diagram = new List<Diagram>() { new Diagram() { Name = "Model1" } } };
            }
        }

        #region Methods

        private string GetDesignerProperty(string key)
        {
            if (!_createdMappingDesigner)
                CreateDesigner();

            if (Designer.Options.DesignerInfoPropertySet.DesignerProperties.Count(p => p.Name.Equals(key)) > 0)
                return Designer.Options.DesignerInfoPropertySet.DesignerProperties.Where(p => p.Name.Equals(key)).First().Value;

            return null;
        }

        private void SetDesignerProperty(string key, string value)
        {
            if (GetDesignerProperty(key) == null)
                Designer.Options.DesignerInfoPropertySet.DesignerProperties.Add(new DesignerProperty() { Name = key, Value = value });
            else
            {
                var property = Designer.Options.DesignerInfoPropertySet.DesignerProperties.Where(p => p.Name.Equals(key)).First();
                property.Value = value;
            }
        }

        #endregion

        private Designer Designer
        {
            get
            {
                if (_edmx.Designers.Count == 0)
                {
                    _edmx.Designers.Add(new Designer());
                }

                return _edmx.Designers.First();
            }
        }
    }
}
