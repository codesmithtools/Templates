using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CodeSmith.Engine;
using CodeSmith.SchemaHelper.Util;

namespace CodeSmith.SchemaHelper.NHibernate
{
    public static class NHibernateUtilities
    {
        public static readonly string[] AssociationDefaultAttributes = new[] { "name", "table", "class", "inverse", NotNull };
        private static Regex _nonCharRegex = new Regex(@"\W", RegexOptions.Compiled);

        private static MapCollection _toNHibernateTypeMap;
        private static MapCollection _fromNHibernateTypeMap;
        private static MapCollection _fromNHibernateNullableTypeMap;

        public const string MapExtension = ".hbm.xml";
        public const string FileName = "file-name";
        public const string NHibernateType = "nhibernate-type";
        public const string GeneratorClass = "generator-class";
        public const string Length = "length";
        public const string Lazy = "lazy";
        public const string Cascade = "cascade";
        public const string Generated = "generated";
        public const string UnsavedValue = "unsaved-value";
        public const string NotNull = "not-null";

        public static void Merge(EntityManager destinationManager, EntityManager sourceManager, string defaultNamespace)
        {
            foreach (var entity in destinationManager.Entities)
                PrepDestination(entity, defaultNamespace);

            foreach (var sourceEntity in sourceManager.Entities)
            {
                var destinationEntity = destinationManager.Entities.FirstOrDefault(e => IsMatchingEntity(e, sourceEntity));
                if (destinationEntity == null)
                    continue;

                Merge(destinationEntity, sourceEntity);
            }
        }

        private static bool IsMatchingEntity(IEntity destination, IEntity source)
        {
            string destinationName, sourceName;

            if (destination is CommandEntity)
            {
                destinationName = NamingConventions.CleanName(destination.EntityKeyName, false);
                sourceName = source.EntityKeyName;
            }
            else
            {
                destinationName = destination.GetSafeName();
                sourceName = source.GetSafeName();
            }

            return destinationName == sourceName;
        }

        private static void PrepDestination(IEntity entity, string defaultNamespace)
        {
            entity.ExtendedProperties.Clear();
            entity.Properties.ForEach(p => p.ExtendedProperties.Clear());

            entity.Namespace = defaultNamespace;
            
            entity.ExtendedProperties.Add(Lazy, "true");
            entity.ExtendedProperties.Add(FileName, entity is CommandEntity
                ? NamingConventions.CleanName(entity.EntityKeyName, false)
                : entity.Name);

            foreach (var property in entity.Key.Properties)
            {
                PrepProperty(property);

                property.ExtendedProperties.Add(GeneratorClass, entity.Key.IsIdentity ? "native" : "assigned");
            }

            foreach (var property in entity.Properties)
            {
                PrepProperty(property);

                var schemaProperty = property as ISchemaProperty;
                if (schemaProperty != null && schemaProperty.IsRowVersion)
                {
                    property.ExtendedProperties.Add(Generated, "always");
                    property.ExtendedProperties.Add(UnsavedValue, "null");
                }
            }

            foreach (var searchCriteria in entity.SearchCriteria)
                foreach (var property in searchCriteria.Properties)
                    PrepProperty(property);

            foreach (var association in entity.Associations)
                PrepAssociation(association);
        }

        private static void PrepProperty(IProperty property)
        {
            var nhibType = ToNHibernateType(property);
            property.ExtendedProperties[NHibernateType] = nhibType;
            property.ExtendedProperties[NotNull] = !property.IsNullable ? "true" : "false";
            
            if (nhibType.EndsWith("String") && property.Size > 0)
                property.ExtendedProperties[Length] = property.Size;
        }

        private static void PrepAssociation(Association assocication)
        {
            bool isNullable;

            switch (assocication.AssociationType)
            {
                case AssociationType.ManyToOne:
                case AssociationType.ManyToZeroOrOne:

                    isNullable = assocication.Properties.All(ap => ap.Property.IsNullable);
                    assocication.ExtendedProperties.Add(NotNull, !isNullable ? "true" : "false");

                    break;

                case AssociationType.OneToMany:
                case AssociationType.ZeroOrOneToMany:

                    isNullable = assocication.Properties.Any(ap => ap.Property.IsNullable);
                    assocication.ExtendedProperties.Add(Cascade, isNullable ? "all" : "all-delete-orphan");
                    assocication.ExtendedProperties.Add(Lazy, "true");

                    break;

                case AssociationType.ManyToMany:

                    assocication.ExtendedProperties.Add(Cascade, "all");
                    assocication.ExtendedProperties.Add(Lazy, "true");

                    break;
            }
        }

        private static void Merge(IEntity destinationEntity, IEntity sourceEntity)
        {
            if (sourceEntity == null)
                return;

            destinationEntity.SetName(sourceEntity.Name);
            destinationEntity.Namespace = sourceEntity.Namespace;
            foreach(var pair in sourceEntity.ExtendedProperties)
                destinationEntity.ExtendedProperties.AddOrSet(pair);

            Merge(destinationEntity.Properties, sourceEntity.Properties);
            Merge(destinationEntity.Key.Properties, sourceEntity.Key.Properties);
            Merge(destinationEntity.Associations, sourceEntity.Associations);
        }

        private static void Merge(IEnumerable<IProperty> destinationProperties, IEnumerable<IProperty> sourceProperties)
        {
            foreach (var sourceProperty in sourceProperties)
            {
                var sourceColumn = sourceProperty.GetSafeName();

                var destinationProperty = destinationProperties.FirstOrDefault(p => p.GetSafeName() == sourceColumn);
                if (destinationProperty == null)
                    continue;

                destinationProperty.SetName(sourceProperty.Name);
                foreach (var pair in sourceProperty.ExtendedProperties)
                    destinationProperty.ExtendedProperties.AddOrSet(pair);
            }
        }

        private static void Merge(IEnumerable<Association> destinationAssociations, IEnumerable<Association> sourceAssociations)
        {
            foreach (var sourceAssociation in sourceAssociations)
            {
                var destinationAssociation = destinationAssociations.FirstOrDefault(a => IsAssociationMatch(a, sourceAssociation));
                if (destinationAssociation == null)
                    continue;

                destinationAssociation.Name = sourceAssociation.Name;
                foreach (var pair in sourceAssociation.ExtendedProperties)
                    destinationAssociation.ExtendedProperties.AddOrSet(pair);
            }
        }

        private static bool IsAssociationMatch(Association destination, Association source)
        {
            if (destination.ForeignEntity.Name != source.ForeignEntity.Name)
                return false;

            return GetAssociationKey(destination) == GetAssociationKey(source);
        }

        private static string GetAssociationKey(Association association)
        {
            IEnumerable<string> columns;
            string type;

            switch(association.AssociationType)
            {
                case AssociationType.ManyToOne:
                case AssociationType.ManyToZeroOrOne:
                    columns = association.Properties.Select(p => p.Property.GetSafeName());
                    type = "ManyTo";
                    break;

                case AssociationType.ManyToMany:
                case AssociationType.OneToMany:
                case AssociationType.ZeroOrOneToMany:
                    columns = association.Properties.Select(p => p.ForeignProperty.GetSafeName());
                    type = "ToMany";
                    break;

                default:
                    return String.Empty;
            }

            return columns.Aggregate(String.Empty, (current, column) => String.Concat(current, column, "|")) + type;
        }

        public static bool IsIgnoredCommand(CommandEntity command)
        {
            return command.Properties.Any(p => _nonCharRegex.IsMatch(p.KeyName));
        }

        public static string ToNHibernateType(IProperty property)
        {
            var schemaProperty = (ISchemaProperty)property;
            var dbType = schemaProperty.DataType.ToString();

            var nhibernateType = ToNHibernateTypeMap.ContainsKey(dbType)
                ? ToNHibernateTypeMap[dbType]
                : "String";

            if (property.Size > 1)
            {
                if (nhibernateType == "AnsiChar")
                    nhibernateType = "AnsiString";
                else if (nhibernateType == "Char")
                    nhibernateType = "String";
            }

            return nhibernateType;
        }

        private static MapCollection ToNHibernateTypeMap
        {
            get
            {
                if (_toNHibernateTypeMap == null)
                {
                    string path;
                    if (!Map.TryResolvePath("DbTypeToNHibernate", String.Empty, out path) && TemplateContext.Current != null)
                    {
                        // If the mapping file wasn't found in the maps folder than look it up in the common folder.
                        string baseDirectory = Path.GetFullPath(Path.Combine(TemplateContext.Current.RootCodeTemplate.CodeTemplateInfo.DirectoryName, @"..\..\Common"));
                        if (!Map.TryResolvePath("DbTypeToNHibernate", baseDirectory, out path))
                        {
                            baseDirectory = Path.Combine(TemplateContext.Current.RootCodeTemplate.CodeTemplateInfo.DirectoryName, "Common");
                            Map.TryResolvePath("DbTypeToNHibernate", baseDirectory, out path);
                        }
                    }

                    // Prevents a NullReferenceException from occurring.
                    _toNHibernateTypeMap = File.Exists(path) ? Map.Load(path) : new MapCollection();
                }

                return _toNHibernateTypeMap;
            }
        }

        public static string FromNHibernateType(string nhibernateType, int? length)
        {
            var systemType = FromNHibernateTypeMap.ContainsKey(nhibernateType)
                ? FromNHibernateTypeMap[nhibernateType]
                : "System.String";

            if (length.HasValue && length > 1)
            {
                if (systemType == "System.Char")
                    return "System.String";
            }

            return systemType;
        }

        private static MapCollection FromNHibernateTypeMap
        {
            get
            {
                if (_fromNHibernateTypeMap == null)
                {
                    string path;
                    if (!Map.TryResolvePath("NHibernateToSystemType", String.Empty, out path) && TemplateContext.Current != null)
                    {
                        // If the mapping file wasn't found in the maps folder than look it up in the common folder.
                        string baseDirectory = Path.GetFullPath(Path.Combine(TemplateContext.Current.RootCodeTemplate.CodeTemplateInfo.DirectoryName, @"..\..\Common"));
                        if (!Map.TryResolvePath("NHibernateToSystemType", baseDirectory, out path))
                        {
                            baseDirectory = Path.Combine(TemplateContext.Current.RootCodeTemplate.CodeTemplateInfo.DirectoryName, "Common");
                            Map.TryResolvePath("NHibernateToSystemType", baseDirectory, out path);
                        }
                    }

                    // Prevents a NullReferenceException from occurring.
                    _fromNHibernateTypeMap = File.Exists(path) ? Map.Load(path) : new MapCollection();
                }

                return _fromNHibernateTypeMap;
            }
        }

        public static string FromNHibernateNullableType(string nhibernateType, int? length)
        {
            var systemType = FromNHibernateNullableTypeMap.ContainsKey(nhibernateType)
                ? FromNHibernateNullableTypeMap[nhibernateType]
                : "System.String";

            if (length.HasValue && length > 1)
            {
                if (systemType == "System.Char?")
                    return "System.String";
            }

            return systemType;
        }

        private static MapCollection FromNHibernateNullableTypeMap
        {
            get
            {
                if (_fromNHibernateNullableTypeMap == null)
                {
                    string path;
                    if (!Map.TryResolvePath("NHibernateToNullableType", String.Empty, out path) && TemplateContext.Current != null)
                    {
                        // If the mapping file wasn't found in the maps folder than look it up in the common folder.
                        string baseDirectory = Path.GetFullPath(Path.Combine(TemplateContext.Current.RootCodeTemplate.CodeTemplateInfo.DirectoryName, @"..\Common"));
                        if (!Map.TryResolvePath("NHibernateToNullableType", baseDirectory, out path))
                        {
                            baseDirectory = Path.Combine(TemplateContext.Current.RootCodeTemplate.CodeTemplateInfo.DirectoryName, "Common");
                            Map.TryResolvePath("NHibernateToNullableType", baseDirectory, out path);
                        }
                    }

                    // Prevents a NullReferenceException from occurring.
                    _fromNHibernateNullableTypeMap = File.Exists(path) ? Map.Load(path) : new MapCollection();
                }

                return _fromNHibernateNullableTypeMap;
            }
        }
    }
}