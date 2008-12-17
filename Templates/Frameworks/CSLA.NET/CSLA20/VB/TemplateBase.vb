#Region ""
'==============================================================================================
' CSLA 2.0 CodeSmith Templates for VB
' Author: Allan Nielsen from C# version by Ricky A. Supit (rsupit@hotmail.com)
'
' This software is provided free of charge. Feel free to use and modify anyway you want it.
'==============================================================================================
#End Region

Imports System
Imports System.Collections
Imports System.Text
Imports System.Data
Imports System.ComponentModel
Imports System.Text.RegularExpressions
Imports CodeSmith.Engine
Imports SchemaExplorer
Imports CodeSmith.CustomProperties
Imports System.Xml
Imports Microsoft.VisualBasic

Namespace CodeSmith.Csla
    Public Class TemplateBase
        Inherits CodeSmith.Engine.CodeTemplate

#Region "Constants"
        'member level variable prefix
        Private Const MemberPrefix As String = "_"
        'variable and parameter space replacement
        Private Const SpaceReplacement As String = "_"
        'base class suffix
        Private Const BaseSuffix As String = "Base"
        'dal commandText formats (stored procedure name)
        Private Const FetchCommandFormat As String = "esp_{0}_select"
        Private Const InsertCommandFormat As String = "esp_{0}_insert"
        Private Const UpdateCommandFormat As String = "esp_{0}_update"
        Private Const DeleteCommandFormat As String = "esp_{0}_delete"
        'database connection format
        Private Const DbConnectionFormat As String = "Database.{0}Connection"
        'database owner format
        Private Const DbOwnerFormat As String = "[{0}]."
        'factory method formats
        Private Const FactoryNewFormat As String = "New{0}"
        Private Const FactoryGetFormat As String = "Get{0}"
        Private Const FactoryDeleteFormat As String = "Delete{0}"
        Private Const FactoryGetListFormat As String = "Get{0}By{1}"
        'generic type parameters
        Private Const GenericTypeObjectParameter As String = "T"
        Protected Const GenericTypeChildParameter As String = "C"
        Private Const GenericTypeParentParameter As String = "P"
        'number of spaces to use for indentation, set to 0 to use tab indentation
        Shared IndentLevelSpaces As Integer = 0
        'option to minimize use of StackTrace
        Shared MinimizeStackTraceUse As Boolean = True

#End Region

#Region "Templates"
        'property template
        Private Const PROPERTY_TEMPLATE As String = "[BASEINDENT][COMMENT]" _
    + "[ATTRIBUTE]" _
            + "[BASEINDENT][MODIFIERS] [READWRITE] Property [PROPNAME]() As [TYPE]" _
            + "[NOINLINE]" _
            + "[BASEINDENT][INDENT]Get" _
            + "[CANREAD]" _
            + "[BASEINDENT][INDENT][INDENT]Return [VARNAME]" _
            + "[BASEINDENT][INDENT]End Get" _
            + "[SETTER]" _
    + "[BASEINDENT]End Property"
        'property set template
        Private Const SETTER_TEMPLATE As String = _
            "[NOINLINE]" _
            + "[BASEINDENT][INDENT]Set(ByVal value As [TYPE])" _
            + "[CANWRITE]" _
            + "[CONVERTNULL]" _
            + "[BASEINDENT][INDENT][INDENT]If [COMPARE] Then" _
            + "[BASEINDENT][INDENT][INDENT][INDENT][VARNAME] = value" _
            + "[PROPCHANGED]" _
            + "[BASEINDENT][INDENT][INDENT]End If" _
            + "[BASEINDENT][INDENT]End Set"
        Private Const CANREAD_TEMPLATE As String = "[BASEINDENT][INDENT][INDENT]CanReadProperty(True)"
        Private Const CANWRITE_TEMPLATE As String = "[BASEINDENT][INDENT][INDENT]CanWriteProperty(True)"
        Private Const CANREAD_NOSTACK_TEMPLATE As String = "[BASEINDENT][INDENT][INDENT]CanReadProperty([PROPNAMESTRING], True)"
        Private Const CANWRITE_NOSTACK_TEMPLATE As String = "[BASEINDENT][INDENT][INDENT]CanWriteProperty([PROPNAMESTRING], True)"
        Private Const PROPCHANGED_TEMPLATE As String = "[BASEINDENT][INDENT][INDENT][INDENT]PropertyHasChanged([PROPNAMESTRING])"
        Private Const CONVERTNULL_TEMPLATE As String = "[BASEINDENT][INDENT][INDENT]If value Is Nothing Then value = String.Empty"
        'property comment template
        Private Const PROPERTY_COMMENT As String = "[BASEINDENT]''' <summary>" _
            + "[BASEINDENT]''' [GETSET] the [PROPNAME] of the current object." _
            + "[BASEINDENT]''' </summary>" _
            + "[BASEINDENT]''' <value>A [TYPE] that represents the [PROPNAME] of the current object.</value>" _
            + "[BASEINDENT]''' <returns>[TYPE] representing the return value.</returns>" _
            + "[BASEINDENT]''' <remarks></remarks>"
#End Region

#Region " Object Definition "
        Private _classNamespace As String = String.Empty

        <Category("1. Object"), _
        Description("Optional - The namespace that the generated Classes will be a member of.")> _
        Public Property ClassNamespace() As String
            Get
                Return _classNamespace
            End Get
            Set(ByVal Value As String)
                _classNamespace = Value
            End Set
        End Property
#End Region

#Region " Object Options "
        Private _propertyAuthorization As PropertyAccessSecurity = PropertyAccessSecurity.Both
        Private _authorizationRules As Boolean = True
        Private _transactionType As TransactionalTypes = TransactionalTypes.None

        <Category("3. Options"), _
        Description("What type of access security to use on properties?")> _
        Public Property PropertyAuthorization() As PropertyAccessSecurity
            Get
                Return _propertyAuthorization
            End Get
            Set(ByVal Value As PropertyAccessSecurity)
                _propertyAuthorization = Value
            End Set
        End Property

        <Category("3. Options"), _
        Description("Use access security when using New/Get/Save/Delete method.")> _
        Public Property AuthorizationRules() As Boolean
            Get
                Return _authorizationRules
            End Get
            Set(ByVal Value As Boolean)
                _authorizationRules = Value
            End Set
        End Property

        <Category("3. Options"), _
        Description("What type of Transaction should this Business Object use?")> _
        Public Property TransactionalType() As TransactionalTypes
            Get
                Return _transactionType
            End Get
            Set(ByVal Value As TransactionalTypes)
                _transactionType = Value
            End Set
        End Property
#End Region

#Region " Code Generation "
        Private _codeGenMethod As CodeGenerationMethod = CodeGenerationMethod.Single
        Private _classType As GenerationClassType = GenerationClassType.Generated

        <Category("4. Code Generation"), _
        Description("Required - Generation Method. options are Single class, Partial class, Base class")> _
        Public Property GenerationMethod() As CodeGenerationMethod
            Get
                Return _codeGenMethod
            End Get
            Set(ByVal Value As CodeGenerationMethod)
                _codeGenMethod = Value
            End Set
        End Property

        <Category("4. Code Generation"), _
        Description("Required - Class Type. Generated class or User class.")> _
        Public Property ClassType() As GenerationClassType
            Get
                Return _classType
            End Get
            Set(ByVal Value As GenerationClassType)
                _classType = Value
            End Set
        End Property
#End Region

#Region " Other non visual properties "
        Private _baseIndentLevel As Integer = 0
        <Browsable(False)> _
        Public Property BaseIndentLevel() As Integer
            Get
                Return _baseIndentLevel
            End Get
            Set(ByVal Value As Integer)
                _baseIndentLevel = Value
            End Set
        End Property

        Private _xmlFilePath As String = String.Empty
        <Browsable(False)> _
        Public Property XmlFilePath() As String
            Get
                Return _xmlFilePath
            End Get
            Set(ByVal Value As String)
                _xmlFilePath = Value
            End Set
        End Property
#End Region

#Region " Rendering helper "

#Region " Properties and Methods "

        ''' <summary>
        ''' Return constraints to use when define generics class
        ''' </summary>
        ''' <param name="objInfo"></param>
        ''' <param name="level"></param>
        ''' <returns></returns>
        Public Function GetConstraint(ByVal objInfo As ObjectInfo, ByVal level As Integer) As String
            Return Indent(level, False) + objInfo.Constraint.Replace(Environment.NewLine, Indent(level, True))
        End Function

        ''' <summary>
        ''' Iterate through all properties and return their member declarations.
        ''' </summary>
        ''' <param name="objInfo"></param>
        ''' <param name="level"></param>
        ''' <returns></returns>
        Public Function GetMemberDeclarations(ByVal objInfo As ObjectInfo, ByVal level As Integer) As String
            Dim members As String = Indent(level, True) + "' declare members"
            For Each prop As PropertyInfo In objInfo.Properties
                ' if csla-type, later...
                If Not objInfo.ChildCollection.Contains(prop) Then
                    ' if pk (non composite) and type of Guid, default value is Guid.NewGuid
                    If objInfo.UniqueProperties.Count = 1 AndAlso objInfo.UniqueProperties.Contains(prop) AndAlso prop.Type = "Guid" Then
                        members += Indent(level, True) + GetMemberDeclaration(prop.MemberAccess, prop.Type, prop.MemberName, "Guid.NewGuid()")
                    Else
                        members += Indent(level, True) + GetMemberDeclaration(prop)
                    End If
                End If
            Next

            ' add csla-type child objects (not simple type but csla-type class)
            If objInfo.HasChild Then
                members += Environment.NewLine + Indent(level, True) + "' declare child member(s)"
            End If
            For Each prop As PropertyInfo In objInfo.ChildCollection
                members += Indent(level, True) + GetMemberDeclaration(prop)
            Next
            If members.Length > 0 Then members = members.Substring(2)
            Return members
        End Function

        Public Function GetMemberDeclaration(ByVal prop As PropertyInfo) As String
            Return GetMemberDeclaration(prop.MemberAccess, prop.Type, prop.MemberName, prop.DefaultValue)
        End Function

        ' return member declaration statement
        Public Function GetMemberDeclaration(ByVal access As String, ByVal type As String, ByVal name As String, ByVal init As String) As String
            If init.Length > 0 Then init = " = " + init
            Return String.Format("{0} {1} As {2}{3}", access, name, type, init)
        End Function

        ''' <summary>
        ''' Iterate through object's properties and return all property statements
        ''' </summary>
        ''' <param name="objInfo"></param>
        ''' <param name="level"></param>
        ''' <returns></returns>
        Public Function GetPropertyDeclarations(ByVal objInfo As ObjectInfo, ByVal level As Integer) As String
            Dim props As String = String.Empty
            For Each prop As PropertyInfo In objInfo.Properties
                If Not objInfo.ChildCollection.Contains(prop) Then
                    props += GetPropertyDeclaration(prop, level)
                End If
            Next
            For Each prop As PropertyInfo In objInfo.ChildCollection
                props += GetPropertyDeclaration(prop, level)
            Next
            If props.Length > 0 Then props = props.Substring(2)
            Return props
        End Function

        ' return property statement
        Public Function GetPropertyDeclaration(ByVal prop As PropertyInfo, ByVal level As Integer) As String
            Dim str As String = PROPERTY_TEMPLATE
            Dim propNameAsString As String = """" + prop.Name + """"

            str = str.Replace("[COMMENT]", PROPERTY_COMMENT)

            If prop.IsReadOnly Then
                str = str.Replace("[READWRITE]", "ReadOnly")
                str = str.Replace("[GETSET]", "Get")
            Else
                str = str.Replace("[SETTER]", SETTER_TEMPLATE)
                str = str.Replace("[GETSET]", "Get or Set")
            End If

            If prop.Type.ToLower = "smartdate" Then
                Dim NewStr As String = str  'get-set as string 
                NewStr = NewStr.Replace("[TYPE]", "String")
                NewStr = NewStr.Replace("[PROPNAME]", prop.Name + "String")
                NewStr = NewStr.Replace("[VARNAME]", prop.MemberName + ".Text")
                NewStr = NewStr.Replace("[CONVERTNULL]", CONVERTNULL_TEMPLATE)

                If MinimizeStackTraceUse Then
                    ' property name is "<propertyName>String"
                    If prop.Authorization = PropertyAccessSecurity.Read OrElse prop.Authorization = PropertyAccessSecurity.Both Then
                        NewStr = NewStr.Replace("[CANREAD]", CANREAD_NOSTACK_TEMPLATE)
                    End If
                    If prop.Authorization = PropertyAccessSecurity.Write OrElse prop.Authorization = PropertyAccessSecurity.Both Then
                        NewStr = NewStr.Replace("[CANWRITE]", CANWRITE_NOSTACK_TEMPLATE)
                    End If
                    NewStr = NewStr.Replace("[PROPCHANGED]", PROPCHANGED_TEMPLATE)
                    NewStr = NewStr.Replace("[PROPNAMESTRING]", """" + prop.Name + "String""")
                End If

                str = str + NewStr
            End If

            If prop.IsPrimaryKey Then
                str = str.Replace("[ATTRIBUTE]", String.Format("[BASEINDENT]<System.ComponentModel.DataObjectField({0}, {1})> _", prop.IsPrimaryKey.ToString().ToLower(), prop.IsIdentity.ToString().ToLower()))
            End If

            If MinimizeStackTraceUse Then
                str = str.Replace("[NOINLINE]", String.Empty)
            Else
                str = str.Replace("[NOINLINE]", "[BASEINDENT][INDENT]<System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _")
            End If

            If prop.Authorization = PropertyAccessSecurity.Read OrElse prop.Authorization = PropertyAccessSecurity.Both Then
                If MinimizeStackTraceUse Then
                    str = str.Replace("[CANREAD]", CANREAD_NOSTACK_TEMPLATE)
                Else
                    str = str.Replace("[CANREAD]", CANREAD_TEMPLATE)
                End If
            End If

            If prop.Authorization = PropertyAccessSecurity.Write OrElse prop.Authorization = PropertyAccessSecurity.Both Then
                If MinimizeStackTraceUse Then
                    str = str.Replace("[CANWRITE]", CANWRITE_NOSTACK_TEMPLATE)
                Else
                    str = str.Replace("[CANWRITE]", CANWRITE_TEMPLATE)
                End If
            End If

            str = str.Replace("[PROPCHANGED]", PROPCHANGED_TEMPLATE)

            If MinimizeStackTraceUse Then
                str = str.Replace("[PROPNAMESTRING]", propNameAsString)
            End If

            str = str.Replace("[MODIFIERS]", prop.Modifiers)
            str = str.Replace("[PROPNAME]", prop.Name)
            If prop.Type.ToLower = "smartdate" Then 'get in 
                str = str.Replace("[TYPE]", "DateTime")
                str = str.Replace("[VARNAME]", prop.MemberName + ".Date")
            Else
                str = str.Replace("[TYPE]", prop.Type)
                str = str.Replace("[VARNAME]", prop.MemberName)
            End If

            If prop.Type.ToLower = "string" Then
                'case insensitive comparison
                str = str.Replace("[COMPARE]", String.Format("Not {0}.Equals(value)", prop.MemberName))
                str = str.Replace("[CONVERTNULL]", CONVERTNULL_TEMPLATE)
            Else
                str = str.Replace("[COMPARE]", String.Format("Not {0}.Equals(value)", prop.MemberName))
            End If

            str = str.Replace("[BASEINDENT]", Indent(level, True))
            str = str.Replace("[INDENT]", Indent())
            str = Regex.Replace(str, "\[\w+\]", "")   'clean up unused tags.
            Return str
        End Function

#End Region                     'Properties and Methods

#Region " Validation Rules "
        ''' <summary>
        ''' Iterate through all properties and return commonly used validation rule statments
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <param name="level"></param>
        ''' <param name="isShared"></param>
        ''' <returns></returns>
        Public Function GetCommonValidationRules(ByVal obj As ObjectInfo, ByVal level As Integer, Optional ByVal isShared As Boolean = False) As String
            Dim rules As String = String.Empty
            Dim prop As PropertyInfo
            For Each prop In obj.Properties
                rules += GetCommonValidationRule(prop, level, isShared)
            Next
            If rules.Length > 0 Then rules = rules.Substring(2)
            Return rules
        End Function

        ''' <summary>
        ''' return commonly used validation rule statement:
        ''' - StringRequired
        ''' - StringMaxLength
        ''' </summary>
        ''' <param name="prop"></param>
        ''' <param name="level"></param>
        ''' <param name="isShared"></param>
        ''' <param name="objType"></param>
        ''' <returns></returns>
        Private Function GetCommonValidationRule(ByVal prop As PropertyInfo, ByVal level As Integer, Optional ByVal isShared As Boolean = False) As String
            Dim rules As String = String.Empty
            Dim instance As String = String.Empty
            If Not isShared Then instance = "Instance"

            If prop.Type.ToLower = "string" Then
                If prop.IsRequired Then
                    rules += Indent(level, True) + String.Format("ValidationRules.Add{0}Rule(AddressOf Csla.Validation.CommonRules.StringRequired, ""{1}"")", instance, prop.Name)
                End If
                If prop.MaxSize > 0 Then
                    rules += Indent(level, True) + String.Format("ValidationRules.Add{0}Rule(AddressOf Csla.Validation.CommonRules.StringMaxLength, New Csla.Validation.CommonRules.MaxLengthRuleArgs(""{1}"", {2}))", instance, prop.Name, prop.MaxSize)
                End If
            Else
                If prop.Type.ToLower = "smartdate" AndAlso prop.IsRequired Then
                    rules += Indent(level, True) + String.Format("ValidationRules.Add{0}Rule(AddressOf Csla.Validation.CommonRules.StringRequired, ""{1}"")", instance, prop.Name + "String")
                End If
            End If
            If rules.Length > 0 Then
                Dim dent As String = Indent(level, True) + "'"
                rules = String.Format("{0} {1} rules{2}", dent, prop.Name, rules)
            End If
            Return rules
        End Function
#End Region                     'Validation Rules

#Region " Data Access "
        ''' <summary>
        ''' return datareader get field statement
        ''' </summary>
        ''' <param name="prop"></param>
        ''' <returns></returns>
        Public Function GetReaderAssignmentStatement(ByVal prop As PropertyInfo) As String
            Return DalHelper.ReaderStatement(prop.MemberName, prop.Type, prop.DbColumnName)
        End Function

        ''' <summary>
        ''' return parameter assignment for filter command
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <param name="level"></param>
        ''' <returns></returns>
        Public Function GetFilterParameters(ByVal obj As ObjectInfo, ByVal level As Integer) As String
            Dim statement As String = String.Empty

            Dim prop As PropertyInfo
            For Each prop In obj.FilterProperties
                statement += GetParameterStatement(prop, String.Empty, "criteria", True, level)
            Next
            If statement.Length > 0 Then statement = statement.Substring(2)
            Return statement
        End Function

        ''' <summary>
        ''' return parameter assignments for get command
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <param name="level"></param>
        ''' <returns></returns>
        Public Function GetFetchParameters(ByVal obj As ObjectInfo, ByVal level As Integer) As String
            Return GetFetchParameters(obj, String.Empty, "criteria", level)
        End Function

        ' internal method that return parameter assignment for get command
        Public Function GetFetchParameters(ByVal obj As ObjectInfo, ByVal parPrefix As String, ByVal varPrefix As String, ByVal level As Integer) As String
            Dim statement As String = String.Empty

            Dim prop As PropertyInfo
            For Each prop In obj.UniqueProperties
                statement += GetParameterStatement(prop, parPrefix, varPrefix, True, level)
            Next
            If statement.Length > 0 Then statement = statement.Substring(2)
            Return statement
        End Function

        ''' <summary>
        ''' return paramenter assignments for insert command
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <param name="level"></param>
        ''' <returns></returns>
        Public Function GetInsertParameters(ByVal obj As ObjectInfo, ByVal level As Integer) As String
            Dim statement As String = String.Empty
            Dim outputStatement As String = String.Empty

            For Each prop As PropertyInfo In obj.Properties
                If prop.UpdateToDb Then
                    If Not prop.IsDbComputed Then
                        statement += GetParameterStatement(prop, String.Empty, "Me", True, level)
                    Else
                        outputStatement += GetParameterStatement(prop, "new_", "Me", False, level)
                    End If
                End If
            Next
            If statement.Length > 0 Then statement = statement.Substring(2)
            Return statement + outputStatement
        End Function

        ''' <summary>
        ''' return paramenter assignments for insert sql store procedure
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <param name="level"></param>
        ''' <returns></returns>
        Public Function GetInsertSqlParameters(ByVal obj As ObjectInfo, ByVal level As Integer) As String
            Dim statement As String = String.Empty
            Dim outputStatement As String = String.Empty
            Dim fullStatement As String = String.Empty

            For i As Int32 = 0 To obj.Properties.Count - 1
                Dim prop As PropertyInfo = CType(obj.Properties.Item(i), PropertyInfo)
                If prop.UpdateToDb Then
                    If Not prop.IsDbComputed Then
                        statement += GetSqlParameterStatement(prop, String.Empty, "Me", True, level) + ","
                    Else
                        outputStatement += GetSqlParameterStatement(prop, "new_", "Me", False, level) + ","
                    End If
                End If
            Next
            If statement.Length > 0 Then statement = statement.Substring(2)
            fullStatement = statement + outputStatement
            Return fullStatement.Substring(0, fullStatement.Length - 1)
        End Function

        ''' <summary>
        ''' return paramenter assignments for update command
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <param name="level"></param>
        ''' <returns></returns>
        Public Function GetUpdateParameters(ByVal obj As ObjectInfo, ByVal level As Integer) As String
            Dim statement As String = String.Empty
            Dim outputStatement As String = String.Empty

            Dim prop As PropertyInfo
            For Each prop In obj.Properties
                If prop.UpdateToDb Then
                    If Not prop.IsDbComputed Then
                        statement += GetParameterStatement(prop, String.Empty, "Me", True, level)
                    Else
                        If prop.IsIdentity Then
                            statement += GetParameterStatement(prop, String.Empty, "Me", True, level)
                        Else
                            outputStatement += GetParameterStatement(prop, "new_", "Me", False, level)
                        End If
                    End If
                End If
            Next
            If statement.Length > 0 Then statement = statement.Substring(2)
            Return statement + outputStatement
        End Function

        ' internal method that return parameter assigment
        Public Function GetParameterStatement(ByVal prop As PropertyInfo, ByVal parPrefix As String, ByVal varPrefix As String, ByVal input As Boolean, ByVal level As Integer) As String
            If Not prop.HasDbColumn Then Return String.Empty
            Dim statement As String = String.Empty
            Dim varName As String
            Dim varType As String = prop.Type

            'check if criteria property or variable
            If varPrefix = String.Empty OrElse varPrefix = "Me" Then
                varName = prop.MemberName
            Else
                varName = varPrefix + "." + prop.Name
            End If

            If prop.Type.ToLower = "smartdate" Then 'special treatment on smartdate :(
                If input Then
                    varName += ".DBValue"
                Else
                    varName += "Date"
                    varType = "DateTime"
                End If
            End If
            statement = Indent(level, True) + DalHelper.ParameterAssignmentStatement(parPrefix + prop.DbColumnName, varName)
            If Not input Then
                statement += Indent(level, True) + String.Format("cm.Parameters(""@{0}"").Direction = ParameterDirection.Output", parPrefix + prop.DbColumnName)
            End If
            Return statement
        End Function

        ' internal method that return parameter assigment
        Public Function GetSqlParameterStatement(ByVal prop As PropertyInfo, ByVal parPrefix As String, ByVal varPrefix As String, ByVal input As Boolean, ByVal level As Integer) As String
            If Not prop.HasDbColumn Then Return String.Empty
            Dim statement As String = String.Empty
            Dim varName As String

            statement = Indent(level, True) + "@" + parPrefix + prop.DbColumnName + " As " + prop.SqlType
            If Not input Then
                statement += " OUTPUT"
            End If
            Return statement
        End Function

        ''' <summary>
        ''' return statement to return output paramenter for insert command
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <param name="level"></param>
        ''' <returns></returns>
        Public Function GetInsertReturnParameterStatements(ByVal obj As ObjectInfo, ByVal level As Integer) As String
            Dim statement As String = String.Empty

            Dim prop As PropertyInfo
            For Each prop In obj.Properties
                If prop.HasDbColumn Then
                    If prop.IsDbComputed Then
                        statement += Indent(level, True) + GetReturnParameterStatement(prop)
                    End If
                End If
            Next
            Return statement
        End Function

        ''' <summary>
        ''' return statement to return output parameter for update command
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <param name="level"></param>
        ''' <returns></returns>
        Public Function GetUpdateReturnParameterStatements(ByVal obj As ObjectInfo, ByVal level As Integer) As String
            Dim statement As String = String.Empty

            Dim prop As PropertyInfo
            For Each prop In obj.Properties
                If prop.HasDbColumn Then
                    If prop.IsDbComputed AndAlso Not prop.IsIdentity Then
                        statement += Indent(level, True) + GetReturnParameterStatement(prop)
                    End If
                End If
            Next
            Return statement
        End Function

        'internal method to return statement for retrieve output parameter
        Public Function GetReturnParameterStatement(ByVal prop As PropertyInfo) As String
            If Not prop.HasDbColumn Then Return String.Empty
            Dim var As String = prop.MemberName
            Dim par As String = "new_" + prop.DbColumnName
            Dim type As String = prop.Type

            If prop.Type.ToLower = "smartdate" Then
                var += ".Date"
                type = "DateTime"
            End If

            Return DalHelper.ParameterReturnStatement(var, type, par)
        End Function

        Public Function GetNewNameValuePair(ByVal obj As ObjectInfo) As String
            Dim keyProp As PropertyInfo = CType(obj.UniqueProperties(0), PropertyInfo)
            Dim valueProp As PropertyInfo = Nothing
            Dim prop As PropertyInfo
            For Each prop In obj.Properties
                If Not prop.IsPrimaryKey Then
                    valueProp = prop
                    Exit For
                End If
            Next
            Dim format As String = "New NameValuePair(dr.{0}(""{1}""), dr.{2}(""{3}""))"
            Return String.Format(format, DalHelper.GetReaderMethod(keyProp.Type), keyProp.DbColumnName, DalHelper.GetReaderMethod(valueProp.Type), valueProp.DbColumnName)
        End Function
#End Region

#Region " Factory Methods and Criteria "
        ''' <summary>
        ''' return parameter declaration in factory get collection
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <returns></returns>
        Public Function GetFactoryFilterDeclarationArguments(ByVal obj As ObjectInfo) As String
            Dim para As String = String.Empty
            Dim prop As PropertyInfo
            For Each prop In obj.FilterProperties
                para += String.Format(", ByVal {1} As {0}", prop.Type, VbHelper.GetCamelCaseName(prop.Name))
            Next
            If (para.Length > 0) Then para = para.Substring(2)
            Return para
        End Function

        ''' <summary>
        ''' return parameter call/pass statement in factory get collection
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <returns></returns>
        Public Function GetFactoryFilterCallArguments(ByVal obj As ObjectInfo) As String
            Dim para As String = String.Empty
            Dim prop As PropertyInfo
            For Each prop In obj.FilterProperties
                para += String.Format(", {0}", VbHelper.GetCamelCaseName(prop.Name))
            Next
            If (para.Length > 0) Then para = para.Substring(2)
            Return para
        End Function

        ''' <summary>
        ''' Return assignment statement on filter criteria
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <param name="level"></param>
        ''' <param name="useMember"></param>
        ''' <returns></returns>
        Public Function GetFactoryFilterAssignments(ByVal obj As ObjectInfo, ByVal level As Integer, ByVal useMember As Boolean) As String
            Dim members As String = String.Empty
            Dim prop As PropertyInfo
            For Each prop In obj.FilterProperties
                If useMember Then
                    members += Indent(level, True) + String.Format("Me.{0} = {1}", prop.MemberName, VbHelper.GetCamelCaseName(prop.Name))
                Else
                    members += Indent(level, True) + String.Format("Me.{0} = {1}", prop.Name, VbHelper.GetCamelCaseName(prop.Name))
                End If
            Next
            If (members.Length > 0) Then members = members.Substring(2)
            Return members
        End Function

        Public Function GetFactoryNewDeclarationArguments(ByVal obj As ObjectInfo) As String
            Return GetFactoryDeclarationArguments(obj, True)
        End Function

        Public Function GetFactoryNewCallArguments(ByVal obj As ObjectInfo) As String
            Return GetFactoryCallArguments(obj, True, False)
        End Function

        Public Function GetFactoryNewAssignments(ByVal obj As ObjectInfo, ByVal level As Integer) As String
            If obj.IsCollection Then Return String.Empty
            If obj.HasIdentity Then Return String.Empty
            If obj.HasObjectGeneratedKey Then Return String.Empty

            Dim members As String = String.Empty
            Dim prop As PropertyInfo
            For Each prop In obj.UniqueProperties
                members += Indent(level, True) + String.Format("Me.{0} = {1}", prop.MemberName, VbHelper.GetCamelCaseName(prop.Name))
            Next
            If (members.Length > 0) Then members = members.Substring(2)
            Return members
        End Function

        Public Function GetFactoryDeclarationArguments(ByVal obj As ObjectInfo) As String
            Return GetFactoryDeclarationArguments(obj, False)
        End Function

        Public Function GetFactoryDeclarationArguments(ByVal obj As ObjectInfo, ByVal isNew As Boolean) As String
            If obj.IsCollection Then Return String.Empty
            If obj.HasIdentity AndAlso isNew Then Return String.Empty
            If obj.HasObjectGeneratedKey AndAlso isNew Then Return String.Empty

            Dim para As String = String.Empty
            Dim prop As PropertyInfo
            For Each prop In obj.UniqueProperties
                para += String.Format(", ByVal {1} As {0}", prop.Type, VbHelper.GetCamelCaseName(prop.Name))
            Next
            If (para.Length > 0) Then para = para.Substring(2)
            Return para
        End Function

        Public Function GetFactoryCallArguments(ByVal obj As ObjectInfo) As String
            Return GetFactoryCallArguments(obj, False, False)
        End Function

        Public Function GetDeleteSelfCriteriaCallArguments(ByVal obj As ObjectInfo) As String
            Return GetFactoryCallArguments(obj, False, True)
        End Function

        Public Function GetFactoryCallArguments(ByVal obj As ObjectInfo, ByVal isNew As Boolean, ByVal useMember As Boolean) As String
            If obj.IsCollection Then Return String.Empty
            If obj.HasIdentity AndAlso isNew Then Return String.Empty
            If obj.HasObjectGeneratedKey AndAlso isNew Then Return String.Empty

            Dim para As String = String.Empty
            Dim prop As PropertyInfo
            For Each prop In obj.UniqueProperties
                If useMember Then
                    para += String.Format(", {0}", prop.MemberName)
                Else
                    para += String.Format(", {0}", VbHelper.GetCamelCaseName(prop.Name))
                End If
            Next
            If (para.Length > 0) Then para = para.Substring(2)
            Return para
        End Function

        Public Function GetCriteriaDeclarationArguments(ByVal obj As ObjectInfo) As String
            Return GetFactoryDeclarationArguments(obj)
        End Function

        Public Function GetCriteriaMemberAssignment(ByVal prop As PropertyInfo) As String
            Return String.Format("Me.{0} = {1}", prop.Name, VbHelper.GetCamelCaseName(prop.Name))
        End Function

        Public Function GetCriteriaPropertyDeclaration(ByVal prop As PropertyInfo) As String
            Return String.Format("Public {1} As {0}", prop.Type, prop.Name)
        End Function
#End Region

#Region " Indent "
        Public Function Indent() As String
            Return Indent(1)
        End Function

        Public Function Indent(ByVal level As Integer) As String
            Return Indent(level, False)
        End Function

        Public Function Indent(ByVal level As Integer, ByVal NewLine As Boolean) As String
            Dim str As String = String.Empty

            If (NewLine) Then str = Environment.NewLine
            If (IndentLevelSpaces > 0) Then
                str += New String(" "c, level * IndentLevelSpaces)
            Else
                Dim tab As Char = Strings.Chr(9)
                str += New String(tab, level)
            End If
            Return str
        End Function
#End Region                     'Indent
#End Region

#Region " Object Class "
        Public Class ObjectInfo
#Region " Template Settings "
            Private _transactionType As TransactionalTypes
            Private _propertyAuthorization As PropertyAccessSecurity
            Private _useSecurity As Boolean
            Private _codeGenMethod As CodeGenerationMethod = CodeGenerationMethod.Single
            Private _classType As GenerationClassType = GenerationClassType.Generated
            Private _rootCommand As CommandSchema = Nothing
#End Region

#Region " Parent Properties "
            Private _parent As String = ""
            Public ReadOnly Property Parent() As String
                Get
                    If (IsGeneratedBase AndAlso _parent.Length > 0) Then Return _parent + BaseSuffix
                    Return _parent
                End Get
            End Property
            Public ReadOnly Property ParentType() As String
                Get
                    If (IsGeneratedBase AndAlso _parent.Length > 0) Then Return GenericTypeParentParameter
                    Return _parent
                End Get
            End Property
            Public ReadOnly Property ParentSuffix() As String
                Get
                    If Not IsGeneratedBase Then Return String.Empty
                    Return String.Format("(Of {0})", ParentType)
                End Get
            End Property
            Public ReadOnly Property ParentNameAndSuffix() As String
                Get
                    Return Parent + ParentSuffix
                End Get
            End Property
#End Region                     'Parent Properties

#Region " Child Properties "
            Public ReadOnly Property ChildType() As String
                Get
                    If IsGeneratedBase Then Return GenericTypeChildParameter
                    Return _child
                End Get
            End Property
            Private _child As String = ""
            Public ReadOnly Property Child() As String
                Get
                    If IsGeneratedBase Then Return _child + BaseSuffix
                    Return _child
                End Get
            End Property
            Public ReadOnly Property CustomChild() As String
                Get
                    Return _child
                End Get
            End Property
            Public ReadOnly Property ChildSuffix() As String
                Get
                    If Not IsGeneratedBase Then Return String.Empty
                    Return String.Format("(Of {0})", ChildType)
                End Get
            End Property
            Public ReadOnly Property ChildNameAndSuffix() As String
                Get
                    Return Child + ChildSuffix
                End Get
            End Property
#End Region                     'Child Properties

#Region " Properties "
            Private _objectName As String
            Public ReadOnly Property Name() As String
                Get
                    If IsGeneratedBase Then Return _objectName + BaseSuffix
                    Return _objectName
                End Get
            End Property
            Public ReadOnly Property CustomName() As String
                Get
                    Return _objectName
                End Get
            End Property
            Public ReadOnly Property Suffix() As String
                Get
                    If Not IsGeneratedBase Then Return String.Empty
                    If IsCollection Then Return String.Format("(Of {0}{2}, {1}{3})", Type, ChildType, Constraint, ChildConstraint)
                    Return String.Format("(Of {0}{1})", Type, Constraint)
                End Get
            End Property
            Public ReadOnly Property NameAndSuffix() As String
                Get
                    Return Name + Suffix
                End Get
            End Property
            Public ReadOnly Property Type() As String
                Get
                    If IsGeneratedBase Then Return GenericTypeObjectParameter
                    Return _objectName
                End Get
            End Property
            Private _childCollection As ArrayList = New ArrayList
            Public ReadOnly Property ChildCollection() As ArrayList
                Get
                    Return _childCollection
                End Get
            End Property

            Private _uniqueProperties As ArrayList = New ArrayList
            Public ReadOnly Property UniqueProperties() As ArrayList
                Get
                    Return _uniqueProperties
                End Get
            End Property

            Private _filterProperties As ArrayList = New ArrayList
            Public Property FilterProperties() As ArrayList
                Get
                    Return _filterProperties
                End Get
                Set(ByVal Value As ArrayList)
                    _filterProperties = Value
                End Set
            End Property
            Private _properties As ArrayList = New ArrayList
            Public ReadOnly Property Properties() As ArrayList
                Get
                    Return _properties
                End Get
            End Property

            Private _objectType As ObjectType
            Public ReadOnly Property CslaObjectType() As ObjectType
                Get
                    Return _objectType
                End Get
            End Property
#End Region                     'Properties

#Region " Methods "
            Public ReadOnly Property MemberAccess() As String
                Get
                    If (IsGeneratedBase) Then
                        Return "Protected"
                    Else
                        Return "Private"
                    End If
                End Get
            End Property
            Public ReadOnly Property NewMethodName() As String
                Get
                    Return String.Format(FactoryNewFormat, _objectName)
                End Get
            End Property
            Public ReadOnly Property GetMethodName() As String
                Get
                    Return String.Format(FactoryGetFormat, _objectName)
                End Get
            End Property
            Public ReadOnly Property DeleteMethodName() As String
                Get
                    Return String.Format(FactoryDeleteFormat, _objectName)
                End Get
            End Property
            Public ReadOnly Property NewChildMethodName() As String
                Get
                    Return String.Format(FactoryNewFormat, _child)
                End Get
            End Property
            Public ReadOnly Property GetChildMethodName() As String
                Get
                    Return String.Format(FactoryGetFormat, _child)
                End Get
            End Property
            Public ReadOnly Property LocalMethodModifiers() As String
                Get
                    If (IsGeneratedBase) Then
                        Return MemberAccess + " Overridable"
                    End If
                    Return MemberAccess
                End Get
            End Property
            Public ReadOnly Property UseSecurity() As Boolean
                Get
                    Return _useSecurity
                End Get
            End Property
            Public ReadOnly Property PropertyAuthorization() As PropertyAccessSecurity
                Get
                    Return _propertyAuthorization
                End Get
            End Property
#End Region

#Region " Object Characteristics "
            Public ReadOnly Property GenerationMethod() As CodeGenerationMethod
                Get
                    Return _codeGenMethod
                End Get
            End Property
            Public ReadOnly Property IsCollection() As Boolean
                Get
                    Select Case _objectType
                        Case ObjectType.EditableRoot
                            Return False
                        Case ObjectType.EditableRootList
                            Return True
                        Case ObjectType.EditableChild
                            Return False
                        Case ObjectType.EditableChildList
                            Return True
                        Case ObjectType.EditableSwitchable
                            Return False
                        Case ObjectType.NameValueList
                            Return True
                        Case ObjectType.ReadOnlyRoot
                            Return False
                        Case ObjectType.ReadOnlyRootList
                            Return True
                        Case ObjectType.ReadOnlyChild
                            Return False
                        Case ObjectType.ReadOnlyChildList
                            Return True
                    End Select
                    Return False
                End Get
            End Property
            Public ReadOnly Property IsReadOnly() As Boolean
                Get
                    Select Case _objectType
                        Case ObjectType.EditableRoot
                            Return False
                        Case ObjectType.EditableRootList
                            Return False
                        Case ObjectType.EditableChild
                            Return False
                        Case ObjectType.EditableChildList
                            Return False
                        Case ObjectType.EditableSwitchable
                            Return False
                        Case ObjectType.NameValueList
                            Return True
                        Case ObjectType.ReadOnlyRoot
                            Return True
                        Case ObjectType.ReadOnlyRootList
                            Return True
                        Case ObjectType.ReadOnlyChild
                            Return True
                        Case ObjectType.ReadOnlyChildList
                            Return True
                    End Select
                    Return False
                End Get
            End Property
            Public ReadOnly Property IsChild() As Boolean
                Get
                    Select Case _objectType
                        Case ObjectType.EditableRoot
                            Return False
                        Case ObjectType.EditableRootList
                            Return False
                        Case ObjectType.EditableChild
                            Return True
                        Case ObjectType.EditableChildList
                            Return True
                        Case ObjectType.EditableSwitchable
                            Return True
                        Case ObjectType.NameValueList
                            Return False
                        Case ObjectType.ReadOnlyRoot
                            Return False
                        Case ObjectType.ReadOnlyRootList
                            Return False
                        Case ObjectType.ReadOnlyChild
                            Return True
                        Case ObjectType.ReadOnlyChildList
                            Return True
                    End Select
                    Return False
                End Get
            End Property

            Public ReadOnly Property IsGeneratedClass() As Boolean
                Get
                    If (_codeGenMethod = CodeGenerationMethod.Single) Then Return True
                    If (_classType = GenerationClassType.Generated) Then Return True
                    Return False
                End Get
            End Property
            Public ReadOnly Property IsUserClass() As Boolean
                Get
                    If (_codeGenMethod = CodeGenerationMethod.Single) Then Return True
                    If (_classType = GenerationClassType.User) Then Return True
                    Return False
                End Get
            End Property
            Public ReadOnly Property IsSingle() As Boolean
                Get
                    Return _codeGenMethod = CodeGenerationMethod.Single
                End Get
            End Property
            Public ReadOnly Property IsBase() As Boolean
                Get
                    Return _codeGenMethod = CodeGenerationMethod.SplitBase
                End Get
            End Property
            Public ReadOnly Property IsPartial() As Boolean
                Get
                    Return _codeGenMethod = CodeGenerationMethod.SplitPartial
                End Get
            End Property
            Public ReadOnly Property IsGeneratedBase() As Boolean
                Get
                    Return _codeGenMethod = CodeGenerationMethod.SplitBase AndAlso _classType = GenerationClassType.Generated
                End Get
            End Property
            Public ReadOnly Property IsUserBase() As Boolean
                Get
                    Return _codeGenMethod = CodeGenerationMethod.SplitBase AndAlso _classType = GenerationClassType.User
                End Get
            End Property
            Public ReadOnly Property IsGeneratedPartial() As Boolean
                Get
                    Return _codeGenMethod = CodeGenerationMethod.SplitPartial AndAlso _classType = GenerationClassType.Generated
                End Get
            End Property
            Public ReadOnly Property IsUserPartial() As Boolean
                Get
                    Return _codeGenMethod = CodeGenerationMethod.SplitPartial AndAlso _classType = GenerationClassType.User
                End Get
            End Property
            Public ReadOnly Property HasIdentity() As Boolean
                Get
                    Dim prop As PropertyInfo
                    For Each prop In UniqueProperties
                        If prop.IsIdentity Then Return True
                    Next
                    Return False
                End Get
            End Property
            Public ReadOnly Property HasChild() As Boolean
                Get
                    Return _childCollection.Count > 0
                End Get
            End Property
            Public ReadOnly Property HasObjectGeneratedKey() As Boolean
                Get
                    Return _uniqueProperties.Count = 1 AndAlso DirectCast(_uniqueProperties(0), PropertyInfo).Type = "Guid"
                End Get
            End Property
#End Region

#Region " Keywords "

            Private _access As String = "Public"

            Public ReadOnly Property Access() As String
                Get
                    Return _access
                End Get
            End Property

            Public ReadOnly Property Modifiers() As String
                Get
                    Dim mods As String = Access
                    If IsGeneratedBase Then
                        mods += " MustInherit"
                    End If
                    If IsPartial Then
                        mods += " Partial"
                    End If
                    Return mods
                End Get
            End Property

            Public ReadOnly Property [Inherits]() As String
                Get
                    'if user class for base type
                    If IsUserBase Then
                        If _objectType = ObjectType.NameValueList Then
                            Return String.Format(" {0}{1}", Name, BaseSuffix)
                        End If
                        If IsCollection Then Return String.Format(" {0}{1}(Of {2}, {3})", Name, BaseSuffix, Type, ChildType)
                        Return String.Format(" {0}{1}(Of {2})", Name, BaseSuffix, Type)
                    End If
                    'other types
                    Select Case _objectType
                        Case ObjectType.EditableRoot, ObjectType.EditableChild, ObjectType.EditableSwitchable
                            Return String.Format(" Csla.BusinessBase(Of {0})", Type)
                        Case ObjectType.ReadOnlyRoot, ObjectType.ReadOnlyChild
                            Return String.Format(" Csla.ReadOnlyBase(Of {0})", Type)
                        Case ObjectType.EditableChildList, ObjectType.EditableRootList
                            Return String.Format(" Csla.BusinessListBase(Of {0}, {1})", Type, ChildType)
                        Case ObjectType.NameValueList
                            Return String.Format(" Csla.NameValueListBase(Of {0}, {1})", (CType(_uniqueProperties(0), PropertyInfo)).Type, (CType(_properties(1), PropertyInfo)).Type)
                        Case ObjectType.ReadOnlyRootList, ObjectType.ReadOnlyChildList
                            Return String.Format(" Csla.ReadOnlyListBase(Of {0}, {1})", Type, ChildType)
                    End Select
                    Return String.Empty
                End Get
            End Property

            Public ReadOnly Property Constraint() As String
                Get
                    If Not IsGeneratedBase Then Return String.Empty
                    Select Case _objectType
                        Case ObjectType.EditableRoot, ObjectType.EditableChild, ObjectType.EditableSwitchable, ObjectType.ReadOnlyRoot, ObjectType.ReadOnlyChild
                            Return String.Format(" As {1}(Of {0})", Type, Name)
                        Case ObjectType.EditableChildList, ObjectType.ReadOnlyRootList
                            If IsGeneratedBase Then
                                Return String.Format(" As {1}(Of {0}, {2})", Type, Name, ChildType)
                            End If
                        Case ObjectType.EditableRootList
                            If IsGeneratedBase Then
                                Return String.Format(" As BusinessListBase(Of {0}, {1})", Type, ChildType)
                            End If
                    End Select
                    Return String.Empty
                End Get
            End Property

            Public ReadOnly Property ChildConstraint() As String
                Get
                    If Not IsGeneratedBase Then Return String.Empty
                    Select Case _objectType
                        Case ObjectType.EditableChildList, ObjectType.EditableRootList, ObjectType.ReadOnlyRootList, ObjectType.ReadOnlyChildList
                            Return String.Format(" As {0}", ChildNameAndSuffix)
                    End Select
                    Return String.Empty
                End Get
            End Property
#End Region

#Region " Data Access "
            Public ReadOnly Property TransactionType() As TransactionalTypes
                Get
                    Return _transactionType
                End Get
            End Property
            Public ReadOnly Property UseAdoTransaction() As Boolean
                Get
                    Return _transactionType = TransactionalTypes.Ado
                End Get
            End Property
            Public ReadOnly Property UseTransactionalAttribute() As Boolean
                Get
                    Return _transactionType = TransactionalTypes.EnterpriseService Or _transactionType = TransactionalTypes.TransactionScope
                End Get
            End Property
            Dim _dbName As String = String.Empty
            Public ReadOnly Property DbConnection() As String
                Get
                    Return String.Format(DbConnectionFormat, _dbName)
                End Get
            End Property
            Dim _dbOwner As String = String.Empty
            Public ReadOnly Property DbOwner() As String
                Get
                    Return String.Format(DbOwnerFormat, _dbOwner)
                End Get
            End Property
            Dim _dbTableOrViewName As String = String.Empty
            Public ReadOnly Property TableOrViewName() As String
                Get
                    Return _dbTableOrViewName
                End Get
            End Property
            Public ReadOnly Property FetchCommandText() As String
                Get
                    If (Not _rootCommand Is Nothing) Then
                        Return _rootCommand.Name
                    End If
                    Return String.Format(FetchCommandFormat, _objectName.ToLower)
                End Get
            End Property
            Public ReadOnly Property InsertCommandText() As String
                Get
                    Return String.Format(InsertCommandFormat, _objectName.ToLower)
                End Get
            End Property
            Public ReadOnly Property UpdateCommandText() As String
                Get
                    Return String.Format(UpdateCommandFormat, _objectName.ToLower)
                End Get
            End Property
            Public ReadOnly Property DeleteCommandText() As String
                Get
                    Return String.Format(DeleteCommandFormat, _objectName.ToLower)
                End Get
            End Property
#End Region

#Region " Constructors "
            Public Sub New(ByVal template As CodeTemplate)

                If (Not TemplateHelper.IsObjectType(template.CodeTemplateInfo)) Then
                    Throw New ArgumentException(String.Format("Template '{0}' is not a business Object template type", template.CodeTemplateInfo.FileName))
                End If

                Dim xmlpath As String = CType(template.GetProperty("XmlFilePath"), String)
                Dim isFromXml As Boolean = (xmlpath <> Nothing AndAlso xmlpath.Length > 0)
                If isFromXml Then
                    LoadFromXml(template)
                Else
                    LoadFromSchema(template)
                End If
            End Sub

            Private Sub LoadFromXml(ByVal template As CodeTemplate)
                _objectName = CType(template.GetProperty("ObjectName"), String)

                'template settings
                _transactionType = CType(template.GetProperty("TransactionalType"), TransactionalTypes)
                _propertyAuthorization = CType(template.GetProperty("PropertyAuthorization"), PropertyAccessSecurity)
                _useSecurity = CType(template.GetProperty("AuthorizationRules"), Boolean)
                _codeGenMethod = CType(template.GetProperty("GenerationMethod"), CodeGenerationMethod)
                _classType = CType(template.GetProperty("ClassType"), GenerationClassType)

                'read from xml file
                Dim path As String = CType(template.GetProperty("XmlFilePath"), String)

                Dim xtr As XmlTextReader = New XmlTextReader(path)

                While xtr.Read()
                    If (xtr.NodeType = XmlNodeType.Element AndAlso xtr.LocalName.ToLower() = "object") Then
                        If (xtr.GetAttribute("name") = _objectName) Then
                            _objectType = CType([Enum].Parse(GetType(ObjectType), xtr.GetAttribute("type"), True), ObjectType)
                            _child = xtr.GetAttribute("child")
                            _parent = xtr.GetAttribute("parent")

                            While xtr.Read()
                                If (xtr.NodeType = XmlNodeType.EndElement AndAlso xtr.LocalName.ToLower() = "properties") Then
                                    Exit While
                                End If
                                If (xtr.NodeType = XmlNodeType.Element AndAlso xtr.LocalName.ToLower() = "property") Then
                                    Dim prop As PropertyInfo = New PropertyInfo(xtr, Me)
                                    _properties.Add(prop)

                                    If (prop.IsCollection AndAlso prop.IsCslaClass) Then
                                        _childCollection.Add(prop)
                                    End If
                                    If (prop.IsPrimaryKey) Then
                                        _uniqueProperties.Add(prop)
                                    End If
                                    If (prop.IsFilterKey) Then
                                        _filterProperties.Add(prop)
                                    End If
                                End If
                            End While

                            Exit While 'finish
                        End If
                    End If
                End While
                xtr.Close()

                'validate object
                Validate()
            End Sub

            Private Sub LoadFromSchema(ByVal template As CodeTemplate)
                _objectType = TemplateHelper.ToObjectType(template)

                'object, child, and parent name
                _objectName = CType(template.GetProperty("ObjectName"), String)
                If (IsCollection) Then
                    _child = CType(template.GetProperty("ChildName"), String)
                End If
                If (IsChild) Then
                    _parent = CType(template.GetProperty("ParentName"), String)
                End If
                If (_parent Is Nothing) Then _parent = String.Empty

                'child collections
                Dim types As StringCollection = CType(template.GetProperty("ChildCollectionNames"), StringCollection)
                Dim names As StringCollection = CType(template.GetProperty("ChildPropertyNames"), StringCollection)
                If Not types Is Nothing AndAlso Not names Is Nothing AndAlso types.Count > 0 AndAlso names.Count > 0 Then
                    Dim maxCount As Integer = names.Count
                    If types.Count < names.Count Then
                        maxCount = types.Count
                    End If
                    Dim i As Integer
                    For i = 0 To maxCount - 1 Step i + 1
                        If (names(i).TrimEnd() <> String.Empty AndAlso types(i).TrimEnd() <> String.Empty) Then
                            Dim prop As PropertyInfo = New PropertyInfo(names(i), types(i), Me)
                            _properties.Add(prop)
                            _childCollection.Add(prop)
                        End If
                    Next
                End If

                'table, view schema
                Dim table As TableSchema = CType(template.GetProperty("RootTable"), TableSchema)
                Dim view As ViewSchema = CType(template.GetProperty("RootView"), ViewSchema)
                Dim command As CommandSchema = CType(template.GetProperty("RootCommand"), CommandSchema)
                Dim resultSetIndex As Integer = CType(template.GetProperty("ResultSetIndex"), Integer)
                If (table Is Nothing AndAlso view Is Nothing AndAlso command Is Nothing) Then
                    Throw New Exception("RootCommand, RootTable or RootView is required.")
                End If

                Dim uniqueColumns As StringCollection = CType(template.GetProperty("UniqueColumnNames"), StringCollection)
                If (uniqueColumns Is Nothing) Then uniqueColumns = New StringCollection

                Dim filterColumns As StringCollection = CType(template.GetProperty("FilterColumnNames"), StringCollection)
                If (filterColumns Is Nothing) Then filterColumns = New StringCollection

                If (Not command Is Nothing) Then
                    LoadProperties(command, resultSetIndex, uniqueColumns, filterColumns)
                ElseIf (Not table Is Nothing) Then
                    LoadProperties(table)
                Else
                    LoadProperties(view, uniqueColumns, filterColumns)
                End If
                'template settings
                _transactionType = CType(template.GetProperty("TransactionalType"), TransactionalTypes)
                _propertyAuthorization = CType(template.GetProperty("PropertyAuthorization"), PropertyAccessSecurity)
                _useSecurity = CType(template.GetProperty("AuthorizationRules"), Boolean)
                _codeGenMethod = CType(template.GetProperty("GenerationMethod"), CodeGenerationMethod)
                _classType = CType(template.GetProperty("ClassType"), GenerationClassType)
                _rootCommand = CType(template.GetProperty("RootCommand"), CommandSchema)

                'validate object
                Validate()
            End Sub

            Private Sub LoadProperties(ByVal table As TableSchema)
                _dbName = table.Database.Name
                _dbOwner = table.Owner
                _dbTableOrViewName = table.Name
                Dim col As ColumnSchema
                For Each col In table.Columns
                    Dim prop As PropertyInfo = New PropertyInfo(col, Me)

                    _properties.Add(prop)
                    If (prop.IsPrimaryKey) Then
                        _uniqueProperties.Add(prop)
                    End If
                    If (prop.IsFilterKey) Then
                        _filterProperties.Add(prop)
                    End If
                Next
            End Sub

            Private Sub LoadProperties(ByVal view As ViewSchema, ByVal uniqueColumns As StringCollection, ByVal filterColumns As StringCollection)
                _dbName = view.Database.Name
                _dbOwner = view.Owner
                _dbTableOrViewName = view.Name
                Dim col As ViewColumnSchema
                For Each col In view.Columns
                    'need case insensitive
                    Dim isUniqueMember As Boolean = uniqueColumns.ToString().ToLower().IndexOf(col.Name.ToLower()) >= 0
                    Dim isFilterMember As Boolean = filterColumns.ToString().ToLower().IndexOf(col.Name.ToLower()) >= 0

                    Dim prop As PropertyInfo = New PropertyInfo(col, Me, isUniqueMember, isFilterMember)

                    _properties.Add(prop)

                    If (prop.IsPrimaryKey) Then
                        _uniqueProperties.Add(prop)
                    End If
                    If (prop.IsFilterKey) Then
                        _filterProperties.Add(prop)
                    End If
                Next
            End Sub

            Private Sub LoadProperties(ByVal command As CommandSchema, ByVal resultSetIndex As Integer, ByVal uniqueColumns As StringCollection, ByVal filterColumns As StringCollection)
                _dbName = command.Database.Name
                _dbOwner = command.Owner
                _dbTableOrViewName = ""

                Dim col As CommandResultColumnSchema
                For Each col In command.CommandResults(resultSetIndex).Columns
                    Dim isUniqueMember As Boolean = False
                    Dim isFilterMember As Boolean = False
                    If (resultSetIndex = 0 AndAlso CslaObjectType <> ObjectType.NameValueList) Then
                        Dim isParameterMember As Boolean = command.InputParameters.Contains("@" + col.Name) Or command.InputParameters.Contains("@" + col.Name.Replace(" ", SpaceReplacement))
                        isUniqueMember = isParameterMember AndAlso Not IsCollection
                        isFilterMember = isParameterMember AndAlso IsCollection
                    Else
                        isUniqueMember = uniqueColumns.ToString().ToLower().IndexOf(col.Name.ToLower()) >= 0
                        isFilterMember = filterColumns.ToString().ToLower().IndexOf(col.Name.ToLower()) >= 0
                    End If

                    Dim prop As PropertyInfo = New PropertyInfo(col, Me, isUniqueMember, isFilterMember)

                    _properties.Add(prop)

                    If (prop.IsPrimaryKey) Then
                        _uniqueProperties.Add(prop)
                    End If
                    If (prop.IsFilterKey) Then
                        _filterProperties.Add(prop)
                    End If
                Next
            End Sub

            Private Sub Validate()
                If (_objectName Is Nothing OrElse _objectName.Length = 0) Then
                    Throw New Exception("ObjectName is required.")
                End If
                If (_uniqueProperties.Count = 0 AndAlso Not IsCollection) Then
                    Throw New Exception("Unique Column is required.")
                End If
                If (Not IsReadOnly AndAlso IsChild AndAlso IsCollection AndAlso (_parent Is Nothing OrElse _parent.Length = 0)) Then
                    Throw New Exception("Parent is required.")
                End If
                If (IsCollection AndAlso (_child Is Nothing OrElse _child.Length = 0) AndAlso CslaObjectType <> ObjectType.NameValueList) Then
                    Throw New Exception("Child is required.")
                End If
            End Sub
#End Region                     'Constructors
        End Class
#End Region

#Region " Property Class "
        Public Class PropertyInfo
            Private _parent As ObjectInfo
            Private _name As String
            Private _type As String
            Private _sqlType As String
            Private _access As String = "Public"
            Private _defaultValue As String = String.Empty
            Private _dbColumnName As String = String.Empty
            Private _isIdentity As Boolean = False
            Private _isPrimaryKey As Boolean = False
            Private _isFilterKey As Boolean = False
            Private _isRequired As Boolean = False
            Private _isCollection As Boolean = False
            Private _isCslaClass As Boolean = False
            Private _maxSize As Integer = -1

            Private _isReadOnly As Boolean = False
            Private _isComputed As Boolean = False
            Private _isTimestamp As Boolean = False
            Private _updateToDb As Boolean = True

            Public ReadOnly Property Name() As String
                Get
                    Return _name
                End Get
            End Property

            Public ReadOnly Property Type() As String
                Get
                    Return _type
                End Get
            End Property

            Public ReadOnly Property SqlType() As String
                Get
                    Return _sqlType
                End Get
            End Property

            Public ReadOnly Property Access() As String
                Get
                    Return _access
                End Get
            End Property

            'property/field-modifiers, used as part of property declaration
            Public ReadOnly Property Modifiers() As String
                Get
                    Dim mods As String = Access
                    If (_parent.IsGeneratedBase) Then
                        mods += " Overridable"
                    End If
                    Return mods
                End Get
            End Property

            Public ReadOnly Property MemberName() As String
                Get
                    Return MemberPrefix + VbHelper.GetCamelCaseName(Me.Name)
                End Get
            End Property
            Public ReadOnly Property MemberAccess() As String
                Get
                    Return _parent.MemberAccess
                End Get
            End Property
            Public ReadOnly Property DefaultValue() As String
                Get
                    Return _defaultValue
                End Get
            End Property
            Public ReadOnly Property Authorization() As PropertyAccessSecurity
                Get
                    Return _parent.PropertyAuthorization
                End Get
            End Property
            Public ReadOnly Property DbColumnName() As String
                Get
                    Return _dbColumnName
                End Get
            End Property
            Public ReadOnly Property IsReadOnly() As Boolean
                Get
                    'read only field is:
                    'parent is read only or object is read only
                    'identity column or part of primary key column(s)
                    'collection object or csla child object
                    If (_parent.IsReadOnly OrElse _isReadOnly OrElse IsIdentity OrElse IsPrimaryKey OrElse IsCollection OrElse IsCslaClass OrElse _isTimestamp OrElse _isComputed) Then
                        Return True
                    End If
                    Return False
                End Get
            End Property
            Public ReadOnly Property IsIdentity() As Boolean
                Get
                    Return _isIdentity
                End Get
            End Property
            Public ReadOnly Property IsPrimaryKey() As Boolean
                Get
                    Return _isPrimaryKey
                End Get
            End Property
            Public ReadOnly Property IsFilterKey() As Boolean
                Get
                    Return _isFilterKey
                End Get
            End Property
            Public ReadOnly Property IsDbComputed() As Boolean
                Get
                    If (IsIdentity OrElse _isTimestamp OrElse _isComputed) Then
                        Return True
                    End If
                    Return False
                End Get
            End Property
            Public ReadOnly Property HasDbColumn() As Boolean
                Get
                    Return _dbColumnName <> String.Empty
                End Get
            End Property
            Public ReadOnly Property UpdateToDb() As Boolean
                Get
                    Return _updateToDb AndAlso HasDbColumn
                End Get
            End Property
            Public ReadOnly Property IsRequired() As Boolean
                Get
                    Return _isRequired
                End Get
            End Property
            Public ReadOnly Property IsCollection() As Boolean
                Get
                    Return _isCollection
                End Get
            End Property

            Public ReadOnly Property IsCslaClass() As Boolean
                Get
                    Return _isCslaClass
                End Get
            End Property

            Public ReadOnly Property MaxSize() As Integer
                Get
                    Return _maxSize
                End Get
            End Property

            Public Sub New(ByVal column As DataObjectBase, ByVal parent As ObjectInfo)
                _parent = parent
                Load(column)
            End Sub

            Public Sub New(ByVal column As DataObjectBase, ByVal parent As ObjectInfo, ByVal isPrimaryKey As Boolean, ByVal isFilterKey As Boolean)
                _parent = parent
                Load(column)
                _isPrimaryKey = isPrimaryKey
                _isFilterKey = isFilterKey
            End Sub

            Public Sub New(ByVal cslaCollName As String, ByVal cslaCollType As String, ByVal parent As ObjectInfo)
                _parent = parent
                _name = VbHelper.MakeProper(cslaCollName)
                _type = cslaCollType
                _isCslaClass = True
                _isCollection = True
                If Not parent.IsReadOnly Then
                    _defaultValue = String.Format("{0}.New{0}()", _type)
                End If
            End Sub

            Public Sub New(ByVal xtr As XmlTextReader, ByVal parent As ObjectInfo)
                _parent = parent
                While xtr.MoveToNextAttribute()
                    Select Case xtr.LocalName.ToLower()
                        Case "name"
                            _name = VbHelper.MakeProper(xtr.Value)
                        Case "type"
                            _type = VbHelper.ConvertType(xtr.Value)
                        Case "access"
                            _access = xtr.Value
                        Case "default"
                            _defaultValue = xtr.Value
                        Case "dbcolumnname"
                            _dbColumnName = xtr.Value
                        Case "updatetodb"
                            _updateToDb = Boolean.Parse(xtr.Value)
                        Case "isidentity"
                            _isIdentity = Boolean.Parse(xtr.Value)
                        Case "isprimarykey"
                            _isPrimaryKey = Boolean.Parse(xtr.Value)
                        Case "isfilterkey"
                            _isFilterKey = Boolean.Parse(xtr.Value)
                        Case "isrequired"
                            _isRequired = Boolean.Parse(xtr.Value)
                        Case "isreadonly"
                            _isReadOnly = Boolean.Parse(xtr.Value)
                        Case "iscomputed"
                            _isComputed = Boolean.Parse(xtr.Value)
                        Case "maxsize"
                            _maxSize = Integer.Parse(xtr.Value)
                        Case "iscollection"
                            _isCollection = Boolean.Parse(xtr.Value)
                        Case "iscslaclass"
                            _isCslaClass = Boolean.Parse(xtr.Value)
                        Case Else
                    End Select
                End While
                If _name = String.Empty Then
                    Throw New Exception("Name is required in property")
                End If
                If _type = String.Empty Then
                    Throw New Exception("Type is required in property")
                End If
                If _defaultValue.Length = 0 Then
                    _defaultValue = VbHelper.GetDefaultValue(_type)
                End If
            End Sub

            Private Sub Load(ByVal col As DataObjectBase)
                _dbColumnName = col.Name
                _name = VbHelper.GetPropertyName(col)

                _type = VbHelper.GetVariableType(col)
                _sqlType = VbHelper.GetSqlType(col)
                _defaultValue = VbHelper.GetDefaultValue(col)
                _isIdentity = VbHelper.IsIdentity(col)
                If TypeOf (col) Is ColumnSchema Then
                    _isPrimaryKey = (CType(col, ColumnSchema)).IsPrimaryKeyMember
                    _isFilterKey = (CType(col, ColumnSchema)).IsForeignKeyMember
                End If
                _isRequired = Not col.AllowDBNull
                _isComputed = VbHelper.IsComputed(col)
                _isTimestamp = VbHelper.IsTimestamp(col)
                ' fixsize string is <= 8000
                If _type = "String" AndAlso col.Size <= 8000 AndAlso col.NativeType.ToLower <> "text" AndAlso col.NativeType.ToLower <> "ntext" Then
                    _maxSize = col.Size
                End If
            End Sub
        End Class
#End Region

#Region " Helpers "

#Region " Dal Helper "
        Public Class DalHelper
            Private Sub New()
            End Sub

            Public Shared Function ParameterReturnStatement(ByVal varName As String, ByVal varType As String, ByVal parName As String) As String
                Dim format As String = "{0} = CType(cm.Parameters(""@{2}"").Value,{1})"
                Return String.Format(format, varName, varType, parName.Replace(" ", SpaceReplacement))
            End Function

            Public Shared Function ParameterAssignmentStatement(ByVal parName As String, ByVal varName As String) As String
                Dim format As String = "cm.Parameters.AddWithValue(""@{0}"", {1})"
                Return String.Format(format, parName.Replace(" ", SpaceReplacement), varName)
            End Function

            Public Shared Function ReaderStatement(ByVal varName As String, ByVal varType As String, ByVal colName As String) As String
                Dim method As String = GetReaderMethod(varType)

                If (varType = "SmartDate") Then
                    Return String.Format("{0} = dr.GetSmartDate(""{1}"", {0}.EmptyIsMin)", varName, colName)
                End If

                If (method = String.Empty) Then
                    Return String.Format("{0} = CType(dr(""{2}""),{1})", varName, varType, colName)
                Else
                    Return String.Format("{0} = dr.{1}(""{2}"")", varName, method, colName)
                End If
            End Function

            Public Shared Function GetReaderMethod(ByVal varType As String) As String
                Dim val As String = String.Empty
                Select Case varType.ToLower
                    Case "smartdate", "datetime", "guid", "date"
                        Return "Get" + varType
                    Case "string", "double", "byte", "decimal", "single"
                        Return "Get" + varType.Substring(0, 1).ToUpper() + varType.Substring(1)
                    Case "boolean"
                        Return "GetBoolean"
                    Case "short"
                        Return "GetInt16"
                    Case "integer"
                        Return "GetInt32"
                    Case "long"
                        Return "GetInt64"
                End Select
                Return String.Empty
            End Function
        End Class
#End Region                     'Dal Helper

#Region " Column VbHelper "
        Public Class VbHelper
            Private Sub New()
            End Sub

            'convert C# variable types to VB so that the same 
            ' XML document can create either language type
            Public Shared Function ConvertType(ByVal varType As String) As String
                Select Case varType.ToLower
                    Case "bool"
                        Return "Boolean"
                    Case "int", "int32"
                        Return "Integer"
                    Case "int16"
                        Return "Short"
                    Case "int64"
                        Return "Long"
                    Case "uint16"
                        Return "UShort"
                    Case "uint", "uint32"
                        Return "UInteger"
                    Case "uint64"
                        Return "ULong"
                    Case "float"
                        Return "Single"
                    Case "datetime"
                        Return "Date"
                    Case Else
                        Return varType.Substring(0, 1).ToUpper() + varType.Substring(1)
                End Select
            End Function

            'remove underscore and convert to pascal case
            Public Shared Function MakeProper(ByVal name As String) As String
                Dim arrstr() As String = name.Split("_"c)
                Dim properName As String = String.Empty
                For i As Integer = 0 To arrstr.Length - 1
                    If arrstr(i) <> String.Empty Then
                        Dim str As String = arrstr(i).Replace("_", "")
                        properName += str.Substring(0, 1).ToUpper() + str.Substring(1).ToLower()
                    End If
                Next
                Return properName
            End Function

            Public Shared Function GetCamelCaseName(ByVal val As String) As String
                If val.Length <= 2 Then
                    Return val.ToLower()
                End If
                Return val.Substring(0, 1).ToLower() + val.Substring(1)
            End Function

            'logic was taken from http://www.devx.com/vb2themax/Tip/19612
            Public Function ConvertToSingular(ByVal plural As String) As String
                Dim lower As String = plural.ToLower()
                Dim result As String = String.Empty

                'rule out a few exceptions
                If (lower = "feet") Then
                    result = "Foot"
                ElseIf (lower = "geese") Then
                    result = "Goose"
                ElseIf (lower = "men") Then
                    result = "Man"
                ElseIf (lower = "women") Then
                    result = "Woman"
                ElseIf (lower = "criteria") Then
                    result = "Criterion"
                    'plural uses "ies" if word ends with "y" preceeded by a non-vowel
                ElseIf (lower.EndsWith("ies") AndAlso "aeiou".IndexOf(lower.Substring(lower.Length - 4, 1)) < 0) Then
                    result = plural.Substring(0, plural.Length - 3) + "y"
                ElseIf (lower.EndsWith("es") AndAlso "lt".IndexOf(lower.Substring(lower.Length - 3, 1)) < 0) Then
                    result = plural.Substring(0, plural.Length - 2)
                ElseIf (lower.EndsWith("s")) Then
                    result = plural.Substring(0, plural.Length - 1)
                Else
                    result = plural  'table name may not in plural form so, just return its name
                End If
                Return result
            End Function

            ' logic was taken from http://www.devx.com/vb2themax/Tip/19611
            Public Function ConvertToPlural(ByVal singular As String) As String
                Dim lower As String = singular.ToLower()
                Dim result As String = String.Empty

                'rule out a few exceptions
                If (lower = "foot") Then
                    result = "Feet"
                ElseIf (lower = "goose") Then
                    result = "Geese"
                ElseIf (lower = "man") Then
                    result = "Men"
                ElseIf (lower = "woman") Then
                    result = "Women"
                ElseIf (lower = "criterion") Then
                    result = "Criteria"
                    'plural uses "ies" if word ends with "y" preceeded by a non-vowel
                ElseIf (lower.EndsWith("y") AndAlso "aeiou".IndexOf(lower.Substring(lower.Length - 2, 1)) < 0) Then
                    result = singular.Substring(0, singular.Length - 1) + "eis"
                ElseIf (lower.EndsWith("o") AndAlso "aeiou".IndexOf(lower.Substring(lower.Length - 2, 1)) < 0) Then
                    result = singular + "es"
                Else
                    result = singular + "s"
                End If
                Return result
            End Function

            Public Shared Function GetPropertyName(ByVal col As DataObjectBase) As String
                Dim name As String = col.Name.Substring(0, 1).ToUpper() + col.Name.Substring(1)

                'convert ex:START_DATE to StartDate
                If (name.IndexOf("_") >= 0 OrElse name.IndexOf(" ") >= 0 OrElse name = name.ToUpper()) Then
                    name = VbHelper.MakeProper(name)
                End If

                'fix name that contain table prefix
                'ex table:Project column:ProjectName, ProjectDescription
                If TypeOf col Is ColumnSchema Then
                    name = name.Replace((CType(col, ColumnSchema)).Table.Name, "")
                End If

                If name.EndsWith("TypeCode") Then
                    name = name.Substring(0, name.Length - 4)
                End If

                Return name
            End Function

            Public Shared Function GetDefaultValue(ByVal variableType As String) As String
                Select Case variableType
                    Case "SmartDate"
                        Return "New SmartDate(True)"
                    Case "Guid"
                        Return "Guid.Empty"
                    Case "String"
                        Return "String.Empty"
                    Case "Boolean"
                        Return "False"
                    Case "Double"
                        Return "0"
                    Case "Byte"
                        Return "0"
                    Case "Decimal"
                        Return "0"
                    Case "Single"
                        Return "0"
                    Case "Short"
                        Return "0"
                    Case "Integer"
                        Return "0"
                    Case "Long"
                        Return "0"
                    Case Else
                        Return ""
                End Select
            End Function

            Public Shared Function GetDefaultValue(ByVal col As DataObjectBase) As String
                Select Case col.DataType
                    Case DbType.Guid
                        Return "Guid.Empty"
                    Case DbType.AnsiString
                        Return "String.Empty"
                    Case DbType.AnsiStringFixedLength
                        Return "String.Empty"
                    Case DbType.String
                        Return "String.Empty"
                    Case DbType.StringFixedLength
                        Return "String.Empty"
                    Case DbType.Boolean
                        Return "False"
                    Case DbType.Date, DbType.DateTime
                        If (col.AllowDBNull) Then
                            Dim colName As String = col.Name.ToLower()
                            If (colName.IndexOf("begin") >= 0) Then Return "New SmartDate(true)"
                            If (colName.IndexOf("active") >= 0) Then Return "New SmartDate(true)"
                            If (colName.IndexOf("start") >= 0) Then Return "New SmartDate(true)"
                            If (colName.IndexOf("from") >= 0) Then Return "New SmartDate(true)"
                            Return "New SmartDate(false)"
                        Else
                            If (col.Name.ToLower().IndexOf("timestamp") >= 0) Then
                                Return "New SmartDate(DateTime.Now)"
                            Else
                                Return "New SmartDate(DateTime.Today)"
                            End If
                        End If
                    Case DbType.VarNumeric, DbType.VarNumeric
                        Return "0"
                    Case DbType.Currency
                        Return "0"
                    Case DbType.Decimal
                        Return "0"
                    Case DbType.Double
                        Return "0"
                    Case DbType.Int16
                        Return "0"
                    Case DbType.Int32
                        Return "0"
                    Case DbType.Int64
                        Return "0"
                    Case DbType.Single
                        Return "0"
                    Case DbType.Byte
                        Return "0"
                    Case DbType.UInt16
                        Return "0"
                    Case DbType.UInt32
                        Return "0"
                    Case DbType.UInt64
                        Return "0"
                    Case Else
                        Return ""
                End Select
            End Function

            Public Shared Function GetVariableType(ByVal col As DataObjectBase) As String
                Select Case col.DataType
                    Case DbType.AnsiString, DbType.AnsiStringFixedLength, DbType.String, DbType.StringFixedLength
                        Return "String"
                    Case DbType.Binary
                        Return "Byte()"
                    Case DbType.Boolean
                        Return "Boolean"
                    Case DbType.Byte
                        Return "Byte"
                    Case DbType.Date, DbType.DateTime
                        Return "SmartDate"
                    Case DbType.VarNumeric, DbType.Decimal, DbType.Currency
                        Return "Decimal"
                    Case DbType.Double
                        Return "Double"
                    Case DbType.Guid
                        Return "Guid"
                    Case DbType.Int16
                        Return "Short"
                    Case DbType.Int32
                        Return "Integer"
                    Case DbType.Int64
                        Return "Long"
                    Case DbType.Object
                        Return "Object"
                    Case DbType.SByte
                        Return "SByte"
                    Case DbType.Single
                        Return "Single"
                    Case DbType.Time
                        Return "TimeSpan"
                    Case DbType.UInt16
                        Return "UInt16"
                    Case DbType.UInt32
                        Return "UInt32"
                    Case DbType.UInt64
                        Return "UInt64"
                    Case Else
                        Return "__UNKNOWN__" + col.NativeType
                End Select
            End Function

            Public Shared Function GetSqlType(ByVal col As DataObjectBase) As String
                Select Case col.NativeType
                    Case "bigint"
                        Return "BigInt"
                    Case "binary"
                        Return "Binary(" & col.Size.ToString & ")"
                    Case "bit"
                        Return "Bit"
                    Case "char"
                        Return "Char(" & col.Size.ToString & ")"
                    Case "datetime"
                        Return "DateTime"
                    Case "decimal"
                        Return "Decimal(" & col.Precision.ToString & "," & col.Scale.ToString & ")"
                    Case "float"
                        Return "Float"
                    Case "image"
                        Return "Image"
                    Case "int"
                        Return "Int"
                    Case "money"
                        Return "Money"
                    Case "nchar"
                        Return "NChar(" & col.Size.ToString & ")"
                    Case "ntext"
                        Return "NText"
                    Case "numeric"
                        Return "Numeric(" & col.Precision.ToString & "," & col.Scale.ToString & ")"
                    Case "nvarchar"
                        Return "NVarChar(" & col.Size.ToString & ")"
                    Case "real"
                        Return "Real"
                    Case "smalldatetime"
                        Return "SmallDateTime"
                    Case "smallint"
                        Return "SmallInt"
                    Case "smallmoney"
                        Return "SmallMoney"
                    Case "sql_variant"
                        Return "Variant"
                    Case "text"
                        Return "Text"
                    Case "timestamp"
                        Return "Timestamp"
                    Case "tinyint"
                        Return "TinyInt"
                    Case "uniqueidentifier"
                        Return "UniqueIdentifier"
                    Case "varbinary"
                        Return "VarBinary(" & col.Size.ToString & ")"
                    Case "varchar"
                        Return "VarChar(" & col.Size.ToString & ")"
                    Case Else
                        Return "__UNKNOWN__" & col.NativeType
                End Select
            End Function

            Public Shared Function IsIdentity(ByVal col As DataObjectBase) As Boolean
                If (col.ExtendedProperties("CS_IsIdentity") Is Nothing) Then Return False
                Return CType(col.ExtendedProperties("CS_IsIdentity").Value, Boolean)
            End Function

            Public Shared Function IsComputed(ByVal col As DataObjectBase) As Boolean
                If (col.ExtendedProperties("CS_IsComputed") Is Nothing) Then Return False
                Return CType(col.ExtendedProperties("CS_IsComputed").Value, Boolean)
            End Function

            Public Shared Function IsTimestamp(ByVal col As DataObjectBase) As Boolean
                Dim var As Boolean = False
                If (col.NativeType.ToLower() = "timestamp") Then
                    var = True
                ElseIf (col.Name.ToLower().IndexOf("timestamp") >= 0) Then
                    var = True
                End If
                Return var
            End Function
        End Class
#End Region

#Region " Template VbHelper "
        Public Class TemplateHelper
            Private Sub New()
            End Sub

            Public Shared Function IsObjectType(ByVal info As ICodeTemplateInfo) As Boolean
                Select Case info.FileName.ToLower()
                    Case "editableroot.cst", "editablerootlist.cst", "editablechild.cst", "editablechildlist.cst", "editableswitchable.cst", "namevaluelist.cst", "readonlyroot.cst", "readonlyrootlist.cst", "readonlychild.cst", "readonlychildlist.cst", "storeprocedures.cst"
                        Return True
                    Case Else
                        Return False
                End Select
            End Function

            Public Shared Function ToObjectType(ByVal templete As CodeTemplate) As ObjectType
                Select Case templete.CodeTemplateInfo.FileName.ToLower()
                    Case "editableroot.cst"
                        Return ObjectType.EditableRoot
                    Case "editablerootlist.cst"
                        Return ObjectType.EditableRootList
                    Case "editablechild.cst"
                        Return ObjectType.EditableChild
                    Case "editablechildlist.cst"
                        Return ObjectType.EditableChildList
                    Case "editableswitchable.cst"
                        Return ObjectType.EditableSwitchable
                    Case "namevaluelist.cst"
                        Return ObjectType.NameValueList
                    Case "readonlyroot.cst"
                        Return ObjectType.ReadOnlyRoot
                    Case "readonlyrootlist.cst"
                        Return ObjectType.ReadOnlyRootList
                    Case "readonlychild.cst"
                        Return ObjectType.ReadOnlyChild
                    Case "readonlychildlist.cst"
                        Return ObjectType.ReadOnlyChildList
                    Case "storeprocedures.cst"
                        Return CType(templete.GetProperty("ObjectType"), ObjectType)
                End Select
                Throw New ArgumentOutOfRangeException("Template is not an Object Type template")
            End Function

            Public Shared Function GetCompiledTemplate(ByVal templatePath As String) As CodeTemplate
                Dim compiler As CodeTemplateCompiler = New CodeTemplateCompiler(templatePath)
                compiler.Compile()

                If (compiler.Errors.Count > 0) Then
                    Dim errString As String = "Error Compiling Template" & Environment.NewLine
                    errString += "- " + templatePath + Environment.NewLine

                    Dim err As System.CodeDom.Compiler.CompilerError
                    For Each err In compiler.Errors
                        errString += err.ErrorText + Environment.NewLine
                    Next
                    Throw New ApplicationException(errString)
                End If

                Return compiler.CreateInstance()

            End Function
        End Class
#End Region
#End Region

#Region " Enumerations "

        Public Enum SPType
            SPSelect
            SPUpdate
            SPInsert
            SPDelete
            SPNameValueList
            SPList
            SPExists
        End Enum

        Public Enum ObjectType
            EditableRoot
            EditableRootList
            EditableChild
            EditableChildList
            EditableSwitchable
            NameValueList
            ReadOnlyRoot
            ReadOnlyRootList
            ReadOnlyChild
            ReadOnlyChildList
        End Enum

        Public Enum CodeGenerationMethod
            [Single]
            SplitPartial
            SplitBase
        End Enum

        Public Enum GenerationClassType
            Generated
            User
        End Enum

        Public Enum PropertyAccessSecurity
            None
            Both
            Read
            Write
        End Enum

        Public Enum TransactionalTypes
            None
            Ado
            EnterpriseService
            TransactionScope
        End Enum

#End Region

    End Class
End Namespace