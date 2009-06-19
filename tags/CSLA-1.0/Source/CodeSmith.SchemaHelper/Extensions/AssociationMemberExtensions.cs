using System;
using System.Collections.Generic;
using System.Linq;
using CodeSmith.Engine;
using CodeSmith.SchemaHelper.Util;

namespace CodeSmith.SchemaHelper
{
    /// <summary>
    /// Extension Methods for AssociationMemberCollectionExtensions
    /// </summary>
    public static class AssociationMemberCollectionExtensions
    {
        public static List<SearchCriteria> ListSearchCriteria(this AssociationMember member)
        {
            return member.SearchCriteria.Where(sc => !sc.IsUniqueResult && sc.MethodName.Contains(member.Name)).ToList();
        }

        public static string BuildObjectInitializer(this AssociationMember member)
        {
            foreach (var memberBase in member.AssociationEntity().GetUniqueSearchCriteriaMembers())
            {
                if (memberBase.ColumnName == member.ColumnName)
                {
                    return string.Format("{0} = {1}", NamingConventions.PropertyName(member.ColumnName), NamingConventions.VariableName(member.ColumnName));
                }
            }

            var output = member.BuildCriteriaObjectInitializer(member.TableName);

            if(!output.StartsWith(member.Entity.ResolveCriteriaVariableName(member.ColumnName), StringComparison.InvariantCultureIgnoreCase))
                return string.Format("{0} = {1}", member.Entity.ResolveCriteriaPropertyName(member.ColumnName), member.Entity.ResolveCriteriaVariableName(member.ColumnName));

            return output;
        }

        public static string BuildParametersVariable(this AssociationMember member)
        {
            foreach (var memberBase in member.AssociationEntity().GetUniqueSearchCriteriaMembers())
            {
                if (memberBase.ColumnName == member.ColumnName)
                {
                    return string.Format("{0} {1}", member.SystemType, NamingConventions.VariableName(member.ColumnName));
                }
            }

            return member.BuildParametersVariablesCriteria(false);
        }


        public static string BuildOneToZeroOrZeroObjectInitializer(this AssociationMember member)
        {
            foreach (var memberBase in member.Entity.GetUniqueSearchCriteriaMembers())
            {
                if (memberBase.ColumnName == member.LocalColumn.Name)
                {
                    return string.Format("{0} = {1}", NamingConventions.PropertyName(member.LocalColumn.Name), NamingConventions.VariableName(member.LocalColumn.Name));
                }
            }

            var output = member.BuildCriteriaObjectInitializer(member.TableName);

            if (!output.StartsWith(member.Entity.ResolveCriteriaVariableName(member.LocalColumn.Name), StringComparison.InvariantCultureIgnoreCase))
                return string.Format("{0} = {1}", member.Entity.ResolveCriteriaPropertyName(member.LocalColumn.Name), member.Entity.ResolveCriteriaVariableName(member.LocalColumn.Name));

            return output;
        }

        public static string BuildOneToZeroOrZeroParametersVariable(this AssociationMember member)
        {
            foreach (var memberBase in member.AssociationEntity().GetUniqueSearchCriteriaMembers())
            {
                if (memberBase.ColumnName == member.ColumnName)
                {
                    return string.Format("{0} {1}", member.SystemType, NamingConventions.VariableName(member.LocalColumn.Name));
                }
            }

            return member.BuildParametersVariablesCriteria(false);
        }


        public static Entity AssociationEntity(this AssociationMember member)
        {
            return new Entity(member.Table);
        }

        public static List<SearchCriteria> AssociationEntityListSearchCriteria(this AssociationMember member)
        {
            return  member.AssociationEntity().SearchCriteria
                    .Where(sc =>
                        sc.MethodName.EndsWith(member.LocalColumn.Name) ||
                        sc.MethodName.EndsWith(member.LocalColumn.ColumnName) ||
                        sc.MethodName.EndsWith(member.Name) ||
                        sc.MethodName.EndsWith(member.ColumnName) && 
                        !sc.IsUniqueResult)
                    .ToList();
        }

        public static string ResolveManyToOneNameConflict(this AssociationMember member, Entity entity)
        {
            string propertyName = Util.NamingConventions.PropertyName(member.ColumnName);

            foreach (AssociationMember association in entity.ManyToOne)
            {
                if(association.PropertyName == propertyName)
                {
                    return entity.ResolveCriteriaColumnName(member.ColumnName);
                }
            }

            return member.ColumnName;
        }

        public static bool HasByteArrayColumn(this AssociationMember member)
        {
            if (member.Table.Columns.Contains(member.LocalColumn.Name))
                return DbTypeToDataReaderMethod[member.Table.Columns[member.LocalColumn.Name].DataType.ToString(), "GetValue"] == "GetBytes" || member.IsRowVersion;

            if (member.Table.Columns.Contains(member.ColumnName))
                return DbTypeToDataReaderMethod[member.Table.Columns[member.ColumnName].DataType.ToString(), "GetValue"] == "GetBytes" || member.IsRowVersion;

            if (member.AssociationEntity().Table.Columns.Contains(member.LocalColumn.Name))
                return DbTypeToDataReaderMethod[member.AssociationEntity().Table.Columns[member.LocalColumn.Name].DataType.ToString(), "GetValue"] == "GetBytes" || member.IsRowVersion;

            if (member.AssociationEntity().Table.Columns.Contains(member.ColumnName))
                return DbTypeToDataReaderMethod[member.AssociationEntity().Table.Columns[member.ColumnName].DataType.ToString(), "GetValue"] == "GetBytes" || member.IsRowVersion;

            return false;
        }

        #region Internal Properties and MemberBases

        internal static MapCollection _dbTypeToDataReaderMethod;
        internal static MapCollection DbTypeToDataReaderMethod
        {
            get
            {
                if (_dbTypeToDataReaderMethod == null)
                {
                    string path;
                    if (Map.TryResolvePath("DbType-DataReaderMethod", "", out path))
                        _dbTypeToDataReaderMethod = Map.Load(path);
                }
                return _dbTypeToDataReaderMethod;
            }
        }

        #endregion
    }
}