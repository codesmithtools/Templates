#region 
//==============================================================================================
// CSLA 2.x CodeSmith Templates for C#
// Author: Ricky A. Supit (rsupit@hotmail.com)
//
// This software is provided free of charge. Feel free to use and modify anyway you want it.
//==============================================================================================
#endregion
using System;
using System.Collections;
using System.Text;
using System.Data;
using System.ComponentModel;
using System.Text.RegularExpressions;

using CodeSmith.Engine;
using SchemaExplorer;
using CodeSmith.CustomProperties;

using System.Xml;


namespace CodeSmith.Csla
{
    public class TemplateBase : CodeSmith.Engine.CodeTemplate 
    {

        #region Constants
        //member level variable prefix
        const string MemberPrefix = "_";
        //base class suffix
        const string BaseSuffix = "Base";
        //dal commandText formats (stored procedure name)
        const string FetchCommandFormat = "Get{0}";
        const string InsertCommandFormat = "Add{0}";
        const string UpdateCommandFormat = "Update{0}";
        const string DeleteCommandFormat = "Delete{0}";
        //database connection format
        const string DbConnectionFormat = "Database.{0}";
        //factory method formats
        const string FactoryNewFormat = "New{0}";
        const string FactoryGetFormat = "Get{0}";
        const string FactoryDeleteFormat = "Delete{0}";
        const string FactoryGetListFormat = "Get{0}By{1}";
        //generic type parameters
        const string GenericTypeObjectParameter = "T";
        protected const string GenericTypeChildParameter = "C";
        const string GenericTypeParentParameter = "P";
        //number of spaces to use for indentation, set to 0 to use tab indentation
        static int IndentLevelSpaces = 0; 
        static bool HandleNullableFields = true;
        //framework base class
        const string BusinessBase = "Csla.BusinessBase";
        const string BusinessListBase = "Csla.BusinessListBase";
        const string ReadOnlyBase = "Csla.ReadOnlyBase";
        const string ReadOnlyListBase = "Csla.ReadOnlyListBase";
        const string NameValueListBase = "Csla.NameValueListBase";
        #endregion //Constants

        #region Object Definition
        private string _classNamespace = "";
        [CodeTemplateProperty(CodeTemplatePropertyOption.Optional),
        Category("1. Object"),
        Description("Optional - The namespace that the generated Classes will be a member of.")]
        public string ClassNamespace
        {
            get { return _classNamespace; }
            set { if (value == null) value = ""; _classNamespace = value; }
        }

        private string _dalNamespace = "";
        [CodeTemplateProperty(CodeTemplatePropertyOption.Optional),
        Category("1. Object"),
        Description("Optional - The DAL namespace that the generated Classes will use.")]
        public string DalNamespace
        {
            get { return _dalNamespace; }
            set { if (value == null) value = ""; _dalNamespace = value; }
        }

        private string _baseClass = "";
        [CodeTemplateProperty(CodeTemplatePropertyOption.Optional),
        Category("1. Object"),
        Description("Optional - The customized base class that the generated class will inherit from.")]
        public string BaseClass
        {
            get { return _baseClass; }
            set { if (value == null) value = ""; _baseClass = value; }
        }
        #endregion //Object Definition

        #region Object Options
        private PropertyAccessSecurity _propertyAuthorization = PropertyAccessSecurity.Both;
        private bool _authorizationRules = true;

        [CodeTemplateProperty(CodeTemplatePropertyOption.Optional),
        Category("3. Options"),
        Description("What type of access security to use on properties?")]
        public PropertyAccessSecurity PropertyAuthorization
        {
            get { return _propertyAuthorization; }
            set { _propertyAuthorization = value; }
        }

        [CodeTemplateProperty(CodeTemplatePropertyOption.Optional),
        Category("3. Options"),
        Description("Use access security when using New/Get/Save/Delete method.")]
        public bool AuthorizationRules
        {
            get { return _authorizationRules; }
            set { _authorizationRules = value; }
        }

        private TransactionalTypes _transactionType = TransactionalTypes.None;

        [CodeTemplateProperty(CodeTemplatePropertyOption.Required),
        Category("3. Options"),
        Description("What type of Transaction should this Business Object use?")]
        public TransactionalTypes TransactionalType
        {
            get { return _transactionType; }
            set { _transactionType = value; }
        }

        #endregion //Object Options

        #region Code Generation
        private CodeGenerationMethod _codeGenMethod = CodeGenerationMethod.SplitPartial;
        private GenerationClassType _classType = GenerationClassType.Generated;

        [CodeTemplateProperty(CodeTemplatePropertyOption.Required),
        Category("4. Code Generation"),
        Description("Required - Generation Method. options are Single class, Partial class, Base class")]
        public CodeGenerationMethod GenerationMethod
        {
            get { return _codeGenMethod; }
            set { _codeGenMethod = value; }
        }

        [CodeTemplateProperty(CodeTemplatePropertyOption.Required),
        Category("4. Code Generation"),
        Description("Required - Class Type. Generated class or User class.")]
        public GenerationClassType ClassType
        {
            get { return _classType; }
            set { _classType = value; }
        }
        #endregion //Code Generation

        #region Other non visual properties
        private int _baseIndentLevel = 0;
        [Browsable(false)]
        public int BaseIndentLevel
        {
            get { return _baseIndentLevel; }
            set { _baseIndentLevel = value; }
        }
        private string _xmlFilePath = string.Empty;
        [Browsable(false)]
        public string XmlFilePath
        {
            get { return _xmlFilePath; }
            set { if (value == null) value = ""; _xmlFilePath = value; }
        }
	
        #endregion //Other non visual properties

        #region Rendering helper

        #region Properties and Methods
        /// <summary>
        /// return constraints to use when define generics class
        /// </summary>
        /// <param name="objInfo"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public string GetConstraint(ObjectInfo objInfo, int level)
        {
            string constraint = string.Empty;
            constraint = Indent(level, false) + objInfo.Constraint.Replace("\r\n", Indent(level, true));
            return constraint;
        }
        public string GetMemberDeclaration(PropertyInfo prop)
        {
            return GetMemberDeclaration(prop.MemberAccess, prop.Type, prop.MemberName, prop.DefaultValue);
        }
        // return member declaration statement
        public string GetMemberDeclaration(string access, string type, string name, string init)
        {
            if (init.Length > 0) init = " = " + init;
            return string.Format("{0} {1} {2}{3};", access, type, name, init);
        }
        #endregion //Properties and Methods

        #region Validation Rules
        /// <summary>
        /// Iterate through all properties and return commonly used validation rule statments
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public string GetCommonValidationRules(ObjectInfo obj, int level)
        {
            string rules = string.Empty;
            foreach (PropertyInfo prop in obj.Properties)
            {
                rules += GetCommonValidationRule(prop, level);
            }
            if (rules.Length > 0) rules = rules.Substring(2);
            return rules;
        }
        /// <summary>
        /// return commonly used validation rule statement:
        /// - StringRequired
        /// - StringMaxLength
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        private string GetCommonValidationRule(PropertyInfo prop, int level)
        {
            string rules = string.Empty;
            foreach (RuleInfo info in prop.ValidationRules)
            {
                rules += Indent(level, true)
                    + string.Format("ValidationRules.AddRule({0}, {1});", info.HandlerText, info.ArgumentText);
            }
            if (rules.Length > 0)
            {
                string indent = Indent(level, true) + "//";
                rules = string.Format("{0}{0} {1}{0}{2}", indent, prop.Name, rules);
            }
            return rules;
        }
        #endregion //Validation Rules

        #region Data Access
        /// <summary>
        /// return datareader get field statement
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public string GetReaderAssignmentStatement(PropertyInfo prop)
        {
            return DalHelper.ReaderStatement(prop.MemberName, prop.Type, prop.DbColumnName);
        }
        /// <summary>
        /// return parameter assignment for filter command
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public string GetFilterParameters(ObjectInfo obj, int level)
        {
            string statement = string.Empty;

            foreach (PropertyInfo prop in obj.FilterProperties)
            {
                statement += GetParameterStatement(prop, "", "criteria", true, level);
            }
            if (statement.Length > 0) statement = statement.Substring(2);
            return statement;
        }
        /// <summary>
        /// return parameter assignments for get command
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public string GetFetchParameters(ObjectInfo obj, int level)
        {
            return GetFetchParameters(obj, "", "criteria", level);
        }
        // internal method that return parameter assignment for get command
        public string GetFetchParameters(ObjectInfo obj, string parPrefix, string varPrefix, int level)
        {
            string statement = string.Empty;
            
            foreach (PropertyInfo prop in obj.UniqueProperties)
            {
                statement += GetParameterStatement(prop, parPrefix, varPrefix, true, level);
            }
            if (statement.Length > 0) statement = statement.Substring(2);
            return statement;
        }
        /// <summary>
        /// return paramenter assignments for insert command
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public string GetInsertParameters(ObjectInfo obj, int level)
        {
            string statement = string.Empty;
            string outputStatement = string.Empty;
            
            foreach (PropertyInfo prop in obj.Properties)
            {
                if (!prop.UpdateToDb) continue;

                if (!prop.IsDbComputed)
				{
                    if (prop.DefaultValue.Length > 0 && prop.AllowDbNull && prop.Type != "SmartDate")
						statement += GetDefaultConditionStatement(prop, "", level++);
                    statement += GetParameterStatement(prop, "", "", true, level);
                    if (prop.DefaultValue.Length > 0 && prop.AllowDbNull && prop.Type != "SmartDate")
                    {
                        statement += Indent(level - 1, true) + "else";
                        statement += Indent(level--, true) + DalHelper.ParameterAssignmentStatement(prop.DbColumnName, "DBNull.Value");
                    }
                }
                else
                {
                    outputStatement += GetParameterStatement(prop, "New", "", false, level);
                }
            }
            if (statement.Length > 0) statement = statement.Substring(2);
            return statement + outputStatement;
        }
        /// <summary>
        /// return paramenter assignments for update command
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public string GetUpdateParameters(ObjectInfo obj, int level)
        {
            string statement = string.Empty;
            string outputStatement = string.Empty;
            
            foreach (PropertyInfo prop in obj.Properties)
            {
                if (!prop.UpdateToDb) continue;

                if(prop.IsTimestamp)   //if timestamp add input parameter to support concurrency
                    statement += GetParameterStatement(prop, "", "", true, level);

                if (!prop.IsDbComputed)
				{
                    if (prop.DefaultValue.Length > 0 && prop.AllowDbNull && prop.Type != "SmartDate")
						statement += GetDefaultConditionStatement(prop, "", level++);
                    statement += GetParameterStatement(prop, "", "", true, level);
                    if (prop.DefaultValue.Length > 0 && prop.AllowDbNull && prop.Type != "SmartDate")
                    {
                        statement += Indent(level-1, true) + "else";
                        statement += Indent(level--, true) + DalHelper.ParameterAssignmentStatement(prop.DbColumnName, "DBNull.Value");
                    }
				}
                else
                {
                    if (prop.IsIdentity)
                        statement += GetParameterStatement(prop, "", "", true, level);
                    else
                        outputStatement += GetParameterStatement(prop, "New", "", false, level);
                }
            }
            if (statement.Length > 0) statement = statement.Substring(2);
            return statement + outputStatement;
        }

        // internal method that return parameter assigment
        public string GetParameterStatement(PropertyInfo prop, string parPrefix, string varPrefix, bool input, int level)
        {
            if (!prop.HasDbColumn) return string.Empty;
            string statement = string.Empty;
            string varName;
            string varType = prop.Type;

            //check if criteria property or variable
            if (varPrefix == "")
                varName = prop.MemberName;
            else if (varPrefix == "this")
                varName = varPrefix + "." + prop.MemberName;
            else
                varName = varPrefix + "." + prop.Name;

            if (prop.Type == "SmartDate") //special treatment on smartdate :(
            {
                if (input) 
                    varName += ".DBValue";
                else
                {
                    varName += "Date";
                    varType = "DateTime";
                }
            }
            statement = Indent(level, true)
                        + DalHelper.ParameterAssignmentStatement(parPrefix + prop.DbColumnName, varName);

            if(prop.IsTimestamp && prop.Type == "byte[]")  //Timestamp datatype converted to byte[]
                statement += Indent(level, true)
                        + string.Format("cm.Parameters[\"@{0}\"].SqlDbType = SqlDbType.Timestamp;", parPrefix + prop.DbColumnName);

            if (!input)
            {
                statement += Indent(level, true) 
                        + string.Format("cm.Parameters[\"@{0}\"].Direction = ParameterDirection.Output;", parPrefix + prop.DbColumnName);
            }
            return statement;
        }
        /// <summary>
        /// return statement to return output paramenter for insert command
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public string GetInsertReturnParameterStatements(ObjectInfo obj, int level)
        {
            string statement = string.Empty;

            foreach (PropertyInfo prop in obj.Properties)
            {
                if (!prop.HasDbColumn) continue;
                if (prop.IsDbComputed)
                    statement += Indent(level, true) + GetReturnParameterStatement(prop);
            }
            return statement;
        }
        /// <summary>
        /// return statement to return output parameter for update command
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public string GetUpdateReturnParameterStatements(ObjectInfo obj, int level)
        {
            string statement = string.Empty;

            foreach (PropertyInfo prop in obj.Properties)
            {
                if (!prop.HasDbColumn) continue;
                if (prop.IsDbComputed && !prop.IsIdentity)
                    statement += Indent(level, true) + GetReturnParameterStatement(prop);
            }
            return statement;
        }
        //internal method to return statement for retrieve output parameter
        public string GetReturnParameterStatement(PropertyInfo prop)
        {
            if (!prop.HasDbColumn) return string.Empty;
            string var = prop.MemberName;
            string par = "New" + prop.DbColumnName;
            string type = prop.Type;

            if (prop.Type == "SmartDate")
            {
                var += ".Date";
                type = "DateTime";
            }

            return DalHelper.ParameterReturnStatement(var, type, par); 
        }
        /// <summary>
        /// return statement to determine if nullable column equal to default value
        /// </summary>
		public string GetDefaultConditionStatement(PropertyInfo prop, string varPrefix, int level)
		{
			string statement = string.Empty;
            if(varPrefix.Length>0)
				if(prop.Type == "string")
					statement = Indent(level, true) + string.Format("if ({0}.{1}.Length > 0)", varPrefix, prop.MemberName);	
				else
					statement = Indent(level, true) + string.Format("if ({0}.{1} != {2})", varPrefix, prop.MemberName, prop.DefaultValue);
            else
				if(prop.Type == "string")
					statement = Indent(level, true) + string.Format("if ({0}.Length > 0)", prop.MemberName);
				else 
					statement = Indent(level, true) + string.Format("if ({0} != {1})", prop.MemberName, prop.DefaultValue);
			return statement;
		}
        public string GetNewNameValuePair(ObjectInfo obj)
        {
            PropertyInfo keyProp = (PropertyInfo)obj.UniqueProperties[0];
            PropertyInfo valueProp = null;
            foreach (PropertyInfo prop in obj.Properties)
            {
                if (!prop.IsPrimaryKey)
                {
                    valueProp = prop;
                    break;
                }
            }
            string format = "new NameValuePair(dr.{0}(\"{1}\"), dr.{2}(\"{3}\"))";
            return string.Format(format, DalHelper.GetReaderMethod(keyProp.Type), keyProp.DbColumnName,
                                           DalHelper.GetReaderMethod(valueProp.Type), valueProp.DbColumnName);
        }
        #endregion

        #region Factory Methods and Criteria 
        /// <summary>
        /// return parameter declaration in factory get collection
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public string GetFactoryFilterDeclarationArguments(ObjectInfo obj, params string[] args)
        {
            return GetMethodDeclaration(obj.FilterProperties, args);
        }
        /// <summary>
        /// return parameter call/pass statement in factory get collection
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string GetFactoryDPFilterCallArguments(ObjectInfo obj)
        {
            if (obj.FilterProperties.Count == 0) return string.Empty;
            if (obj.FilterProperties.Count == 1)
            {
                PropertyInfo prop = obj.FilterProperties[0] as PropertyInfo;
                return string.Format("new SingleCriteria<{0}, {1}>({2})", obj.Type, prop.Type, CsHelper.GetCamelCaseName(prop.Name));
            }
            string para = GetMethodArguments(obj.FilterProperties);
            if (para.Length > 0)
            {
                para = "new FilterCriteria(" + para + ")";
            }
            return para;
        }
        public string GetFactoryDPFilterDeclarationArguments(ObjectInfo obj, params string[] args)
        {
            if (obj.FilterProperties.Count == 0) return string.Join(", ", args);
            string result = string.Empty;
            if (obj.FilterProperties.Count == 1)
            {
                PropertyInfo prop = obj.FilterProperties[0] as PropertyInfo;
                result = string.Format("SingleCriteria<{0}, {1}> criteria", obj.Type, prop.Type);
            }
            else
                result = string.Format("FilterCriteria criteria");
            return result + (args.Length > 0 ? ", " : "") + string.Join(", ", args);
        }
        /// <summary>
        /// return assignment statement on filter criteria
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="level"></param>
        /// <param name="useMember"></param>
        /// <returns></returns>
        public string GetFactoryFilterAssignments(ObjectInfo obj, int level, bool useMember)
        {
            string members = string.Empty;
            foreach (PropertyInfo prop in obj.FilterProperties)
            {
                members += Indent(level, true) + string.Format("this.{0} = {1};", useMember? prop.MemberName: prop.Name, CsHelper.GetCamelCaseName(prop.Name));
            }
            if (members.Length > 0) members = members.Substring(2);
            return members;
        }
        //New
        public string GetFactoryNewDeclarationArguments(ObjectInfo obj, params string[] args)
        {
            if (obj.HasIdentity || obj.HasObjectGeneratedKey || obj.IsCollection)
                return string.Join(", ", args);
            return GetMethodDeclaration(obj.UniqueProperties, args);
        }
        public string GetFactoryNewCallArguments(ObjectInfo obj, params string[] args)
        {
            if (obj.HasIdentity || obj.HasObjectGeneratedKey || obj.IsCollection)
                return string.Join(", ", args);
            return GetMethodArguments(obj.UniqueProperties, args);
        }
        public string GetFactoryNewDPCallArguments(ObjectInfo obj)
        {
            if (obj.HasIdentity) return string.Empty;
            if (obj.HasObjectGeneratedKey) return string.Empty;
            return GetFactoryDPCallArguments(obj);
        }
        public string GetFactoryNewDPDeclarationArgutments(ObjectInfo obj, params string[] args)
        {
            if (obj.HasIdentity) return string.Join(", ", args);
            if (obj.HasObjectGeneratedKey) return string.Join(", ", args);
            return GetFactoryDPDeclarationArguments(obj, args);
        }
        public string GetFactoryDeclarationArguments(ObjectInfo obj)
        {
            return GetFactoryDeclarationArguments(obj, false);
        }
        public string GetFactoryDeclarationArguments(ObjectInfo obj, bool isNew)
        {
            if (obj.IsCollection) return string.Empty;
            if (obj.HasIdentity && isNew) return string.Empty;
            if (obj.HasObjectGeneratedKey && isNew) return string.Empty;

            string para = string.Empty;
            foreach (PropertyInfo prop in obj.UniqueProperties)
            {
                para += string.Format(", {0} {1}", prop.Type, CsHelper.GetCamelCaseName(prop.Name));
            }
            if (para.Length > 0) para = para.Substring(2);
            return para;
        }
        public string GetFactoryDPCallArguments(ObjectInfo obj)
        {
            if (obj.IsCollection) return string.Empty;
            if (obj.UniqueProperties.Count == 1)
            {
                PropertyInfo prop = obj.UniqueProperties[0] as PropertyInfo;
                return string.Format("new SingleCriteria<{0}, {1}>({2})", obj.Type, prop.Type, CsHelper.GetCamelCaseName(prop.Name));
            }
            string para = GetMethodArguments(obj.UniqueProperties);
            if (para.Length > 0)
            {
                para = "new Criteria(" + para + ")";
            }
            return para;
        }
        public string GetFactoryDPDeclarationArguments(ObjectInfo obj, params string[] args)
        {
            if (obj.IsCollection) return string.Join(", ", args);
            string result = string.Empty;
            if (obj.UniqueProperties.Count == 1)
            {
                PropertyInfo prop = obj.UniqueProperties[0] as PropertyInfo;
                result = string.Format("SingleCriteria<{0}, {1}> criteria", obj.Type, prop.Type);
            }
            else
                result =  string.Format("Criteria criteria");
            return result + (args.Length > 0 ? ", " : "") + string.Join(", ", args);
        }
        public string GetCriteriaDeclarationArguments(ObjectInfo obj)
        {
            return GetFactoryDeclarationArguments(obj);
        }
        public string GetCriteriaMemberAssignment(PropertyInfo prop)
        {
            return string.Format("this.{0} = {1};", prop.Name, CsHelper.GetCamelCaseName(prop.Name));
        }
        public string GetCriteriaPropertyDeclaration(PropertyInfo prop) 
        {
            return string.Format("public {0} {1};", prop.Type, prop.Name);
        }
        public string GetMethodDeclaration(ArrayList props, params string[] args)
        {
            ArrayList list = new ArrayList();
            foreach (PropertyInfo prop in props)
            {
                list.Add(string.Format("{0} {1}", prop.Type, CsHelper.GetCamelCaseName(prop.Name)));
            }
            list.AddRange(args);
            return string.Join(", ", (string[])list.ToArray(typeof(string)));
        }
        public string GetMethodArguments(ArrayList props, params string[] args)
        {
            ArrayList list = new ArrayList();
            foreach (PropertyInfo prop in props)
            {
                list.Add(CsHelper.GetCamelCaseName(prop.Name));
            }
            list.AddRange(args);
            return string.Join(", ", (string[])list.ToArray(typeof(string)));
        }
        #endregion

        #region Indent
        public string Indent()
        {
            return Indent(1);
        }
        public string Indent(int level)
        {
            return Indent(level, false);
        }

        public string Indent(int level, bool newLine)
        {
            string str = string.Empty;

            if (newLine) str = "\r\n";
            if (IndentLevelSpaces > 0)
                str += new string(' ', level * IndentLevelSpaces);
            else
                str += new string('\t', level);

            return str;
        }
        #endregion //Indent
        #endregion

        #region Object Class
        public class ObjectInfo
        {
            #region Template Settings
            private TransactionalTypes _transactionType = TransactionalTypes.None;
            private PropertyAccessSecurity _propertyAuthorization = PropertyAccessSecurity.Both;
            private bool _useSecurity = true;
            private CodeGenerationMethod _codeGenMethod = CodeGenerationMethod.Single;
            private GenerationClassType _classType = GenerationClassType.Generated;
            private string _baseClass = string.Empty;
            #endregion

            #region Parent Properties
            private string _parent = "";
            public string Parent
            {
                get 
                {
                    return _parent;
                }
            }
            public string ParentType
            {
                get
                {
                    return _parent;
                }
            }
            #endregion //Parent Properties

            #region Child Properties
            public string ChildType
            {
                get
                {
                    return _child;
                }
            }
            private string _child = "";
            public string Child
            {
                get 
                {
                    return _child; 
                }
            }
            public string CustomChild
            {
                get { return _child; }
            }
            #endregion //Child Properties

            #region Properties
            private string _namespace;
            public string Namespace
            {
                get { return _namespace; }
                set { _namespace = value; }
            }
	
            private string _objectName;
            public string Name
            {
                get 
                {
                    return _objectName;
                }
            }
            public string CustomName
            {
                get { return _objectName; }
            }
            public string Type
            {
                get
                {
                    return _objectName;
                }
            }
            private ArrayList _childCollection = new ArrayList();
            public ArrayList ChildCollection
            {
                get { return _childCollection; }
            }

            private ArrayList _uniqueProperties = new ArrayList();
            public ArrayList UniqueProperties
            {
                get { return _uniqueProperties; }
            }

            private ArrayList _filterProperties = new ArrayList();
            public ArrayList FilterProperties
            {
                get { return _filterProperties; }
                set { _filterProperties = value; }
            }
            private ArrayList _properties = new ArrayList();
            public ArrayList Properties
            {
                get { return _properties; }
            }

            private ObjectType _objectType;
            public ObjectType CslaObjectType
            {
                get { return _objectType; }
            }
            private string _dalNamespace;
            public string DalNamespace
            {
                get { return _dalNamespace; }
            }
            #endregion //Properties

            #region Methods
            public string MemberAccess
            {
                get 
                {
                    return "private";
                }
            }
            public string NewMethodName
            {
                get { return string.Format(FactoryNewFormat, _objectName); }
            }
            public string GetMethodName
            {
                get { return string.Format(FactoryGetFormat, _objectName); }
            }
            public string DeleteMethodName
            {
                get { return string.Format(FactoryDeleteFormat, _objectName); }
            }
            public string NewChildMethodName
            {
                get { return string.Format(FactoryNewFormat, _child); }
            }
            public string GetChildMethodName
            {
                get { return string.Format(FactoryGetFormat, _child); }
            }
            public string LocalMethodModifiers
            {
                get
                {
                    return MemberAccess;
                }
            }
            public bool UseSecurity
            {
                get { return _useSecurity; }
            }
            public PropertyAccessSecurity PropertyAuthorization
            {
                get { return _propertyAuthorization; }
            }
            #endregion
            
            #region Object Characteristics
            public CodeGenerationMethod GenerationMethod
            {
                get { return _codeGenMethod; }
            }
            public bool IsCollection
            {
                get 
                {
                    switch (_objectType)
                    {
                        case ObjectType.EditableRoot: return false;
                        case ObjectType.EditableRootList: return true;
                        case ObjectType.EditableChild: return false;
                        case ObjectType.EditableChildList: return true;
                        case ObjectType.EditableSwitchable: return false;
                        case ObjectType.NameValueList: return true;
                        case ObjectType.ReadOnlyRoot: return false;
                        case ObjectType.ReadOnlyRootList: return true;
                        case ObjectType.ReadOnlyChild: return false;
                        case ObjectType.ReadOnlyChildList: return true;
                    }
                    return false;
                }
            }
            public bool IsReadOnly
            {
                get
                {
                    switch (_objectType)
                    {
                        case ObjectType.EditableRoot: return false;
                        case ObjectType.EditableRootList: return false;
                        case ObjectType.EditableChild: return false;
                        case ObjectType.EditableChildList: return false;
                        case ObjectType.EditableSwitchable: return false;
                        case ObjectType.NameValueList: return true;
                        case ObjectType.ReadOnlyRoot: return true;
                        case ObjectType.ReadOnlyRootList: return true;
                        case ObjectType.ReadOnlyChild: return true;
                        case ObjectType.ReadOnlyChildList: return true;
                    }
                    return false;
                }
            }
            public bool IsChild
            {
                get
                {
                    switch (_objectType)
                    {
                        case ObjectType.EditableRoot: return false;
                        case ObjectType.EditableRootList: return false;
                        case ObjectType.EditableChild: return true;
                        case ObjectType.EditableChildList: return true;
                        case ObjectType.EditableSwitchable: return true;
                        case ObjectType.NameValueList: return false;
                        case ObjectType.ReadOnlyRoot: return false;
                        case ObjectType.ReadOnlyRootList: return false;
                        case ObjectType.ReadOnlyChild: return true;
                        case ObjectType.ReadOnlyChildList: return true;
                    }
                    return false;
                }
            }

            public bool IsGeneratedClass
            {
                get
                {
                    if (_codeGenMethod == CodeGenerationMethod.Single) return true;
                    if (_classType == GenerationClassType.Generated) return true;
                    return false;
                }
            }
            public bool IsUserClass
            {
                get
                {
                    if (_codeGenMethod == CodeGenerationMethod.Single) return true;
                    if (_classType == GenerationClassType.User) return true;
                    return false;
                }
            }
            public bool IsSingle
            {
                get { return _codeGenMethod == CodeGenerationMethod.Single; }
            }
            public bool IsPartial
            {
                get { return _codeGenMethod == CodeGenerationMethod.SplitPartial ;}
            }
            public bool IsGeneratedPartial
            {
                get { return _codeGenMethod == CodeGenerationMethod.SplitPartial && _classType == GenerationClassType.Generated; }
            }
            public bool IsUserPartial
            {
                get { return _codeGenMethod == CodeGenerationMethod.SplitPartial && _classType == GenerationClassType.User; }
            }
            public bool HasIdentity
            {
                get
                {
                    foreach (PropertyInfo prop in UniqueProperties)
                    {
                        if (prop.IsIdentity) return true;
                    }
                    return false;
                }
            }
            public bool HasDbComputed
            {
                get
                {
                    foreach (PropertyInfo prop in Properties)
                    {
                        if (prop.IsDbComputed) return true;
                    }
                    return false;
                }
            }
            public bool HasTimestamp
            {
                get
                {
                    foreach (PropertyInfo prop in Properties)
                    {
                        if (prop.IsTimestamp) return true;
                    }
                    return false;
                }
            }
            public bool HasChild
            {
                get
                {                    
                    return _childCollection.Count>0;
                }
            }
            public bool HasObjectGeneratedKey
            {
                get
                {
                    return _uniqueProperties.Count == 1 && ((PropertyInfo)_uniqueProperties[0]).Type == "Guid";
                }
            }
            #endregion

            #region Keywords
            private string _access = "public";
            public string Access
            {
                get { return _access; }
            }
            public string Modifiers
            {
                get
                {
                    string modifiers = Access;
                    if (IsPartial) 
                        modifiers += " partial";
                    return modifiers;
                }
            }
            public string Inherits
            {
                get
                {
                    //other types
                    switch (_objectType)
                    {
                        case ObjectType.EditableRoot:
                        case ObjectType.EditableChild:
                        case ObjectType.EditableSwitchable:
                        case ObjectType.ReadOnlyRoot:
                        case ObjectType.ReadOnlyChild:
                            return string.Format(" : {0}<{1}>", _baseClass, Type);
                        case ObjectType.EditableChildList:
                        case ObjectType.EditableRootList:
                        case ObjectType.ReadOnlyRootList:
                        case ObjectType.ReadOnlyChildList:
                            return string.Format(" : {0}<{1}, {2}>", _baseClass, Type, ChildType);
                        case ObjectType.NameValueList:
                            return string.Format(" : {0}<{1}, {2}>", _baseClass, ((PropertyInfo)_uniqueProperties[0]).Type, ((PropertyInfo)_properties[1]).Type);
                    }
                    return string.Empty;
                }
            }
            public string Constraint
            {
                get
                {
                    switch (_objectType)
                    {
                        case ObjectType.EditableRoot:
                        case ObjectType.EditableChild:
                        case ObjectType.EditableSwitchable:
                        case ObjectType.ReadOnlyRoot:
                        case ObjectType.ReadOnlyChild:
                            return string.Format("where {0} : {1}", Type , Name);
                        case ObjectType.EditableChildList:
                        case ObjectType.EditableRootList:
                        case ObjectType.ReadOnlyRootList:
                        case ObjectType.ReadOnlyChildList:
                            return string.Format("where {0} : {1}\r\nwhere {2} : {3}", Type, Name, ChildType, Child);
                    }
                    return string.Empty;

                }
            }
            #endregion

            #region Data Access
            public TransactionalTypes TransactionType
            {
                get { return _transactionType; }
            }
            public bool UseAdoTransaction
            {
                get { return _transactionType == TransactionalTypes.Ado; }
            }
            public bool UseTransactionalAttribute
            {
                get { return _transactionType == TransactionalTypes.EnterpriseService || _transactionType == TransactionalTypes.TransactionScope; }
            }
            string _dbName = string.Empty;
            public string DbConnection
            {
                get { return string.Format(DbConnectionFormat, _dbName); }
            }
            public string LinqDataContext
            {
                get { return string.Format("{0}.{1}DataContext", _dalNamespace, _dbName); }
            }
            string _entityName = string.Empty;
            public string EntityName
            {
                get { return _entityName; }
            }
            public string EntitySetName
            {
                get { return CsHelper.ConvertToPlural(_entityName); }
            }
            string _fetchCommand = string.Empty;
            public string FetchCommandText
            {
                get { return _fetchCommand; }
            }
            string _insertCommand = string.Empty;
            public string InsertCommandText
            {
                get { return _insertCommand; }
            }
            string _updateCommand = string.Empty;
            public string UpdateCommandText
            {
                get { return _updateCommand; }
            }
            string _deleteCommand = string.Empty;
            public string DeleteCommandText
            {
                get { return _deleteCommand; }
            }
            #endregion

            #region Constructors
            public ObjectInfo(CodeTemplate template)
            {
                if (!TemplateHelper.IsObjectType(template.CodeTemplateInfo))
                    throw new ArgumentException(string.Format("Template '{0}' is not a business object template type", template.CodeTemplateInfo.FileName));

                Initialize(template);

                string xmlpath = (string)template.GetProperty("XmlFilePath");
                bool isFromXml = (xmlpath != null && xmlpath.Length > 0);
                if (isFromXml)
                    LoadFromXml(template);
                else
                    LoadFromSchema(template);

                if (_baseClass.Length == 0)
                {
                    //assign base class
                    switch (_objectType)
                    {
                        case ObjectType.EditableRoot:
                        case ObjectType.EditableChild:
                        case ObjectType.EditableSwitchable:
                            _baseClass = BusinessBase;
                            break;
                        case ObjectType.ReadOnlyRoot:
                        case ObjectType.ReadOnlyChild:
                            _baseClass = ReadOnlyBase;
                            break;
                        case ObjectType.EditableChildList:
                        case ObjectType.EditableRootList:
                            _baseClass = BusinessListBase;
                            break;
                        case ObjectType.ReadOnlyRootList:
                        case ObjectType.ReadOnlyChildList:
                            _baseClass = ReadOnlyListBase;
                            break;
                        case ObjectType.NameValueList:
                            _baseClass = NameValueListBase;
                            break;
                    }
                }

                //validate object
                Validate();
            }

            private void LoadFromXml(CodeTemplate template)
            {
                //read from xml file
                string path = (string)template.GetProperty("XmlFilePath");

                XmlTextReader xtr = new XmlTextReader(path);

                if(!MoveToObject(xtr, _objectName))
                    throw new ApplicationException(string.Format("Object {0} does not exist!", _objectName));
             
                //read object attributes
                while (xtr.MoveToNextAttribute())
	            {
                    switch (xtr.LocalName.ToLower())
                    {
                        case "namespace":
                            _namespace = xtr.Value;
                            break;
                        case "dalnamespace":
                            _dalNamespace = xtr.Value;
                            break;
                        case "access":                            
                            _access = xtr.Value;
                            break;
                        case "type":
                            _objectType = (ObjectType)Enum.Parse(typeof(ObjectType), xtr.Value, true);
                            break;
                        case "base":
                            _baseClass = xtr.Value;
                            break;
                    }
	            }
                if (_entityName == string.Empty)
                    _entityName = _objectName;

                //read object elements
                while (xtr.Read())
                {
                    if (xtr.NodeType == XmlNodeType.EndElement 
                        && xtr.LocalName.ToLower() == "object")
                        break;  //reach end of object node
                    if (xtr.NodeType == XmlNodeType.Element) {
                        switch (xtr.LocalName.ToLower())
                        {
                            case "properties":
                                LoadProperties(xtr); //read properties
                                break;
                            case "transactionaltype":
                                _transactionType = (TransactionalTypes)Enum.Parse(typeof(TransactionalTypes), xtr.ReadElementString());
                                break;
                            case "propertyauthorization":
                                _propertyAuthorization = (PropertyAccessSecurity)Enum.Parse(typeof(PropertyAccessSecurity), xtr.ReadElementString());
                                break;
                            case "authorizationrules":
                                _useSecurity = bool.Parse(xtr.ReadElementString());
                                break;
                            case "relationship":
                                while (xtr.MoveToNextAttribute())
                                {
                                    switch (xtr.LocalName.ToLower())
                                    {
                                        case "parent":
                                            _parent = xtr.Value;
                                            break;
                                        case "child":
                                            _child = xtr.Value;
                                            break;
                                    }
                                }
                                break;
                            case "dbcommands":
                                _dbName = xtr.GetAttribute("DbName");
                                while (xtr.Read())
                                {
                                    if (xtr.NodeType == XmlNodeType.EndElement
                                        && xtr.LocalName.ToLower() == "dbcommands")
                                        break;  //reach end of properties node
                                    if (xtr.NodeType == XmlNodeType.Element)
                                    {
                                        switch (xtr.LocalName.ToLower())
	                                    {
		                                    case "fetchcommand":
                                                _fetchCommand = xtr.ReadElementString();
                                                break;
                                            case "insertcommand":
                                                _insertCommand = xtr.ReadElementString();
                                                break;
                                            case "updatecommand":
                                                _updateCommand = xtr.ReadElementString();
                                                break;
                                            case "deletecommand":
                                                _deleteCommand = xtr.ReadElementString();
                                                break;
	                                    }
                                    }
                                }
                                break;
                        }
                    }
                    
                }   //whild(xtr.Read())

                xtr.Close();

            }

            private bool MoveToObject(XmlTextReader xtr, string objectName)
            {
                while (xtr.Read())
                {
                    if (xtr.NodeType == XmlNodeType.Element 
                        && xtr.LocalName.ToLower() == "object")
                    {
                        if(xtr["name"] != null && xtr.GetAttribute("name") == objectName)
                            return true;
                        if (xtr["Name"] != null && xtr.GetAttribute("Name") == objectName)
                            return true;
                    }
                }
                return false;
            }

            private void LoadProperties(XmlTextReader xtr)
            {
                while (xtr.Read())
                {
                    if (xtr.NodeType == XmlNodeType.EndElement 
                        && xtr.LocalName.ToLower() == "properties")
                        break;  //reach end of properties node
                    if (xtr.NodeType == XmlNodeType.Element 
                        && xtr.LocalName.ToLower() == "property")
                    {
                        PropertyInfo prop = new PropertyInfo(xtr, this);
                        _properties.Add(prop);

                        if (prop.IsChildCollection)
                            _childCollection.Add(prop);
                        if (prop.IsPrimaryKey)
                            _uniqueProperties.Add(prop);
                        if (prop.IsFilterKey)
                            _filterProperties.Add(prop);
                    }
                }
            }

            private void LoadFromSchema(CodeTemplate template)
            {
                _objectType = TemplateHelper.ToObjectType(template.CodeTemplateInfo);

                //child, and parent name
                if (IsCollection)
                    _child = (string)template.GetProperty("ChildName");
                if (IsChild)
                    _parent = (string)template.GetProperty("ParentName");
                if (_parent == null) _parent = string.Empty;

                //child collections
                StringCollection types = (StringCollection)template.GetProperty("ChildCollectionTypes");
                StringCollection names = (StringCollection)template.GetProperty("ChildPropertyNames");
                StringCollection sets = (StringCollection)template.GetProperty("ChildEntitySets");
                if (types != null && types.Count > 0)
                {
                    for (int i = 0; i < types.Count; i++)
                    {
                        string type = types[i].TrimEnd();
                        if (type == string.Empty) continue;

                        string name = (names != null && i < names.Count) ? names[i] : type;
                        string set = (sets != null && i < sets.Count) ? sets[i] : types[i];

                        PropertyInfo prop = new PropertyInfo(name, type, set, this);
                        _properties.Add(prop);
                        _childCollection.Add(prop);
                    }
                }

                //table, view schema
                TableSchema table = (TableSchema)template.GetProperty("RootTable");
                ViewSchema view = (ViewSchema)template.GetProperty("RootView");
				CommandSchema command = (CommandSchema)template.GetProperty("RootCommand");
				int resultSetIndex = (int)template.GetProperty("ResultSetIndex");
                if (table == null && view == null && command == null)
                    throw new Exception("RootCommand, RootTable or RootView is required.");

                StringCollection uniqueColumns = (StringCollection)template.GetProperty("UniqueColumnNames");
                if (uniqueColumns == null) uniqueColumns = new StringCollection();

                StringCollection filterColumns = (StringCollection)template.GetProperty("FilterColumnNames");
                if (filterColumns == null) filterColumns = new StringCollection();

				if (command != null)
				{
                    _dbName = ((SchemaObjectBase)command).Database.Name;
                    _entityName = CsHelper.ConvertToSingular(((SchemaObjectBase)command).Name);
                    if (resultSetIndex == 0) _fetchCommand = command.Name;
                    LoadProperties(command, resultSetIndex, uniqueColumns, filterColumns);
				}
                else if (table != null)
                {
                    _dbName = ((SchemaObjectBase)table).Database.Name;
                    _entityName = CsHelper.ConvertToSingular(((SchemaObjectBase)table).Name);
                    LoadProperties(table);
                }
                else
                {
                    _dbName = ((SchemaObjectBase)view).Database.Name;
                    _entityName = CsHelper.ConvertToSingular(((SchemaObjectBase)view).Name);
                    LoadProperties(view, uniqueColumns, filterColumns);
                }
            }

            private void LoadProperties(TableSchema table)
            {
                foreach (ColumnSchema col in table.Columns)
                {
                    PropertyInfo prop = new PropertyInfo(col, this);
                    
                    _properties.Add(prop);
                    if (prop.IsPrimaryKey)
                        _uniqueProperties.Add(prop);
                    if (prop.IsFilterKey)
                        _filterProperties.Add(prop);
                }
            }

            private void LoadProperties(ViewSchema view, StringCollection uniqueColumns, StringCollection filterColumns)
            {
                foreach (ViewColumnSchema col in view.Columns)
                {
                    //need case insensitive
                    //bool isUniqueMember = uniqueColumns.Contains(col.Name);
                    bool isUniqueMember = uniqueColumns.ToString().ToLower().IndexOf(col.Name.ToLower()) >= 0;
                    bool isFilterMember = filterColumns.ToString().ToLower().IndexOf(col.Name.ToLower()) >= 0;

                    PropertyInfo prop = new PropertyInfo(col, this, isUniqueMember, isFilterMember);

                    _properties.Add(prop);

                    if (prop.IsPrimaryKey)
                        _uniqueProperties.Add(prop);
                    if (prop.IsFilterKey)
                        _filterProperties.Add(prop);
                }
            }
			
			private void LoadProperties(CommandSchema command, int resultSetIndex, StringCollection uniqueColumns, StringCollection filterColumns)
			{
                foreach (CommandResultColumnSchema col in command.CommandResults[resultSetIndex].Columns)
                {
					bool isUniqueMember = false;
					bool isFilterMember = false;
					if (resultSetIndex == 0 && !IsChild && CslaObjectType != ObjectType.NameValueList)
					{
						bool isParameterMember = command.InputParameters.Contains("@" + col.Name);
						isUniqueMember = isParameterMember && !IsCollection;
						isFilterMember = isParameterMember && IsCollection;
					}
					else
					{
						isUniqueMember = uniqueColumns.ToString().ToLower().IndexOf(col.Name.ToLower()) >= 0;
						isFilterMember = filterColumns.ToString().ToLower().IndexOf(col.Name.ToLower()) >= 0;
					}

                    PropertyInfo prop = new PropertyInfo(col, this, isUniqueMember, isFilterMember);

                    _properties.Add(prop);

                    if (prop.IsPrimaryKey)
                        _uniqueProperties.Add(prop);
                    if (prop.IsFilterKey)
                        _filterProperties.Add(prop);
                }
			}
            private void Initialize(CodeTemplate template)
            {
                _namespace = (string)template.GetProperty("ClassNamespace");
                _dalNamespace = (string)template.GetProperty("DalNamespace");
                if (_dalNamespace == string.Empty) _dalNamespace = "Dal";
                _objectName = (string)template.GetProperty("ObjectName");
                _baseClass = (string)template.GetProperty("BaseClass");

                //template settings
                _transactionType = (TransactionalTypes)template.GetProperty("TransactionalType");
                _propertyAuthorization = (PropertyAccessSecurity)template.GetProperty("PropertyAuthorization");
                _useSecurity = (bool)template.GetProperty("AuthorizationRules");
                _codeGenMethod = (CodeGenerationMethod)template.GetProperty("GenerationMethod");
                _classType = (GenerationClassType)template.GetProperty("ClassType");

                //db commands
                _fetchCommand = string.Format(FetchCommandFormat, _objectName);
                _insertCommand = string.Format(InsertCommandFormat, _objectName);
                _updateCommand = string.Format(UpdateCommandFormat, _objectName);
                _deleteCommand = string.Format(DeleteCommandFormat, _objectName);
            }
            private void Validate()
            {
                if (_objectName == null || _objectName.Length == 0)
                    throw new Exception("ObjectName is required.");
                if (_uniqueProperties.Count == 0 && !IsCollection)
                    throw new Exception("Unique Column(s) is required.");
                if (IsCollection && (_child == null || _child.Length == 0) && CslaObjectType != ObjectType.NameValueList)
                    throw new Exception("Child is required.");
            }
            #endregion //Constructors
        }
        #endregion //Object Class

        #region Property Class
        public class PropertyInfo
        {
            private ObjectInfo _parent;
            private string _name;
            private string _type;
            private string _nativeType;
            private string _access = "public";
            private string _defaultValue = string.Empty;
            private string _dbColumnName = string.Empty;
            private string _dbRefTableName = string.Empty;
            private bool _isIdentity = false;
            private bool _isPrimaryKey = false;
            private bool _isFilterKey = false;
            private bool _allowDbNull = false;
            private bool _isChildCollection = false;
            private bool _isReadOnly = false;
            private bool _isComputed = false;
            private bool _isTimestamp = false;

            private ArrayList _validationRules = new ArrayList();
            public string Name
            {
                get { return _name; }
            }

            public string Type
            {
                get { return _type; }
            }
            public string NativeType
            {
                get { return _nativeType; }
            }
            public string Access
            {
                get { return _access; }
            }

            //property/field-modifiers, used as part of property declaration
            public string Modifiers
            {
                get
                {
                    string modifiers = Access;
                    return modifiers;
                }
            }
            //PropertyInfo name used during property registration
            public string PropertyInfoName { get { return _name + "Property"; } }
            public string MemberName
            {
                get { return MemberPrefix + CsHelper.GetCamelCaseName(this.Name); }
            }
            public string MemberAccess
            {
                get { return _parent.MemberAccess; }
            }
            public string DefaultValue
            {
                get { return _defaultValue; }
            }
            public PropertyAccessSecurity Authorization 
            {
                get { return _parent.PropertyAuthorization; }
            }
            public string DbColumnName 
            {
                get { return _dbColumnName; }
            }
            public string DbRefTableName
            {
                get { return _dbRefTableName; }
            }
            public bool IsReadOnly
            {
                get 
                {
                    //read only field is:
                    //parent is read only or object is read only
                    //identity column or part of primary key column(s)
                    //collection object or csla child object
                    if (_parent.IsReadOnly || _isReadOnly || _isIdentity || _isPrimaryKey 
                                    || _isChildCollection || _isTimestamp || _isComputed)
                        return true;
                    return false;
                }
            }
            public bool IsIdentity
            {
                get { return _isIdentity; }
            }
            public bool IsPrimaryKey
            {
                get { return _isPrimaryKey; }
            }
            public bool IsFilterKey
            {
                get { return _isFilterKey; }
            }
            public bool IsDbComputed
            {
                get
                {
                    if (_isIdentity || _isTimestamp || _isComputed) 
                        return true;

                    return false;
                }
            }
            public bool IsTimestamp
            {
                get { return _isTimestamp; }
            }
            public bool IsSmartType
            {
                get { return _type == "SmartDate"; }
            }
            public bool HasDbColumn
            {
                get { return _dbColumnName != string.Empty; }
            }
            public bool UpdateToDb
            {
                get { return !_isReadOnly && HasDbColumn; }
            }
            public bool AllowDbNull
            {
                get { return HandleNullableFields && _allowDbNull; }
            }
            public bool IsChildCollection
            {
                get { return _isChildCollection; }
            }
            public ArrayList ValidationRules
            {
                get { return _validationRules; }
            }
            public PropertyInfo(DataObjectBase column, ObjectInfo parent)
            {
                _parent = parent;
                Load(column);
            }
            public PropertyInfo(DataObjectBase column, ObjectInfo parent, bool isPrimaryKey, bool isFilterKey)
            {
                _parent = parent;
                Load(column);
                _isPrimaryKey = isPrimaryKey;
                _isFilterKey = isFilterKey;
            }
            public PropertyInfo(string cslaCollName, string cslaCollType, string dbRefTable, ObjectInfo parent)
            {
                _parent = parent;
                _name = cslaCollName;
                _type = cslaCollType;
                _nativeType = _type == "SmartDate" ? "DateTime" : _type;
                _dbRefTableName = dbRefTable;

                _isChildCollection = true;
                if(!parent.IsReadOnly)
                    _defaultValue = string.Format("{0}.New{0}()", _type);
            }
            public PropertyInfo(XmlTextReader xtr, ObjectInfo parent)
            {
                _parent = parent;
                while (xtr.MoveToNextAttribute())
                {                    
                    switch (xtr.LocalName.ToLower())
                    {
                        case "name": 
                            _name = xtr.Value;
                            break;
                        case "type":
                            _type = xtr.Value;
                            break;
                        case "nativetype":
                            _nativeType = xtr.Value;
                            break;
                        case "access":
                            _access = xtr.Value;
                            break;
                        case "default":
                            _defaultValue = xtr.Value;
                            break;
                        case "dbcolumnname":
                            _dbColumnName = xtr.Value;
                            break;
                        case "entitysetname":
                            _dbRefTableName = xtr.Value;
                            break;
                        case "isidentity":
                            _isIdentity = bool.Parse(xtr.Value);
                            break;
                        case "isprimarykey":
                            _isPrimaryKey = bool.Parse(xtr.Value);
                            break;
                        case "isfilterkey": //
                            _isFilterKey = bool.Parse(xtr.Value);
                            break;
                        case "allowdbnull":
                            _allowDbNull = bool.Parse(xtr.Value);
                            break;
                        case "isreadonly":
                            _isReadOnly = bool.Parse(xtr.Value);
                            break;
                        case "ischildcollection":
                            _isChildCollection = bool.Parse(xtr.Value);
                            break;
                    }
	            }
                xtr.MoveToElement();
                if (!xtr.IsEmptyElement)
                {
                    while (xtr.Read())
                    {
                        if (xtr.NodeType == XmlNodeType.EndElement
                            && xtr.LocalName.ToLower() == "property")
                            break;  //reach end of property node
                        if (xtr.NodeType == XmlNodeType.Element
                            && xtr.LocalName.ToLower() == "validationrules")
                        {
                            LoadValidationRules(xtr);
                        }
                    }
                }
                if (_name == string.Empty)
                    throw new Exception("Name is required in property");
                if (_type == string.Empty)
                    throw new Exception("Type is required in property");

                if(_nativeType == null || _nativeType == string.Empty)
                    _nativeType = _type == "SmartDate" ? "DateTime" : _type;
                if (_defaultValue.Length == 0)
                    _defaultValue = CsHelper.GetDefaultValue(_type);
                //check for timestamp
                if (_type.ToLower() == "timestamp")
                {
                    _type = "byte[]";
                    _isTimestamp = true;
                }
            }
            private void Load(DataObjectBase col)
            {
                _dbColumnName = col.Name;
                _name = CsHelper.GetPropertyName(col);

                _type = CsHelper.GetVariableType(col);
                _nativeType = _type == "SmartDate" ? "DateTime" : _type;
                if (col.AllowDBNull && _type != "string") _nativeType += "?";   //nullable when value types 

                _defaultValue = CsHelper.GetDefaultValue(col);
                _isIdentity = CsHelper.IsIdentity(col);
                ColumnSchema column = col as ColumnSchema;
                if (column != null)
                {
                    _isPrimaryKey = column.IsPrimaryKeyMember;
                    _isFilterKey = column.IsForeignKeyMember;
                }
                _allowDbNull = col.AllowDBNull;
                _isComputed = CsHelper.IsComputed(col);
                _isTimestamp = CsHelper.IsTimestamp(col);

                //required string
                if (_allowDbNull == false && _type == "string")
                    _validationRules.Add(new RuleInfo(this, CommonRuleHandler.StringRequired, string.Empty, 0));
                //fixsize string is <= 8000, add max length rule
                if (_type == "string" && col.Size > 0 && col.Size <= 8000 && col.NativeType != "text" && col.NativeType != "ntext")
                    _validationRules.Add(new RuleInfo(this, CommonRuleHandler.StringMaxLength, col.Size.ToString(), 0));
                //email
                if (_type == "string" && _name.ToLower().IndexOf("email")>=0)
                    _validationRules.Add(new RuleInfo(this, CommonRuleHandler.RegExMatch, "Email", 0));
                //ssn
                if (_type == "string" && _name.ToLower().IndexOf("ssn") >= 0)
                    _validationRules.Add(new RuleInfo(this, CommonRuleHandler.RegExMatch, "SSN", 0));
            }
            private void LoadValidationRules(XmlTextReader xtr)
            {
                if (xtr.IsEmptyElement) return;
                while (xtr.Read())
                {
                    if (xtr.NodeType == XmlNodeType.EndElement
                        && xtr.LocalName.ToLower() == "validationrules")
                        break;  //reach end of property node
                    if (xtr.NodeType == XmlNodeType.Element
                        && Enum.IsDefined(typeof(CommonRuleHandler), xtr.LocalName))
                    {
                        RuleInfo rule = new RuleInfo(this, xtr);
                        _validationRules.Add(rule);
                    }
                }
            }
        }
        #endregion //Property Class

        #region RuleInfo
        public class RuleInfo
        {
            private string _targetName;
            private string _targetType;
            private CommonRuleHandler _handler;
            private string _argument;
            private int _priority;

            public CommonRuleHandler Handler
            {
                get { return _handler; }
            }
            public string HandlerText
            {
                get
                {
                    if (_handler == CommonRuleHandler.GenericsMaxValue)
                        return string.Format("CommonRules.MaxValue<{0}>", _targetType);
                    if (_handler == CommonRuleHandler.GenericsMinValue)
                        return string.Format("CommonRules.MinValue<{0}>", _targetType);
                    return "CommonRules." + _handler.ToString();
                }
            }
            public string ArgumentText
            {
                get
                {
                    string argValue = _argument;    //for generic rule
                    if(_targetType.ToLower() == "smartdate")
                        argValue = string.Format("new SmartDate(\"{0}\")", _argument);
                    else if(_targetType.ToLower() == "string")
                        argValue = string.Format("\"{0}\"", _argument);

                    switch (_handler)
                    {
                        case CommonRuleHandler.StringRequired:
                            return string.Format("{0}", _targetName);
                        case CommonRuleHandler.StringMaxLength:
                            return string.Format("new CommonRules.MaxLengthRuleArgs({0}, {1})", _targetName, _argument);
                        case CommonRuleHandler.IntegerMinValue:
                            return string.Format("new CommonRules.IntegerMinValueRuleArgs({0}, {1})", _targetName, _argument);
                        case CommonRuleHandler.IntegerMaxValue:
                            return string.Format("new CommonRules.IntegerMaxValueRuleArgs({0}, {1})", _targetName, _argument);
                        case CommonRuleHandler.GenericsMinValue:
                            return string.Format("new CommonRules.MinValueRuleArgs<{0}>({1}, {2})", _targetType, _targetName, argValue);
                        case CommonRuleHandler.GenericsMaxValue:
                            return string.Format("new CommonRules.MaxValueRuleArgs<{0}>({1}, {2})", _targetType, _targetName, argValue);
                        case CommonRuleHandler.RegExMatch:
                            if ("SSN,Email".IndexOf(_argument) >= 0)
                                return string.Format("new CommonRules.RegExRuleArgs({0}, CommonRules.RegExPatterns.{1})", _targetName, _argument);
                            else
                                return string.Format("new CommonRules.RegExRuleArgs({0}, @\"{1}\")", _targetName, _argument);
                        default:
                            return string.Empty;
                    }
                }
            }
            public int Priority
            {
                get { return _priority; }
            }
            public RuleInfo(PropertyInfo target, CommonRuleHandler handler, string argument, int priority)
            {
                _targetName = target.PropertyInfoName;
                _targetType = target.Type;
                _handler = handler;
                _argument = argument;
                _priority = priority;                
            }
            public RuleInfo(PropertyInfo target, XmlTextReader xtr) 
            {
                _targetName = target.PropertyInfoName;
                _targetType = target.Type;
                _handler = (CommonRuleHandler)Enum.Parse(typeof(CommonRuleHandler), xtr.LocalName);
                _priority = 0;
                _argument = xtr.ReadElementString();
            }
        }
        public enum CommonRuleHandler
        {
            StringRequired, StringMaxLength, IntegerMinValue, IntegerMaxValue, GenericsMinValue, GenericsMaxValue, RegExMatch
        }
        #endregion

        #region Helpers

        #region Dal Helper
        public class DalHelper
        {
            private DalHelper() { }
            public static string ParameterReturnStatement(string varName, string varType, string parName)
            {
                string format = "{0} = ({1})cm.Parameters[\"@{2}\"].Value;";
                return string.Format(format, varName, varType, parName);
            }
            public static string ParameterAssignmentStatement(string parName, string varName)
            {
                string format = "cm.Parameters.AddWithValue(\"@{0}\", {1});";
                return string.Format(format, parName, varName);
            }
            public static string ReaderStatement(string varName, string varType, string colName)
            {
                string method = GetReaderMethod(varType);

                if (varType == "SmartDate")
                    return string.Format("{0} = dr.GetSmartDate(\"{1}\", {0}.EmptyIsMin);", varName, colName);

                //if (varType == "byte[]")
                //    return string.Format("dr.{1}(\"{2}\", 0, {0}, 0, int.MaxValue);", varName, method, colName);

                if (method == string.Empty)
                    return string.Format("{0} = ({1})dr[\"{2}\"];", varName, varType, colName);
                else
                    return string.Format("{0} = dr.{1}(\"{2}\");", varName, method, colName);
            }
            public static string GetReaderMethod(string varType)
            {
                string val = string.Empty;
                switch (varType)
                {
                    case "SmartDate":
                    case "DateTime":
                    case "Guid": return "Get" + varType;
                    case "string":
                    case "double":
                    case "byte":
                    case "decimal":
                    case "float":
                        return "Get" + varType.Substring(0, 1).ToUpper() + varType.Substring(1);
                    case "bool": return "GetBoolean";
                    case "short": return "GetInt16";
                    case "int": return "GetInt32";
                    case "long": return "GetInt64";
                }
                return string.Empty;
            }
        }
        #endregion //Dal Helper

        #region Column CsHelper
        public class CsHelper
        {
            private CsHelper() { }

            //remove underscore or space and convert to pascal case
            public static string MakeProper(string name)
            {
                string[] arrstr = name.Split("_ ".ToCharArray());
                string properName = "";
                for (int i = 0; i < arrstr.Length; i++)
                {
                    if (arrstr[i] == "") continue;
                    string str = arrstr[i].Replace("_", "");
                    properName += str.Substring(0, 1).ToUpper() + str.Substring(1).ToLower();
                }
                return properName;
            }

            public static string GetCamelCaseName(string val)
            {
                if (val.Length <= 2) return val.ToLower();
                return val.Substring(0, 1).ToLower() + val.Substring(1);
            }

            //logic was taken from http://www.devx.com/vb2themax/Tip/19612
            public static string ConvertToSingular(string plural)
            {
                string lower = plural.ToLower();
                string result = "";

                //rule out a few exceptions
                if (lower == "feet")
                    result = "Foot";
                else if (lower == "geese")
                    result = "Goose";
                else if (lower == "men")
                    result = "Man";
                else if (lower == "women")
                    result = "Woman";
                else if (lower == "criteria")
                    result = "Criterion";
                //plural uses "ies" if word ends with "y" preceeded by a non-vowel
                else if (lower.EndsWith("ies") && "aeiou".IndexOf(lower.Substring(lower.Length - 4, 1)) < 0)
                    result = plural.Substring(0, plural.Length - 3) + "y";
                else if (lower.EndsWith("s"))
                    result = plural.Substring(0, plural.Length - 1);
                else
                    result = plural;  //table name may not in plural form so, just return its name

                return result;
            }

            // logic was taken from http://www.devx.com/vb2themax/Tip/19611
            public static string ConvertToPlural(string singular)
            {
                string lower = singular.ToLower();
                string result = "";

                //rule out a few exceptions
                if (lower == "foot")
                    result = "Feet";
                else if (lower == "goose")
                    result = "Geese";
                else if (lower == "man")
                    result = "Men";
                else if (lower == "woman")
                    result = "Women";
                else if (lower == "criterion")
                    result = "Criteria";
                //plural uses "ies" if word ends with "y" preceeded by a non-vowel
                else if (lower.EndsWith("y") && "aeiou".IndexOf(lower.Substring(lower.Length - 2, 1)) < 0)
                    result = singular.Substring(0, singular.Length - 1) + "ies";
                else if (lower.EndsWith("o") && "aeiou".IndexOf(lower.Substring(lower.Length - 2, 1)) < 0)
                    result = singular + "es";
                else
                    result = singular + "s";

                return result;
            }

            public static string GetPropertyName(DataObjectBase col)
            {
                string name = col.Name;

                //ensure first character is capital letter
                name = name.Substring(0, 1).ToUpper() + name.Substring(1);

                //convert ex:START_DATE or Start Date to StartDate
                if (name.IndexOf('_') >= 0 || name.IndexOf(' ') >= 0 || name == name.ToUpper())
                    name = CsHelper.MakeProper(name);

                //fix name that contain table prefix
                //ex table:Project column:ProjectName, ProjectDescription
                if(col is ColumnSchema)
                    name = name.Replace(((ColumnSchema)col).Table.Name, "");

                if (name.EndsWith("TypeCode"))
                    name = name.Substring(0, name.Length - 4);

				if(name.Length == 0)
					throw new Exception("Column " + col.Name + " has resulted blank property name");
					
                return name;
            }
            public static string GetDefaultValue(string variableType)
            {
                switch (variableType)
                {
                    case "SmartDate": return "new SmartDate(true)";
                    case "Guid": return "Guid.Empty";
                    case "string": return "string.Empty";
                    case "bool": return "false";
                    case "double": return "0";
                    case "byte": return "0";
                    case "decimal": return "0";
                    case "float": return "0";
                    case "short": return "0";
                    case "int": return "0";
                    case "long": return "0";
                    default: return "";
                }
            }
            public static string GetDefaultValue(DataObjectBase col)
            { 
                switch (col.DataType)
                {
                    case DbType.Guid: return "Guid.Empty";
                    case DbType.AnsiString: return "string.Empty";
                    case DbType.AnsiStringFixedLength: return "string.Empty";
                    case DbType.String: return "string.Empty";
                    case DbType.StringFixedLength: return "string.Empty";
                    case DbType.Boolean: return "false";
                    case DbType.Date:
                    case DbType.DateTime:
                        if (col.AllowDBNull)
                        {
                            string colName = col.Name.ToLower();
                            if (colName.IndexOf("end") >= 0) return "new SmartDate(SmartDate.EmptyValue.MaxDate)";
                            if (colName.IndexOf("inactive") >= 0) return "new SmartDate(SmartDate.EmptyValue.MaxDate)";
                            if (colName.IndexOf("finish") >= 0) return "new SmartDate(SmartDate.EmptyValue.MaxDate)";
                            return "new SmartDate(SmartDate.EmptyValue.MinDate)";
                        }
                        else
                        {
                            if (col.Name.ToLower().IndexOf("timestamp") >= 0)
                                return "new SmartDate(DateTime.Now)";
                            else
                                return "new SmartDate(DateTime.Today)";
                        }
                    case DbType.VarNumeric: return "0";
                    case DbType.Currency: return "0";
                    case DbType.Decimal: return "0";
                    case DbType.Double: return "0";
                    case DbType.Int16: return "0";
                    case DbType.Int32: return "0";
                    case DbType.Int64: return "0";
                    case DbType.Single: return "0";
                    case DbType.Byte: return "0";
                    case DbType.UInt16: return "0";
                    case DbType.UInt32: return "0";
                    case DbType.UInt64: return "0";
                    default: return "";
                }
            }
            public static string GetVariableType(DataObjectBase col)
            {
                switch (col.DataType)
                {
                    case DbType.AnsiString: return "string";
                    case DbType.AnsiStringFixedLength: return "string";
                    case DbType.String: return "string";
                    case DbType.StringFixedLength: return "string";
                    case DbType.Binary: return "byte[]";
                    case DbType.Boolean: return "bool";
                    case DbType.Byte: return "byte";
                    case DbType.Currency: return "decimal";
                    case DbType.Date: return "SmartDate";
                    case DbType.DateTime: return "SmartDate";
                    case DbType.VarNumeric: return "decimal";
                    case DbType.Decimal: return "decimal";
                    case DbType.Double: return "double";
                    case DbType.Guid: return "Guid";
                    case DbType.Int16: return "short";
                    case DbType.Int32: return "int";
                    case DbType.Int64: return "long";
                    case DbType.Object: return "object";
                    case DbType.SByte: return "sbyte";
                    case DbType.Single: return "float";
                    case DbType.Time: return "TimeSpan";
                    case DbType.UInt16: return "ushort";
                    case DbType.UInt32: return "uint";
                    case DbType.UInt64: return "ulong";
                    default: return "__UNKNOWN__" + col.NativeType;
                }
            }
            public static bool IsIdentity(DataObjectBase col)
            {
                if (col.ExtendedProperties["CS_IsIdentity"] == null) return false;
                return (bool)col.ExtendedProperties["CS_IsIdentity"].Value;
            }
            public static bool IsComputed(DataObjectBase col)
            {
                if (col.ExtendedProperties["CS_IsComputed"] == null) return false;
                return (bool)col.ExtendedProperties["CS_IsComputed"].Value;
            }
            public static bool IsTimestamp(DataObjectBase col)
            {
                bool var = false;
                if (col.NativeType.ToLower() == "timestamp")
                    var = true;
                else if (col.Name.ToLower().IndexOf("timestamp") >= 0)
                    var = true;
                return var;
            }
        }
        #endregion

        #region Template CsHelper
        public class TemplateHelper
        {
            private TemplateHelper() { }

            public static bool IsObjectType(ICodeTemplateInfo info)
            {
                switch (info.FileName.ToLower())
                {
                    case "editableroot.cst":
                    case "editablerootlist.cst":
                    case "editablechild.cst":
                    case "editablechildlist.cst":
                    case "editableswitchable.cst":
                    case "namevaluelist.cst":
                    case "readonlyroot.cst":
                    case "readonlyrootlist.cst":
                    case "readonlychild.cst":
                    case "readonlychildlist.cst": return true;
                    default: return false;
                }
            }

            public static ObjectType ToObjectType(ICodeTemplateInfo info)
            {
                switch (info.FileName.ToLower())
                {
                    case "editableroot.cst": return ObjectType.EditableRoot;
                    case "editablerootlist.cst": return ObjectType.EditableRootList;
                    case "editablechild.cst": return ObjectType.EditableChild;
                    case "editablechildlist.cst": return ObjectType.EditableChildList;
                    case "editableswitchable.cst": return ObjectType.EditableSwitchable;
                    case "namevaluelist.cst": return ObjectType.NameValueList;
                    case "readonlyroot.cst": return ObjectType.ReadOnlyRoot;
                    case "readonlyrootlist.cst": return ObjectType.ReadOnlyRootList;
                    case "readonlychild.cst": return ObjectType.ReadOnlyChild;
                    case "readonlychildlist.cst": return ObjectType.ReadOnlyChildList;
                }
                throw new ArgumentOutOfRangeException("Template is not an Object Type template");
            }

            public static CodeTemplate GetCompiledTemplate(string templatePath)
            {

                CodeTemplateCompiler compiler = new CodeTemplateCompiler(templatePath);
	            compiler.Compile();

                if (compiler.Errors.Count > 0)
                {
                    string errString = "Error Compiling Template\r\n";
                    errString += "- " + templatePath + "\r\n";

                    foreach (System.CodeDom.Compiler.CompilerError err in compiler.Errors)
                    {   
                        errString += err.ErrorText + "\r\n";
                    }
                    throw new ApplicationException(errString);
                }

                return compiler.CreateInstance();

            }
        }
        #endregion
        #endregion

        #region Enumerations
        public enum ObjectType
        {
            EditableRoot,
            EditableRootList,
            EditableChild,
            EditableChildList,
            EditableSwitchable,
            NameValueList,
            ReadOnlyRoot,
            ReadOnlyRootList,
            ReadOnlyChild,
            ReadOnlyChildList
        }

        public enum CodeGenerationMethod { Single, SplitPartial }
        public enum GenerationClassType { Generated, User }
        public enum PropertyAccessSecurity { None, Both, Read, Write }
        public enum TransactionalTypes { None, Ado, EnterpriseService, TransactionScope }
        #endregion
    }
}
