using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using CodeSmith.Data.Caching;
using CodeSmith.Data.Linq;
using CodeSmith.Data.Linq.Dynamic;

namespace CodeSmith.Data.Audit
{
    /// <summary>
    /// A class to create an <see cref="AuditLog"/> from the changes in a <see cref="DataContext"/>.
    /// </summary>
    public static class AuditManager
    {
        private const BindingFlags _defaultBinding = BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy;
        private const BindingFlags _defaultStaticBinding = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
        private const string _nullText = "{null}";

        private static readonly ReaderWriterLockSlim _auditableLock = new ReaderWriterLockSlim();
        private static readonly Dictionary<Type, bool> _auditableCache = new Dictionary<Type, bool>();

        private static readonly ReaderWriterLockSlim _notAuditedLock = new ReaderWriterLockSlim();
        private static readonly Dictionary<MemberInfo, bool> _notAuditedCache = new Dictionary<MemberInfo, bool>();

        private static readonly ReaderWriterLockSlim _alwaysAuditLock = new ReaderWriterLockSlim();
        private static readonly Dictionary<MemberInfo, bool> _alwaysAuditCache = new Dictionary<MemberInfo, bool>();

        private static readonly ReaderWriterLockSlim _displayColumnLock = new ReaderWriterLockSlim();
        private static readonly Dictionary<Type, MetaDataMember> _displayColumnCache = new Dictionary<Type, MetaDataMember>();

        private static readonly ReaderWriterLockSlim _formatterLock = new ReaderWriterLockSlim();
        private static readonly Dictionary<MemberInfo, MethodInfo> _formatterCache = new Dictionary<MemberInfo, MethodInfo>();

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

            ChangeSet changeSet = dataContext.GetChangeSet();
            return CreateAuditLog(dataContext, changeSet);
        }

        public static AuditLog CreateAuditLog(DataContext dataContext, ChangeSet changeSet)
        {
            var auditLog = new AuditLog();
            auditLog.Date = DateTime.Now;
            auditLog.Username = GetCurrentUserName();

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

        /// <summary>
        /// Refreshes the current properties in the audit log.
        /// </summary>
        /// <param name="log">The audit log to refresh.</param>
        public static void Refresh(AuditLog log)
        {
            if (log == null)
                return;

            // update current values because the entites can change after submit
            foreach (var auditEntity in log.Entities)
            {
                // don't need to update deletes
                if (auditEntity.Action == AuditAction.Delete)
                    continue;

                // if current is stored, it will be updated on submit
                object current = auditEntity.Current;
                if (current == null)
                    continue;

                // update the key value
                foreach (var key in auditEntity.Keys.Where(k => k.MetaDataMember != null))
                    key.Value = GetKeyValue(key.MetaDataMember, current);

                // update the property values
                foreach (var property in auditEntity.Properties.Where(p => p.MetaDataMember != null))
                {
                    try
                    {
                        var dataMember = property.MetaDataMember;
                        Type underlyingType = GetUnderlyingType(dataMember.Type);
                        var boxedValue = dataMember.MemberAccessor.GetBoxedValue(current);
                        var value = GetValue(dataMember.Member, underlyingType, boxedValue, current);
                        property.Current = value;
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError(ex.Message);
                        property.Current = "{error}";
                    }                    
                }
            }
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

                AuditEntity auditEntity = CreateAuditEntity(dataContext, entity, action);
                auditLog.Entities.Add(auditEntity);
            }
        }

        private static AuditEntity CreateAuditEntity(DataContext dataContext, object entity, AuditAction action)
        {
            var metaType = dataContext.Mapping.GetMetaType(entity.GetType());
            var rootType = metaType.InheritanceRoot == null
                       ? metaType.Type
                       : metaType.InheritanceRoot.Type;

            var table = dataContext.GetTable(rootType);

            var auditEntity = new AuditEntity();
            auditEntity.Action = action;
            auditEntity.Type = metaType.Type.FullName;
            auditEntity.Current = entity;
            if (action != AuditAction.Insert)
                auditEntity.Original = table.GetOriginalEntityState(entity);

            AddAuditKeys(metaType, entity, auditEntity);
            AddAuditProperties(metaType, table, entity, auditEntity);

            return auditEntity;
        }

        private static void AddAuditKeys(MetaType metaType, object entity, AuditEntity auditEntity)
        {
            foreach (var dataMember in metaType.IdentityMembers)
            {
                var auditProperty = new AuditKey();
                try
                {
                    auditProperty.MetaDataMember = dataMember;
                    auditProperty.Name = dataMember.Member.Name;
                    auditProperty.Type = dataMember.Type.FullName;

                    object value = GetKeyValue(dataMember, entity);

                    auditProperty.Value = value;
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.Message);
                    auditProperty.Value = "{error}";
                }
                auditEntity.Keys.Add(auditProperty);
            }
        }

        private static void AddAuditProperties(MetaType metaType, ITable table, object entity, AuditEntity auditEntity)
        {
            var modifiedMembers = table.GetModifiedMembers(entity);

            foreach (var dataMember in metaType.PersistentDataMembers)
            {
                if (dataMember.IsVersion || dataMember.IsAssociation || HasNotAuditedAttribute(dataMember.Member))
                    continue;

                var memberInfo = dataMember.Member;
                var modifiedMemberInfo = modifiedMembers.FirstOrDefault(m => m.Member == memberInfo);

                if (auditEntity.Action == AuditAction.Update && modifiedMemberInfo.Member == null && !HasAlwaysAuditAttribute(dataMember.Member))
                    continue; // this means the property was not changed, skip it

                var auditProperty = CreateAuditProperty(dataMember, modifiedMemberInfo, entity, auditEntity);
                if (auditProperty != null)
                    auditEntity.Properties.Add(auditProperty);
            }

            AddAssociationProperties(metaType, table, entity, auditEntity);
        }

        private static AuditProperty CreateAuditProperty(MetaDataMember dataMember, ModifiedMemberInfo modifiedMemberInfo, object entity, AuditEntity auditEntity)
        {
            var auditProperty = new AuditProperty();
            try
            {
                auditProperty.MetaDataMember = dataMember;
                auditProperty.Name = dataMember.Member.Name;
                
                Type underlyingType = GetUnderlyingType(dataMember.Type);
                auditProperty.Type = underlyingType.FullName;

                if (auditProperty.Type == _binaryType || dataMember.IsDeferred)
                    return auditProperty;

                if (auditEntity.Action == AuditAction.Update && modifiedMemberInfo.Member != null)
                {
                    auditProperty.Current = GetValue(modifiedMemberInfo.Member, underlyingType, modifiedMemberInfo.CurrentValue, entity);
                    auditProperty.Original = GetValue(modifiedMemberInfo.Member, underlyingType, modifiedMemberInfo.OriginalValue, entity);
                    return auditProperty;
                }

                var value = GetValue(dataMember.Member, underlyingType, dataMember.MemberAccessor.GetBoxedValue(entity), entity);
                if (value == null)
                    return null; // ignore null properties?

                if (auditEntity.Action == AuditAction.Delete)
                    auditProperty.Original = value;
                else
                    auditProperty.Current = value;
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                if (auditEntity.Action == AuditAction.Delete)
                    auditProperty.Original = "{error}";
                else
                    auditProperty.Current = "{error}";
            }

            return auditProperty;
        }

        private static void AddAssociationProperties(MetaType metaType, ITable table, object entity, AuditEntity auditEntity)
        {
            foreach (var association in metaType.Associations)
            {
                if (association.IsMany || HasNotAuditedAttribute(association.ThisMember.Member))
                    continue;

                // keys can contain multiple columns.
                // if none of them are found in the change list, skip.
                bool foundMember = false;

                foreach (var keyMember in association.ThisKey)
                {
                    if (!auditEntity.Properties.Contains(keyMember.Name))
                        continue;

                    var property = auditEntity.Properties[keyMember.Name];
                    property.IsForeignKey = association.IsForeignKey;
                    foundMember = true;
                }

                if (!foundMember)
                    continue;

                var auditProperty = new AuditProperty();

                try
                {
                    var thisMember = association.ThisMember;
                    auditProperty.Name = thisMember.Name;
                    auditProperty.Type = thisMember.Type.FullName;
                    auditProperty.IsAssociation = true;
                    auditProperty.ForeignKey = string.Join(",", association.ThisKey.Select(k => k.Name).ToArray());

                    var displayMember = GetDisplayMember(association.OtherType);

                    //this will get the fkey entity with out causing a load
                    object currentChildEntity = thisMember.DeferredValueAccessor.GetBoxedValue(entity);
                    object currentValue = GetAssociationValue(association, displayMember, currentChildEntity, table, entity);

                    //if there is nothing set for the fkey on insert and delete, skip
                    if (auditEntity.Action != AuditAction.Update && currentValue == null)
                        continue;

                    if (auditEntity.Action == AuditAction.Delete)
                        auditProperty.Original = currentValue;
                    else
                        auditProperty.Current = currentValue ?? _nullText;

                    if (auditEntity.Action == AuditAction.Update && table != null)
                    {
                        // get original value for updates
                        object original = table.GetOriginalEntityState(entity);
                        if (original != null)
                        {
                            //this will get the fkey entity with out causing a load
                            object originalChildEntity = thisMember.DeferredValueAccessor.GetBoxedValue(original);
                            auditProperty.Original = GetAssociationValue(association, displayMember, originalChildEntity, table, original) ?? _nullText;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.Message);
                    if (auditEntity.Action == AuditAction.Delete)
                        auditProperty.Original = "{error}";
                    else
                        auditProperty.Current = "{error}";
                }

                auditEntity.Properties.Add(auditProperty);
            } // foreach
        }

        private static object GetAssociationValue(MetaAssociation association, MetaDataMember childDisplayMember, object childEntity, ITable table, object entity)
        {
            if (childEntity != null)
                return GetValue(childDisplayMember, childEntity);

            if (table == null)
                return null;

            var dataContext = table.Context;
            if (dataContext == null)
                return null;

            try
            {
                var sb = new StringBuilder();
                var fkeyValues = new List<object>();

                // build dymamic query
                for (int i = 0; i < association.ThisKey.Count; i++)
                {
                    object v = association.ThisKey[i].MemberAccessor.GetBoxedValue(entity);

                    if (sb.Length > 0)
                        sb.Append(" and ");

                    if (v != null || association.OtherKey[i].CanBeNull)
                    {
                        sb.AppendFormat("{0} == @{1}", association.OtherKey[i].Name, fkeyValues.Count);
                        fkeyValues.Add(v);
                    }
                }

                if (fkeyValues.Count == 0)
                    return null;

                // get the fkey table
                var fkeyTable = dataContext.GetTable(association.OtherType.Type);
                var query = fkeyTable
                    .Where(sb.ToString(), fkeyValues.ToArray())
                    .Select(childDisplayMember.Name)
                    .Take(1);
                var cache = QueryResultCache.FromCache(query.Cast<object>(), null);
                var value = cache.FirstOrDefault();

                return GetValue(childDisplayMember.Member,
                    GetUnderlyingType(childDisplayMember.Type),
                    value, entity);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                return "{error}";
            }
        }

        private static object GetValue(MemberInfo memberInfo, Type valueType, object value, object entity)
        {
            if (value == null)
                return null;
            if (valueType == null)
                valueType = value.GetType();

            try
            {
                object returnValue = valueType.IsEnum ? Enum.GetName(valueType, value) : value;

                using (_formatterLock.ReadLock())
                {
                    MethodInfo formatMethod = null;
                    if (!_formatterCache.TryGetValue(memberInfo, out formatMethod))
                    {
                        using (_formatterLock.WriteLock())
                        {
                            var formatAttribute = GetAttribute<AuditPropertyFormatAttribute>(memberInfo);
                            if (formatAttribute == null)
                            {
                                _formatterCache.Add(memberInfo, null);
                                return returnValue;
                            }

                            Type formatterType = formatAttribute.FormatType;

                            //first: static object MethodName(MemberInfo memberInfo, object value, object entity)
                            formatMethod = formatterType.GetMethod(formatAttribute.MethodName, _defaultStaticBinding, null,
                                new[] { typeof(MethodInfo), typeof(object), typeof(object) }, null);

                            //next: static object MethodName(MemberInfo memberInfo, object value)
                            if (formatMethod == null)
                                formatMethod = formatterType.GetMethod(formatAttribute.MethodName, _defaultStaticBinding, null,
                                    new[] { typeof(MethodInfo), typeof(object) }, null);

                            //last: static object MethodName(object value)
                            if (formatMethod == null)
                                formatMethod = formatterType.GetMethod(formatAttribute.MethodName, _defaultStaticBinding, null,
                                    new[] { typeof(object) }, null);

                            _formatterCache.Add(memberInfo, formatMethod);
                        }
                    }

                    if (formatMethod == null)
                        return returnValue;

                    var args = formatMethod.GetParameters();

                    try
                    {
                        if (args.Length == 3)
                            return formatMethod.Invoke(null, new[] { memberInfo, returnValue, entity });

                        if (args.Length == 2)
                            return formatMethod.Invoke(null, new[] { memberInfo, returnValue });

                        return formatMethod.Invoke(null, new[] { returnValue });
                    }
                    catch
                    {
                        // eat format error?
                        return returnValue;
                    }
                } // using lock
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                return "{error}";
            }
        }

        private static object GetValue(MetaDataMember dataMember, object entity)
        {
            if (dataMember == null || entity == null)
                return null;

            try
            {
                Type underlyingType = GetUnderlyingType(dataMember.Type);
                object value = dataMember.MemberAccessor.GetBoxedValue(entity);

                return GetValue(dataMember.Member, underlyingType, value, entity);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                return "{error}";
            }
        }

        private static object GetKeyValue(MetaDataMember dataMember, object entity)
        {
            object value = dataMember.MemberAccessor.GetBoxedValue(entity);
            if (dataMember.Type.IsEnum)
                value = Enum.GetName(dataMember.Type, value);
            return value;
        }

        private static MetaDataMember GetDisplayMember(MetaType rowType)
        {
            if (rowType == null)
                return null;

            var entityType = rowType.Type;

            using (_displayColumnLock.ReadLock())
            {
                MetaDataMember displayMember = null;

                if (_displayColumnCache.TryGetValue(entityType, out displayMember))
                    return displayMember;

                using (_displayColumnLock.WriteLock())
                {
                    var displayAttribute = GetAttribute<DisplayColumnAttribute>(entityType);

                    // first try DisplayColumnAttribute property
                    if (displayAttribute != null)
                        displayMember = rowType.DataMembers.FirstOrDefault(m => m.Name == displayAttribute.DisplayColumn);

                    // try first string property
                    if (displayMember == null)
                        displayMember = rowType.DataMembers.FirstOrDefault(m => m.Type == typeof(string));

                    // try second property
                    if (displayMember == null && rowType.DataMembers.Count > 1)
                        displayMember = rowType.DataMembers[1];

                    _displayColumnCache.Add(entityType, displayMember);
                }
                return displayMember;
            }
        }

        private static bool HasNotAuditedAttribute(MemberInfo memberInfo)
        {
            using (_notAuditedLock.ReadLock())
            {
                if (_notAuditedCache.ContainsKey(memberInfo))
                    return _notAuditedCache[memberInfo];

                using (_notAuditedLock.WriteLock())
                {
                    bool result = HasAttribute(memberInfo, typeof(NotAuditedAttribute));
                    _notAuditedCache.Add(memberInfo, result);
                    return result;
                }
            }
        }

        private static bool HasAlwaysAuditAttribute(MemberInfo memberInfo)
        {
            using (_alwaysAuditLock.ReadLock())
            {
                if (_alwaysAuditCache.ContainsKey(memberInfo))
                    return _alwaysAuditCache[memberInfo];

                using (_alwaysAuditLock.WriteLock())
                {
                    bool result = HasAttribute(memberInfo, typeof(AlwaysAuditAttribute));
                    _alwaysAuditCache.Add(memberInfo, result);
                    return result;
                }
            }
        }

        private static bool HasAuditAttribute(object entity)
        {
            Type entityType = entity.GetType();

            using (_auditableLock.ReadLock())
            {
                if (_auditableCache.ContainsKey(entityType))
                    return _auditableCache[entityType];

                using (_auditableLock.WriteLock())
                {
                    bool result = HasAttribute(entityType, typeof(AuditAttribute));
                    _auditableCache.Add(entityType, result);
                    return result;
                }
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

        private static TAttribute GetAttribute<TAttribute>(MemberInfo memberInfo) where TAttribute : Attribute
        {
            Type attributeType = typeof(TAttribute);

            TAttribute attribute = memberInfo.GetCustomAttributes(attributeType, true).FirstOrDefault() as TAttribute;
            if (attribute != null)
                return attribute;

            // try the metadata object
            MemberInfo declaringType = memberInfo;
            if (memberInfo.MemberType != MemberTypes.TypeInfo && memberInfo.DeclaringType != null)
                declaringType = memberInfo.DeclaringType;

            var metadataTypeAttribute = declaringType.GetCustomAttributes(typeof(MetadataTypeAttribute), true).FirstOrDefault() as MetadataTypeAttribute;
            if (metadataTypeAttribute == null)
                return null;

            Type metadataType = metadataTypeAttribute.MetadataClassType;
            if (metadataType == null)
                return null;

            if (memberInfo.MemberType == MemberTypes.TypeInfo)
                return metadataType.GetCustomAttributes(attributeType, true).FirstOrDefault() as TAttribute;

            MemberInfo metaInfo = metadataType.GetMember(memberInfo.Name, _defaultBinding).FirstOrDefault();
            if (metaInfo == null)
                return null;
            return metaInfo.GetCustomAttributes(attributeType, true).FirstOrDefault() as TAttribute;
        }

        private static Type GetUnderlyingType(Type type)
        {
            Type t = type;
            bool isNullable = t.IsGenericType && (t.GetGenericTypeDefinition() == typeof(Nullable<>));
            if (isNullable)
                return Nullable.GetUnderlyingType(t);

            return t;
        }

        private static string GetCurrentUserName()
        {
            if (HostingEnvironment.IsHosted)
            {
                IPrincipal currentUser = null;
                HttpContext current = HttpContext.Current;
                if (current != null)
                    currentUser = current.User;

                if ((currentUser != null) && (currentUser.Identity != null))
                    return currentUser.Identity.Name;
            }

            return Environment.UserName;
        }

        #region Disposable Lock Classes

        /// <summary>
        /// Gets the read lock.
        /// </summary>
        /// <returns></returns>
        private static IDisposable ReadLock(this ReaderWriterLockSlim lockSlim)
        {
            lockSlim.EnterUpgradeableReadLock();
            return new DisposableLock(lockSlim.ExitUpgradeableReadLock);
        }

        /// <summary>
        /// Gets the write lock.
        /// </summary>
        /// <returns></returns>
        private static IDisposable WriteLock(this ReaderWriterLockSlim lockSlim)
        {
            lockSlim.EnterWriteLock();
            return new DisposableLock(lockSlim.ExitWriteLock);
        }

        #region Nested type: DisposableLock

        private class DisposableLock : IDisposable
        {
            private readonly Action _exitAction;

            public DisposableLock(Action exitAction)
            {
                _exitAction = exitAction;
            }

            void IDisposable.Dispose()
            {
                _exitAction.Invoke();
            }
        }

        #endregion

        #endregion
    }
}