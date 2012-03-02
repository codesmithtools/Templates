using System;

namespace CodeSmith.SchemaHelper
{
    /// <summary>
    /// Extension Methods for SearchCriteriaExtensions
    /// </summary>
    public static class SearchCriteriaExtensions
    {
        public static string BuildObjectInitializer(this SearchCriteria sc)
        {
            return sc.BuildObjectInitializer(false);
        }

        public static string BuildObjectInitializer(this SearchCriteria sc, bool isObjectFactory)
        {
            string parameters = String.Empty;

            if (sc.SearchCriteriaType == SearchCriteriaType.ForeignKey)
            {
                foreach (AssociationProperty property in sc.ForeignProperties)
                {
                    var propertyName = isObjectFactory ? String.Format("item.{0}", property.Property.Name) : property.Property.VariableName;
                    var resolvedPropertyName = property.Property.Name;
                    if(isObjectFactory)
                    {
                        //if (sc.IsChild)
                        //{
                        //    resolvedPropertyName = property.Property.Name;
                        //    var nullable = property.ForeignProperty.IsNullable && property.ForeignProperty.SystemType != "System.String" ? ".Value" : String.Empty;
                        //    propertyName = string.Format("item.{0}{1}", property.ForeignProperty.Name, nullable);
                        //}
                        //else
                        //{
                            resolvedPropertyName = property.ForeignProperty.Name;
                            var nullable = property.Property.IsNullable && property.Property.SystemType != "System.String" && property.Property.SystemType != "System.Byte()" ? ".Value" : String.Empty;
                            propertyName = string.Format("item.{0}{1}", property.Property.Name, nullable);
                        //}
                    }

                    parameters += string.Format("\r\n        {0} = {1}", resolvedPropertyName, propertyName);
                }
            }
            else
            {
                #region Handle anything not a ForeignKey.

                foreach (IProperty property in sc.Properties)
                {
                    var propertyName = isObjectFactory ? String.Format("item.{0}", property.Name) : property.VariableName;
                    parameters += String.Format("\r\n        {0} = {1}{2}", property.Name, propertyName, property.IsNullable && property.SystemType != "System.String" && property.SystemType != "System.Byte()" ? ".Value" : String.Empty);
                }

                #endregion
            }

            return parameters.TrimStart(new[] { '\r', '\n' });
        }

        public static string BuildUpdateStatements(this SearchCriteria sc, string associationPropertyName)
        {
            return sc.BuildUpdateStatements(associationPropertyName, "item.");
        }

        public static string BuildUpdateStatements(this SearchCriteria sc, string associationPropertyName, string prefix)
        {
            string parameters = String.Empty;

            if (sc.SearchCriteriaType == SearchCriteriaType.ForeignKey)
            {
                foreach (AssociationProperty property in sc.Properties)
                {
                    var propertyName = property.Property.Name;
                    var resolvedPropertyName = property.Property.Name;

                    //if (sc.IsChild)
                    //{
                    //    var nullable = property.ForeignProperty.IsNullable && property.ForeignProperty.SystemType != "System.String" ? ".Value" : String.Empty;
                    //    resolvedPropertyName = property.Property.Name;
                    //    propertyName = String.Format("{0}{1}", property.ForeignProperty.Name, nullable);
                    //}
                    //else
                    //{
                        var nullable = property.Property.IsNullable && property.Property.SystemType != "System.String" && property.Property.SystemType != "System.Byte[]" ? ".Value" : String.Empty;
                        resolvedPropertyName = property.ForeignProperty.Name;
                        propertyName = String.Format("{0}{1}", property.Property.Name, nullable);
                    //}

                    parameters += String.Format("\r\n        {0}.{1} = {2}{3}", associationPropertyName, resolvedPropertyName, prefix, propertyName);
                }
            }
            else
            {
                foreach (IProperty property in sc.Properties)
                {
                    parameters += String.Format("\r\n        {0}.{1} = {1}{2}", associationPropertyName, property.Name, prefix);
                }
            }

            return parameters.TrimStart(new[] { '\r', '\n' });
        }
    }
}
