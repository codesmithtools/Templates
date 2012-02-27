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
            string parameters = string.Empty;

            if (sc.SearchCriteriaType == SearchCriteriaType.ForeignKey)
            {
                foreach (AssociationProperty property in sc.ForeignProperties)
                {
                    var propertyName = isObjectFactory ? String.Format("item.{0}", property.Property.Name) : property.Property.VariableName;
                    var resolvedPropertyName = property.Property.Name;
                    if(isObjectFactory)
                    {
                        if (sc.IsChild)
                        {
                            if (property.ForeignProperty.IsNullable && property.ForeignProperty.SystemType != "System.String") continue;

                            resolvedPropertyName = property.Property.Name;
                            propertyName = String.Format("item.{0}", property.ForeignProperty.Name);
                        }
                        else
                        {
                            if (property.Property.IsNullable && property.Property.SystemType != "System.String" && property.Property.SystemType != "System.Byte[]") continue;

                            resolvedPropertyName = property.ForeignProperty.Name;
                            propertyName = String.Format("item.{0}", property.Property.Name);
                        }
                    }

                    parameters += String.Format(", {0} = {1}", resolvedPropertyName, propertyName);
                }
            }
            else
            {
                #region Handle anything not a ForeignKey.

                foreach (IProperty property in sc.Properties)
                {
                    if(property.IsNullable && property.SystemType != "System.String" && property.SystemType != "System.Byte[]") continue;

                    var propertyName = isObjectFactory ? String.Format("item.{0}", property.Name) : property.VariableName;
                    parameters += String.Format(", {0} = {1}", property.Name, propertyName);
                }

                #endregion
            }

            return parameters.TrimStart(new[] { ',', ' ' });
        }

        public static string BuildNullableObjectInitializer(this SearchCriteria sc)
        {
            return sc.BuildNullableObjectInitializer(false);
        }

        public static string BuildNullableObjectInitializer(this SearchCriteria sc, bool isObjectFactory)
        {
            string parameters = string.Empty;

            if (sc.SearchCriteriaType == SearchCriteriaType.ForeignKey)
            {
                foreach (AssociationProperty property in sc.Properties)
                {
                    var propertyName = isObjectFactory ? String.Format("item.{0}", property.Property.Name) : property.Property.VariableName;
                    var resolvedPropertyName = property.Property.Name;
                    if (isObjectFactory)
                    {
                        if (sc.IsChild)
                        {
                            if ((property.ForeignProperty.IsNullable && property.ForeignProperty.SystemType != "System.String") == false) continue;

                            resolvedPropertyName = property.Property.Name;
                            var nullable = property.ForeignProperty.IsNullable && property.ForeignProperty.SystemType != "System.String" ? ".Value" : string.Empty;
                            propertyName = String.Format("item.{0}{1}", property.ForeignProperty.Name, nullable);
                        }
                        else
                        {
                            if ((property.Property.IsNullable && property.Property.SystemType != "System.String" && property.Property.SystemType != "System.Byte[]") == false) continue;

                            resolvedPropertyName = property.ForeignProperty.Name;
                            propertyName = String.Format("item.{0}.Value", property.Property.Name);
                        }
                    }

                    parameters += String.Format("\r\n\t\t\t\tif({1}.HasValue) criteria.{0} = {1};", resolvedPropertyName, propertyName);
                }
            }
            else
            {
                #region Handle anything not a ForeignKey.

                foreach (IProperty property in sc.Properties)
                {
                    if((property.IsNullable && property.SystemType != "System.String" && property.SystemType != "System.Byte[]") == false) continue;

                    var propertyName = isObjectFactory ? String.Format("item.{0}", property.Name) : property.VariableName;
                    parameters += String.Format("\r\n\t\t\t\tif({1}.HasValue) criteria.{0} = {1}.Value;", property.Name, propertyName);
                }

                #endregion
            }

            return parameters.TrimStart(new[] { '\r', '\n', '\t' });
        }

        public static string BuildUpdateStatements(this SearchCriteria sc, string associationPropertyName)
        {
            return sc.BuildUpdateStatements(associationPropertyName, "item.");
        }

        public static string BuildUpdateStatements(this SearchCriteria sc, string associationPropertyName, string prefix)
        {
            string parameters = string.Empty;

            if (sc.SearchCriteriaType == SearchCriteriaType.ForeignKey)
            {
                foreach (AssociationProperty property in sc.Properties)
                {
                    var propertyName = property.Property.Name;
                    var resolvedPropertyName = property.Property.Name;

                    if (sc.IsChild)
                    {
                        var nullable = property.ForeignProperty.IsNullable && property.ForeignProperty.SystemType != "System.String" ? ".Value" : string.Empty;
                        resolvedPropertyName = property.Property.Name;
                        propertyName = String.Format("{0}{1}", property.ForeignProperty.Name, nullable);
                    }
                    else
                    {
                        var nullable = property.Property.IsNullable && property.Property.SystemType != "System.String" && property.Property.SystemType != "System.Byte[]" ? ".Value" : string.Empty;
                        resolvedPropertyName = property.ForeignProperty.Name;
                        propertyName = String.Format("{0}{1}", property.Property.Name, nullable);
                    }

                    parameters += String.Format("\r\n\t\t\t\t{0}.{1} = {2}{3};", associationPropertyName, resolvedPropertyName, prefix, propertyName);
                }
            }
            else
            {
                #region Handle anything not a ForeignKey.

                foreach (IProperty property in sc.Properties)
                {
                    parameters += String.Format("\r\n\t\t\t\t{0}.{1} = {1}{2};", associationPropertyName, property.Name, prefix);
                }

                #endregion
            }

            return parameters.TrimStart(new[] { '\r', '\n' });
        }
    }
}
