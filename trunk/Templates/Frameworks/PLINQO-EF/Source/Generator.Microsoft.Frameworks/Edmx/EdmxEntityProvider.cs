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
using System.Diagnostics;
using System.Linq;

using CodeSmith.SchemaHelper;
using LinqToEdmx;
using LinqToEdmx.Model.Conceptual;
using Association = LinqToEdmx.Model.Conceptual.Association;

namespace Generator.Microsoft.Frameworks
{
    public class EdmxEntityProvider : IEntityProvider
    {
        #region Members

        private const string EdmxExtension = ".edmx";
        private string _fileName = String.Empty;

        #endregion

        #region Constructors

        public EdmxEntityProvider(string fileName)
        {
            _fileName = fileName;
        }

        #endregion

        #region Methods

        #region Provider Methods

        public bool Validate()
        {
            if (string.IsNullOrEmpty(_fileName))
                return false;

            _fileName = System.IO.Path.GetFullPath(_fileName);
            if (!System.IO.File.Exists(_fileName)) return false;

            if (!System.IO.Path.HasExtension(_fileName) || !EdmxExtension.Equals(System.IO.Path.GetExtension(_fileName), StringComparison.InvariantCultureIgnoreCase))
                return false;

            Edmx edmx = null;

            try
            {
                edmx = Edmx.Load(_fileName);
            }
            catch (Exception)
            {
            }

            return edmx != null;
        }

        public void Load()
        {
            if (!Validate()) return;

            var edmx = Edmx.Load(_fileName);

            var conceptualNamespace = edmx.GetItems<ConceptualSchema>().First().Namespace;
            EntityNamespace = GetDesignerProperty(edmx, EdmxConstants.EntityNamespace) ?? conceptualNamespace;
            ContextNamespace = GetDesignerProperty(edmx, EdmxConstants.ContextNamespace) ?? EntityNamespace;

            DataContextName = edmx.GetItems<EntityContainer>().First().Name;
            LazyLoadingEnabled = edmx.GetItems<EntityContainer>().First().LazyLoadingEnabled ?? false;

            CreateEntityTypes(edmx, conceptualNamespace);
            CreateEntityFunctions(edmx, conceptualNamespace);
            CreateComplexTypes(edmx, conceptualNamespace);
        }

        public void Save()
        {
        }

        #endregion

        #region Helpers

        private static void CreateEntityTypes(Edmx edmx, string conceptualNamespace)
        {
            var associations = edmx.GetItems<Association>();
            var associationSets = edmx.GetItems<EntityContainer.AssociationSetLocalType>();

            // First loop through the EntityTypes and populate the EntityCollection with the Entity / Columns.
            foreach (var entity in edmx.GetItems<EntityType>())
            {
                if (entity == null)
                    continue;

                if (Configuration.Instance.ExcludeRegexIsMatch(entity.Name))
                {
                    Trace.WriteLine(String.Format("Skipping EntityType: '{0}', the EntityType was excluded!", entity.Name));

                    EntityStore.Instance.ExcludedEntityCollection.Add(entity.Name, new ConceptualEntity(entity, conceptualNamespace));
                    continue;
                }

                EntityStore.Instance.EntityCollection.Add(entity.Name, new ConceptualEntity(entity, associations, associationSets, conceptualNamespace));
            }
        }

        private static void CreateEntityFunctions(Edmx edmx, string conceptualNamespace)
        {
            // First loop through the Functions and populate the EntityCollection with the Entity / Columns.
            foreach (var entity in edmx.GetItems<EntityContainer.FunctionImportLocalType>())
            {
                if (entity == null)
                    continue;

                if (Configuration.Instance.ExcludeRegexIsMatch(entity.Name))
                {
                    Trace.WriteLine(String.Format("Skipping EntityType: '{0}', the EntityType was excluded!", entity.Name));

                    EntityStore.Instance.ExcludedEntityCollection.Add(entity.Name, new FunctionEntity(entity, conceptualNamespace));
                    continue;
                }

                EntityStore.Instance.EntityCollection.Add(entity.Name, new FunctionEntity(entity, conceptualNamespace));
            }
        }

        private static void CreateComplexTypes(Edmx edmx, string conceptualNamespace)
        {
            // First loop through the EntityTypes and populate the EntityCollection with the Entity / Columns.
            foreach (var entity in edmx.GetItems<ComplexType>())
            {
                if (entity == null)
                    continue;

                if (Configuration.Instance.ExcludeRegexIsMatch(entity.Name))
                {
                    Trace.WriteLine(String.Format("Skipping EntityType: '{0}', the EntityType was excluded!", entity.Name));

                    EntityStore.Instance.ExcludedEntityCollection.Add(entity.Name, new ComplexEntity(entity, conceptualNamespace));
                    continue;
                }

                EntityStore.Instance.EntityCollection.Add(entity.Name, new ComplexEntity(entity, conceptualNamespace));
            }
        }

        private static string GetDesignerProperty(Edmx edmx, string key)
        {
            var hasDesignerProperties = edmx.Designers != null && edmx.Designers.First().Options != null &&
                                        edmx.Designers.First().Options.DesignerInfoPropertySet != null &&
                                        edmx.Designers.First().Options.DesignerInfoPropertySet.DesignerProperties != null;

            if (hasDesignerProperties && edmx.Designers.First().Options.DesignerInfoPropertySet.DesignerProperties.Count(p => p.Name.Equals(key)) > 0)
                return edmx.Designers.First().Options.DesignerInfoPropertySet.DesignerProperties.Where(p => p.Name.Equals(key)).First().Value;

            return null;
        }

        #endregion

        #endregion

        #region Properties

        public string Name
        {
            get
            {
                return "Entity Framework Entity Provider";
            }
        }

        public string Description
        {
            get
            {
                return "Entity Framework 4 Provider";
            }
        }

        public string EntityNamespace { get; private set; }

        public string ContextNamespace { get; private set; }

        public string DataContextName { get; private set; }

        public bool LazyLoadingEnabled { get; private set; }

        #endregion

    }
}