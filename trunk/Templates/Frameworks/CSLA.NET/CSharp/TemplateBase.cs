#region 
//==============================================================================================
// CSLA 2.0 CodeSmith Templates for C#
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
        const string DbConnectionFormat = "Database.{0}Connection";
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
        //option to minimize use of StackTrace
        static bool MinimizeStackTraceUse = true;
        static bool HandleNullableFields = true;
        #endregion //Constants

        #region Templates
        //property template
        const string PROPERTY_TEMPLATE =
            "[ATTRIBUTE]"
            + "[BASEINDENT][MODIFIERS] [TYPE] [PROPNAME]"
            + "[BASEINDENT]{"
            + "[GETATTRIBUTE]"
            + "[BASEINDENT][INDENT]get"
            + "[BASEINDENT][INDENT]{"
            + "[CANREAD]"
            + "[BASEINDENT][INDENT][INDENT]return [VARNAME];"
            + "[BASEINDENT][INDENT]}"
            + "[SETTER]"
            + "[BASEINDENT]}\r\n";
        //property set template
        const string SETTER_TEMPLATE =
            "[SETATTRIBUTE]"
            + "[BASEINDENT][INDENT]set"
            + "[BASEINDENT][INDENT]{"
            + "[CANWRITE]"
            + "[CONVERTNULL]"
            + "[BASEINDENT][INDENT][INDENT]if ([COMPARE])"
            + "[BASEINDENT][INDENT][INDENT]{"
            + "[BASEINDENT][INDENT][INDENT][INDENT][VARNAME] = value;"
            + "[PROPCHANGED]"
            + "[BASEINDENT][INDENT][INDENT]}"
            + "[BASEINDENT][INDENT]}";
        const string CANREAD_TEMPLATE = "[BASEINDENT][INDENT][INDENT]CanReadProperty(true);";
        const string CANWRITE_TEMPLATE = "[BASEINDENT][INDENT][INDENT]CanWriteProperty(true);";
        const string CANREAD_NOSTACK_TEMPLATE = "[BASEINDENT][INDENT][INDENT]CanReadProperty([PROPNAMESTRING], true);";
        const string CANWRITE_NOSTACK_TEMPLATE = "[BASEINDENT][INDENT][INDENT]CanWriteProperty([PROPNAMESTRING], true);";
        const string PROPCHANGED_TEMPLATE = "[BASEINDENT][INDENT][INDENT][INDENT]PropertyHasChanged([PROPNAMESTRING]);";
        const string CONVERTNULL_TEMPLATE = "[BASEINDENT][INDENT][INDENT]if (value == null) value = string.Empty;";

        #endregion

        #region Object Definition
        private string _classNamespace = "";
        [CodeTemplateProperty(CodeTemplatePropertyOption.Optional),
        Category("1. Object"),
        Description("Optional - The namespace that the generated Classes will be a member of.")]
        public string ClassNamespace
        {
            get { return _classNamespace; }
            set { _classNamespace = value; }
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
        private CodeGenerationMethod _codeGenMethod = CodeGenerationMethod.Single;
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
            set { _xmlFilePath = value; }
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
        /// <summary>
        /// Iterate through all properties and return their member declarations.
        /// </summary>
        /// <param name="objInfo"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public string GetMemberDeclarations(ObjectInfo objInfo, int level)
        {
            string members = Indent(level, true) + "//declare members";
            foreach (PropertyInfo prop in objInfo.Properties)
            {
                //if csla-type, later...
                if (objInfo.ChildCollection.Contains(prop))
                    continue;
                //if pk (non composite) and type of Guid, default value is Guid.NewGuid()
                if (objInfo.UniqueProperties.Count==1 && objInfo.UniqueProperties.Contains(prop) && prop.Type == "Guid")
                    members += Indent(level, true) + GetMemberDeclaration(prop.MemberAccess, prop.Type, prop.MemberName, "Guid.NewGuid()");
                else
                    members += Indent(level, true) + GetMemberDeclaration(prop);
            }

            //add csla-type child objects (not simple type but csla-type class)
            if (objInfo.HasChild)
                members += "\r\n" + Indent(level, true) + "//declare child member(s)";
            foreach (PropertyInfo prop in objInfo.ChildCollection)
            {
                members += Indent(level, true) + GetMemberDeclaration(prop);
            }
            if (members.Length > 0) members = members.Substring(2);
            return members;
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
        /// <summary>
        /// Iterate through object's properties and return all property statements
        /// </summary>
        /// <param name="objInfo"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public string GetPropertyDeclarations(ObjectInfo objInfo, int level)
        {
            string props = string.Empty;
            foreach (PropertyInfo prop in objInfo.Properties)
            {
                //skip child collection until later. 
                //exclude timestamp, this field only to support concurrency
                if (!objInfo.ChildCollection.Contains(prop) && !prop.IsTimestamp)
                    props += GetPropertyDeclaration(prop, level);
            }
            foreach (PropertyInfo prop in objInfo.ChildCollection)
            {
                props += GetPropertyDeclaration(prop, level);
            }
            if (props.Length > 0) props = props.Substring(2);
            return props;
        }
        // return property statement
        public string GetPropertyDeclaration(PropertyInfo property, int level)
        {
            string str = PROPERTY_TEMPLATE;
            string propNameAsString = "\"" + property.Name + "\"";

            if (property.Type == "SmartDate")
            {
                string newStr = str;    //get-set as string 
                if(!property.IsReadOnly)
                    newStr = newStr.Replace("[SETTER]", SETTER_TEMPLATE);
                newStr = newStr.Replace("[TYPE]", "string");
                newStr = newStr.Replace("[PROPNAME]", property.Name + "String");
                newStr = newStr.Replace("[VARNAME]", property.MemberName + ".Text");
                newStr = newStr.Replace("[CONVERTNULL]", CONVERTNULL_TEMPLATE);

                if (MinimizeStackTraceUse)
                {   //property name is "<propertyName>String"
                    if (property.Authorization == PropertyAccessSecurity.Read || property.Authorization == PropertyAccessSecurity.Both)
                        newStr = newStr.Replace("[CANREAD]", CANREAD_NOSTACK_TEMPLATE);
                    if (property.Authorization == PropertyAccessSecurity.Write || property.Authorization == PropertyAccessSecurity.Both)
                        newStr = newStr.Replace("[CANWRITE]", CANWRITE_NOSTACK_TEMPLATE);
                    newStr = newStr.Replace("[PROPCHANGED]", PROPCHANGED_TEMPLATE);
                    newStr = newStr.Replace("[PROPNAMESTRING]", "\"" + property.Name + "String\"");
                }

                str = str + newStr;
            }
            if (property.IsPrimaryKey)
                str = str.Replace("[ATTRIBUTE]", string.Format("[BASEINDENT][System.ComponentModel.DataObjectField({0}, {1})]", property.IsPrimaryKey.ToString().ToLower(), property.IsIdentity.ToString().ToLower()));
            if (!property.IsReadOnly && property.Type != "SmartDate")
                str = str.Replace("[SETTER]", SETTER_TEMPLATE);

            if (property.Authorization == PropertyAccessSecurity.Read || property.Authorization == PropertyAccessSecurity.Both)
            {
                if(MinimizeStackTraceUse)
                    str = str.Replace("[CANREAD]", CANREAD_NOSTACK_TEMPLATE);
                else
                    str = str.Replace("[CANREAD]", CANREAD_TEMPLATE);
            }
            if (property.Authorization == PropertyAccessSecurity.Write || property.Authorization == PropertyAccessSecurity.Both)
            {
                if (MinimizeStackTraceUse)
                    str = str.Replace("[CANWRITE]", CANWRITE_NOSTACK_TEMPLATE);
                else
                    str = str.Replace("[CANWRITE]", CANWRITE_TEMPLATE);
            }

            str = str.Replace("[PROPCHANGED]", PROPCHANGED_TEMPLATE);

            if (MinimizeStackTraceUse)
                str = str.Replace("[PROPNAMESTRING]", propNameAsString);
            else
            {
                str = str.Replace("[GETATTRIBUTE]", "[BASEINDENT][INDENT][System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]");
                str = str.Replace("[SETATTRIBUTE]", "[BASEINDENT][INDENT][System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]");
            }

            str = str.Replace("[MODIFIERS]", property.Modifiers);
            str = str.Replace("[PROPNAME]", property.Name);
            if (property.Type == "SmartDate")  //get in 
            {
                str = str.Replace("[TYPE]", "DateTime");
                str = str.Replace("[VARNAME]", property.MemberName + ".Date");
            }
            else
            {
                str = str.Replace("[TYPE]", property.Type);
                str = str.Replace("[VARNAME]", property.MemberName);
            }
            if (property.Type == "string")
            {
                //case insensitive comparison
                //str = str.Replace("[COMPARE]", string.Format("string.Compare({0}, value, true)!=0", property.MemberName));
                str = str.Replace("[COMPARE]", string.Format("!{0}.Equals(value)", property.MemberName));
                str = str.Replace("[CONVERTNULL]", CONVERTNULL_TEMPLATE);
            }
            else
                str = str.Replace("[COMPARE]", string.Format("!{0}.Equals(value)", property.MemberName));

            str = str.Replace("[BASEINDENT]", Indent(level, true));
            str = str.Replace("[INDENT]", Indent());
            str = Regex.Replace(str, @"\[\w+\]", "");   //clean up unused tags.
            return str;
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
            if (prop.Type == "string")
            {
                if (prop.IsRequired)
                    rules += Indent(level, true)
                        + string.Format("ValidationRules.AddRule(CommonRules.StringRequired, \"{0}\");", prop.Name);

                if(prop.MaxSize>0)
                    rules += Indent(level, true)
                        + string.Format("ValidationRules.AddRule(CommonRules.StringMaxLength, new CommonRules.MaxLengthRuleArgs(\"{0}\", {1}));", prop.Name, prop.MaxSize);
            }
            else
            {
                if( prop.Type == "SmartDate" && prop.IsRequired)
                    rules += Indent(level, true)
                        + string.Format("ValidationRules.AddRule(CommonRules.StringRequired, \"{0}\");", prop.Name + "String");
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
                    if (HandleNullableFields && prop.DefaultValue.Length > 0 && !prop.IsRequired && prop.Type != "SmartDate")
						statement += GetDefaultConditionStatement(prop, "", level++);
                    statement += GetParameterStatement(prop, "", "", true, level);
                    if (HandleNullableFields && prop.DefaultValue.Length > 0 && !prop.IsRequired && prop.Type != "SmartDate")
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
                    if (HandleNullableFields && prop.DefaultValue.Length > 0 && !prop.IsRequired && prop.Type != "SmartDate")
						statement += GetDefaultConditionStatement(prop, "", level++);
                    statement += GetParameterStatement(prop, "", "", true, level);
                    if (HandleNullableFields && prop.DefaultValue.Length > 0 && !prop.IsRequired && prop.Type != "SmartDate")
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
        /// <returns></returns>
        public string GetFactoryFilterDeclarationArguments(ObjectInfo obj)
        {
            string para = string.Empty;
            foreach (PropertyInfo prop in obj.FilterProperties)
            {
                para += string.Format(", {0} {1}", prop.Type, CsHelper.GetCamelCaseName(prop.Name));
            }
            if (para.Length > 0) para = para.Substring(2);
            return para;
        }
        /// <summary>
        /// return parameter call/pass statement in factory get collection
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string GetFactoryFilterCallArguments(ObjectInfo obj)
        {
            string para = string.Empty;
            foreach (PropertyInfo prop in obj.FilterProperties)
            {
                para += string.Format(", {0}", CsHelper.GetCamelCaseName(prop.Name));
            }
            if (para.Length > 0) para = para.Substring(2);
            return para;
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
        public string GetFactoryNewDeclarationArguments(ObjectInfo obj)
        {
            return GetFactoryDeclarationArguments(obj, true);
        }
        public string GetFactoryNewCallArguments(ObjectInfo obj)
        {
            return GetFactoryCallArguments(obj, true, false);
        }
        public string GetFactoryNewAssignments(ObjectInfo obj, int level)
        {
            if (obj.IsCollection) return string.Empty;
            if (obj.HasIdentity) return string.Empty;
            if (obj.HasObjectGeneratedKey) return string.Empty;

            string members = string.Empty;
            foreach (PropertyInfo prop in obj.UniqueProperties)
            {
                members += Indent(level, true) + string.Format("this.{0} = {1};", prop.MemberName, CsHelper.GetCamelCaseName(prop.Name));
            }
            if (members.Length > 0) members = members.Substring(2);
            return members;
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
        public string GetFactoryCallArguments(ObjectInfo obj)
        {
            return GetFactoryCallArguments(obj, false, false);
        }
        public string GetDeleteSelfCriteriaCallArguments(ObjectInfo obj)
        {
            return GetFactoryCallArguments(obj, false, true);
        }
        public string GetFactoryCallArguments(ObjectInfo obj, bool isNew, bool useMember)
        {
            if (obj.IsCollection) return string.Empty;
            if (obj.HasIdentity && isNew) return string.Empty;
            if (obj.HasObjectGeneratedKey && isNew) return string.Empty;

            string para = string.Empty;
            foreach (PropertyInfo prop in obj.UniqueProperties)
            {
                para += string.Format(", {0}", useMember ? prop.MemberName : CsHelper.GetCamelCaseName(prop.Name));
            }
            if (para.Length > 0) para = para.Substring(2);
            return para;
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
            private TransactionalTypes _transactionType;
            private PropertyAccessSecurity _propertyAuthorization;
            private bool _useSecurity;
            private CodeGenerationMethod _codeGenMethod = CodeGenerationMethod.Single;
            private GenerationClassType _classType = GenerationClassType.Generated;
			private CommandSchema _rootCommand = null;
            #endregion

            #region Parent Properties
            private string _parent = "";
            public string Parent
            {
                get 
                {
                    if (IsGeneratedBase && _parent.Length>0) return _parent + BaseSuffix;
                    return _parent;
                }
            }
            public string ParentType
            {
                get
                {
                    if (IsGeneratedBase && _parent.Length > 0) return GenericTypeParentParameter;
                    return _parent;
                }
            }
            public string ParentSuffix
            {
                get
                {
                    if (!IsGeneratedBase) return string.Empty;
                    return string.Format("<{0}>", ParentType);
                }
            }
            public string ParentNameAndSuffix
            {
                get { return Parent + ParentSuffix; }
            }
            #endregion //Parent Properties

            #region Child Properties
            public string ChildType
            {
                get
                {
                    if (IsGeneratedBase) return GenericTypeChildParameter;
                    return _child;
                }
            }
            private string _child = "";
            public string Child
            {
                get 
                {
                    if (IsGeneratedBase) return _child + BaseSuffix;
                    return _child; 
                }
            }
            public string CustomChild
            {
                get { return _child; }
            }
            public string ChildSuffix
            {
                get
                {
                    if (!IsGeneratedBase) return string.Empty;
                    return string.Format("<{0}>", ChildType);
                }
            }
            public string ChildNameAndSuffix
            {
                get { return Child + ChildSuffix; }
            }
            #endregion //Child Properties

            #region Properties
            private string _objectName;
            public string Name
            {
                get 
                {
                    if (IsGeneratedBase) return _objectName + BaseSuffix;
                    return _objectName;
                }
            }
            public string CustomName
            {
                get { return _objectName; }
            }
            public string Suffix
            {
                get
                {
                    if(!IsGeneratedBase) return string.Empty;
                    if (IsCollection) return string.Format("<{0}, {1}>", Type, ChildType);
                    return string.Format("<{0}>", Type); 
                }
            }
            public string NameAndSuffix
            {
                get { return Name + Suffix; }
            }
            public string Type
            {
                get
                {
                    if (IsGeneratedBase) return GenericTypeObjectParameter;
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
            #endregion //Properties

            #region Methods
            public string MemberAccess
            {
                get 
                {
                    if (IsGeneratedBase)
                        return "protected";
                    else
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
                    if (IsGeneratedBase)
                        return MemberAccess + " virtual";
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
            public bool IsBase
            {
                get { return _codeGenMethod == CodeGenerationMethod.SplitBase; }
            }
            public bool IsPartial
            {
                get { return _codeGenMethod == CodeGenerationMethod.SplitPartial ;}
            }
            public bool IsGeneratedBase
            {
                get { return _codeGenMethod == CodeGenerationMethod.SplitBase && _classType == GenerationClassType.Generated; }
            }
            public bool IsUserBase
            {
                get { return _codeGenMethod == CodeGenerationMethod.SplitBase && _classType == GenerationClassType.User; }
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
                    if (IsGeneratedBase) 
                        modifiers += " abstract";
                    if (IsPartial) 
                        modifiers += " partial";
                    return modifiers;
                }
            }
            public string Inherits
            {
                get
                {
                    //if user class for base type
                    if(IsUserBase) 
                    {
                        if(_objectType == ObjectType.NameValueList)
                            return string.Format(" : {0}{1}", Name, BaseSuffix);
                        if (IsCollection) return string.Format(" : {0}{1}<{2}, {3}>", Name, BaseSuffix, Type, ChildType);
                        return string.Format(" : {0}{1}<{2}>", Name, BaseSuffix, Type);
                    }
                    //other types
                    switch (_objectType)
                    {
                        case ObjectType.EditableRoot:
                        case ObjectType.EditableChild:
                        case ObjectType.EditableSwitchable:
                            return string.Format(" : Csla.BusinessBase<{0}>", Type);
                        case ObjectType.ReadOnlyRoot:
                        case ObjectType.ReadOnlyChild:
                            return string.Format(" : Csla.ReadOnlyBase<{0}>", Type);
                        case ObjectType.EditableChildList:
                        case ObjectType.EditableRootList:
                            return string.Format(" : Csla.BusinessListBase<{0}, {1}>", Type, ChildType);
                        case ObjectType.NameValueList:
                            return string.Format(" : Csla.NameValueListBase<{0}, {1}>", ((PropertyInfo)_uniqueProperties[0]).Type, ((PropertyInfo)_properties[1]).Type);
                        case ObjectType.ReadOnlyRootList:
                        case ObjectType.ReadOnlyChildList:
                            return string.Format(" : Csla.ReadOnlyListBase<{0}, {1}>", Type, ChildType);
                    }
                    return string.Empty;
                }
            }
            public string Constraint
            {
                get
                {
                    if (!IsGeneratedBase) return string.Empty;
                    switch (_objectType)
                    {
                        case ObjectType.EditableRoot:
                        case ObjectType.EditableChild:
                        case ObjectType.EditableSwitchable:
                        case ObjectType.ReadOnlyRoot:
                        case ObjectType.ReadOnlyChild:
                            return string.Format("where {0} : {1}", Type , NameAndSuffix);
                        case ObjectType.EditableChildList:
                        case ObjectType.EditableRootList:
                        case ObjectType.ReadOnlyRootList:
                        case ObjectType.ReadOnlyChildList:
                            return string.Format("where {0} : {1}\r\nwhere {2} : {3}", Type, NameAndSuffix, ChildType, ChildNameAndSuffix);
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
            public string FetchCommandText
            {
                get 
				{
					if (_rootCommand != null)
						return _rootCommand.Name;
					return string.Format(FetchCommandFormat, _objectName); 
				}
            }
            public string InsertCommandText
            {
                get { return string.Format(InsertCommandFormat, _objectName); }
            }
            public string UpdateCommandText
            {
                get { return string.Format(UpdateCommandFormat, _objectName); }
            }
            public string DeleteCommandText
            {
                get { return string.Format(DeleteCommandFormat, _objectName); }
            }
            #endregion

            #region Constructors
            public ObjectInfo(CodeTemplate template)
            {
                if (!TemplateHelper.IsObjectType(template.CodeTemplateInfo))
                    throw new ArgumentException(string.Format("Template '{0}' is not a business object template type", template.CodeTemplateInfo.FileName));

                string xmlpath = (string)template.GetProperty("XmlFilePath");
                bool isFromXml = (xmlpath != null && xmlpath.Length > 0);
                if (isFromXml)
                    LoadFromXml(template);
                else
                    LoadFromSchema(template);                  
            }

            private void LoadFromXml(CodeTemplate template)
            {
                _objectName = (string)template.GetProperty("ObjectName");

                //template settings
                _transactionType = (TransactionalTypes)template.GetProperty("TransactionalType");
                _propertyAuthorization = (PropertyAccessSecurity)template.GetProperty("PropertyAuthorization");
                _useSecurity = (bool)template.GetProperty("AuthorizationRules");
                _codeGenMethod = (CodeGenerationMethod)template.GetProperty("GenerationMethod");
                _classType = (GenerationClassType)template.GetProperty("ClassType");

                //read from xml file
                string path = (string)template.GetProperty("XmlFilePath");

                XmlTextReader xtr = new XmlTextReader(path);

                while (xtr.Read())
                {
                    if (xtr.NodeType == XmlNodeType.Element && xtr.LocalName.ToLower() == "object")
                    {
                        if (xtr.GetAttribute("name") == _objectName)
                        {
                            _objectType = (ObjectType)Enum.Parse(typeof(ObjectType), xtr.GetAttribute("type"), true);
                            _child = xtr.GetAttribute("child");
                            _parent = xtr.GetAttribute("parent");

                            //object propertiesTag = xtr.NameTable.Add("properties");
                            //object propertyTag = xtr.NameTable.Add("property");
                            while (xtr.Read())
                            {
                                if (xtr.NodeType == XmlNodeType.EndElement && xtr.LocalName.ToLower() == "properties")
                                    break;
                                if (xtr.NodeType == XmlNodeType.Element && xtr.LocalName.ToLower() == "property")
                                {
                                    PropertyInfo prop = new PropertyInfo(xtr, this);
                                    _properties.Add(prop);

                                    if (prop.IsCollection && prop.IsCslaClass)
                                        _childCollection.Add(prop);
                                    if (prop.IsPrimaryKey)
                                        _uniqueProperties.Add(prop);
                                    if (prop.IsFilterKey)
                                        _filterProperties.Add(prop);
                                }
                            }

                            break;  //finish
                        }
                    }
                }
                xtr.Close();

                //validate object
                Validate();
            }

            private void LoadFromSchema(CodeTemplate template)
            {
                _objectType = TemplateHelper.ToObjectType(template.CodeTemplateInfo);

                //object, child, and parent name
                _objectName = (string)template.GetProperty("ObjectName");
                if (IsCollection)
                    _child = (string)template.GetProperty("ChildName");
                if (IsChild)
                    _parent = (string)template.GetProperty("ParentName");
                if (_parent == null) _parent = string.Empty;

                //child collections
                StringCollection types = (StringCollection)template.GetProperty("ChildCollectionNames");
                StringCollection names = (StringCollection)template.GetProperty("ChildPropertyNames");
                if (types != null && names != null && types.Count > 0 && names.Count > 0)
                {
                    int maxCount = types.Count < names.Count ? types.Count : names.Count;

                    for (int i = 0; i < maxCount; i++)
                    {
                        if (names[i].TrimEnd() != string.Empty && types[i].TrimEnd() != string.Empty)
                        {
                            PropertyInfo prop = new PropertyInfo(names[i], types[i], this);
                            _properties.Add(prop);
                            _childCollection.Add(prop);
                        }
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
                    LoadProperties(command, resultSetIndex, uniqueColumns, filterColumns);
				}
                else if (table != null)
                {
                    LoadProperties(table);
                }
                else
                {
                    LoadProperties(view, uniqueColumns, filterColumns);
                }
                //template settings
                _transactionType = (TransactionalTypes)template.GetProperty("TransactionalType");
                _propertyAuthorization = (PropertyAccessSecurity)template.GetProperty("PropertyAuthorization");
                _useSecurity = (bool)template.GetProperty("AuthorizationRules");
                _codeGenMethod = (CodeGenerationMethod)template.GetProperty("GenerationMethod");
                _classType = (GenerationClassType)template.GetProperty("ClassType");
				_rootCommand = (CommandSchema)template.GetProperty("RootCommand");

                //validate object
                Validate();
            }

            private void LoadProperties(TableSchema table)
            {
                _dbName = table.Database.Name;
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
                _dbName = view.Database.Name;
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
                _dbName = command.Database.Name;
				
                foreach (CommandResultColumnSchema col in command.CommandResults[resultSetIndex].Columns)
                {
					bool isUniqueMember = false;
					bool isFilterMember = false;
					if (resultSetIndex == 0 && CslaObjectType != ObjectType.NameValueList)
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
            private void Validate()
            {
                if (_objectName == null || _objectName.Length == 0)
                    throw new Exception("ObjectName is required.");
                if (_uniqueProperties.Count == 0 && !IsCollection)
                    throw new Exception("Unique Column(s) is required.");
                if (!IsReadOnly && IsChild && IsCollection && (_parent == null || _parent.Length == 0))
                    throw new Exception("Parent is required.");
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
            private string _access = "public";
            private string _defaultValue = string.Empty;
            private string _dbColumnName = string.Empty;
            private bool _isIdentity = false;
            private bool _isPrimaryKey = false;
            private bool _isFilterKey = false;
            private bool _isRequired = false;
            private bool _isCollection = false;
            private bool _isCslaClass = false;
            private int _maxSize = -1;

            private bool _isReadOnly = false;
            private bool _isComputed = false;
            private bool _isTimestamp = false;
            private bool _updateToDb = true;
            public string Name
            {
                get { return _name; }
            }

            public string Type
            {
                get { return _type; }
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
                    if (_parent.IsGeneratedBase)
                        modifiers += " virtual";
                    return modifiers;
                }
            }

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
            public bool IsReadOnly
            {
                get 
                {
                    //read only field is:
                    //parent is read only or object is read only
                    //identity column or part of primary key column(s)
                    //collection object or csla child object
                    if (_parent.IsReadOnly || _isReadOnly || IsIdentity || IsPrimaryKey 
                                    || IsCollection || IsCslaClass
                                    || _isTimestamp || _isComputed)
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
                    if (IsIdentity || _isTimestamp || _isComputed) 
                        return true;

                    return false;
                }
            }
            public bool IsTimestamp
            {
                get { return _isTimestamp; }
            }
            public bool HasDbColumn
            {
                get { return _dbColumnName != string.Empty; }
            }
            public bool UpdateToDb
            {
                get { return _updateToDb && HasDbColumn; }
            }
            public bool IsRequired
            {
                get { return _isRequired; }
            }
            public bool IsCollection
            {
                get { return _isCollection; }
            }
            public bool IsCslaClass
            {
                get { return _isCslaClass; }
            }
            public int MaxSize
            {
                get { return _maxSize; }
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
            public PropertyInfo(string cslaCollName, string cslaCollType, ObjectInfo parent)
            {
                _parent = parent;
                _name = cslaCollName;
                _type = cslaCollType;
                _isCslaClass = true;
                _isCollection = true;
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
                        case "access":
                            _access = xtr.Value;
                            break;
                        case "default":
                            _defaultValue = xtr.Value;
                            break;
                        case "dbcolumnname":
                            _dbColumnName = xtr.Value;
                            break;
                        case "updatetodb":
                            _updateToDb = bool.Parse(xtr.Value);
                            break;
                        case "isidentity":
                            _isIdentity = bool.Parse(xtr.Value);
                            break;
                        case "isprimarykey":
                            _isPrimaryKey = bool.Parse(xtr.Value);
                            break;
                        case "isfilterkey":
                            _isFilterKey = bool.Parse(xtr.Value);
                            break;
                        case "isrequired":
                            _isRequired = bool.Parse(xtr.Value);
                            break;
                        case "isreadonly":
                            _isReadOnly = bool.Parse(xtr.Value);
                            break;
                        case "iscomputed":
                            _isComputed = bool.Parse(xtr.Value);
                            break;
                        case "maxsize":
                            _maxSize = int.Parse(xtr.Value);
                            break;
                        case "iscollection":
                            _isCollection = bool.Parse(xtr.Value);
                            break;
                        case "iscslaclass":
                            _isCslaClass = bool.Parse(xtr.Value);
                            break;
                        default:
                            break;
                    }
	            }
                if (_name == string.Empty)
                    throw new Exception("Name is required in property");
                if (_type == string.Empty)
                    throw new Exception("Type is required in property");

                if (_defaultValue.Length == 0)
                    _defaultValue = CsHelper.GetDefaultValue(_type);
            }
            private void Load(DataObjectBase col)
            {
                _dbColumnName = col.Name;
                _name = CsHelper.GetPropertyName(col);

                _type = CsHelper.GetVariableType(col);
                _defaultValue = CsHelper.GetDefaultValue(col);
                _isIdentity = CsHelper.IsIdentity(col);
                if (col is ColumnSchema)
                {
                    _isPrimaryKey = ((ColumnSchema)col).IsPrimaryKeyMember;
                    _isFilterKey = ((ColumnSchema)col).IsForeignKeyMember;
                }
                _isRequired = !col.AllowDBNull;
                _isComputed = CsHelper.IsComputed(col);
                _isTimestamp = CsHelper.IsTimestamp(col);
                //fixsize string is <= 8000
                if (_type=="string" && col.Size <= 8000 && col.NativeType!="text" && col.NativeType!="ntext") 
                    _maxSize = col.Size;
            }
        }
        #endregion //Property Class

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
            public string ConvertToSingular(string plural)
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
                else if (lower.EndsWith("es") && "lt".IndexOf(lower.Substring(lower.Length - 3, 1)) < 0)
                    result = plural.Substring(0, plural.Length - 2);
                else if (lower.EndsWith("s"))
                    result = plural.Substring(0, plural.Length - 1);
                else
                    result = plural;  //table name may not in plural form so, just return its name

                return result;
            }

            // logic was taken from http://www.devx.com/vb2themax/Tip/19611
            public string ConvertToPlural(string singular)
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
                            if (colName.IndexOf("begin") >= 0) return "new SmartDate(true)";
                            if (colName.IndexOf("active") >= 0) return "new SmartDate(true)";
                            if (colName.IndexOf("start") >= 0) return "new SmartDate(true)";
                            if (colName.IndexOf("from") >= 0) return "new SmartDate(true)";
                            return "new SmartDate(false)";
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

        public enum CodeGenerationMethod { Single, SplitPartial, SplitBase }
        public enum GenerationClassType { Generated, User }
        public enum PropertyAccessSecurity { None, Both, Read, Write }
        public enum TransactionalTypes { None, Ado, EnterpriseService, TransactionScope }
        #endregion
    }
}
