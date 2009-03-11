using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace CodeSmith.Data.Audit
{
    /// <summary>
    /// A class to create an <see cref="AuditLog"/> from the changes in a <see cref="DataContext"/>.
    /// </summary>
    public static class AuditManager
    {
        private const BindingFlags _defaultBinding = BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy;
        private static readonly ReaderWriterLockSlim _auditableLock = new ReaderWriterLockSlim();
        private static readonly Dictionary<Type, bool> _auditableCache = new Dictionary<Type, bool>();

        private static readonly ReaderWriterLockSlim _notAuditedLock = new ReaderWriterLockSlim();
        private static readonly Dictionary<MemberInfo, bool> _notAuditedCache = new Dictionary<MemberInfo, bool>();
        private static readonly string _binaryType = typeof(Binary).FullName;

        /// <summary>
        /// Creates the <see cref="AuditLog"/> of changes form the specified <see cref="DataContext"/>.
        /// </summary>
        /// <param name="dataContext">The <see cref="DataContext"/> to get the changes from.</param>
        /// <returns>An instance of <see cref="AuditLog"/> that is an audit log of the changes to <see cref="DataContext"/>.</returns>
        /// <remarks>
        /// An entity has to be marked with the <see cref="AuditAttribute"/> for audit data to be collected for that entity.
        /// </remarks>
        public static AuditLog CreateAuditLog(DataContext dataContext)
        {
            if (dataContext == null)
                throw new ArgumentNullException("dataContext");

            var auditLog = new AuditLog();

            ChangeSet changeSet = dataContext.GetChangeSet();
            AddAuditEntities(dataContext, auditLog, AuditAction.Delete, changeSet.Deletes);
            AddAuditEntities(dataContext, auditLog, AuditAction.Insert, changeSet.Inserts);
            AddAuditEntities(dataContext, auditLog, AuditAction.Update, changeSet.Updates);

            return auditLog;
        }

        /// <summary>
        /// Merges the audit logs together.
        /// </summary>
        /// <param name="logs">The list logs to merge together.</param>
        /// <returns>An instance of <see cref="AuditLog"/> with all other logs merged in.</returns>
        public static AuditLog MergeAuditLogs(params AuditLog[] logs)
        {
            if (logs == null)
                throw new ArgumentNullException("logs");
            if (logs.Length == 0)
                return null;
            if (logs.Length == 1)
                return logs[0];

            //use first log as final log
            var mergedLog = logs[0];
            for (int i = 1; i < logs.Length; i++)
            {                
                var current = logs[i];                
                foreach (var entity in current.Entities)
                {
                    // compare all entities to final, merge duplicates, add new
                    int index = mergedLog.Entities.IndexOf(entity);
                    if (index >= 0)
                        MergeAuditEntity(entity, mergedLog.Entities[index]);
                    else
                        mergedLog.Entities.Add(entity);
                }
            }

            return mergedLog;
        }

        private static void MergeAuditEntity(AuditEntity source, AuditEntity destination)
        {
            //destination.Action = source.Action;  // merge action?
            foreach (var property in source.Properties)
            {
                if (destination.Properties.Contains(property.Name))
                    // only update the current property, keep the original
                    destination.Properties[property.Name].Current = property.Current;
                else
                    destination.Properties.Add(property);
            }
        }

        private static void AddAuditEntities(DataContext dataContext, AuditLog auditLog, AuditAction action, IEnumerable<object> entities)
        {
            foreach (var entity in entities)
            {
                if (!HasAuditAttribute(entity))
                    continue;

                AuditEntity auditEntity = GetAuditEntity(dataContext, entity, action);
                auditLog.Entities.Add(auditEntity);
            }
        }

        private static AuditEntity GetAuditEntity(DataContext dataContext, object entity, AuditAction action)
        {
            ITable table = dataContext.GetTable(entity.GetType());
            MetaTable metaTable = dataContext.Mapping.GetTable(entity.GetType());
            
            var auditEntity = new AuditEntity();
            auditEntity.Action = action;
            auditEntity.Type = table.ElementType.FullName;

            AddAuditKeys(metaTable, entity, auditEntity);
            if (action == AuditAction.Update)
                AddAuditProperties(table, entity, auditEntity);
            else
                AddAuditProperties(metaTable, entity, auditEntity);

            return auditEntity;
        }

        private static void AddAuditKeys(MetaTable metaTable, object entity, AuditEntity auditEntity)
        {
            foreach (var dataMember in metaTable.RowType.IdentityMembers)
            {
                var auditProperty = new AuditKey();
                auditProperty.Name = dataMember.Member.Name;
                auditProperty.Type = dataMember.Type.FullName;
                auditProperty.Value = dataMember.MemberAccessor.GetBoxedValue(entity);                
                auditEntity.Keys.Add(auditProperty);
            }
        }

        /// <summary>
        /// For inserts and deletes, all the properties current values are included in the log.
        /// </summary>
        private static void AddAuditProperties(MetaTable metaTable, object entity, AuditEntity auditEntity)
        {
            foreach (var dataMember in metaTable.RowType.DataMembers)
            {
                if (dataMember.IsVersion || dataMember.IsAssociation || HasNotAuditedAttribute(dataMember.Member))
                    continue;

                var auditProperty = new AuditProperty();
                auditProperty.Name = dataMember.Member.Name;
                auditProperty.Type = GetUnderlyingType(dataMember.Type).FullName;

                if (auditProperty.Type != _binaryType && !dataMember.IsDeferred)
                {
                    if (auditEntity.Action == AuditAction.Delete)
                        auditProperty.Original = dataMember.MemberAccessor.GetBoxedValue(entity);
                    else
                        auditProperty.Current = dataMember.MemberAccessor.GetBoxedValue(entity);
                }

                auditEntity.Properties.Add(auditProperty);
            }
        }

        /// <summary>
        /// For updated entities, only the modified properties are included in the log.
        /// </summary>
        private static void AddAuditProperties(ITable table, object entity, AuditEntity auditEntity)
        {
            ModifiedMemberInfo[] modified = table.GetModifiedMembers(entity);
            
            foreach (ModifiedMemberInfo info in modified)
            {
                if (HasNotAuditedAttribute(info.Member))
                    continue;

                var auditProperty = new AuditProperty();
                auditProperty.Name = info.Member.Name;

                var propertyInfo = info.Member as PropertyInfo;
                if (propertyInfo != null)
                    auditProperty.Type = GetUnderlyingType(propertyInfo.PropertyType).FullName;

                if (auditProperty.Type != _binaryType)
                {
                    auditProperty.Current = info.CurrentValue;
                    auditProperty.Original = info.OriginalValue;
                }

                auditEntity.Properties.Add(auditProperty);
            }
        }

        private static bool HasNotAuditedAttribute(MemberInfo memberInfo)
        {            
            _notAuditedLock.EnterUpgradeableReadLock();
            try
            {
                if (_notAuditedCache.ContainsKey(memberInfo))
                    return _notAuditedCache[memberInfo];

                _notAuditedLock.EnterWriteLock();
                try
                {
                    bool result = HasAttribute(memberInfo, typeof(NotAuditedAttribute));
                    _notAuditedCache.Add(memberInfo, result);
                    return result;
                }
                finally
                {
                    _notAuditedLock.ExitWriteLock();
                }
            }
            finally
            {
                _notAuditedLock.ExitUpgradeableReadLock();
            }
        }

        private static bool HasAuditAttribute(object entity)
        {
            Type entityType = entity.GetType();

            _auditableLock.EnterUpgradeableReadLock();
            try
            {
                if (_auditableCache.ContainsKey(entityType))
                    return _auditableCache[entityType];

                _auditableLock.EnterWriteLock();
                try
                {
                    bool result = HasAttribute(entityType, typeof(AuditAttribute));
                    _auditableCache.Add(entityType, result);
                    return result;
                }
                finally
                {
                    _auditableLock.ExitWriteLock();
                }
            }
            finally
            {
                _auditableLock.ExitUpgradeableReadLock();
            }
            
        }

        private static bool HasAttribute(MemberInfo memberInfo, Type attributeType)
        {
            if (memberInfo.IsDefined(attributeType, true))
                return true;

            // try the metadata object
            MemberInfo declaringType = memberInfo;
            if (memberInfo.MemberType != MemberTypes.TypeInfo && memberInfo.DeclaringType != null)
                declaringType = memberInfo.DeclaringType;

            var metadataTypeAttribute = declaringType.GetCustomAttributes(typeof(MetadataTypeAttribute), true).FirstOrDefault() as MetadataTypeAttribute;
            if (metadataTypeAttribute == null)
                return false;

            Type metadataType = metadataTypeAttribute.MetadataClassType;
            if (metadataType == null)
                return false;

            if (memberInfo.MemberType == MemberTypes.TypeInfo)
                return metadataType.IsDefined(attributeType, true);

            MemberInfo metaInfo = metadataType.GetMember(memberInfo.Name, _defaultBinding).FirstOrDefault();
            return metaInfo != null && metaInfo.IsDefined(attributeType, true);
        }

        private static Type GetUnderlyingType(Type type)
        {
            Type t = type;
            bool isNullable = t.IsGenericType && (t.GetGenericTypeDefinition() == typeof(Nullable<>));
            if (isNullable)
                return Nullable.GetUnderlyingType(t);

            return t;
        }
    }
}