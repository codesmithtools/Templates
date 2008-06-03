using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace DbmlSchema
{
    #region Utility Classes
    internal static class DbmlConstant
    {
        // Fields
        public const string AccessModifier = "AccessModifier";
        public const string Argument = "Argument";
        public const string Association = "Association";
        public const string AutoSync = "AutoSync";
        public const string BaseType = "BaseType";
        public const string CanBeNull = "CanBeNull";
        public const string Cardinality = "Cardinality";
        public const string Class = "Class";
        public const string Column = "Column";
        public const string Connection = "Connection";
        public const string ConnectionString = "ConnectionString";
        public const string ContextNamespace = "ContextNamespace";
        public const string Database = "Database";
        public const string DataContextBaseTypeDefault = "System.Data.Linq.DataContext";
        public const string DbmlNamespace = "http://schemas.microsoft.com/linqtosql/dbml/2007";
        public const string DbType = "DbType";
        public const string DeleteFunction = "DeleteFunction";
        public const string DeleteOnNull = "DeleteOnNull";
        public const string DeleteRule = "DeleteRule";
        public const string Direction = "Direction";
        public const string ElementType = "ElementType";
        public const string EntityBase = "EntityBase";
        public const string EntityNamespace = "EntityNamespace";
        public const string Expression = "Expression";
        public const string ExternalMapping = "ExternalMapping";
        public const string Function = "Function";
        public const string FunctionId = "FunctionId";
        public const string HasMultipleResults = "HasMultipleResults";
        public const string Id = "Id";
        public const string IdRef = "IdRef";
        public const string InheritanceCode = "InheritanceCode";
        public const string InsertFunction = "InsertFunction";
        public const string IsComposable = "IsComposable";
        public const string IsDbGenerated = "IsDbGenerated";
        public const string IsDelayLoaded = "IsDelayLoaded";
        public const string IsDiscriminator = "IsDiscriminator";
        public const string IsForeignKey = "IsForeignKey";
        public const string IsInheritanceDefault = "IsInheritanceDefault";
        public const string IsPrimaryKey = "IsPrimaryKey";
        public const string IsReadOnly = "IsReadOnly";
        public const string IsVersion = "IsVersion";
        public const string Member = "Member";
        public const string Method = "Method";
        public const string Mode = "Mode";
        public const string Modifier = "Modifier";
        public const string Name = "Name";
        public const string OtherKey = "OtherKey";
        public const string Parameter = "Parameter";
        public const string Provider = "Provider";
        public const string Return = "Return";
        public const string Serialization = "Serialization";
        public const string SettingsObjectName = "SettingsObjectName";
        public const string SettingsPropertyName = "SettingsPropertyName";
        public const string Storage = "Storage";
        public const string Table = "Table";
        public const string TableFunction = "TableFunction";
        public const string ThisKey = "ThisKey";
        public const string Type = "Type";
        public const string UpdateCheck = "UpdateCheck";
        public const string UpdateFunction = "UpdateFunction";
        public const string Version = "Version";
    }

    public static class DbmlFile
    {
        private static XmlSerializer _serializer = new XmlSerializer(typeof(Database), DbmlConstant.DbmlNamespace);

        public static void Save(Database db, string fileName)
        {
            XmlWriterSettings ws = new XmlWriterSettings();
            ws.Encoding = Encoding.UTF8;
            ws.Indent = true;

            XmlSerializerNamespaces xsn = new XmlSerializerNamespaces();
            xsn.Add("", DbmlConstant.DbmlNamespace);

            Debug.WriteLine("Saving Dbml File:" + fileName);
            using (XmlWriter w = XmlWriter.Create(fileName, ws))
            {
                _serializer.Serialize(w, db, xsn);
                w.Flush();
                w.Close();
            }
        }

        public static Database Load(string fileName)
        {
            if (!File.Exists(fileName))
                return null;

            Debug.WriteLine("Loading Dbml File:" + fileName);
            Database db;

            using (StreamReader sr = new StreamReader(fileName, Encoding.UTF8))
            {
                XmlReader xr = XmlReader.Create(sr);
                db = _serializer.Deserialize(xr) as Database;
            }

            return db;
        }

    }

    public static class Naming
    {

        public static string GetModifier(AccessModifier access)
        {
            return access.ToString().ToLower();
        }

        public static string GetModifier(AccessModifier access, ClassModifier modifier)
        {
            if (modifier == ClassModifier.None)
                return access.ToString().ToLower();
            else
                return string.Format("{0} {1}", access.ToString().ToLower(), modifier.ToString().ToLower());
        }

        public static string GetModifier(AccessModifier access, MemberModifier modifier)
        {
            if (modifier == MemberModifier.None)
                return access.ToString().ToLower();
            else
                return string.Format("{0} {1}", access.ToString().ToLower(), modifier.ToString().ToLower());
        }

    }
    #endregion

    #region Schema Classes

    [Serializable]
    [XmlType(Namespace = DbmlConstant.DbmlNamespace)]
    [XmlRoot(DbmlConstant.Database, Namespace = DbmlConstant.DbmlNamespace, IsNullable = false)]
    public class Database : IName
    {
        private AccessModifier _accessModifier;
        private string _baseType;
        private string _class;
        private Connection _connection;
        private string _contextNamespace;
        private string _entityBase;
        private string _entityNamespace;
        private bool _externalMapping;
        private FunctionCollection _functions;
        private ClassModifier _modifier;
        private string _name;
        private string _provider;
        private SerializationMode _serialization;
        private TableCollection _tables;

        public Database()
        {
            _accessModifier = AccessModifier.Public;
            _externalMapping = false;
            _modifier = ClassModifier.None;
            _serialization = SerializationMode.None;
            _tables = new TableCollection();
            _functions = new FunctionCollection();
        }

        [XmlElement("Connection")]
        public Connection Connection
        {
            get { return _connection; }
            set { _connection = value; }
        }

        [XmlElement("Table")]
        public TableCollection Tables
        {
            get { return _tables; }
            set { _tables = value; }
        }

        public bool ShouldSerializeTables()
        {
            return _tables.Count > 0;
        }

        [XmlElement("Function")]
        public FunctionCollection Functions
        {
            get { return _functions; }
            set { _functions = value; }
        }

        public bool ShouldSerializeFunctions()
        {
            return _functions.Count > 0;
        }

        [XmlAttribute("Name")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [XmlAttribute("EntityNamespace")]
        public string EntityNamespace
        {
            get { return _entityNamespace; }
            set { _entityNamespace = value; }
        }

        [XmlAttribute("ContextNamespace")]
        public string ContextNamespace
        {
            get { return _contextNamespace; }
            set { _contextNamespace = value; }
        }

        [XmlAttribute("Class")]
        public string Class
        {
            get { return ClassSpecified ? _class : _name; }
            set { _class = value; _classSpecified = true; }
        }

        private bool _classSpecified = false;

        [XmlIgnore]
        public bool ClassSpecified
        {
            get { return _classSpecified; }
            set { _classSpecified = value; }
        }

        [XmlAttribute("AccessModifier")]
        [DefaultValue(AccessModifier.Public)]
        public AccessModifier AccessModifier
        {
            get { return _accessModifier; }
            set { _accessModifier = value; }
        }

        [XmlAttribute("Modifier")]
        [DefaultValue(ClassModifier.None)]
        public ClassModifier ClassModifier
        {
            get { return _modifier; }
            set { _modifier = value; }
        }

        [XmlAttribute("BaseType")]
        public string BaseType
        {
            get { return _baseType; }
            set { _baseType = value; }
        }

        [XmlAttribute("Provider")]
        public string Provider
        {
            get { return _provider; }
            set { _provider = value; }
        }

        [XmlAttribute("ExternalMapping")]
        [DefaultValue(false)]
        public bool ExternalMapping
        {
            get { return _externalMapping; }
            set { _externalMapping = value; }
        }

        [XmlAttribute("Serialization")]
        [DefaultValue(SerializationMode.None)]
        public SerializationMode Serialization
        {
            get { return _serialization; }
            set { _serialization = value; }
        }

        [XmlAttribute("EntityBase")]
        public string EntityBase
        {
            get { return _entityBase; }
            set { _entityBase = value; }
        }

        private Dictionary<string, DbmlSchema.Type> _typeNameCache;
        private Dictionary<string, DbmlSchema.Type> _typeIdCache;
        private bool _isTypeCacheCreated = false;

        private void CreateTypeCache()
        {
            _typeNameCache = new Dictionary<string, DbmlSchema.Type>();
            _typeIdCache = new Dictionary<string, DbmlSchema.Type>();

            foreach (Table t in this.Tables)
                CreateTypeCache(t.Type);

            foreach (Function f in this.Functions)
                foreach (DbmlSchema.Type furnctionType in f.ElementTypes)
                    CreateTypeCache(furnctionType);

            _isTypeCacheCreated = true;
        }

        private void CreateTypeCache(DbmlSchema.Type parentType)
        {
            if (!string.IsNullOrEmpty(parentType.Name))
                _typeNameCache.Add(parentType.Name, parentType);

            if (!string.IsNullOrEmpty(parentType.Id))
                _typeIdCache.Add(parentType.Id, parentType);

            foreach (DbmlSchema.Type childType in parentType.DerivedTypes)
                CreateTypeCache(childType);
        }

        public DbmlSchema.Type GetTypeByName(string name)
        {
            if (!_isTypeCacheCreated)
                CreateTypeCache();

            DbmlSchema.Type type = null;
            _typeNameCache.TryGetValue(name, out type);

            return type;
        }


        public DbmlSchema.Type GetTypeById(string id)
        {
            if (!_isTypeCacheCreated)
                CreateTypeCache();

            DbmlSchema.Type type = null;
            _typeIdCache.TryGetValue(id, out type);

            return type;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }

    [Serializable]
    [XmlType(Namespace = DbmlConstant.DbmlNamespace)]
    public class Connection
    {
        private string _connectionString;
        private ConnectionMode _mode;
        private string _provider;
        private string _settingsObjectName;
        private string _settingsPropertyName;

        public Connection()
        {
            _mode = ConnectionMode.ConnectionString;
        }

        [XmlAttribute("Mode")]
        [DefaultValue(ConnectionMode.ConnectionString)]
        public ConnectionMode Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        [XmlAttribute("ConnectionString")]
        public string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }

        [XmlAttribute("SettingsObjectName")]
        public string SettingsObjectName
        {
            get { return _settingsObjectName; }
            set { _settingsObjectName = value; }
        }

        [XmlAttribute("SettingsPropertyName")]
        public string SettingsPropertyName
        {
            get { return _settingsPropertyName; }
            set { _settingsPropertyName = value; }
        }

        [XmlAttribute("Provider")]
        public string Provider
        {
            get { return _provider; }
            set { _provider = value; }
        }

    }

    [Serializable]
    [XmlType(Namespace = DbmlConstant.DbmlNamespace)]
    public class Return
    {
        private string _dbType;
        private string _type;

        [XmlAttribute("Type")]
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        [XmlAttribute("DbType")]
        public string DbType
        {
            get { return _dbType; }
            set { _dbType = value; }
        }
    }

    [Serializable]
    [XmlType(Namespace = DbmlConstant.DbmlNamespace)]
    public class Parameter : IName
    {
        private string _dbType;
        private ParameterDirection _direction;
        private string _name;
        private string _parameterName;
        private string _type;

        public Parameter()
        {
            _direction = ParameterDirection.In;
        }

        [XmlAttribute("Name")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [XmlAttribute("Parameter")]
        public string ParameterName
        {
            get { return ParameterNameSpecified ? _parameterName : _name; }
            set { _parameterName = value; _parameterNameSpecified = true; }
        }

        private bool _parameterNameSpecified = false;

        [XmlIgnore]
        public bool ParameterNameSpecified
        {
            get { return _parameterNameSpecified; }
            set { _parameterNameSpecified = value; }
        }

        [XmlAttribute("Type")]
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        [XmlAttribute("DbType")]
        public string DbType
        {
            get { return _dbType; }
            set { _dbType = value; }
        }

        [XmlAttribute("Direction")]
        [DefaultValue(ParameterDirection.In)]
        public ParameterDirection Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }

        public override string ToString()
        {
            return this.Name;
        }

    }

    [Serializable]
    [XmlType(Namespace = DbmlConstant.DbmlNamespace)]
    public class Function : IName, IProcessed
    {
        private AccessModifier _accessModifier;
        private bool _hasMultipleResults;
        private string _id;
        private bool _isComposable;
        private TypeCollection _elementTypes;
        private Return _return;
        private string _method;
        private MemberModifier _modifier;
        private string _name;
        private ParameterCollection _parameters;

        public Function()
        {
            _accessModifier = AccessModifier.Public;
            _modifier = MemberModifier.None;
            _isComposable = false;
            _hasMultipleResults = false;
            _elementTypes = new TypeCollection();
            _parameters = new ParameterCollection();
        }

        [XmlElement("Parameter")]
        public ParameterCollection Parameters
        {
            get { return _parameters; }
            set { _parameters = value; }
        }

        public bool ShouldSerializeParameters()
        {
            return _parameters.Count > 0;
        }

        [XmlElement("ElementType")]
        public TypeCollection ElementTypes
        {
            get { return _elementTypes; }
            set { _elementTypes = value; }
        }

        public bool ShouldSerializeElementTypes()
        {
            return _elementTypes.Count > 0;
        }

        [XmlElement("Return")]
        public Return Return
        {
            get { return _return; }
            set { _return = value; }
        }

        [XmlAttribute("Name")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [XmlAttribute("Id", DataType = "ID")]
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        [XmlAttribute("Method")]
        public string Method
        {
            get { return MethodSpecified ? _method : _name; }
            set { _method = value; _methodSpecified = true; }
        }

        private bool _methodSpecified = false;

        [XmlIgnore]
        public bool MethodSpecified
        {
            get { return _methodSpecified; }
            set { _methodSpecified = value; }
        }

        [XmlAttribute("AccessModifier")]
        [DefaultValue(AccessModifier.Public)]
        public AccessModifier AccessModifier
        {
            get { return _accessModifier; }
            set { _accessModifier = value; }
        }

        [XmlAttribute("Modifier")]
        [DefaultValue(MemberModifier.None)]
        public MemberModifier Modifier
        {
            get { return _modifier; }
            set { _modifier = value; }
        }

        [XmlAttribute("HasMultipleResults")]
        [DefaultValue(false)]
        public bool HasMultipleResults
        {
            get { return _hasMultipleResults; }
            set { _hasMultipleResults = value; }
        }

        [XmlAttribute("IsComposable")]
        [DefaultValue(false)]
        public bool IsComposable
        {
            get { return _isComposable; }
            set { _isComposable = value; }
        }

        #region IProcessed Members
        private bool _isProcessed = false;

        [XmlIgnore]
        public bool IsProcessed
        {
            get { return _isProcessed; }
            set { _isProcessed = value; }
        }
        #endregion

        public DbmlSchema.Type GetElementTypeById(string id)
        {
            foreach (DbmlSchema.Type t in ElementTypes)
                if (t.Id == id)
                    return t;

            return null;
        }

        public DbmlSchema.Type GetElementTypeByName(string name)
        {
            foreach (DbmlSchema.Type t in ElementTypes)
                if (t.Name == name)
                    return t;

            return null;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }

    [Serializable]
    [XmlType(Namespace = DbmlConstant.DbmlNamespace)]
    public class Type : IName
    {
        private AccessModifier _accessModifier;
        private string _id;
        private string _idRef;
        private string _inheritanceCode;
        private bool _isInheritanceDefault;
        private AssociationCollection _associations;
        private ColumnCollection _columns;
        private ClassModifier _modifier;
        private string _name;
        private TypeCollection _derivedTypes;

        public Type()
        {
            _accessModifier = AccessModifier.Public;
            _modifier = ClassModifier.None;
            _isInheritanceDefault = false;
            _associations = new AssociationCollection();
            _columns = new ColumnCollection();
            _derivedTypes = new TypeCollection();
        }

        [XmlElement("Column")]
        public ColumnCollection Columns
        {
            get { return _columns; }
            set { _columns = value; }
        }

        public bool ShouldSerializeColumns()
        {
            return _columns.Count > 0;
        }

        [XmlElement("Association")]
        public AssociationCollection Associations
        {
            get { return _associations; }
            set { _associations = value; }
        }

        public bool ShouldSerializeAssociations()
        {
            return _associations.Count > 0;
        }

        [XmlElement("Type")]
        public TypeCollection DerivedTypes
        {
            get { return _derivedTypes; }
            set { _derivedTypes = value; }
        }

        public bool ShouldSerializeDerivedTypes()
        {
            return _derivedTypes.Count > 0;
        }

        [XmlAttribute("IdRef", DataType = "IDREF")]
        public string IdRef
        {
            get { return _idRef; }
            set { _idRef = value; }
        }

        [XmlAttribute("Id", DataType = "ID")]
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        [XmlAttribute("Name")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [XmlAttribute("InheritanceCode")]
        public string InheritanceCode
        {
            get { return _inheritanceCode; }
            set { _inheritanceCode = value; }
        }

        [XmlAttribute("IsInheritanceDefault")]
        [DefaultValue(false)]
        public bool IsInheritanceDefault
        {
            get { return _isInheritanceDefault; }
            set { _isInheritanceDefault = value; }
        }

        [XmlAttribute("AccessModifier")]
        [DefaultValue(AccessModifier.Public)]
        public AccessModifier AccessModifier
        {
            get { return _accessModifier; }
            set { _accessModifier = value; }
        }

        [XmlAttribute("Modifier")]
        [DefaultValue(ClassModifier.None)]
        public ClassModifier Modifier
        {
            get { return _modifier; }
            set { _modifier = value; }
        }
        
        [XmlIgnore]
        public IEnumerable<Association> EntityRefAssociations
        {
            get
            {
                foreach (Association a in Associations)
                {
                    if (a.IsForeignKey || a.Cardinality == Cardinality.One)
                        yield return a;
                }
            }
        }

        [XmlIgnore]
        public IEnumerable<Association> EntitySetAssociations
        {
            get
            {
                foreach (Association a in Associations)
                {
                    if (!a.IsForeignKey && a.Cardinality == Cardinality.Many)
                        yield return a;
                }
            }
        }

        [XmlIgnore]
        public IEnumerable<Column> PrimaryKeyColumns
        {
             get
            {
                foreach (Column c in Columns)
                {
                    if (c.IsPrimaryKey)
                        yield return c;
                }
            }
        }

        public Column GetColumnByMember(string member)
        {
            if (string.IsNullOrEmpty(member))
                return null;

            foreach (Column c in Columns)
                if (c.Member.Equals(member))
                    return c;

            return null;
        }

        public List<Column> GetColumnsByMembers(string[] members)
        {
            List<Column> cols = new List<Column>();

            if (members == null || members.Length == 0)
                return cols;

            foreach (Column c in Columns)
                if (Array.Exists<string>(members, delegate (string s) { return c.Member.Equals(s); } ))
                    cols.Add(c);

            return cols;
        }

        public Association GetForeignKeyAssociation(Column c)
        {
            foreach (Association a in Associations)
                if (a.IsForeignKey && a.ThisKey.Equals(c.Member))
                    return a;

            return null;
        }

        

        public override string ToString()
        {
            return this.Name;
        }

    }

    [Serializable]
    [XmlType(Namespace = DbmlConstant.DbmlNamespace)]
    public class Association : IMember, IProcessed
    {
        private AccessModifier _accessModifier;
        private Cardinality _cardinality;
        private bool _deleteOnNull;
        private string _deleteRule;
        private bool _isForeignKey;
        private string _member;
        private MemberModifier _modifier;
        private string _name;
        private string _otherKey;
        private string _storage;
        private string _thisKey;
        private string _type;

        public Association()
        {
            _accessModifier = AccessModifier.Public;
            _modifier = MemberModifier.None;
            _cardinality = Cardinality.Many;
            _deleteOnNull = false;
            _isForeignKey = false;
        }

        [XmlAttribute("Name")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [XmlAttribute("Member")]
        public string Member
        {
            get { return _member; }
            set { _member = value; }
        }

        [XmlAttribute("Storage")]
        public string Storage
        {
            get { return StorageSpecified ? _storage : "_" + this.Member; }
            set { _storage = value; _storageSpecified = true; }
        }

        private bool _storageSpecified = false;

        [XmlIgnore]
        public bool StorageSpecified
        {
            get { return _storageSpecified; }
            set { _storageSpecified = value; }
        }

        [XmlAttribute("AccessModifier")]
        [DefaultValue(AccessModifier.Public)]
        public AccessModifier AccessModifier
        {
            get { return _accessModifier; }
            set { _accessModifier = value; }
        }

        [XmlAttribute("Modifier")]
        [DefaultValue(MemberModifier.None)]
        public MemberModifier Modifier
        {
            get { return _modifier; }
            set { _modifier = value; }
        }

        [XmlAttribute("ThisKey")]
        public string ThisKey
        {
            get { return _thisKey; }
            set { _thisKey = value; }
        }

        [XmlAttribute("OtherKey")]
        public string OtherKey
        {
            get { return _otherKey; }
            set { _otherKey = value; }
        }

        [XmlAttribute("Type")]
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        [XmlAttribute("IsForeignKey")]
        [DefaultValue(false)]
        public bool IsForeignKey
        {
            get { return _isForeignKey; }
            set { _isForeignKey = value; }
        }

        [XmlAttribute("Cardinality")]
        [DefaultValue(Cardinality.Many)]
        public Cardinality Cardinality
        {
            get { return _cardinality; }
            set { _cardinality = value; }
        }

        [XmlAttribute("DeleteRule")]
        public string DeleteRule
        {
            get { return _deleteRule; }
            set { _deleteRule = value; }
        }

        [XmlAttribute("DeleteOnNull")]
        [DefaultValue(false)]
        public bool DeleteOnNull
        {
            get { return _deleteOnNull; }
            set { _deleteOnNull = value; }
        }

        #region IProcessed Members
        private bool _isProcessed = false;

        [XmlIgnore]
        public bool IsProcessed
        {
            get { return _isProcessed; }
            set { _isProcessed = value; }
        }
        #endregion

        public AssociationKey ToKey()
        {
            return new AssociationKey(this.Name, this.IsForeignKey);
        }

        public AssociationKey ToOtherKey()
        {
            return new AssociationKey(this.Name, !this.IsForeignKey);
        }

        public override string ToString()
        {
            return this.Name;
        }

    }

    [Serializable]
    [XmlType(Namespace = DbmlConstant.DbmlNamespace)]
    public class Column : IMember, IProcessed
    {
        private AccessModifier _accessModifier;
        private AutoSync _autoSync;
        private bool _canBeNull;
        private string _dbType;
        private string _expression;
        private bool _isDbGenerated;
        private bool _isDelayLoaded;
        private bool _isDiscriminator;
        private bool _isPrimaryKey;
        private bool _isReadOnly;
        private bool _isVersion;
        private string _member;
        private MemberModifier _modifier;
        private string _name;
        private string _storage;
        private string _type;
        private UpdateCheck _updateCheck;

        public Column()
        {
            _accessModifier = AccessModifier.Public;
            _modifier = MemberModifier.None;
            _autoSync = AutoSync.Default;
            _canBeNull = true;
            _isDbGenerated = false;
            _isDelayLoaded = false;
            _isDiscriminator = false;
            _isPrimaryKey = false;
            _isReadOnly = false;
            _isVersion = false;
            _updateCheck = UpdateCheck.Always;
        }

        [XmlAttribute("Name")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [XmlAttribute("Member")]
        public string Member
        {
            get { return MemberSpecified ? _member : _name; }
            set { _member = value; _memberSpecified = true; }
        }

        private bool _memberSpecified = false;

        [XmlIgnore]
        public bool MemberSpecified
        {
            get { return _memberSpecified; }
            set { _memberSpecified = value; }
        }

        [XmlAttribute("Storage")]
        public string Storage
        {
            get { return StorageSpecified ? _storage : "_" + this.Member; }
            set { _storage = value; _storageSpecified = true; }
        }

        private bool _storageSpecified = false;

        [XmlIgnore]
        public bool StorageSpecified
        {
            get { return _storageSpecified; }
            set { _storageSpecified = value; }
        }

        [XmlAttribute("AccessModifier")]
        [DefaultValue(AccessModifier.Public)]
        public AccessModifier AccessModifier
        {
            get { return _accessModifier; }
            set { _accessModifier = value; }
        }

        [XmlAttribute("Modifier")]
        [DefaultValue(MemberModifier.None)]
        public MemberModifier Modifier
        {
            get { return _modifier; }
            set { _modifier = value; }
        }

        [XmlAttribute("Type")]
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        [XmlAttribute("DbType")]
        public string DbType
        {
            get { return _dbType; }
            set { _dbType = value; }
        }

        [XmlAttribute("IsReadOnly")]
        [DefaultValue(false)]
        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            set { _isReadOnly = value; }
        }

        [XmlAttribute("IsPrimaryKey")]
        [DefaultValue(false)]
        public bool IsPrimaryKey
        {
            get { return _isPrimaryKey; }
            set { _isPrimaryKey = value; }
        }

        [XmlAttribute("IsDbGenerated")]
        [DefaultValue(false)]
        public bool IsDbGenerated
        {
            get { return _isDbGenerated; }
            set { _isDbGenerated = value; }
        }

        [XmlAttribute("CanBeNull")]
        public bool CanBeNull
        {
            get { return _canBeNull; }
            set { _canBeNull = value; }
        }

        [XmlAttribute("UpdateCheck")]
        [DefaultValue(UpdateCheck.Always)]
        public UpdateCheck UpdateCheck
        {
            get { return _updateCheck; }
            set { _updateCheck = value; }
        }

        [XmlAttribute("IsDiscriminator")]
        [DefaultValue(false)]
        public bool IsDiscriminator
        {
            get { return _isDiscriminator; }
            set { _isDiscriminator = value; }
        }

        [XmlAttribute("Expression")]
        public string Expression
        {
            get { return _expression; }
            set { _expression = value; }
        }

        [XmlAttribute("IsVersion")]
        [DefaultValue(false)]
        public bool IsVersion
        {
            get { return _isVersion; }
            set { _isVersion = value; }
        }

        [XmlAttribute("IsDelayLoaded")]
        [DefaultValue(false)]
        public bool IsDelayLoaded
        {
            get { return _isDelayLoaded; }
            set { _isDelayLoaded = value; }
        }

        [XmlAttribute("AutoSync")]
        [DefaultValue(AutoSync.Default)]
        public AutoSync AutoSync
        {
            get { return _autoSync; }
            set { _autoSync = value; }
        }

        #region IProcessed Members
        private bool _isProcessed = false;

        [XmlIgnore]
        public bool IsProcessed
        {
            get { return _isProcessed; }
            set { _isProcessed = value; }
        }
        #endregion

        public override string ToString()
        {
            return this.Name;
        }

        private static readonly Regex _sizeRegex = new Regex(@"(?<Size>\d+)", RegexOptions.Compiled);
        private int? _size;
        private bool _isDbTypeParsed = false;

        [XmlIgnore]
        public int? Size
        {
            get 
            {
                if (!_size.HasValue && !_isDbTypeParsed)
                {
                    _size = GetDbTypeSize();
                    _isDbTypeParsed = true;
                }

                return _size; 
            }
            set { _size = value; }
        }

        private int? GetDbTypeSize()
        {
            int size = 0;
            if (string.IsNullOrEmpty(this.DbType))
                return null;

            Match m = _sizeRegex.Match(this.DbType);
            if (!m.Success)
                return null;

            string temp = m.Groups["Size"].Value;
            if (int.TryParse(temp, out size))
                return size;
            else
                return null;
        }
    }

    [Serializable]
    [XmlType(Namespace = DbmlConstant.DbmlNamespace)]
    public class TableFunctionReturn
    {
        private string _member;

        [XmlAttribute("Member")]
        public string Member
        {
            get { return _member; }
            set { _member = value; }
        }
    }

    [Serializable]
    [XmlType(Namespace = DbmlConstant.DbmlNamespace)]
    public class TableFunctionParameter
    {
        private string _member;
        private string _parameter;
        private Version _version;

        public TableFunctionParameter()
        {
            _version = Version.Current;
        }

        [XmlAttribute("Parameter")]
        public string Parameter
        {
            get { return _parameter; }
            set { _parameter = value; }
        }

        [XmlAttribute("Member")]
        public string Member
        {
            get { return _member; }
            set { _member = value; }
        }

        [XmlAttribute("Version")]
        [DefaultValue(Version.Current)]
        public Version Version
        {
            get { return _version; }
            set { _version = value; }
        }

    }

    [Serializable]
    [XmlType(Namespace = DbmlConstant.DbmlNamespace)]
    public class TableFunction
    {
        private AccessModifier _accessModifier;
        private TableFunctionParameterCollection _arguments;
        private string _functionId;
        private TableFunctionReturn _return;

        public TableFunction()
        {
            _accessModifier = AccessModifier.Public;
            _arguments = new TableFunctionParameterCollection();
        }

        [XmlElement("Argument")]
        public TableFunctionParameterCollection Arguments
        {
            get { return _arguments; }
            set { _arguments = value; }
        }

        public bool ShouldSerializeArguments()
        {
            return _arguments.Count > 0;
        }

        [XmlElement("Return")]
        public TableFunctionReturn Return
        {
            get { return _return; }
            set { _return = value; }
        }

        [XmlAttribute("FunctionId", DataType = "IDREF")]
        public string FunctionId
        {
            get { return _functionId; }
            set { _functionId = value; }
        }

        [XmlAttribute("AccessModifier")]
        [DefaultValue(AccessModifier.Public)]
        public AccessModifier AccessModifier
        {
            get { return _accessModifier; }
            set { _accessModifier = value; }
        }
    }

    [Serializable]
    [XmlType(Namespace = DbmlConstant.DbmlNamespace)]
    public class Table : IName, IProcessed
    {
        private AccessModifier _accessModifier;
        private TableFunction _deleteFunction;
        private TableFunction _insertFunction;
        private string _member;
        private MemberModifier _modifier;
        private string _name;
        private Type _type;
        private TableFunction _updateFunction;

        public Table()
        {
            _accessModifier = AccessModifier.Public;
            _modifier = MemberModifier.None;
        }

        [XmlElement("Type")]
        public Type Type
        {
            get { return _type; }
            set { _type = value; }
        }

        [XmlElement("InsertFunction")]
        public TableFunction InsertFunction
        {
            get { return _insertFunction; }
            set { _insertFunction = value; }
        }

        [XmlElement("UpdateFunction")]
        public TableFunction UpdateFunction
        {
            get { return _updateFunction; }
            set { _updateFunction = value; }
        }

        [XmlElement("DeleteFunction")]
        public TableFunction DeleteFunction
        {
            get { return _deleteFunction; }
            set { _deleteFunction = value; }
        }

        [XmlAttribute("Name")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [XmlAttribute("Member")]
        public string Member
        {
            get { return MemberSpecified ? _member : _name; }
            set { _member = value; _memberSpecified = true; }
        }

        private bool _memberSpecified = false;

        [XmlIgnore]
        public bool MemberSpecified
        {
            get { return _memberSpecified; }
            set { _memberSpecified = value; }
        }

        [XmlAttribute("AccessModifier")]
        [DefaultValue(AccessModifier.Public)]
        public AccessModifier AccessModifier
        {
            get { return _accessModifier; }
            set { _accessModifier = value; }
        }

        [XmlAttribute("Modifier")]
        [DefaultValue(MemberModifier.None)]
        public MemberModifier Modifier
        {
            get { return _modifier; }
            set { _modifier = value; }
        }

        #region IProcessed Members
        private bool _isProcessed = false;

        [XmlIgnore]
        public bool IsProcessed
        {
            get { return _isProcessed; }
            set { _isProcessed = value; }
        }
        #endregion

        public override string ToString()
        {
            return this.Name;
        }

    }

    #endregion

    #region Collections

    [Serializable]
    public class TableCollection : KeyedCollection<string, Table>
    {
        protected override string GetKeyForItem(Table item)
        {
            return item.Name;
        }

        public void Sort()
        {
            List<Table> tables = new List<Table>(base.Items);
            tables.Sort(delegate(Table x, Table y) { return x.Name.CompareTo(y.Name); });
            base.Clear();
            foreach (Table t in tables)
                base.Add(t);
        }
    }

    [Serializable]
    public class ColumnCollection : KeyedCollection<string, Column>, IProcessed
    {
        protected override string GetKeyForItem(Column item)
        {
            return item.Name;
        }

        #region IProcessed Members
        private bool _isProcessed = false;

        [XmlIgnore]
        public bool IsProcessed
        {
            get { return _isProcessed; }
            set { _isProcessed = value; }
        }
        #endregion
    }

    [Serializable]
    public class AssociationKey : IEquatable<AssociationKey>
    {
        public AssociationKey(string name, bool isForeignKey)
        {
            this.isForeignKey = isForeignKey;
            this.name = name;
        }

        private bool isForeignKey;

        public bool IsForeignKey
        {
            get { return isForeignKey; }
        }

        private string name;

        public string Name
        {
            get { return name; }
        }

        public override bool Equals(object obj)
        {
            if (obj is AssociationKey)
                return Equals((AssociationKey)obj);

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            string hash = name + isForeignKey.ToString();
            return hash.GetHashCode();
        }

        public bool Equals(AssociationKey other)
        {
            if (other == null)
                return false;

            return (name == other.Name && isForeignKey == other.IsForeignKey);
        }

        public static AssociationKey CreateForeignKey(string name)
        {
            return new AssociationKey(name, true);
        }

        public static AssociationKey CreatePrimaryKey(string name)
        {
            return new AssociationKey(name, false);
        }
    }

    [Serializable]
    public class AssociationCollection : KeyedCollection<AssociationKey, Association>, IProcessed
    {
        protected override AssociationKey GetKeyForItem(Association item)
        {
            return item.ToKey();
        }

        #region IProcessed Members
        private bool _isProcessed = false;

        [XmlIgnore]
        public bool IsProcessed
        {
            get { return _isProcessed; }
            set { _isProcessed = value; }
        }
        #endregion

    }

    [Serializable]
    public class TypeCollection : KeyedCollection<string, Type>
    {
        protected override string GetKeyForItem(Type item)
        {
            return item.Name;
        }
    }

    [Serializable]
    public class FunctionCollection : KeyedCollection<string, Function>
    {
        protected override string GetKeyForItem(Function item)
        {
            return item.Name;
        }
    }

    [Serializable]
    public class ParameterCollection : KeyedCollection<string, Parameter>
    {
        protected override string GetKeyForItem(Parameter item)
        {
            return item.Name;
        }
    }

    [Serializable]
    public class TableFunctionParameterCollection : KeyedCollection<string, TableFunctionParameter>
    {
        protected override string GetKeyForItem(TableFunctionParameter item)
        {
            return item.Parameter;
        }
    }

    #endregion

    #region Enums

    [Serializable]
    [XmlType(Namespace = DbmlConstant.DbmlNamespace)]
    public enum AutoSync
    {
        Never,
        OnInsert,
        OnUpdate,
        Always,
        Default,
    }

    [Serializable]
    [XmlType(Namespace = DbmlConstant.DbmlNamespace)]
    public enum ClassModifier
    {
        None,
        Sealed,
        Abstract,
    }

    [Serializable]
    [XmlType(Namespace = DbmlConstant.DbmlNamespace)]
    public enum ConnectionMode
    {
        ConnectionString,
        AppSettings,
        WebSettings,
    }

    [Serializable]
    [XmlType(Namespace = DbmlConstant.DbmlNamespace)]
    public enum ParameterDirection
    {
        In,
        Out,
        InOut,
    }

    [Serializable]
    [XmlType(Namespace = DbmlConstant.DbmlNamespace)]
    public enum AccessModifier
    {
        Public,
        Internal,
        Protected,
        ProtectedInternal,
        Private,
    }

    [Serializable]
    [XmlType(Namespace = DbmlConstant.DbmlNamespace)]
    public enum Cardinality
    {
        One,
        Many,
    }

    [Serializable]
    [XmlType(Namespace = DbmlConstant.DbmlNamespace)]
    public enum UpdateCheck
    {
        Always,
        Never,
        WhenChanged,
    }

    [Serializable]
    [XmlType(Namespace = DbmlConstant.DbmlNamespace)]
    public enum MemberModifier
    {
        None,
        Virtual,
        Override,
        New,
        NewVirtual,
    }

    [Serializable]
    [XmlType(Namespace = DbmlConstant.DbmlNamespace)]
    public enum SerializationMode
    {
        None,
        Unidirectional,
    }

    [Serializable]
    [XmlType(Namespace = DbmlConstant.DbmlNamespace)]
    public enum Version
    {
        Current,
        Original,
    }

    #endregion

    #region Interfaces

    public interface IName
    {
        string Name { get; set; }
    }

    public interface IMember : IName
    {
        AccessModifier AccessModifier { get; set; }
        string Member { get; set; }
        MemberModifier Modifier { get; set; }
        string Storage { get; set; }
        string Type { get; set; }
    }

    public interface IProcessed
    {
        bool IsProcessed { get; set; }
    }

    #endregion
}