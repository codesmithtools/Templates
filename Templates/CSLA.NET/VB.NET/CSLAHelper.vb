Option Strict On
Option Explicit On 
Option Compare Text

Imports CodeSmith.Engine
Imports Microsoft.VisualBasic
Imports SchemaExplorer
Imports System
Imports System.ComponentModel
Imports System.Data
Imports System.Diagnostics
Imports System.IO.Directory

Public Class CSLAHelper
    Inherits CodeTemplate

   'This variable is used to control the indent of the 
   'formatted code that is output in conjunction with
   'the Level method
   Const INDENT_LEVEL_SPACES As Integer = 3

#Region "Common Generation Member Variables"
   'Properties common to generate All Templates  
   Private mClassNamespace As System.String = ""
   Private mFLCPrefix As System.String = ""
   Private mObjectName As System.String = ""
   Private mObjectTemplate As TemplateEnum = 0 'EditableRoot
   Private mOutPutDirectory As String = "C:\CSLA\Generated\"
   Private mDeveloperCodeDirectory As String = ""
   Private mRootTable As SchemaExplorer.TableSchema
   Private mSetAsBaseClass As System.Boolean = False
   Private mBaseClassExt As String = ""

#End Region 'Common Generation Member Variables

#Region "Common Generation Properties"

   <CodeTemplateProperty(CodeTemplatePropertyOption.Optional), Category("1. Base Class Options"), Description("Optional - The directory to which every output file is generated.")> Public Property OutPutBaseDirectory() As System.String
      Get

         If Not mOutPutDirectory.EndsWith("\") Then
            Return mOutPutDirectory & "\"
         Else
            Return mOutPutDirectory
         End If

      End Get
      Set(ByVal Value As System.String)
         mOutPutDirectory = Value
      End Set
   End Property

   <CodeTemplateProperty(CodeTemplatePropertyOption.Required), Category("2. Common Template Options"), Description("Required - Generate the object code from this template.")> Public Property ObjectTemplate() As TemplateEnum
      Get
         Return mObjectTemplate
      End Get
      Set(ByVal Value As TemplateEnum)
         mObjectTemplate = Value
      End Set
   End Property

   <CodeTemplateProperty(CodeTemplatePropertyOption.Required), Category("1. Base Class Options"), Description("Required - Set this as the base object class.")> Public Property SetAsBaseClass() As System.Boolean
      Get
         Return mSetAsBaseClass
      End Get
      Set(ByVal Value As System.Boolean)
         mSetAsBaseClass = Value
      End Set
   End Property

   <CodeTemplateProperty(CodeTemplatePropertyOption.Optional), Category("2. Common Template Options"), Description("Optional - Generate the variables and properties for field level concurrency tracking. These will be generated into the custom programming class of the object using this Prefix.")> Public Property FLCPrefix() As System.String
      Get
         Return mFLCPrefix
      End Get
      Set(ByVal Value As System.String)
         mFLCPrefix = Value
      End Set
   End Property

   <CodeTemplateProperty(CodeTemplatePropertyOption.Required), Category("2. Common Template Options"), Description("Required - The Name of the Business Object to Generate. If generating a collection class, then this is the Object Type contained within the collection.")> Public Property ObjectName() As String
      Get
         Return mObjectName
      End Get
      Set(ByVal Value As String)
         mObjectName = Value
      End Set
   End Property

   <CodeTemplateProperty(CodeTemplatePropertyOption.Required), Category("2. Common Template Options"), Description("Required - The Root Table that the object is based on.")> Public Property RootTable() As SchemaExplorer.TableSchema
      Get
         Return mRootTable
      End Get
      Set(ByVal Value As SchemaExplorer.TableSchema)
         mRootTable = Value
      End Set
   End Property

   <CodeTemplateProperty(CodeTemplatePropertyOption.Optional), Category("1. Base Class Options"), Description("Optional - The Extension that the class will use when generating the 'Base' Class. Defaults to 'Base'.")> Public Property BaseClassExt() As String
      Get
         Return mBaseClassExt.Trim.ToString
      End Get
      Set(ByVal Value As String)
         mBaseClassExt = Value
      End Set
   End Property

   <CodeTemplateProperty(CodeTemplatePropertyOption.Optional), Category("1. Base Class Options"), Description("Optional - This is the directory that the custom classes will be generated into. Defaults to the same directory as the base classes. ie.:""")> Public Property OutPutDeveloperCodeDirectory() As String
      Get
         If Not mDeveloperCodeDirectory.Trim.EndsWith("\") Then
            Return mDeveloperCodeDirectory & "\"
         Else
            Return mDeveloperCodeDirectory
         End If
      End Get
      Set(ByVal Value As String)
         mDeveloperCodeDirectory = Value
      End Set
   End Property

#End Region 'Common Generation Properties

#Region "Common Options Member Variables for the Business Object"
   'Common Options for the Business Object
   Private mMemberPrefix As System.String = "m"
   Private mAccessibility As AccessibilityEnum = CType(0, AccessibilityEnum) 'Public
   Private mSerializable As System.Boolean = True
   Private mMustBeInherited As System.Boolean = False
   Private mTransactionType As TransactionEnum = CType(1, TransactionEnum) 'ADO Transaction
#End Region 'Common Options Member Variables for the Business Object

#Region "Common Options Properties for the Business Object"

   <CodeTemplateProperty(CodeTemplatePropertyOption.Optional), Category("3. Class Level Options"), Description("Optional - The namespace that the generated Classes will be a member of.")> Public Property ClassNamespace() As String
      Get
         Return mClassNamespace
      End Get
      Set(ByVal Value As String)
         mClassNamespace = Value
      End Set
   End Property

   <CodeTemplateProperty(CodeTemplatePropertyOption.Optional), Category("3. Class Level Options"), Description("Optional - The prefix for the member variables.")> Public Property MemberPrefix() As String
      Get
         Return mMemberPrefix
      End Get
      Set(ByVal Value As String)
         mMemberPrefix = Value
      End Set
   End Property

   <CodeTemplateProperty(CodeTemplatePropertyOption.Required), Category("3. Class Level Options"), Description("Required - The accessibility of the Class to be generated.")> Public Property Accessibility() As AccessibilityEnum
      Get
         Return mAccessibility
      End Get
      Set(ByVal Value As AccessibilityEnum)
         mAccessibility = Value
      End Set
   End Property

   <CodeTemplateProperty(CodeTemplatePropertyOption.Required), Category("3. Class Level Options"), Description("Required - Should this Class have a Serialized Attiribute?")> Public Property Serializable() As Boolean
      Get
         Return mSerializable
      End Get
      Set(ByVal Value As Boolean)
         mSerializable = Value
      End Set
   End Property

   <CodeTemplateProperty(CodeTemplatePropertyOption.Required), Category("3. Class Level Options"), Description("Required - Should this Class have a Must Inherit Directive? If this is a Base class then it will always declare the MustInherit")> Public Property MustBeInherited() As Boolean
      Get
         Return mMustBeInherited
      End Get
      Set(ByVal Value As Boolean)
         mMustBeInherited = Value
      End Set
   End Property

   <CodeTemplateProperty(CodeTemplatePropertyOption.Required), Category("3. Class Level Options"), Description("Required - What type of Transaction should this Business Object use?")> Public Property TransactionType() As TransactionEnum
      Get
         Return mTransactionType
      End Get
      Set(ByVal Value As TransactionEnum)
         mTransactionType = Value
      End Set
   End Property

#End Region 'Common Options Properties for the Business Object

#Region "Common Collection Class Member Variables"

   'Collection Object Properties
   Private mCollectionName As System.String = ""
   Private mCollectionTemplate As CollectionTemplateEnum = CType(0, CollectionTemplateEnum) 'None
   Private mChildCollectionName As System.String = ""
   Private mChildMemberName As System.String = ""
   Private mParentName As System.String = ""
   Private mParentType As System.String = ""
   Private mAllowSort As Boolean = True
   Private mAllowFind As Boolean = True
   Private mIsCollectionCode As Boolean = False
#End Region 'Common Collection Class Member Variables

#Region "Common Collection Class Properties"
   <CodeTemplateProperty(CodeTemplatePropertyOption.Optional), Category("4. Child Collection Business Object"), Description("The Child Collection's Name. Used for the memeber variable and collection name.")> Public Property ChildCollectionName() As System.String
      Get
         Return mChildCollectionName
      End Get
      Set(ByVal Value As System.String)
         mChildCollectionName = Value
      End Set
   End Property

   <CodeTemplateProperty(CodeTemplatePropertyOption.Optional), Category("4. Child Collection Business Object"), Description("The Child Collection's member variable name.")> Public Property ChildMemberName() As System.String
      Get
         Return mChildMemberName
      End Get
      Set(ByVal Value As System.String)
         mChildMemberName = Value
      End Set
   End Property

   <CodeTemplateProperty(CodeTemplatePropertyOption.Optional), Category("4. Child Collection Business Object"), Description("The Parent Name to which the generated Business Object belongs.")> Public Property ParentName() As System.String
      Get
         Return mParentName
      End Get
      Set(ByVal Value As System.String)
         mParentName = Value
      End Set
   End Property

   <CodeTemplateProperty(CodeTemplatePropertyOption.Optional), Category("4. Child Collection Business Object"), Description("The Parent's Object Type (Class). This is the Object Type that has the contained Child Class.")> Public Property ParentType() As System.String
      Get
         Return mParentType
      End Get
      Set(ByVal Value As System.String)
         mParentType = Value
      End Set
   End Property

   <CodeTemplateProperty(CodeTemplatePropertyOption.Optional), Category("4. Collection Business Object"), Description("The Name of the Business Object Collection to Generate.")> Public Property CollectionName() As System.String
      Get
         Return mCollectionName
      End Get
      Set(ByVal Value As System.String)
         mCollectionName = Value
      End Set
   End Property

   <CodeTemplateProperty(CodeTemplatePropertyOption.Required), Category("4. Collection Business Object"), Description("Generate the Associated Collection Class using this template. If it is left set to 'None', no collection object will be generated.")> Public Property CollectionTemplate() As CollectionTemplateEnum
      Get
         Return mCollectionTemplate
      End Get
      Set(ByVal Value As CollectionTemplateEnum)
         mCollectionTemplate = Value
      End Set
   End Property

   <CodeTemplateProperty(CodeTemplatePropertyOption.Optional), Category("4. Collection Business Object"), Description("Insert the code for the AllowSorting = True in constructor. In the Custom Code collecion class.")> Public Property AllowSort() As Boolean
      Get
         Return mAllowSort
      End Get
      Set(ByVal Value As Boolean)
         mAllowSort = Value
      End Set
   End Property

   <CodeTemplateProperty(CodeTemplatePropertyOption.Optional), Category("4. Collection Business Object"), Description("Insert the code for the AllowFind = True in constructor. In the Custom Code collecion class.")> Public Property AllowFind() As Boolean
      Get
         Return mAllowFind
      End Get
      Set(ByVal Value As Boolean)
         mAllowFind = Value
      End Set
   End Property
#End Region 'Common Collection Class Properties

#Region "Name Value List Member Variables and Properties"

   Private mKeyColumn As String
   Private mValueColumn As String

   <CodeTemplateProperty(CodeTemplatePropertyOption.Optional), Category("5. Name Value List"), Description("Optional - The column name used for the Key in the Name Value List.")> Public Property KeyColumn() As String
      Get
         Return mKeyColumn
      End Get
      Set(ByVal Value As String)
         mKeyColumn = Value
      End Set
   End Property

   <CodeTemplateProperty(CodeTemplatePropertyOption.Optional), Category("5. Name Value List"), Description("Optional - The column name used for the Value in the Name Value List.")> Public Property ValueColumn() As String
      Get
         Return mValueColumn
      End Get
      Set(ByVal Value As String)
         mValueColumn = Value
      End Set
   End Property

#End Region 'Name Value List Member Variables and Properties

#Region "Common Stored Procedure Member Variables"
   'Common Stored Procedure Options
   Private mGeneralSPPrefix As System.String = "csla_"
   Private mInsertPrefix As System.String = "Add"
   Private mUpdatePrefix As System.String = "Update"
   Private mDeletePrefix As System.String = "Delete"
   Private mSelectPrefix As System.String = "Get"
#End Region 'Common Stored Procedure Member Variables

#Region "Common Stored Procedure Options"
   <CodeTemplateProperty(CodeTemplatePropertyOption.Optional), Category("6. Stored Procedure Options"), Description("Optional - The GLOBAL prefix that identifies our own stored procedures.")> Public Property GeneralSPPrefix() As System.String
      Get
         Return mGeneralSPPrefix
      End Get
      Set(ByVal Value As System.String)
         mGeneralSPPrefix = Value
      End Set
   End Property

   <CodeTemplateProperty(CodeTemplatePropertyOption.Optional), Category("6. Stored Procedure Options"), Description("Optional - Prefix to use for all generated INSERT stored procedures.")> Public Property InsertPrefix() As System.String
      Get
         Return mInsertPrefix
      End Get
      Set(ByVal Value As System.String)
         mInsertPrefix = Value
      End Set
   End Property

   <CodeTemplateProperty(CodeTemplatePropertyOption.Optional), Category("6. Stored Procedure Options"), Description("Optional - Prefix to use for all generated UPDATE stored procedures.")> Public Property UpdatePrefix() As System.String
      Get
         Return mUpdatePrefix
      End Get
      Set(ByVal Value As System.String)
         mUpdatePrefix = Value
      End Set
   End Property

   <CodeTemplateProperty(CodeTemplatePropertyOption.Optional), Category("6. Stored Procedure Options"), Description("Optional - Prefix to use for all generated DELETE stored procedures.")> Public Property DeletePrefix() As System.String
      Get
         Return mDeletePrefix
      End Get
      Set(ByVal Value As System.String)
         mDeletePrefix = Value
      End Set
   End Property

   <CodeTemplateProperty(CodeTemplatePropertyOption.Optional), Category("6. Stored Procedure Options"), Description("Optional - Prefix to use for all generated SELECT stored procedures.")> Public Property SelectPrefix() As System.String
      Get
         Return mSelectPrefix
      End Get
      Set(ByVal Value As System.String)
         mSelectPrefix = Value
      End Set
   End Property

#End Region 'Common Stored Procedure Options

#Region "Generic Read Only Hidden Properties/Methods for code generation"

   <Browsable(False)> Public ReadOnly Property IsSwitchable() As Boolean
      'Indicates whether or not this is a swithable object
      Get
         Return ObjectTemplate.ToString = "EditableSwitchable"
      End Get
   End Property

   <Browsable(False)> Public ReadOnly Property IsBaseClass() As Boolean
      'Indicates if this is a Base Class. Will return True if either contion is met:
      '  1. The object name given ends in the indicated base class extension
      '  2. Or Set as Base Class is set to true
      Get

         If SetAsBaseClass Then
            Return True
         Else
            Return ObjectName.EndsWith(BaseClassExt)
         End If

      End Get
   End Property

   <Browsable(False)> Public ReadOnly Property IsBaseCollection() As Boolean
      'Indicates if this is a Base Collection Class. Will return True if either contion is met:
      '  1. The collection name given ends in "Base"
      '  2. Or Set as Base Class is set to true
      Get

         If SetAsBaseClass Then
            Return True

         Else
            Return CollectionName.EndsWith(BaseClassExt)
         End If

      End Get
   End Property

   <Browsable(False)> Public ReadOnly Property GetCustomClassName() As String
      'The Custom Class Name for the Custom collection class
      'Basically, drop the "Base" from the base object class name
      Get

         If IsBaseClass() And ObjectName.EndsWith(BaseClassExt) Then
            Return ObjectName.Substring(0, ObjectName.Length - BaseClassExt.Length)

         Else
            Return ObjectName
         End If

      End Get
   End Property

   <Browsable(False)> Public ReadOnly Property GetCustomCollectionName() As String
      'The Custom Class Name for the Custom collection class
      'Basically, drop the "Base" from the base object class name
      Get

         If IsBaseCollection() And CollectionName.EndsWith(BaseClassExt) Then
            Return CollectionName.Substring(0, CollectionName.Length - BaseClassExt.Length)

         Else
            Return CollectionName
         End If

      End Get
   End Property

   <Browsable(False)> Public ReadOnly Property GetBaseClassName() As String
      Get
         If IsBaseClass() And ObjectName.EndsWith(BaseClassExt) Then
            Return ObjectName
         ElseIf IsBaseClass Then
            Return ObjectName & BaseClassExt
         Else
            Return ObjectName
         End If
      End Get
   End Property

   <Browsable(False)> Public ReadOnly Property GetBaseCollectionName() As String
      Get
         If IsBaseCollection() And CollectionName.EndsWith(BaseClassExt) Then
            Return CollectionName
         ElseIf IsBaseClass Then
            Return CollectionName & BaseClassExt
         Else
            Return CollectionName
         End If
      End Get
   End Property

   <Browsable(False)> Public Property IsCollectionCode() As Boolean
      Get
         Return mIsCollectionCode
      End Get
      Set(ByVal Value As Boolean)
         mIsCollectionCode = Value
      End Set
   End Property

   <Browsable(False)> Public ReadOnly Property GenerateCollection() As Boolean
      Get

         If Len(CollectionName) > 0 And Len(CollectionTemplate.ToString) > 0 And Not CollectionTemplate = 0 Then
            Return True

         Else
            Return False
         End If

      End Get
   End Property

   Public Sub DirExists(ByVal CheckDirectory As String)
      Dim Dir As System.IO.Directory
      If Not Dir.Exists(CheckDirectory) Then
         Dir.CreateDirectory(CheckDirectory)
      End If
   End Sub

#End Region 'Generic Properties for code generation

   Public Function ConstructorDeclaration(Optional ByVal IsBasePropertyOrMethod As Boolean = False, Optional ByVal IsStruct As Boolean = False) As String

      'Returns the correct declaration of the variables, properties and methods even if
      'it is in a base class
      If IsStruct Then Return "Friend"
      If IsBaseClass And IsBasePropertyOrMethod Then
         Return "Public Overridable"
      ElseIf MustBeInherited Or IsBaseClass Then
         Return "Protected"
      Else
         Return "Private"
      End If
   End Function

   Public Function Level(ByVal n As Integer, _
                         Optional ByVal NewLine As Boolean = False) As String
      'Provides formatting for the outputted code
      'The INDENT_LEVEL_SPACES defines how many spaces 
      '  are in a level and is declared at the top of the class
      Dim sLevel As String

      If Not NewLine Then
         sLevel = Space((n * INDENT_LEVEL_SPACES))

      Else
         sLevel = vbNewLine + Space((n * INDENT_LEVEL_SPACES))
      End If

      Return sLevel
   End Function

   Public Function GetClassName(ByVal table As TableSchema) As String

      'Get the Class Name for the singular object class
      'Basically, drop the "Base" from the base object class name
      If (table.Name.EndsWith("es")) Then
         Return table.Name.Substring(0, table.Name.Length - 2)

      ElseIf (table.Name.EndsWith("s")) Then
         Return table.Name.Substring(0, table.Name.Length - 1)

      ElseIf (table.Name.EndsWith("ies")) Then
         Return table.Name.Substring(0, table.Name.Length - 3) & "y"

      Else
         Return table.Name

      End If

   End Function

   Public Function GetPropertyDeclarations(ByVal Column As ColumnSchema, _
                                           Optional ByVal bReadOnly As Boolean = False, _
                                           Optional ByVal addLevel As Integer = 0, _
                                           Optional ByVal sFLCPrefix As String = "", _
                                           Optional ByVal bIsStruct As Boolean = False) As String
      'Generate the "Business Properties and Methods" Region of the code
      Dim PropertyStatement As String
      Dim IsDateType As Boolean = (GetVBVariableType(Column) = "SmartDate")
      Dim txt As String
      Dim InBaseClass As Boolean = Not (ObjectTemplate.ToString = "CustomClassCode")

      'Set the text variable for the Smartdate.
      If IsDateType Then txt = ".Text" Else txt = ""

      'Override the default bReadOnly As specified by programmer - No Set for PKs or TimeStamps
      If (bReadOnly = True OrElse Column.NativeType = "timestamp" OrElse Column.IsPrimaryKeyMember) And Not bIsStruct Then

         PropertyStatement += Level(addLevel + 1, True)

         PropertyStatement += ConstructorDeclaration(InBaseClass, bIsStruct) & " ReadOnly Property " & sFLCPrefix + GetPropertyName(Column) + "() As " + GetPropertyVariableType(Column)

         PropertyStatement += Level(addLevel + 2, True)

         PropertyStatement += "Get"

         PropertyStatement += Level(addLevel + 3, True)

         PropertyStatement += "Return " + GetMemberVariableName(Column, sFLCPrefix) & txt

         PropertyStatement += Level(addLevel + 2, True)

         PropertyStatement += "End Get"

         PropertyStatement += Level(addLevel + 1, True)

         PropertyStatement += "End Property"

         PropertyStatement += Level(addLevel + 1, True)

      Else

         PropertyStatement += Level(addLevel + 1, True)

         PropertyStatement += ConstructorDeclaration(InBaseClass, bIsStruct) & " Property " & sFLCPrefix & GetPropertyName(Column) & "() As " & GetPropertyVariableType(Column)

         'The Get Statement Definition
         PropertyStatement += Level(addLevel + 2, True)

         PropertyStatement += "Get"

         PropertyStatement += Level(addLevel + 3, True)

         PropertyStatement += "Return " & GetMemberVariableName(Column, sFLCPrefix) & txt

         PropertyStatement += Level(addLevel + 2, True)

         PropertyStatement += "End Get"

         PropertyStatement += Level(addLevel + 1, True)

         'The Set Statement Definition
         PropertyStatement += Level(addLevel + 2, True)

         PropertyStatement += "Set(ByVal Value As " & GetPropertyVariableType(Column) & ")"

         PropertyStatement += Level(addLevel + 3, True)

         If Not Column.DataType = DbType.Guid Then
            'Handle the Equals method for Object Types
            PropertyStatement += "If " + GetMemberVariableName(Column, sFLCPrefix) + txt + " <> Value Then"
         Else
            PropertyStatement += "If Not " + GetMemberVariableName(Column, sFLCPrefix) + txt + ".Equals(Value) Then"
         End If

         PropertyStatement += Level(addLevel + 4, True)

         PropertyStatement += GetMemberVariableName(Column, sFLCPrefix) + txt + " = Value"

         If Not bIsStruct Then
            'The assert and the markdirty are not valid for stuctures
            PropertyStatement += Level(addLevel + 4, True)

            PropertyStatement += "'Insert the Rule for the column here .........."

            PropertyStatement += Level(addLevel + 4, True)

            PropertyStatement += "BrokenRules.Assert(" + Chr(34) + Chr(34) + "," + Chr(34) + "Rule for " + sFLCPrefix + GetPropertyName(Column) + " has been Broken." + Chr(34) + "," + Chr(34) + GetPropertyName(Column) + Chr(34) + ",False)"

            PropertyStatement += Level(addLevel + 4, True)

            PropertyStatement += "MarkDirty()"
         End If

         PropertyStatement += Level(addLevel + 3, True)

         PropertyStatement += "End If"

         PropertyStatement += Level(addLevel + 2, True)

         PropertyStatement += "End Set"

         PropertyStatement += Level(addLevel + 1, True)

         PropertyStatement += "End Property"

         PropertyStatement += Level(addLevel + 1, True)
      End If

      Return PropertyStatement
   End Function

   Public Function GetMemberVariableDeclarationStatement(ByVal column As ColumnSchema, _
                                                         Optional ByVal sFLCPrefix As String = "") As String
      '
      Return GetMemberVariableDeclarationStatement(ConstructorDeclaration, column, sFLCPrefix)
   End Function

   Public Function GetMemberVariableDeclarationStatement(ByVal protectionLevel As String, _
                                                         ByVal column As ColumnSchema, _
                                                         Optional ByVal sFLCPrefix As System.String = "", _
                                                         Optional ByVal IsForStructure As Boolean = False) As String
      ' 
      Dim Statement As String
      Statement += protectionLevel + " " + GetMemberVariableName(column, sFLCPrefix) + " As "

      'Check if this is a SmartDate Type
      If Not IsForStructure AndAlso (column.DataType = DbType.Date OrElse column.DataType = DbType.DateTime) Then
         Statement += "New "
      End If

      Statement += GetVBVariableType(column)

      If Not IsForStructure Then
         Statement += " "
         Dim DefaultValue As String = GetMemberVariableDefaultValue(column)

         If (DefaultValue <> "" And column.DataType <> DbType.Date And column.DataType <> DbType.DateTime) Then
            Statement += " = " + DefaultValue
         End If

      End If

      Return Statement
   End Function

   Public Function GetReaderAssignmentStatement(ByVal column As ColumnSchema, _
                                                ByVal index As Int16, _
                                                Optional ByVal StructureName As String = "") As String
      Dim Statement As String

      If Not Len(StructureName) > 0 Then

         If Not column.NativeType = "timestamp" Then
            Statement += GetMemberVariableName(column) + " = "
            Statement += "." + GetReaderMethod(column) + "(" + index.ToString() + ")"
         Else
            Statement += "." + GetReaderMethod(column) + "(" + index.ToString() + ", 0, " + GetMemberVariableName(column) + ", 0, 7)"
         End If

      Else

         If Not column.NativeType = "timestamp" Then
            Statement += StructureName + "." + GetPropertyName(column) + " = "
            Statement += "." + GetReaderMethod(column) + "(" + index.ToString() + ")"
            If GetReaderMethod(column) = "GetSmartDate" Then Statement += ".Text"
         Else
            Statement += "." + GetReaderMethod(column) + "(" + index.ToString() + ", 0, " + GetPropertyName(column) + ", 0, 7)"
         End If

      End If

      Return Statement
   End Function

   Public Function GetCamelCaseName(ByVal value As String) As String
      Return value.Substring(0, 1).ToUpper + value.Substring(1)
   End Function

   Public Function GetMemberVariableName(ByVal column As ColumnSchema, _
                                         Optional ByVal sFLCPrefix As System.String = "") As String
      Dim propertyName As String = GetPropertyName(column)
      Dim memberVariableName As String = MemberPrefix & sFLCPrefix & GetCamelCaseName(propertyName)
      Return memberVariableName
   End Function

   Public Function GetPropertyName(ByVal column As ColumnSchema) As String
      Dim propertyName As String = column.Name

      If (propertyName = column.Table.Name + "Name") Then Return "Name"

      If (propertyName = column.Table.Name + "Description") Then Return "Description"

      If (propertyName.EndsWith("TypeCode")) Then

         propertyName = propertyName.Substring(0, propertyName.Length - 4)
      End If

      Return propertyName
   End Function

   Public Function GetMemberVariableDefaultValue(ByVal column As ColumnSchema) As String

      Select Case column.DataType

         Case DbType.String, DbType.StringFixedLength, DbType.AnsiString, DbType.AnsiStringFixedLength
            Return Chr(34) + Chr(34)

         Case DbType.Boolean
            Return "False"

         Case DbType.Guid
            Return "Guid.NewGuid"

         Case DbType.Binary
            Return ""

         Case Else
            Return "0"
      End Select

   End Function

   Public Function GetSProcParmExt(ByVal column As ColumnSchema) As String

      ' Get the extension to use for Stored procedure
      ' parameter values
      ' Returns ".ToString", ".DBValue"  or ""
      '
      Select Case GetVBVariableType(column)

         Case "String", "Guid"
            Return ".ToString"

         Case "SmartDate"
            Return ".DBValue"

         Case Else
            Return ""
      End Select

   End Function

   Public Function GetVBVariableType(ByVal column As ColumnSchema) As String

      If column.Name.EndsWith("TypeCode") Then Return column.Name

      Select Case column.DataType

         Case DbType.AnsiString
            Return "String"

         Case DbType.AnsiStringFixedLength
            Return "String"

         Case DbType.Binary
            Return "Byte()"

         Case DbType.Boolean
            Return "Boolean"

         Case DbType.Byte
            Return "Byte"

         Case DbType.Currency
            Return "Decimal"

         Case DbType.Date
            Return "SmartDate"

         Case DbType.DateTime
            Return "SmartDate"

         Case DbType.Decimal
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
            Return "Float"

         Case DbType.String
            Return "String"

         Case DbType.StringFixedLength
            Return "String"

         Case DbType.Time
            Return "TimeSpan"

         Case DbType.UInt16
            Return "UShort"

         Case DbType.UInt32
            Return "UInt"

         Case DbType.UInt64
            Return "ULong"

         Case DbType.VarNumeric
            Return "Decimal"

         Case Else
            Return "__UNKNOWN__" + column.NativeType
      End Select

   End Function

   Public Function GetReaderMethod(ByVal column As ColumnSchema) As String

      Select Case (column.DataType)

         Case DbType.Byte
            Return "GetByte"

         Case DbType.Int16
            Return "GetInt16"

         Case DbType.Int32
            Return "GetInt32"

         Case DbType.Int64
            Return "GetInt64"

         Case DbType.AnsiStringFixedLength, DbType.AnsiString, DbType.String, DbType.StringFixedLength
            Return "GetString"

         Case DbType.Boolean
            Return "GetBoolean"

         Case DbType.Guid
            Return "GetGuid"

         Case DbType.Currency, DbType.Decimal
            Return "GetDecimal"

         Case DbType.DateTime
            Return "GetSmartDate"

         Case DbType.Date
            Return "GetSmartDate"

         Case DbType.Binary
            Return "GetBytes"

         Case Else
            Return "GetObject" '"__READER_GET__" + column.DataType
      End Select

   End Function

   Public Function GetSqlDbType(ByVal column As ColumnSchema) As String

      Select Case (column.NativeType)

         Case "bigint"
            Return "BigInt"

         Case "binary"
            Return "Binary"

         Case "bit"
            Return "Bit"

         Case "char"
            Return "Char"

         Case "datetime"
            Return "DateTime"

         Case "decimal"
            Return "Decimal"

         Case "float"
            Return "Float"

         Case "image"
            Return "Image"

         Case "int"
            Return "Int"

         Case "money"
            Return "Money"

         Case "nchar"
            Return "NChar"

         Case "ntext"
            Return "NText"

         Case "numeric"
            Return "Decimal"

         Case "nvarchar"
            Return "NVarChar"

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

         Case "sysname"
            Return "NChar"

         Case "text"
            Return "Text"

         Case "timestamp"
            Return "Timestamp"

         Case "tinyint"
            Return "TinyInt"

         Case "uniqueidentifier"
            Return "UniqueIdentifier"

         Case "varbinary"
            Return "VarBinary"

         Case "varchar"
            Return "VarChar"

         Case Else
            Return "__UNKNOWN__" + column.NativeType
      End Select

   End Function

   Public Function GetPrimaryKeys(ByVal PrimaryKey As ColumnSchemaCollection, _
                                  Optional ByVal prefix As String = "", _
                                  Optional ByVal Access As String = "Private ", _
                                  Optional ByVal nLevel As Integer = 1, _
                                  Optional ByVal sFLCPrefix As String = "", _
                                  Optional ByVal bIsForStructure As Boolean = False) As String
      Dim PKS As String
      Dim i As Integer
      Dim Member As String
      Dim DefaultValue As String

      If Len(prefix) > 0 Then
         Member = prefix

      Else
         Member = ""
      End If

      For i = 0 To PrimaryKey.Count - 1
         If bIsForStructure Then
            DefaultValue = ""
         Else
            DefaultValue = GetMemberVariableDefaultValue(PrimaryKey(i))
         End If

         PKS += Access + " " + Member + sFLCPrefix + GetCamelCaseName(PrimaryKey(i).Name) + " As " + GetVBVariableType(PrimaryKey(i))

         If (DefaultValue <> "") Then
            PKS += " = " + DefaultValue + "    '**PK"
         End If

         If i < PrimaryKey.Count - 1 Then PKS += Level(nLevel, True)
      Next i

      Return PKS
   End Function

   Public Function StringReplacement(ByVal PrimaryKey As ColumnSchemaCollection) As String
      Dim str As String
      Dim i As Integer

      For i = 0 To PrimaryKey.Count - 1
         str += GetMemberVariableName(PrimaryKey(i)) + ".ToString"

         If i < PrimaryKey.Count - 1 Then
            str += " + " + Chr(34) + "." + Chr(34) + " + "
         End If

      Next

      Return str
   End Function

   Public Function StringReplacement(ByVal TblName As String, _
                                     ByVal PrimaryKey As ColumnSchemaCollection) As String
      Dim str As String = Chr(34) + TblName + "." + Chr(34) + " + "
      Dim i As Integer

      For i = 0 To PrimaryKey.Count - 1
         str += GetMemberVariableName(PrimaryKey(i)) + ".ToString"

         If i < PrimaryKey.Count - 1 Then
            str += " + " + Chr(34) + "." + Chr(34) + " + "
         End If

      Next

      Return str
   End Function

   Public Function GetPropertyVariableType(ByVal column As ColumnSchema) As String

      If column.Name.EndsWith("TypeCode") Then
         Return column.Name
      End If

      Select Case column.DataType

         Case DbType.AnsiString, DbType.AnsiStringFixedLength, DbType.Date, DbType.DateTime, DbType.String, DbType.StringFixedLength
            Return "String"

         Case DbType.Binary
            Return "Byte()"

         Case DbType.Byte
            Return "Byte"

         Case DbType.Boolean
            Return "Boolean"

         Case DbType.Currency, DbType.Decimal, DbType.VarNumeric
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
            Return "Float"

         Case DbType.Time
            Return "TimeSpan"

         Case DbType.UInt16
            Return "UShort"

         Case DbType.UInt32
            Return "UInt"

         Case DbType.UInt64
            Return "ULong"

         Case Else
            Return "__UNKNOWN__" + column.NativeType
      End Select

   End Function

   Public Function GetMustInherit(ByVal value As Boolean) As String

      If value = True Then
         Return "MustInherit "
      Else
         Return ""
      End If

   End Function

   Public Function GetContainedObjectName() As String

      If IsCollectionCode Then
         Return GetCustomCollectionName

      Else
         Return GetCustomClassName
      End If

   End Function

   Public Function GetAccessModifier(ByVal accessibility As AccessibilityEnum) As String

      Select Case accessibility

         Case AccessibilityEnum.Public, CType(0, AccessibilityEnum)
            GetAccessModifier = "Public"

         Case AccessibilityEnum.Protected, CType(1, AccessibilityEnum)
            GetAccessModifier = "Protected"

         Case AccessibilityEnum.Friend, CType(2, AccessibilityEnum)
            GetAccessModifier = "Friend"

         Case AccessibilityEnum.ProtectedFriend, CType(3, AccessibilityEnum)
            GetAccessModifier = "Protected Friend"

         Case AccessibilityEnum.Private, CType(4, AccessibilityEnum)
            GetAccessModifier = "Private"

         Case Else
            GetAccessModifier = "Public"
      End Select

      Return GetAccessModifier
   End Function

#Region "Public Enums (Template, CollectionTemplate, Accessibility, Transaction)"
   Public Enum TemplateEnum As Integer
      [EditableRoot] = 0
      [EditableRootCollection] = 1
      [EditableSwitchable] = 2
      [EditableChild] = 3
      [EditableChildCollection] = 4
      [NameValueList] = 5
      [ReadOnly] = 6
      [ReadOnlyCollection] = 7
   End Enum

   Public Enum CollectionTemplateEnum As Integer
      [None] = 0
      [EditableRootCollection] = 1
      [EditableChildCollection] = 2
      [ReadOnlyCollection] = 3
   End Enum

   Public Enum AccessibilityEnum As Integer
      [Public] = 0
      [Protected] = 1
      [Friend] = 2
      [ProtectedFriend] = 3
      [Private] = 4
   End Enum

   Public Enum TransactionEnum As Integer
      [None] = 0
      [ADO] = 1
      [EnterpriseServices] = 2
   End Enum
#End Region 'Public Enums (Template, CollectionTemplate, Accessibility, Transaction)



End Class
