using System;
using System.Data;
using System.Linq;

namespace CodeSmith.SchemaHelper
{
    /// <summary>
    /// Extension Methods for MemberExtensions
    /// </summary>
    public static class MemberExtensions
    {
        public static bool IsDbType(this IProperty property, DbType type)
        {
            var schemaProperty = property as ISchemaProperty;
            if (schemaProperty != null)
                return schemaProperty.DataType == type;

            switch (type)
            {
                case DbType.Guid:
                    return property.BaseSystemType == "System.Guid";
                case DbType.Int16:
                    return property.BaseSystemType == "System.Int16";
                case DbType.Int32:
                    return property.BaseSystemType == "System.Int32";
                case DbType.Int64:
                    return property.BaseSystemType == "System.Int64";
            }

            return false;
        }

        public static string BuildDataObjectField(this IProperty property)
        {
            return property.BuildDataObjectField(false);
        }

        public static string BuildDataObjectField(this IProperty property, bool isSilverlight)
        {
            if (property.IsType(PropertyType.Key))
            {
                if(isSilverlight)
                    return String.Format("{1}#if !SILVERLIGHT{1}        [System.ComponentModel.DataObjectField(true, {0})]{1}#endif", property.IsType(PropertyType.Identity).ToString().ToLower(), Environment.NewLine);

                return String.Format("{1}        [System.ComponentModel.DataObjectField(true, {0})]", property.IsType(PropertyType.Identity).ToString().ToLower(), Environment.NewLine);
            }

            return String.Empty;
        }

        /// <summary>
        /// This is used in the Insert and Update methods to figure out the association for the a fk property.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static string ResolveAssociationPropertyVariable(this IProperty property)
        {
            foreach (Association association in property.Entity.Associations.Where(a => a.AssociationType == AssociationType.ManyToOne || a.AssociationType == AssociationType.ManyToZeroOrOne))
            {
                foreach (AssociationProperty associationProperty in association.Properties)
                {
                    if (property.KeyName == associationProperty.Property.KeyName && property == associationProperty.Property)
                    {
                        var className = associationProperty.ForeignProperty.Entity.VariableName;
                        if (className.Equals("item", StringComparison.InvariantCultureIgnoreCase))
                            className += "1";

                        return className;
                    }
                }
            }

            return String.Empty;
        }

        /// <summary>
        /// This is used in the AddNewCore methods to figure out the association for the a fk property.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static string ResolveAssociationPropertyClassName(this IProperty property)
        {
            foreach (Association association in property.Entity.Associations.Where(a => a.AssociationType == AssociationType.ManyToOne || a.AssociationType == AssociationType.ManyToZeroOrOne))
            {
                foreach (AssociationProperty associationProperty in association.Properties)
                {
                    if (property.KeyName == associationProperty.Property.KeyName  && property == associationProperty.Property)
                    {
                        return associationProperty.ForeignProperty.Entity.Name;
                    }
                }
            }

            return String.Empty;
        }

        /// <summary>
        /// This is used in the Insert and Update methods to figure out the association for the a fk property.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static string ResolveAssociationPropertyVariableWithChildProperty(this IProperty property)
        {
            foreach (Association association in property.Entity.Associations.Where(a => a.AssociationType == AssociationType.ManyToOne || a.AssociationType == AssociationType.ManyToZeroOrOne))
            {
                foreach (AssociationProperty associationProperty in association.Properties)
                {
                    if (property.KeyName == associationProperty.Property.KeyName && property == associationProperty.Property)
                    {
                        var className = associationProperty.ForeignProperty.Entity.VariableName;
                        if (className.Equals("item", StringComparison.InvariantCultureIgnoreCase))
                            className += "1";

                        return String.Format("{0}.{1}", className, associationProperty.ForeignProperty.Name);
                    }
                }
            }

            return String.Empty;
        }
    }
}