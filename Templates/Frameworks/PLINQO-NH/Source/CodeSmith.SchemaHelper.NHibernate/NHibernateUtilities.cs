using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using CodeSmith.Engine;

namespace CodeSmith.SchemaHelper.NHibernate
{
    public static class NHibernateUtilities
    {
        public static readonly string[] AssociationDefaultAttributes = new[] { "name", "table", "class", "inverse" };

        private static MapCollection _toNHibernateTypeMap;
        private static MapCollection _fromNHibernateTypeMap;

        public const string MapExtension = ".hbm.xml";
        public const string FileName = "file-name";
        public const string NHibernateType = "nhibernate-type";
        public const string GeneratorClass = "generator-class";
        public const string Length = "length";
        public const string Lazy = "lazy";
        public const string Cascade = "cascade";
        public const string Generated = "generated";
        public const string UnsavedValue = "unsaved-value";

        public static void Merge(EntityManager destinationManager, EntityManager sourceManager, string defaultNamespace)
        {
            foreach (var entity in destinationManager.Entities)
                PrepDestination(entity, defaultNamespace);

            foreach (var sourceEntity in sourceManager.Entities)
            {
                var safeTable = sourceEntity.GetSafeName();

                var destinationEntity = destinationManager.Entities.FirstOrDefault(e => e.GetSafeName() == safeTable);
                if (destinationEntity == null)
                    continue;

                Merge(destinationEntity, sourceEntity);
            }
        }

        private static void PrepDestination(IEntity entity, string defaultNamespace)
        {
            entity.Namespace = defaultNamespace;

            entity.ExtendedProperties.Add(FileName, entity.Name);
            entity.ExtendedProperties.Add(Lazy, "true");

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
            
            if (nhibType.EndsWith("String") && property.Size > 0)
                property.ExtendedProperties[Length] = property.Size;
        }

        private static void PrepAssociation(Association assocication)
        {
            switch (assocication.AssociationType)
            {
                case AssociationType.ManyToOne:
                case AssociationType.ManyToZeroOrOne:

                    break;

                case AssociationType.OneToMany:
                case AssociationType.ZeroOrOneToMany:

                    var isNullable = assocication.Properties.Any(ap => ap.Property.IsNullable);
                    assocication.ExtendedProperties.Add(Cascade, isNullable ? "all" : "all-delete-orphan");
                    assocication.ExtendedProperties.Add(Lazy, "true");

                    break;

                case AssociationType.ManyToMany:

                    assocication.ExtendedProperties.Add(Cascade, "all");
                    assocication.ExtendedProperties.Add(Lazy, "true");

                    break;

                default:

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

            switch(association.AssociationType)
            {
                case AssociationType.ManyToOne:
                case AssociationType.ManyToZeroOrOne:
                    columns = association.Properties.Select(p => p.Property.GetSafeName());
                    break;

                case AssociationType.ManyToMany:
                case AssociationType.OneToMany:
                case AssociationType.ZeroOrOneToMany:
                    columns = association.Properties.Select(p => p.ForeignProperty.GetSafeName());
                    break;

                default:
                    return String.Empty;
            }

            return columns.Aggregate(String.Empty, (current, column) => String.Concat(current, column, "|"));
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
                    if (!Map.TryResolvePath("DbTypeToNHibernate", String.Empty, out path))
                    {
                        // If the mapping file wasn't found in the maps folder than look it up in the common folder.
                        var baseDirectory = new DirectoryInfo(Assembly.GetExecutingAssembly().Location).Parent.FullName;
                        Map.TryResolvePath("DbTypeToNHibernate", baseDirectory, out path);
                    }

                    _toNHibernateTypeMap = Map.Load(path);
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
                    if (!Map.TryResolvePath("NHibernateToSystemType", String.Empty, out path))
                    {
                        // If the mapping file wasn't found in the maps folder than look it up in the common folder.
                        var baseDirectory = new DirectoryInfo(Assembly.GetExecutingAssembly().Location).Parent.FullName;
                        Map.TryResolvePath("NHibernateToSystemType", baseDirectory, out path);
                    }

                    _fromNHibernateTypeMap = Map.Load(path);
                }

                return _fromNHibernateTypeMap;
            }
        }
    }
}