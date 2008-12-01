'*******************************************************
' CSLA Express v0.1.0
' Author: William McNeill wcm777@gmail.com
'
' Modifications
'*******************************************************
'
'*******************************************************
#Region " IMPORTS "

Option Strict On
Option Explicit On
Option Compare Text

Imports CodeSmith.Engine
Imports SchemaExplorer
Imports System.Text
Imports Microsoft.VisualBasic
Imports System
Imports System.ComponentModel
Imports System.Data
Imports System.Diagnostics
Imports System.IO

#End Region

Public Class CSLAExpress20
  'Inherits CodeTemplate ' <-- comment this line out for development in VS!!

#Region " Template Member Variables "
  ' All of these variables should have a corresponding .vb file in 
  ' the CSLAExpriess\Properties sub-dir that is loaded by
  ' Codesmith as needed by each template file.

  '  Business Object
  Protected m_ObjectName As System.String = ""
  Protected m_ObjectTable As SchemaExplorer.TableSchema
  Protected m_MemberPrefix As System.String = "m_"
  Protected m_ObjectFetchDataSource As SchemaExplorer.ViewSchema

  '  Business Object - Child
  Protected m_ChildType As String = ""
  Protected m_ChildColType As String = ""
  Protected m_ChildColName As String = ""
  Protected m_ChildTable As SchemaExplorer.TableSchema
  Protected m_ChildFetchDataSource As SchemaExplorer.ViewSchema
  Protected m_FK_to_Parent As String = ""

  '  Business Object - Collections
  ' TODO: add property block
  Protected m_UseEnhancedCollection As Boolean = True

  '  Business Object - Parent
  Protected m_ParentType As String = "MyParentType"

  ' Authoriztion Rules - Object Level
  Protected m_CanGetObject As String = ""
  Protected m_CanAddObject As String = ""
  Protected m_CanEditObject As String = ""
  Protected m_CanDeleteObject As String = ""
  Protected m_CanExecuteObject As String = ""

  ' Authorization Rules - Properties Level
  Protected m_AllowRead As String = ""
  Protected m_AllowWrite As String = ""
  Protected m_DenyRead As String = ""
  Protected m_DenyWrite As String = ""

  ' Business Methods
  Protected m_CanReadPropMethods As PropAuthMethods = PropAuthMethods.ThrowOnFalse
  Protected m_CanWritePropMethods As PropAuthMethods = PropAuthMethods.ThrowOnFalse
  Protected m_PropHasChangedMethods As PropHasChangedMethods = PropHasChangedMethods.Parameterless

  ' Data Access
  Protected m_RunLocalCreate As Boolean = True

  ' Validation Rules
  Protected m_ImplementValidation As Boolean = True

  ' File Generation
  Protected m_OutputDirectory As String = ".\Generated\"
  Protected m_OutputToFile As Boolean = True
  Protected m_TodaySubFolder As Boolean = True

  ' General Options - Business Object
  Protected m_UseSmartDate As Boolean
  Protected m_CamelCaseMemberVars As Boolean
  Protected m_NameSpace As String = ""
  Protected m_GenClass As Boolean = True
  Protected m_GenSProcs As Boolean = True
  Protected m_GenTests As Boolean = True
  Protected m_UseSP_ReturnValue As Boolean
  Protected m_Implement_Exists As Boolean = True
  Protected m_AnonymousAccess As Boolean
  Protected m_ExecutionTime As Boolean
  Protected m_AddComments As Boolean = True
  Protected m_UseStructureForChild As Boolean = True

  ' General Options - Database   
  Protected m_ConnectionString As String = ""
  Protected m_DatabaseObjectOwner As String = "dbo"
  Protected m_DeleteTransationalType As TransactionalTypes = TransactionalTypes.TransactionScope
  Protected m_InsertTransationalType As TransactionalTypes = TransactionalTypes.TransactionScope
  Protected m_UpdateTransationalType As TransactionalTypes = TransactionalTypes.TransactionScope
  Protected m_SProc_GenerateDROP As Boolean
  Protected m_GrantExecute As String = ""
  Protected m_ConcurrencyErrMsg As String = "'Row has been edited by another user'"

  ' Stored Procedures
  Protected m_SPInsertPrefix As System.String = "add_"
  Protected m_SPUpdatePrefix As System.String = "update_"
  Protected m_SPDeletePrefix As System.String = "delete_"
  Protected m_SPSelectPrefix As System.String = "get_"
  Protected m_SPExistsPrefix As System.String = "exists_"
  Protected m_SPListPrefix As System.String = "get_"
  Protected m_SPNVLPrefix As System.String = "get_"
  Protected m_SPInsertSuffix As System.String = ""
  Protected m_SPUpdateSuffix As System.String = ""
  Protected m_SPDeleteSuffix As System.String = ""
  Protected m_SPSelectSuffix As System.String = ""
  Protected m_SPExistsSuffix As System.String = ""
  Protected m_SPListSuffix As System.String = ""
  Protected m_SPNVLSuffix As System.String = ""

  ' NameValue List
  Protected m_NVL_NameField As String = ""

  ' Command Object
  Protected m_CommandMethod As String = "" ' command object method name in the factory

  ' Unit Test
  Protected m_MaxEquals As Integer = 4000
  Protected m_TestImports As String = ""
  Protected m_TestExecuteTime As String = "100"

  ' Collection
  Protected m_SwitchableChildMembers As Boolean

#End Region ' Template Member Variables

#Region " Structures "

  Public Structure ColumnDef
    Public AddWithValue As String
    Public AllowDBNull As Boolean
    Public CamelCaseName As String
    Public DataType As String
    Public Declaration As String
    Public IsByteArray As Boolean
    Public IsForeignKeyMember As Boolean
    Public IsIdentity As Boolean
    Public IsMax As Boolean
    Public IsObject As Boolean
    Public IsPrimaryKeyMember As Boolean
    Public IsReadOnly As Boolean
    Public IsSmartDate As Boolean
    Public IsString As Boolean
    Public IsTimeStamp As Boolean
    Public IsWriteOnly As Boolean
    Public MemberName As String
    Public Name As String
    Public NativeType As String
    Public Precision As Byte
    Public ReaderMethod As String
    Public Scale As Integer
    Public Size As Integer
    Public SQLDataType As String
    Public SQLParam As String
    Public TestAssign As String
    Public TestInit As String
    Public TestModify As String
    Public VBDataType As String
  End Structure

  Public Structure BusinessObject
    Public BOType As BOTypes
    Public ColDefs() As ColumnDef
    Public DataSource As String
    Public FetchColDefs() As ColumnDef
    Public FetchDataSource As String
    Public ForeignKey As PrimaryKey
    Public HasByteArray As Boolean
    Public HasChild As Boolean
    Public HasIdentityField As Boolean
    Public HasTimeStampField As Boolean
    Public Name As String
    Public PrimaryKey As PrimaryKey
    Public TimeStamp As String
  End Structure

  Public Structure PrimaryKey
    Public MemberName As String
    Public Name As String
    Public VBDataType As String
    Public SQLParam As String
  End Structure

#End Region ' Structures

#Region " Enums "

  Public Enum PropAuthMethods As Integer
    NotUsed
    Parameterless
    PropertyName
    PropertyName_ThrowOnFalse
    ThrowOnFalse
  End Enum

  Public Enum PropHasChangedMethods As Integer
    NotUsed
    Parameterless
    PropertyName
  End Enum

  Public Enum TransactionalTypes As Integer
    [Default] = 0
    TransactSQL = 1
    EnterpriseServices = 2
    TransactionScope = 3
    Manual = 4
  End Enum

  Public Enum BOTypes
    UnknownType = 0
    EditableRoot = 1
    EditableChild = 2
    EditableRootCollection = 3
    EditableChildCollection = 4
    ReadOnlyChild = 5
    ReadOnlyObject = 6
    ReadOnlyCollection = 7
    NameValueList = 8
    Switchable = 9
    CommandObject = 10
  End Enum

  Private Enum CommentTypes
    Generator = 0
    XML = 1
    SProc = 2
  End Enum

#End Region ' Enums

#Region " Member Varialbles "

  Private m_Object As BusinessObject
  Private m_Child As BusinessObject
  Private m_DataBaseName As String

#End Region ' Member Variables

#Region " VS Development "

  Public Sub SetEnv(ByVal BOType As BOTypes, _
    ByVal ObjectTable As SchemaExplorer.TableSchema, ByVal ObjectFetch As SchemaExplorer.ViewSchema, _
    ByVal ChildTable As SchemaExplorer.TableSchema, ByVal ChildFetch As SchemaExplorer.ViewSchema)

    ' For development use ONLY!!!!!!
    ' Set the properties the backdoor way

    Select Case BOType
      Case BOTypes.EditableRoot
        m_ObjectName = ""
        m_ChildColType = ""
        m_ChildColName = ""
        m_ChildType = ""

      Case BOTypes.EditableChildCollection
        m_ObjectName = ""
        m_ChildType = ""
        m_ParentType = ""

      Case BOTypes.EditableChild
        m_ObjectName = ""

      Case BOTypes.ReadOnlyChild
        m_ObjectName = ""

      Case BOTypes.Switchable
        m_ObjectName = ""
        m_ChildColType = ""
        m_ChildColName = "" ' instance var name of the child collection
        m_ParentType = ""
        m_FK_to_Parent = "ID" ' this is required to correctly identify the table's FK to the parent

      Case BOTypes.ReadOnlyObject
        m_ObjectName = ""
        m_ChildType = ""
        m_ChildColName = "" ' instance var name of the child collection
        m_ParentType = ""
        m_FK_to_Parent = ""

      Case BOTypes.EditableRootCollection
        m_ObjectName = ""
        m_ChildType = ""
        m_ChildColType = ""

      Case BOTypes.ReadOnlyCollection
        m_ObjectName = ""
        m_ChildType = ""
        m_ChildColName = ""
        m_ParentType = ""
        m_FK_to_Parent = ""

      Case BOTypes.NameValueList
        m_ObjectName = ""
        m_ChildType = ""
        m_ChildColName = ""
        m_ParentType = ""
        m_FK_to_Parent = ""
        m_NVL_NameField = ""

      Case BOTypes.CommandObject
        m_ObjectName = ""
        m_ChildType = ""
        m_ChildColName = ""
        m_ParentType = ""
        m_FK_to_Parent = ""
        m_NVL_NameField = ""
        m_CommandMethod = ""

      Case Else
        Debug.Assert(False, "Unhandled BOType")

    End Select

    m_ObjectTable = ObjectTable
    m_ChildTable = ChildTable
    m_ObjectFetchDataSource = ObjectFetch
    m_ChildFetchDataSource = ChildFetch


    '  Business Object
    m_MemberPrefix = "m_"
    ' Authoriztion Rules - Object Level
    m_CanGetObject = ""
    m_CanAddObject = ""
    m_CanEditObject = ""
    m_CanDeleteObject = ""
    m_CanExecuteObject = ""


    ' Authorization Rules - Properties Level
    m_AllowRead = ""
    m_AllowWrite = ""
    m_DenyRead = ""
    m_DenyWrite = ""

    ' Business Methods
    m_CanReadPropMethods = PropAuthMethods.ThrowOnFalse
    m_CanWritePropMethods = PropAuthMethods.ThrowOnFalse
    m_PropHasChangedMethods = PropHasChangedMethods.Parameterless

    ' Data Access
    m_RunLocalCreate = True

    ' Validation Rules
    m_ImplementValidation = True

    ' NEEDS PROPERTY FILE
    m_OutputDirectory = ".\Generated"
    m_OutputToFile = True
    m_UseSmartDate = Not True
    m_CamelCaseMemberVars = Not True
    m_TodaySubFolder = True

    m_ExecutionTime = True

    m_NameSpace = ""

    m_SProc_GenerateDROP = True
    m_GrantExecute = ""
    m_ConnectionString = """Data Source=SQL2005;Integrated Security=True;Initial Catalog=MyDB"""
    'm_UpdateTransationalType = TransactionalTypes.TransactSQL
    m_UseSP_ReturnValue = Not True
    m_AnonymousAccess = True

    m_TestImports = ""

  End Sub

#End Region

#Region " Class Generation "

  Public Function Generate(ByVal BusinessObjectType As String) As String

    Dim sb As StringBuilder = New StringBuilder

    m_Object = New BusinessObject
    With m_Object
      .Name = m_ObjectName
      .BOType = CType(System.Enum.Parse(GetType(BOTypes), BusinessObjectType), BOTypes)
      If .BOType = BOTypes.UnknownType Then Throw New Exception("Unknown Business Object Type")
    End With

    If Not m_Object.BOType = BOTypes.CommandObject Then
      MapSchema()
    End If

    If m_GenClass Then
      sb.Append(BuildBusinessObject())
    End If

    If m_GenSProcs Then
      If Not m_Object.BOType = BOTypes.CommandObject AndAlso _
        Not m_Object.BOType = BOTypes.EditableChildCollection AndAlso _
        Not m_Object.BOType = BOTypes.ReadOnlyChild Then
        If m_GenClass Then
          sb.Append("  '***************************************************" & vbCrLf)
          sb.Append("  '** END BUSINESS OBJECT / BEGIN STORED PROCEDURES **" & vbCrLf)
          sb.Append("  '***************************************************" & vbCrLf)
        End If
        sb.Append(BuildStoredProcedures())
      End If
    End If

    If m_GenTests Then
      If Not m_Object.BOType = BOTypes.EditableChildCollection AndAlso _
        Not m_Object.BOType = BOTypes.EditableChild AndAlso _
        Not m_Object.BOType = BOTypes.ReadOnlyChild Then

        If m_GenClass And Not m_GenSProcs Then
          sb.Append("  '********************************************" & vbCrLf)
          sb.Append("  '** END BUSINESS OBJECT / BEGIN UNIT TESTS **" & vbCrLf)
          sb.Append("  '********************************************" & vbCrLf)
        ElseIf m_GenSProcs Then
          sb.Append("  '**********************************************" & vbCrLf)
          sb.Append("  '** END STORED PROCEDURES / BEGIN UNIT TESTS **" & vbCrLf)
          sb.Append("  '**********************************************" & vbCrLf)
        End If
        sb.Append(BuildUnitTests())
      Else
        'Debug.Assert(False)
      End If

    End If

    If Not m_GenClass AndAlso Not m_GenSProcs AndAlso Not m_GenTests Then
      sb.Append("  '*************************************************" & vbCrLf)
      sb.Append("  '**                NOTHING TO DO                **" & vbCrLf)
      sb.Append("  '*************************************************" & vbCrLf)
    End If

    Return sb.ToString

  End Function

  Private Function BuildBusinessObject() As String

    Dim sb As StringBuilder = New StringBuilder

    Select Case m_Object.BOType

      Case BOTypes.EditableRoot
        sb.Append(DoNamespace(True, "Editable Root Business Object"))
        sb.Append(CSLA_ER)

      Case BOTypes.EditableChild
        sb.Append(DoNamespace(True, "Editable Child Business Object"))
        sb.Append(CSLA_EC)

      Case BOTypes.Switchable
        sb.Append(DoNamespace(True, "Switchable Root/Child Business Object"))
        sb.Append(CSLA_ES)

      Case BOTypes.EditableRootCollection
        sb.Append(DoNamespace(True, "Editable Root Collection Business Object"))
        sb.Append(CSLA_ERC)

      Case BOTypes.EditableChildCollection
        sb.Append(DoNamespace(True, "Editable Child Collection Business Object"))
        sb.Append(CSLA_ECC)

      Case BOTypes.ReadOnlyChild
        sb.Append(DoNamespace(True, "ReadOnly Child Business Object"))
        sb.Append(CSLA_ROCh)

      Case BOTypes.ReadOnlyObject
        sb.Append(DoNamespace(True, "ReadOnly Business Object"))
        sb.Append(CSLA_ROO)

      Case BOTypes.ReadOnlyCollection
        sb.Append(DoNamespace(True, "ReadOnly Collection Business Object"))
        sb.Append(CSLA_ROC)

      Case BOTypes.NameValueList
        If m_NVL_NameField.Length = 0 Then Throw New Exception("You must specify the NVL Name field")
        sb.Append(DoNamespace(True, "NameValue List Business Object"))
        sb.Append(CSLA_NVL)

      Case BOTypes.CommandObject
        If m_CommandMethod.Length = 0 Then Throw New Exception("You must specify the Command Method name")
        sb.Append(DoNamespace(True, "Command Business Object"))
        sb.Append(CSLA_CO)

      Case Else
        Debug.Assert(False, "Unhandled BOType")

    End Select

    If m_NameSpace.Length > 0 Then
      ' This is a bit of a kludge.  Need to add a few extra spaces
      ' when a Namespace is used.  RegEx would be a better way...
      Dim sb2 As StringBuilder = New StringBuilder
      sb2.Append(Replace(sb.ToString, vbCrLf, vbCrLf & Space(2)))
      sb2.Append(vbCrLf)

      sb = New StringBuilder
      If m_AddComments Then
        sb.Append(GetComments(CommentTypes.Generator, String.Empty))
      End If
      sb.Append(Replace(sb2.ToString, Space(2) & vbCrLf, vbCrLf))

      sb2 = New StringBuilder
      sb2.Append(Replace(sb.ToString, vbCrLf & Space(2) & "#", vbCrLf & "#"))
      sb2.Append(DoNamespace(False, Nothing))

      sb = New StringBuilder
      sb.Append(sb2.ToString)
    Else
      If m_AddComments Then
        Dim sb2 As StringBuilder = New StringBuilder
        sb2.Append(GetComments(CommentTypes.Generator, String.Empty) & vbCrLf)
        sb2.Append(sb.ToString)
        sb = New StringBuilder
        sb.Append(sb2.ToString)
      End If

    End If

    If m_OutputToFile Then OutputGeneratedObject(sb, "") 'm_OutputFileName)

    Return sb.ToString

  End Function

  Private Function CSLA_ER() As String

    Dim sb As StringBuilder = New StringBuilder

    sb.Append(DoClass(m_Object.Name, True))
    sb.Append("  Inherits BusinessBase(Of " & m_Object.Name & ")" & vbCrLf & vbCrLf)

    sb.Append(DoRegion("Business Methods", True))
    sb.Append(BusinessMethod_MemberVariables(m_Object.FetchColDefs))
    sb.Append(BusinessMethod_MemberVariablesExecutionTime)
    sb.Append(BuisnessMethod_PropertyBlocks(m_Object.FetchColDefs, False))
    sb.Append(BuisnessMethod_PropertyBlock_ExecutionTime)
    sb.Append(DoRegion("Business Methods", False))

    sb.Append(DoRegion("Validation Rules", True))
    sb.Append(ValidationAddBusinessRules())
    sb.Append(DoRegion("Validation Rules", False))

    sb.Append(DoRegion("Authorization Rules", True))
    sb.Append(AuthorizationProperties(False))
    sb.Append(AuthorizationObjectMethods(False))
    sb.Append(DoRegion("Authorization Rules", False))

    sb.Append(DoRegion("Factory Methods", True))
    sb.Append(FactoryNewObject())
    sb.Append(FactoryGetObject())
    sb.Append(FactoryDeleteObject)
    sb.Append(FactorySaveObject)
    sb.Append(NewObject(True, False))
    sb.Append(DoRegion("Factory Methods", False))

    sb.Append(DoRegion("Data Access", True))
    sb.Append(DataAccess_CriteriaClass())
    sb.Append(DataPortalCreate())
    sb.Append(DataPortalFetch())
    sb.Append(DataPortalInsert())
    sb.Append(DataPortalUpdate())
    sb.Append(DataPortalDeleteSelf)
    sb.Append(DataPortalDelete())
    sb.Append(DataPortalInvoke)
    sb.Append(DoRegion("Data Access", False))

    If m_Implement_Exists Then
      sb.Append(DoRegion("Exists", True))
      sb.Append(CSLA_Command_Exists)
      sb.Append(DoRegion("Exists", False))
    End If

    sb.Append(DoClass(m_Object.Name, False))

    Return sb.ToString

  End Function

  Private Function CSLA_EC() As String

    Dim sb As StringBuilder = New StringBuilder

    sb.Append(DoClass(m_Object.Name, True))
    sb.Append("  Inherits BusinessBase(Of " & m_Object.Name & ")" & vbCrLf & vbCrLf)

    sb.Append(DoRegion("Business Methods", True))
    sb.Append(BusinessMethod_MemberVariables(m_Object.FetchColDefs))
    sb.Append(BuisnessMethod_PropertyBlocks(m_Object.FetchColDefs, False))
    sb.Append(DoRegion("Business Methods", False))

    sb.Append(DoRegion("Validation Rules", True))
    sb.Append(ValidationAddBusinessRules())
    sb.Append(DoRegion("Validation Rules", False))

    sb.Append(DoRegion("Authorization Rules", True))
    sb.Append(AuthorizationProperties(False))
    sb.Append(AuthorizationObjectMethods(False))
    sb.Append(DoRegion("Authorization Rules", False))

    sb.Append(DoRegion("Factory Methods", True))
    sb.Append(FactoryNewObject())
    sb.Append(FactoryGetObject())
    sb.Append(NewObject(True, True))
    sb.Append(NewObject(False, True))
    sb.Append(DoRegion("Factory Methods", False))

    sb.Append(DoRegion("Data Access", True))
    sb.Append(DataPortalFetch())
    sb.Append(DataPortalInsert())
    sb.Append(DataPortalUpdate())
    sb.Append(DataPortalDelete())
    sb.Append(DoRegion("Data Access", False))

    sb.Append(DoClass(m_Object.Name, False))

    Return sb.ToString

  End Function

  Private Function CSLA_ROCh() As String

    Dim sb As StringBuilder = New StringBuilder

    sb.Append(DoClass(m_Object.Name, True))
    sb.Append("  Inherits ReadOnlyBase(Of " & m_Object.Name & ")" & vbCrLf & vbCrLf)

    sb.Append(DoRegion("Business Methods", True))
    sb.Append(BusinessMethod_MemberVariables(m_Object.FetchColDefs))
    m_CanReadPropMethods = PropAuthMethods.NotUsed
    sb.Append(BuisnessMethod_PropertyBlocks(m_Object.FetchColDefs, True))
    sb.Append(DoRegion("Business Methods", False))

    sb.Append(DoRegion("Factory Methods", True))
    sb.Append(FactoryGetObject())
    sb.Append(NewObject(True, False))
    sb.Append(NewObject(False, False))
    sb.Append(DoRegion("Factory Methods", False))

    sb.Append(DoRegion("Data Access", True))
    sb.Append(DataPortalFetch())
    sb.Append(DoRegion("Data Access", False))

    sb.Append(DoClass(m_Object.Name, False))

    Return sb.ToString

  End Function

  Private Function CSLA_ES() As String ' Editable Switchable

    Dim sb As StringBuilder = New StringBuilder

    sb.Append(DoClass(m_Object.Name, True))
    sb.Append("  Inherits BusinessBase(Of " & m_Object.Name & ")" & vbCrLf & vbCrLf)

    sb.Append(DoRegion("Business Methods", True))
    sb.Append(BusinessMethod_MemberVariables(m_Object.FetchColDefs))
    sb.Append(BusinessMethod_MemberVariablesExecutionTime)
    sb.Append(BuisnessMethod_PropertyBlocks(m_Object.FetchColDefs, False))
    sb.Append(BuisnessMethod_PropertyBlock_ExecutionTime)
    sb.Append(DoRegion("Business Methods", False))

    sb.Append(DoRegion("Validation Rules", True))
    sb.Append(ValidationAddBusinessRules())
    sb.Append(DoRegion("Validation Rules", False))

    sb.Append(DoRegion("Authorization Rules", True))
    sb.Append(AuthorizationProperties(False))
    sb.Append(AuthorizationObjectMethods(False))
    sb.Append(DoRegion("Authorization Rules", False))

    sb.Append(DoRegion("Factory Methods", True))
    sb.Append(FactoryNewObject())
    sb.Append(FactoryNewObject(True))
    sb.Append(FactoryGetObject())

    sb.Append(FactoryGetObject(True))

    sb.Append(FactoryDeleteObject)
    sb.Append(FactorySaveObject)
    sb.Append(NewObject(True, False))
    sb.Append(NewObject(False, True))
    sb.Append(DoRegion("Factory Methods", False))

    sb.Append(DoRegion("Data Access", True))
    sb.Append(DataAccess_CriteriaClass())
    sb.Append(DataPortalCreate())
    sb.Append(DataPortalFetch())
    sb.Append(DataPortalInsert())
    sb.Append(DataPortalUpdate())
    sb.Append(DataPortalDeleteSelf)
    sb.Append(DataPortalDelete())
    sb.Append(DataPortalInsert(True))
    sb.Append(DataPortalUpdate(True))
    sb.Append(DataPortalDelete(True))
    sb.Append(DataPortalInvoke)
    sb.Append(DoRegion("Data Access", False))

    If m_Implement_Exists Then
      sb.Append(DoRegion("Exists", True))
      sb.Append(CSLA_Command_Exists)
      sb.Append(DoRegion("Exists", False))
    End If

    sb.Append(DoClass(m_Object.Name, False))

    Return sb.ToString

  End Function

  Private Function CSLA_ERC() As String

    Dim sb As StringBuilder = New StringBuilder

    sb.Append(DoClass(m_Object.Name, True))
    sb.Append("  Inherits BusinessListBase(Of " & m_Object.Name & ", " & m_ChildType & ")" & vbCrLf & vbCrLf)

    sb.Append(DoRegion("Business Methods", True))
    sb.Append(BusinessMethod_MemberVariablesExecutionTime)
    sb.Append(BuisnessMethod_Add())
    If m_UseEnhancedCollection Then
      sb.Append(BuisnessMethod_EnhancedCollection(False))
    End If
    sb.Append(BuisnessMethod_PropertyBlock_ExecutionTime)
    sb.Append(DoRegion("Business Methods", False))

    sb.Append(DoRegion("Authorization Rules", True))
    sb.Append(AuthorizationObjectMethods(False))
    sb.Append(DoRegion("Authorization Rules", False))

    sb.Append(DoRegion("Factory Methods", True))
    sb.Append(FactoryNewObject())
    sb.Append(FactoryGetObject())
    sb.Append(NewObject(True, False))
    sb.Append(DoRegion("Factory Methods", False))

    sb.Append(DoRegion("Data Access", True))
    sb.Append(DataAccess_CriteriaClass())
    sb.Append(DataPortalFetch())
    sb.Append(DataAccess_Update())
    sb.Append(DataPortalInvoke)
    sb.Append(DoRegion("Data Access", False))

    sb.Append(DoClass(m_Object.Name, False))

    Return sb.ToString

  End Function

  Private Function CSLA_ROC() As String

    Dim sb As StringBuilder = New StringBuilder

    sb.Append(DoClass(m_Object.Name, True))
    sb.Append("  Inherits ReadOnlyListBase(Of " & m_Object.Name & ", " & m_ChildType & ")" & vbCrLf & vbCrLf)

    sb.Append(DoRegion("Business Methods", True))
    If m_UseStructureForChild Then sb.Append(BusinessMethod_Structure(m_Object.FetchColDefs))
    sb.Append(BusinessMethod_MemberVariablesExecutionTime)
    If m_UseEnhancedCollection Then
      sb.Append(BuisnessMethod_EnhancedCollection(True))
    End If
    sb.Append(BuisnessMethod_PropertyBlock_ExecutionTime)
    sb.Append(DoRegion("Business Methods", False))

    sb.Append(DoRegion("Authorization Rules", True))
    sb.Append(AuthorizationObjectMethods(True))
    sb.Append(DoRegion("Authorization Rules", False))

    sb.Append(DoRegion("Factory Methods", True))
    sb.Append(FactoryGetObject())
    sb.Append(NewObject(True, False))
    sb.Append(DoRegion("Factory Methods", False))

    sb.Append(DoRegion("Data Access", True))
    sb.Append(DataAccess_CriteriaClass())
    sb.Append(DataPortalFetch())
    sb.Append(DataPortalInvoke)
    sb.Append(DoRegion("Data Access", False))

    sb.Append(DoClass(m_Object.Name, False))

    Return sb.ToString

  End Function

  Private Function CSLA_ECC() As String

    Dim sb As StringBuilder = New StringBuilder

    sb.Append(DoClass(m_Object.Name, True))
    sb.Append("  Inherits BusinessListBase(Of " & m_Object.Name & ", " & m_ChildType & ")" & vbCrLf & vbCrLf)

    sb.Append(DoRegion("Business Methods", True))
    sb.Append(BuisnessMethod_Add())
    If m_UseEnhancedCollection Then
      sb.Append(BuisnessMethod_EnhancedCollection(False))
    End If
    sb.Append(DoRegion("Business Methods", False))

    sb.Append(DoRegion("Factory Methods", True))
    sb.Append(FactoryNewObject())
    sb.Append(FactoryGetObject())
    sb.Append(NewObject(True, True))
    sb.Append(NewObject(False, True))
    sb.Append(DoRegion("Factory Methods", False))

    sb.Append(DoRegion("Data Access", True))
    sb.Append(DataPortalFetch())
    sb.Append(DataAccess_Update())
    sb.Append(DoRegion("Data Access", False))

    sb.Append(DoClass(m_Object.Name, False))

    Return sb.ToString

  End Function

  Private Function CSLA_ROO() As String

    Dim sb As StringBuilder = New StringBuilder

    sb.Append(DoClass(m_Object.Name, True))
    sb.Append("  Inherits ReadOnlyBase(Of " & m_Object.Name & ")" & vbCrLf & vbCrLf)

    sb.Append(DoRegion("Business Methods", True))
    sb.Append(BusinessMethod_MemberVariables(m_Object.FetchColDefs))
    sb.Append(BusinessMethod_MemberVariablesExecutionTime)
    sb.Append(BuisnessMethod_PropertyBlocks(m_Object.FetchColDefs, True))
    sb.Append(BuisnessMethod_PropertyBlock_ExecutionTime)
    sb.Append(DoRegion("Business Methods", False))

    sb.Append(DoRegion("Authorization Rules", True))
    sb.Append(AuthorizationProperties(True))
    sb.Append(AuthorizationObjectMethods(True))
    sb.Append(DoRegion("Authorization Rules", False))

    sb.Append(DoRegion("Factory Methods", True))
    sb.Append(FactoryGetObject())
    sb.Append(NewObject(True, False))
    sb.Append(DoRegion("Factory Methods", False))

    sb.Append(DoRegion("Data Access", True))
    sb.Append(DataAccess_CriteriaClass())
    sb.Append(DataPortalFetch())
    sb.Append(DataPortalInvoke)
    sb.Append(DoRegion("Data Access", False))

    If m_Implement_Exists Then
      sb.Append(DoRegion("Exists", True))
      sb.Append(CSLA_Command_Exists)
      sb.Append(DoRegion("Exists", False))
    End If

    sb.Append(DoClass(m_Object.Name, False))

    Return sb.ToString

  End Function

  Private Function CSLA_NVL() As String

    Dim sb As StringBuilder = New StringBuilder

    Dim strNVL_Name As String = String.Empty
    Dim strNVL_Value As String = String.Empty
    Dim strNVL_DataType As String = String.Empty

    strNVL_Name = m_Object.ColDefs(0).ReaderMethod.Replace(m_Object.ColDefs(0).MemberName & " = ", "dr")

    For Each col As ColumnDef In m_Object.ColDefs
      If col.Name = m_NVL_NameField Then
        strNVL_Value = col.ReaderMethod.Replace(col.MemberName & " = ", "dr")
        strNVL_DataType = col.VBDataType
      End If
    Next

    sb.Append(DoClass(m_Object.Name, True))
    sb.Append("  Inherits NameValueListBase(Of " & m_Object.ColDefs(0).Declaration & ", ")
    sb.Append(strNVL_DataType & ")" & vbCrLf & vbCrLf)

    sb.Append(DoRegion("Business Methods", True))
    sb.Append(BusinessMethod_MemberVariablesExecutionTime)
    sb.Append(BuisnessMethod_PropertyBlock_ExecutionTime)
    sb.Append(DoRegion("Business Methods", False))

    sb.Append(DoRegion("Factory Methods", True))
    sb.Append(Factory_NVL)
    sb.Append(FactoryGetObject())
    sb.Append(NewObject(True, False))
    sb.Append(DoRegion("Factory Methods", False))

    sb.Append(DoRegion("Data Access", True))
    sb.Append(DataPortalFetch(False, strNVL_Name & ", " & strNVL_Value))
    sb.Append(DataPortalInvoke)
    sb.Append(DoRegion("Data Access", False))

    sb.Append(DoClass(m_Object.Name, False))

    Return sb.ToString

  End Function

  Private Function CSLA_CO() As String

    Dim sb As StringBuilder = New StringBuilder

    sb.Append(DoClass(m_Object.Name, True))
    sb.Append("  Inherits CommandBase" & vbCrLf & vbCrLf)

    sb.Append(DoRegion("Authorization Rules", True))
    sb.Append(AuthorizationObjectMethods(True))
    sb.Append(DoRegion("Authorization Rules", False))

    sb.Append(DoRegion("Client-side Code", True))
    sb.Append(CSLA_Command_CS_Code(True))
    sb.Append(DoRegion("Client-side Code", False))

    sb.Append(DoRegion("Factory Methods", True))
    sb.Append(CSLA_Command_FactoryMethods())
    sb.Append(DoRegion("Factory Methods", False))

    sb.Append(DoRegion("Server-side Code", True))
    sb.Append(CSLA_Command_CS_Code(False))
    sb.Append(DoRegion("Server-side Code", False))

    sb.Append(DoClass(m_Object.Name, False))

    Return sb.ToString

  End Function

  Private Function CSLA_Command_Exists() As String

    Dim sb As New StringBuilder

    sb.Append("  Public Shared Function Exists(ByVal " & m_Object.PrimaryKey.Name & " As " & m_Object.PrimaryKey.VBDataType & ") As Boolean" & vbCrLf)
    sb.Append(vbCrLf)
    sb.Append("    Dim result As ExistsCommand" & vbCrLf)
    sb.Append("    result = DataPortal.Execute(Of ExistsCommand)(New ExistsCommand(" & m_Object.PrimaryKey.Name & "))" & vbCrLf)
    sb.Append("    Return result.Exists" & vbCrLf)
    sb.Append(vbCrLf)
    sb.Append("  End Function" & vbCrLf)
    sb.Append(vbCrLf)

    sb.Append("  <Serializable()> _" & vbCrLf)
    sb.Append("  Private Class ExistsCommand" & vbCrLf)
    sb.Append("    Inherits CommandBase" & vbCrLf)
    sb.Append(vbCrLf)
    sb.Append("    Private " & m_Object.PrimaryKey.MemberName & " As " & m_Object.PrimaryKey.VBDataType & vbCrLf)
    sb.Append("    Private " & m_MemberPrefix & "Exists As Boolean" & vbCrLf)
    sb.Append(vbCrLf)
    sb.Append("    Public ReadOnly Property Exists() As Boolean" & vbCrLf)
    sb.Append("      Get" & vbCrLf)
    sb.Append("        Return " & m_MemberPrefix & "Exists" & vbCrLf)
    sb.Append("      End Get" & vbCrLf)
    sb.Append("    End Property" & vbCrLf)
    sb.Append(vbCrLf)
    sb.Append("    Public Sub New(ByVal " & m_Object.PrimaryKey.Name & " As " & m_Object.PrimaryKey.VBDataType & ")" & vbCrLf)
    sb.Append("      " & m_Object.PrimaryKey.MemberName & " = " & m_Object.PrimaryKey.Name & vbCrLf)
    sb.Append("    End Sub" & vbCrLf)
    sb.Append(vbCrLf)
    sb.Append("    Protected Overrides Sub DataPortal_Execute()" & vbCrLf)
    sb.Append(vbCrLf)

    sb.Append("      Using cn As New SqlConnection(" & m_ConnectionString & ")" & vbCrLf)
    sb.Append("        cn.Open()" & vbCrLf)
    sb.Append("        Using cm As SqlCommand = cn.CreateCommand" & vbCrLf)
    sb.Append("          cm.CommandText = """ & m_SPExistsPrefix & m_Object.Name & m_SPExistsSuffix & """" & vbCrLf)
    sb.Append("          cm.CommandType = CommandType.StoredProcedure" & vbCrLf)
    sb.Append("          cm.Parameters.AddWithValue(""@" & m_Object.PrimaryKey.Name & """, " & m_Object.PrimaryKey.MemberName & ")" & vbCrLf)
    sb.Append(vbCrLf)
    sb.Append("          Dim count As Integer = CInt(cm.ExecuteScalar)" & vbCrLf)
    sb.Append("          " & m_MemberPrefix & "Exists = (count > 0)" & vbCrLf)
    sb.Append("        End Using" & vbCrLf)
    sb.Append("      End Using" & vbCrLf)
    sb.Append(vbCrLf)
    sb.Append("    End Sub" & vbCrLf & vbCrLf)
    sb.Append("  End Class" & vbCrLf & vbCrLf)


    Return sb.ToString

  End Function

  Private Function CSLA_Command_CS_Code(ByVal AsClient As Boolean) As String

    Dim sb As New StringBuilder

    If AsClient Then
      sb.Append("  Private mResult As Boolean" & vbCrLf & vbCrLf)

      sb.Append("  Public ReadOnly Property Result() As Boolean" & vbCrLf)
      sb.Append("    Get" & vbCrLf)
      sb.Append("      Return mResult" & vbCrLf)
      sb.Append("    End Get" & vbCrLf)
      sb.Append("  End Property" & vbCrLf & vbCrLf)

      sb.Append("  Private Sub BeforeServer()" & vbCrLf)
      sb.Append("    ' implement code to run on client" & vbCrLf)
      sb.Append("    ' before server is called" & vbCrLf)
      sb.Append("  End Sub" & vbCrLf & vbCrLf)

      sb.Append("  Private Sub AfterServer()" & vbCrLf)
      sb.Append("    ' implement code to run on client" & vbCrLf)
      sb.Append("    ' after server is called" & vbCrLf)
      sb.Append("  End Sub" & vbCrLf & vbCrLf)

    Else
      sb.Append("  Protected Overrides Sub DataPortal_Execute()" & vbCrLf & vbCrLf)
      sb.Append("    ' implement code to run on server" & vbCrLf)
      sb.Append("    ' here - and set result value" & vbCrLf & vbCrLf)

      sb.Append("    mResult = True : Return ' <--- ** REMOVE THIS LINE ONCE REAL CODE IS IN PLACE **" & vbCrLf & vbCrLf)

      sb.Append("    Using cn As New SqlConnection(" & m_ConnectionString & ")" & vbCrLf)
      sb.Append("      cn.Open()" & vbCrLf)
      sb.Append("      Using cm As SqlCommand = cn.CreateCommand" & vbCrLf)
      sb.Append("        cm.CommandText = ""MyCommand_StoredProcedure""" & vbCrLf)
      sb.Append("        cm.CommandType = CommandType.StoredProcedure" & vbCrLf)
      sb.Append("        cm.Parameters.AddWithValue(""@X"", ""XX"")" & vbCrLf)
      sb.Append("        mResult = CBool(cm.ExecuteScalar)" & vbCrLf)
      sb.Append("      End Using" & vbCrLf)
      sb.Append("    End Using" & vbCrLf & vbCrLf)

      sb.Append("  End Sub" & vbCrLf & vbCrLf)

    End If

    Return sb.ToString

  End Function

  Private Function CSLA_Command_FactoryMethods() As String

    Dim sb As New StringBuilder

    sb.Append("  Public Shared Function " & m_CommandMethod & "() As Boolean" & vbCrLf & vbCrLf)
    sb.Append("    Dim cmd As New " & m_Object.Name & vbCrLf)
    sb.Append("    cmd.BeforeServer()" & vbCrLf)
    sb.Append("    cmd = DataPortal.Execute(Of " & m_Object.Name & ")(cmd)" & vbCrLf)
    sb.Append("    cmd.AfterServer()" & vbCrLf)
    sb.Append("    Return cmd.Result" & vbCrLf & vbCrLf)
    sb.Append("  End Function" & vbCrLf & vbCrLf)

    sb.Append(NewObject(True, False))

    Return sb.ToString

  End Function

#End Region ' Class Generation

#Region " Business Object Region "

  Private Function BusinessMethod_MemberVariables(ByRef ColumnDefs() As ColumnDef) As String

    Dim sb As StringBuilder = New StringBuilder

    For Each column As ColumnDef In ColumnDefs
      If column.IsTimeStamp Then
        sb.Append("  Private " & column.MemberName & "(" & column.Size - 1 & ")" & " As " & column.Declaration & vbCrLf)
      ElseIf column.IsWriteOnly Then
        ' Skip
      Else
        sb.Append("  Private " & column.MemberName & " As " & column.Declaration & vbCrLf)
      End If
    Next

    If (m_Object.BOType = BOTypes.EditableChild Or m_Object.BOType = BOTypes.Switchable) And m_Object.PrimaryKey.VBDataType = "Integer" Then
      sb.Append("  Private Shared " & m_MemberPrefix & "LastID As Integer" & vbCrLf)
    End If

    If m_Object.HasChild Then
      sb.Append("  Private " & m_MemberPrefix & m_ChildColType & " As " & _
        m_ChildColType & " = " & m_ChildColType & ".New" & m_ChildColType & "()" & vbCrLf)
    End If

    sb.Append(vbCrLf)

    Return sb.ToString

  End Function

  Private Function BusinessMethod_Structure(ByRef ColumnDefs() As ColumnDef) As String

    Dim sb As StringBuilder = New StringBuilder

    sb.Append("  Public Structure " & m_ChildType & vbCrLf)
    For Each column As ColumnDef In ColumnDefs
      sb.Append("    Public " & column.Name & " As " & column.VBDataType & vbCrLf)
    Next
    sb.Append("  End Structure" & vbCrLf & vbCrLf)

    Return sb.ToString

  End Function

  Private Function BusinessMethod_MemberVariablesExecutionTime() As String

    Dim s As String = ""

    If m_ExecutionTime Then
      s = "  Private " & m_MemberPrefix & "InitTime As Date" & vbCrLf
      s = s & "  Private " & m_MemberPrefix & "ExecTime As TimeSpan" & vbCrLf & vbCrLf
    End If

    Return s

  End Function

  Private Function BuisnessMethod_PropertyBlocks(ByRef ColumnDefs() As ColumnDef, ByVal IsReadOnly As Boolean) As String

    Dim sb As StringBuilder = New StringBuilder
    Dim Column As ColumnDef

    For i As Integer = 0 To UBound(ColumnDefs)
      Column = ColumnDefs(i)
      sb.Append(BuisnessMethod_PropertyBlock(Column, IsReadOnly))
    Next

    If m_Object.HasChild Then
      sb.Append("  Public ReadOnly Property " & m_ChildColName & "() As " & m_ChildColType & vbCrLf)
      sb.Append("    Get" & vbCrLf)
      sb.Append("      Return " & m_MemberPrefix & m_ChildColType & vbCrLf)
      sb.Append("    End Get" & vbCrLf)
      sb.Append("  End Property" & vbCrLf & vbCrLf)

      If Not IsReadOnly Then
        sb.Append("  Public Overrides ReadOnly Property IsValid() As Boolean" & vbCrLf)
        sb.Append("    Get" & vbCrLf)
        If m_Object.HasChild Then
          sb.Append("      Return MyBase.IsValid AndAlso " & m_MemberPrefix & m_ChildColType & ".IsValid" & vbCrLf)
        Else
          sb.Append("      Return MyBase.IsValid" & vbCrLf)
        End If
        sb.Append("    End Get" & vbCrLf)
        sb.Append("  End Property" & vbCrLf & vbCrLf)

        sb.Append("  Public Overrides ReadOnly Property IsDirty() As Boolean" & vbCrLf)
        sb.Append("    Get" & vbCrLf)
        If m_Object.HasChild Then
          sb.Append("      Return MyBase.IsDirty OrElse " & m_MemberPrefix & m_ChildColType & ".IsDirty" & vbCrLf)
        Else
          sb.Append("      Return MyBase.IsDirty" & vbCrLf)
        End If
        sb.Append("    End Get" & vbCrLf)
        sb.Append("  End Property" & vbCrLf & vbCrLf)
      End If
    End If

    sb.Append("  Protected Overrides Function GetIdValue() As Object" & vbCrLf)
    sb.Append("    Return " & m_Object.PrimaryKey.MemberName & vbCrLf)
    sb.Append("  End Function")

    sb.Append(vbCrLf & vbCrLf)

    Return sb.ToString

  End Function

  Private Function BuisnessMethod_PropertyBlock(ByVal Column As ColumnDef, _
    ByVal IsReadOnly As Boolean) As String

    Dim sb As StringBuilder = New StringBuilder

    If Not Column.IsTimeStamp Then
      'If Column.IsReadOnly Then
      If IsReadOnly Then
        sb.Append("  Public ReadOnly Property " & Column.Name & "() As " & Column.VBDataType & CStr(IIf(Column.IsByteArray, "()", "")) & vbCrLf)

        'The Get Statement Definition
        sb.Append("    Get" & vbCrLf)
        ReadPropAuthorization(sb, Column.MemberName)
        sb.Append("      Return " & Column.MemberName & vbCrLf)
        sb.Append("    End Get" & vbCrLf)

        sb.Append("  End Property" & vbCrLf & vbCrLf)

      Else
        If Column.IsReadOnly Then
          sb.Append("  Public ReadOnly Property " & Column.Name & "() As " & Column.VBDataType & CStr(IIf(Column.IsByteArray, "()", "")) & vbCrLf)

          'The Get Statement Definition
          sb.Append("    Get" & vbCrLf)
          ReadPropAuthorization(sb, Column.MemberName)
          sb.Append("      Return " & Column.MemberName & vbCrLf)
          sb.Append("    End Get" & vbCrLf)

          sb.Append("  End Property" & vbCrLf & vbCrLf)

        ElseIf Column.IsWriteOnly Then
          ' Skip sb.Append("  Public WriteOnly Property " & Column.Name & "() As " & Column.VBDataType & CStr(IIf(Column.IsByteArray, "()", "")) & vbCrLf)

        Else
          sb.Append("  Public Property " & Column.Name & "() As " & Column.VBDataType & CStr(IIf(Column.IsByteArray, "()", "")) & vbCrLf)

          'The Get Statement Definition
          sb.Append("    Get" & vbCrLf)
          ReadPropAuthorization(sb, Column.Name)

          If Column.IsSmartDate Then
            sb.Append("      Return " & Column.MemberName & ".Text" & vbCrLf)
          Else
            sb.Append("      Return " & CStr(IIf(Column.IsByteArray, Column.TestAssign, Column.MemberName)) & vbCrLf)
          End If

          sb.Append("    End Get" & vbCrLf & vbCrLf)
        End If

        If Not Column.IsReadOnly AndAlso Not Column.IsWriteOnly Then
          'The Set Statement Definition
          sb.Append("    Set(ByVal Value" & CStr(IIf(Column.IsByteArray, "()", "")) & " As " & Column.VBDataType & ")" & vbCrLf)
          WritePropAuthorization(sb, Column.Name)

          If Not Column.IsObject AndAlso Not Column.IsByteArray Then
            sb.Append("      If " & Column.MemberName & " <> Value Then" & vbCrLf)
          ElseIf Column.IsByteArray Then
            sb.Append("      If BitConverter.ToString(" & Column.TestAssign & ") <> BitConverter.ToString(Value) Then" & vbCrLf)
          Else
            sb.Append("      If Not " & Column.MemberName & ".Equals(Value) Then" & vbCrLf)
          End If

          If Column.IsSmartDate Then
            sb.Append("        " & Column.MemberName & ".Text" & " = Value" & vbCrLf)
          Else
            sb.Append("        " & CStr(IIf(Column.IsByteArray, Column.TestAssign, Column.MemberName)) & " = Value" & vbCrLf)
          End If

          PropHasChanged(sb, Column.Name)
          sb.Append("      End If" & vbCrLf)
          sb.Append("    End Set" & vbCrLf)
          sb.Append("  End Property" & vbCrLf & vbCrLf)

        End If

      End If

    End If

    Return sb.ToString

  End Function

  Private Function BuisnessMethod_PropertyBlock_ExecutionTime() As String

    Dim sb As StringBuilder = New StringBuilder

    If m_ExecutionTime Then
      sb.Append("  Public ReadOnly Property ExecutionTime() As TimeSpan" & vbCrLf)
      sb.Append("    Get" & vbCrLf)
      sb.Append("      Return " & m_MemberPrefix & "ExecTime" & vbCrLf)
      sb.Append("    End Get" & vbCrLf)
      sb.Append("  End Property" & vbCrLf & vbCrLf)

    End If
    Return sb.ToString

  End Function

  Private Function BuisnessMethod_Add() As String

    Dim sb As StringBuilder = New StringBuilder

    sb.Append("  Public Overloads Function Add() As " & m_ChildType & vbCrLf & vbCrLf)
    If m_SwitchableChildMembers Then
      sb.Append("    Dim item As " & m_ChildType.ToLower & " = " & m_ChildType & ".New" & m_ParentType & m_ChildType & vbCrLf & vbCrLf)
    Else
      sb.Append("    Dim item As " & m_ChildType.ToLower & " = " & m_ChildType & ".New" & m_ChildType & vbCrLf & vbCrLf)
    End If
    sb.Append("    MyBase.Add(item)" & vbCrLf)
    sb.Append("    Return item" & vbCrLf & vbCrLf)
    sb.Append("  End Function" & vbCrLf & vbCrLf)

    Return sb.ToString

  End Function

  Private Function BuisnessMethod_EnhancedCollection(ByVal IsReadOnly As Boolean) As String

    Dim sb As StringBuilder = New StringBuilder

    sb.Append("  Public Function ItemByID(ByVal " & m_Object.PrimaryKey.Name & " As " & m_Object.PrimaryKey.VBDataType & ") As " & m_ChildType & vbCrLf & vbCrLf)
    sb.Append("    For Each item As " & m_ChildType & " In Me" & vbCrLf)
    sb.Append("      If item." & m_Object.PrimaryKey.Name & " = " & m_Object.PrimaryKey.Name & " Then" & vbCrLf)
    sb.Append("        Return item" & vbCrLf)
    sb.Append("      End If" & vbCrLf)
    sb.Append("    Next" & vbCrLf & vbCrLf)
    sb.Append("    Return Nothing" & vbCrLf & vbCrLf)
    sb.Append("  End Function" & vbCrLf & vbCrLf)

    sb.Append("  Public Overloads Function Contains(ByVal " & m_Object.PrimaryKey.Name & " As " & m_Object.PrimaryKey.VBDataType & ") As Boolean" & vbCrLf & vbCrLf)
    sb.Append("    For Each item As " & m_ChildType & " In Me" & vbCrLf)
    sb.Append("      If item." & m_Object.PrimaryKey.Name & " = " & m_Object.PrimaryKey.Name & " Then" & vbCrLf)
    sb.Append("        Return True" & vbCrLf)
    sb.Append("      End If" & vbCrLf)
    sb.Append("    Next" & vbCrLf & vbCrLf)
    sb.Append("    Return False" & vbCrLf & vbCrLf)
    sb.Append("  End Function" & vbCrLf & vbCrLf)

    If Not IsReadOnly Then
      sb.Append("  Public Overloads Sub Remove(ByVal " & m_Object.PrimaryKey.Name & " As " & m_Object.PrimaryKey.VBDataType & ")" & vbCrLf & vbCrLf)
      sb.Append("    For Each item As " & m_ChildType & " In Me" & vbCrLf)
      sb.Append("      If item." & m_Object.PrimaryKey.Name & " = " & m_Object.PrimaryKey.Name & " Then" & vbCrLf)
      sb.Append("        Remove(item)" & vbCrLf)
      sb.Append("        Return" & vbCrLf)
      sb.Append("      End If" & vbCrLf)
      sb.Append("    Next" & vbCrLf & vbCrLf)
      sb.Append("  End Sub" & vbCrLf & vbCrLf)

      sb.Append("  Public Overloads Function ContainsDeleted(ByVal " & m_Object.PrimaryKey.Name & " As " & m_Object.PrimaryKey.VBDataType & ") As Boolean" & vbCrLf & vbCrLf)
      sb.Append("    For Each item As " & m_ChildType & " In DeletedList" & vbCrLf)
      sb.Append("      If item." & m_Object.PrimaryKey.Name & " = " & m_Object.PrimaryKey.Name & " Then" & vbCrLf)
      sb.Append("        Return True" & vbCrLf)
      sb.Append("      End If" & vbCrLf)
      sb.Append("    Next" & vbCrLf & vbCrLf)
      sb.Append("    Return False" & vbCrLf & vbCrLf)
      sb.Append("  End Function" & vbCrLf & vbCrLf)

      sb.Append("  Public Function GetBrokenRules() As String" & vbCrLf & vbCrLf)
      sb.Append("    Dim sb As New System.Text.StringBuilder" & vbCrLf & vbCrLf)
      sb.Append("    For Each item As " & m_ChildType & " In Me" & vbCrLf)
      sb.Append("      If Not item.IsValid Then" & vbCrLf)
      sb.Append("        sb.Append(""" & m_Object.PrimaryKey.Name & ": "" & item.ToString & vbCrLf)" & vbCrLf)
      sb.Append("        For Each br As Csla.Validation.BrokenRule In item.BrokenRulesCollection" & vbCrLf)
      sb.Append("          sb.Append(""  "" & br.Description & vbCrLf)" & vbCrLf)
      sb.Append("        Next" & vbCrLf)
      sb.Append("      End If" & vbCrLf)
      sb.Append("    Next" & vbCrLf & vbCrLf)
      sb.Append("    Return sb.ToString" & vbCrLf & vbCrLf)
      sb.Append("  End Function" & vbCrLf & vbCrLf)
    End If

    Return sb.ToString

  End Function

  Private Sub ReadPropAuthorization(ByRef sb As StringBuilder, ByVal propertyName As String)

    Select Case m_CanReadPropMethods
      Case PropAuthMethods.NotUsed
        ' Do nothing
      Case PropAuthMethods.Parameterless
        sb.Append("      CanReadProperty()" & vbCrLf)
      Case PropAuthMethods.PropertyName
        sb.Append("      CanReadProperty(""" & propertyName & """)" & vbCrLf)
      Case PropAuthMethods.PropertyName_ThrowOnFalse
        sb.Append("      CanReadProperty(""" & propertyName & """, True)" & vbCrLf)
      Case PropAuthMethods.ThrowOnFalse
        sb.Append("      CanReadProperty(True)" & vbCrLf)
    End Select

  End Sub

  Private Sub WritePropAuthorization(ByRef sb As StringBuilder, ByVal propertyName As String)

    Select Case m_CanWritePropMethods
      Case PropAuthMethods.NotUsed
        ' Do nothing
      Case PropAuthMethods.Parameterless
        sb.Append("      CanWriteProperty()" & vbCrLf)
      Case PropAuthMethods.PropertyName
        sb.Append("      CanWriteProperty(""" & propertyName & """)" & vbCrLf)
      Case PropAuthMethods.PropertyName_ThrowOnFalse
        sb.Append("      CanWriteProperty(""" & propertyName & """, True)" & vbCrLf)
      Case PropAuthMethods.ThrowOnFalse
        sb.Append("      CanWriteProperty(True)" & vbCrLf)
    End Select

  End Sub

  Private Sub PropHasChanged(ByRef sb As StringBuilder, ByVal propertyName As String)

    Select Case m_PropHasChangedMethods
      Case PropHasChangedMethods.NotUsed
        ' Do nothing
      Case PropHasChangedMethods.Parameterless
        sb.Append("        PropertyHasChanged()" & vbCrLf)
      Case PropHasChangedMethods.PropertyName
        sb.Append("        PropertyHasChanged(""" & propertyName & """)" & vbCrLf)
    End Select

  End Sub

#End Region ' Business Object Region

#Region " Validation Region "

  Private Function ValidationAddBusinessRules() As String

    Dim sb As StringBuilder = New StringBuilder

    sb.Append("  Protected Overrides Sub AddBusinessRules()" & vbCrLf & vbCrLf)

    If m_ImplementValidation Then

      Dim column As ColumnDef
      For i As Integer = 0 To UBound(m_Object.ColDefs)
        column = m_Object.ColDefs(i)
        If Not column.IsReadOnly AndAlso Not column.IsWriteOnly Then
          If ((m_Object.BOType = BOTypes.EditableChild Or m_Object.BOType = BOTypes.Switchable) AndAlso (m_Object.ForeignKey.Name = column.Name)) Then
            ' do nothing 
          Else
            ValidationAddBusinessRule(column, sb)
          End If
        End If
      Next

    Else
      sb.Append("  ' Not Implemented" & vbCrLf & vbCrLf)
    End If

    sb.Append("  End Sub")
    sb.Append(vbCrLf & vbCrLf)

    Return sb.ToString

  End Function

  Private Sub ValidationAddBusinessRule(ByVal column As ColumnDef, ByRef sb As StringBuilder)

    If column.VBDataType = "String" And Not column.IsSmartDate Then

      If Not column.AllowDBNull Then
        sb.Append("    ValidationRules.AddRule(AddressOf Validation.CommonRules.StringRequired, """ & column.Name & """)" & vbCrLf)
      End If

      If Not column.IsMax Then
        sb.Append("    ValidationRules.AddRule(AddressOf Validation.CommonRules.StringMaxLength, _" & vbCrLf)
        sb.Append("      New Validation.CommonRules.MaxLengthRuleArgs(""" & column.Name & """, " & column.Size & "))" & _
          vbCrLf & vbCrLf)
      Else
        sb.Append(vbCrLf)
      End If

    ElseIf column.VBDataType = "Integer" And Not column.AllowDBNull Then
      sb.Append("    ValidationRules.AddRule(AddressOf Validation.CommonRules.IntegerMinValue, _" & vbCrLf)
      sb.Append("      New Validation.CommonRules.IntegerMinValueRuleArgs(""" & column.Name & """, 1))" & _
        vbCrLf & vbCrLf)

    End If
  End Sub

#End Region ' Validation Region

#Region " Authorization Region "

  Private Function AuthorizationProperties(ByVal IsReadOnly As Boolean) As String

    Dim sb As StringBuilder = New StringBuilder

    sb.Append("  Protected Overrides Sub AddAuthorizationRules()" & vbCrLf & vbCrLf)

    If m_AllowRead.Trim.Length > 0 OrElse m_DenyRead.Trim.Length > 0 _
      OrElse m_AllowWrite.Trim.Length > 0 OrElse m_DenyWrite.Trim.Length > 0 Then

      'For Each Column As ColumnDef In ColumnDefs
      Dim column As ColumnDef
      For i As Integer = 0 To UBound(m_Object.ColDefs)
        column = m_Object.ColDefs(i)
        AuthorizationProperty(column, sb, IsReadOnly)
      Next

    Else
      sb.Append("    'TODO: Add authorization rules" & vbCrLf & vbCrLf)

    End If

    sb.Append("  End Sub" & vbCrLf & vbCrLf)

    Return sb.ToString

  End Function

  Private Sub AuthorizationProperty(ByVal column As ColumnDef, ByRef sb As StringBuilder, ByVal IsReadOnly As Boolean)

    Dim b As Boolean

    If m_AllowRead.Trim.Length > 0 Then
      sb.Append("    AuthorizationRules.AllowRead(""" & column.Name & """, " & Auth_GetParamArray(m_AllowRead) & ")" & vbCrLf)
      b = True
    End If

    If m_DenyRead.Trim.Length > 0 Then
      sb.Append("    AuthorizationRules.DenyRead(""" & column.Name & """, " & Auth_GetParamArray(m_DenyRead) & ")" & vbCrLf)
      b = True
    End If

    If m_AllowWrite.Trim.Length > 0 AndAlso Not IsReadOnly Then
      If Not (column.IsIdentity) Then
        sb.Append("    AuthorizationRules.AllowWrite(""" & column.Name & """, " & Auth_GetParamArray(m_AllowWrite) & ")" & vbCrLf)
        b = True
      End If
    End If

    If m_DenyWrite.Trim.Length > 0 AndAlso Not IsReadOnly Then
      If Not (column.IsIdentity) Then
        sb.Append("    AuthorizationRules.DenyWrite(""" & column.Name & """, " & Auth_GetParamArray(m_DenyWrite) & ")" & vbCrLf)
        b = True
      End If
    End If

    If b Then sb.Append(vbCrLf)

  End Sub

  Private Function Auth_GetParamArray(ByVal CsvString As String) As String

    Dim a() As String = Split(CsvString, ",")
    Dim s As String = ""
    Dim i As Integer

    For i = 0 To UBound(a)
      s = s & """" & a(i).Replace(Chr(34), "").Trim() & ""","
    Next

    Return s.Substring(0, s.Length - 1)

  End Function

  Private Function AuthorizationObjectMethods(ByVal IsReadOnly As Boolean) As String

    Dim sb As StringBuilder = New StringBuilder

    If m_Object.BOType = BOTypes.CommandObject Then

      AuthorizationObjectMethod("ExecuteCommand", Split(m_CanExecuteObject, ","), sb)
      sb.Append(vbCrLf)

    Else

      AuthorizationObjectMethod("GetObject", Split(m_CanGetObject, ","), sb)
      sb.Append(vbCrLf)

      If Not IsReadOnly Then
        AuthorizationObjectMethod("AddObject", Split(m_CanAddObject, ","), sb)
        sb.Append(vbCrLf)

        AuthorizationObjectMethod("EditObject", Split(m_CanEditObject, ","), sb)
        sb.Append(vbCrLf)

        AuthorizationObjectMethod("DeleteObject", Split(m_CanDeleteObject, ","), sb)
        sb.Append(vbCrLf)
      End If

    End If

    Return sb.ToString

  End Function

  Private Sub AuthorizationObjectMethod(ByVal Action As String, ByVal Roles() As String, ByRef sb As StringBuilder)

    Dim i As Integer

    sb.Append("  Public Shared Function Can" & Action & "() As Boolean" & vbCrLf & vbCrLf)

    If Roles.Length = 1 AndAlso Roles(0).Length = 0 Then
      sb.Append("    'TODO: Review authorization for Can" & Action & "()" & vbCrLf & vbCrLf)
      sb.Append("    Return " & m_AnonymousAccess.ToString & vbCrLf & vbCrLf)

    ElseIf Roles.Length = 1 Then
      sb.Append("    Return Csla.ApplicationContext.User.IsInRole(""" & Roles(0).Replace(Chr(34), "").Trim() & """)" & vbCrLf & vbCrLf)

    Else
      sb.Append("    Dim result As Boolean" & vbCrLf & vbCrLf)
      For i = 0 To UBound(Roles)
        sb.Append("    If Csla.ApplicationContext.User.IsInRole(""" & Roles(i).Replace(Chr(34), "").Trim() & """) Then" & vbCrLf)
        sb.Append("      result = True" & vbCrLf)
        sb.Append("    End If" & vbCrLf & vbCrLf)
      Next
      sb.Append("    Return result" & vbCrLf & vbCrLf)

    End If

    sb.Append("  End Function" & vbCrLf)

  End Sub

#End Region ' Authorization Region

#Region " Factory Methods Region "

  Private Function FactoryNewObject(Optional ByVal SwitchableAsChild As Boolean = False) As String

    Dim sb As StringBuilder = New StringBuilder
    Dim strAccessLevel As String = ""
    Dim strObject As String = ""
    Dim strCriteria As String = ""

    If m_Object.BOType = BOTypes.EditableRoot Or m_Object.BOType = BOTypes.EditableRootCollection Then
      strAccessLevel = "Public" : strObject = m_Object.Name : strCriteria = "(New Criteria(Nothing))"

    ElseIf m_Object.BOType = BOTypes.EditableChild Or m_Object.BOType = BOTypes.EditableChildCollection Then
      strAccessLevel = "Friend" : strObject = m_Object.Name : strCriteria = "(New Criteria(Nothing))"

    ElseIf m_Object.BOType = BOTypes.Switchable Then
      If SwitchableAsChild Then
        strAccessLevel = "Friend" : strObject = m_Object.Name : strCriteria = "(New ChildCriteria)"
      Else
        strAccessLevel = "Public" : strObject = m_Object.Name : strCriteria = "(New Criteria)"
      End If
    End If

    If SwitchableAsChild Then
      sb.Append("  " & strAccessLevel & " Shared Function New" & m_ParentType & strObject & "() As " & m_Object.Name & vbCrLf & vbCrLf)
    Else
      sb.Append("  " & strAccessLevel & " Shared Function New" & strObject & "() As " & m_Object.Name & vbCrLf & vbCrLf)
    End If

    If m_Object.BOType = BOTypes.EditableRoot Or m_Object.BOType = BOTypes.EditableChild Then
      sb.Append("    If Not CanAddObject() Then" & vbCrLf)
      sb.Append("      Throw New System.Security.SecurityException(""User not authorized to add a " & strObject & """)" & vbCrLf)
      sb.Append("    End If" & vbCrLf & vbCrLf)
    End If

    If m_RunLocalCreate And m_Object.BOType = BOTypes.EditableChild Then ' Pg. 390
      sb.Append("    Return New " & strObject & vbCrLf & vbCrLf)

    ElseIf m_Object.BOType = BOTypes.EditableRootCollection Or m_Object.BOType = BOTypes.EditableChildCollection Then
      sb.Append("    Return New " & strObject & "()" & vbCrLf & vbCrLf)

    Else
      sb.Append("    Return DataPortal.Create(Of " & strObject & ")" & strCriteria & vbCrLf & vbCrLf)
    End If

    sb.Append("  End Function" & vbCrLf & vbCrLf)

    'If m_Object.BOType = BOTypes.Switchable Then

    'End If

    Return sb.ToString

  End Function

  Private Function FactoryGetObject(Optional ByVal SwitchableAsChild As Boolean = False) As String

    Dim sb As StringBuilder = New StringBuilder
    Dim strAccessLevel As String = ""
    Dim strObject As String = ""
    Dim strVar As String = "", strType As String = ""


    If m_Object.BOType = BOTypes.EditableRoot Or m_Object.BOType = BOTypes.ReadOnlyObject Then
      strAccessLevel = "Public" : strObject = m_Object.Name : strVar = "ByVal " & m_Object.PrimaryKey.Name & " As " & m_Object.PrimaryKey.VBDataType

    ElseIf m_Object.BOType = BOTypes.EditableChild Or m_Object.BOType = BOTypes.EditableChildCollection Or m_Object.BOType = BOTypes.ReadOnlyChild Then
      strAccessLevel = "Friend" : strObject = m_Object.Name : strVar = "ByVal dr As SafeDataReader"

    ElseIf m_Object.BOType = BOTypes.ReadOnlyCollection Then
      strAccessLevel = "Public" : strObject = m_Object.Name ': strVar = ""
      'strAccessLevel = "Public" : strObject = m_Object.Name : strVar = "ByVal " & m_Object.PrimaryKey.Name & " As " & m_Object.PrimaryKey.VBDataType

    ElseIf m_Object.BOType = BOTypes.EditableRootCollection Then
      strAccessLevel = "Public" : strObject = m_Object.Name ': strVar = ""

    ElseIf m_Object.BOType = BOTypes.Switchable Then
      If SwitchableAsChild Then
        strAccessLevel = "Friend" : strObject = m_Object.Name : strVar = "ByVal dr As SafeDataReader"
      Else
        strAccessLevel = "Public" : strObject = m_Object.Name : strVar = "ByVal " & m_Object.PrimaryKey.Name & " As " & m_Object.PrimaryKey.VBDataType
      End If

    ElseIf m_Object.BOType = BOTypes.NameValueList Then
      strAccessLevel = "Public" : strObject = m_Object.Name ': strVar = ""
    End If

    sb.Append("  " & strAccessLevel & " Shared Function Get" & strObject & "(" & strVar & ") As " & strObject & vbCrLf & vbCrLf)

    If m_Object.BOType = BOTypes.EditableRoot Or m_Object.BOType = BOTypes.EditableChild Then
      sb.Append("    If Not CanGetObject() Then" & vbCrLf)
      sb.Append("      Throw New System.Security.SecurityException(""User not authorized to view a " & strObject & """)" & vbCrLf)
      sb.Append("    End If" & vbCrLf & vbCrLf)
    End If

    If m_Object.BOType = BOTypes.EditableRoot Or m_Object.BOType = BOTypes.ReadOnlyObject _
      Or (m_Object.BOType = BOTypes.Switchable And Not SwitchableAsChild) Then
      sb.Append("    Return DataPortal.Fetch(Of " & strObject & ")(New Criteria(" & m_Object.PrimaryKey.Name & "))" & vbCrLf & vbCrLf)

    ElseIf m_Object.BOType = BOTypes.EditableRootCollection Or m_Object.BOType = BOTypes.ReadOnlyCollection Then
      sb.Append("    Return DataPortal.Fetch(Of " & strObject & ")(New Criteria(Nothing))" & vbCrLf & vbCrLf)

    ElseIf m_Object.BOType = BOTypes.EditableChild Or m_Object.BOType = BOTypes.EditableChildCollection _
      Or (m_Object.BOType = BOTypes.Switchable And SwitchableAsChild) Or m_Object.BOType = BOTypes.ReadOnlyChild Then
      sb.Append("    Return New " & strObject & "(dr)" & vbCrLf & vbCrLf)

    ElseIf m_Object.BOType = BOTypes.NameValueList Then
      sb.Append("    If mList Is Nothing Then" & vbCrLf)
      sb.Append("      Return DataPortal.Fetch(Of " & strObject & ") _" & vbCrLf)
      sb.Append("        (New Criteria(GetType(" & strObject & ")))" & vbCrLf)
      sb.Append("    End If" & vbCrLf & vbCrLf)
      sb.Append("    Return mList" & vbCrLf & vbCrLf)

    Else
      Debug.Assert(False, "Unhandled BOType")
    End If
    sb.Append("  End Function" & vbCrLf & vbCrLf)

    Return sb.ToString

  End Function

  Private Function FactoryDeleteObject() As String

    Dim sb As StringBuilder = New StringBuilder

    sb.Append("  Public Shared Sub Delete" & m_Object.Name & "(ByVal " & m_Object.PrimaryKey.Name & " As " & m_Object.PrimaryKey.VBDataType & ")" & vbCrLf & vbCrLf)
    'If m_CanDeleteObject.Length > 0 Then
    sb.Append("    If Not CanDeleteObject() Then" & vbCrLf)
    sb.Append("      Throw New System.Security.SecurityException(""User not authorized to remove a " & m_Object.Name & """)" & vbCrLf)
    sb.Append("    End If" & vbCrLf & vbCrLf)
    'End If
    sb.Append("    DataPortal.Delete(New Criteria(" & m_Object.PrimaryKey.Name & "))" & vbCrLf & vbCrLf)
    sb.Append("  End Sub" & vbCrLf & vbCrLf)

    Return sb.ToString

  End Function

  Private Function FactorySaveObject() As String

    Dim sb As StringBuilder = New StringBuilder

    sb.Append("  Public Overrides Function Save() As " & m_Object.Name & vbCrLf)
    sb.Append(vbCrLf)

    sb.Append("    If IsDeleted AndAlso Not CanDeleteObject() Then" & vbCrLf)
    sb.Append("      Throw New System.Security.SecurityException(""User not authorized to remove a " & m_Object.Name & """)" & vbCrLf)
    sb.Append("    End If" & vbCrLf)
    sb.Append(vbCrLf)

    sb.Append("    If IsNew AndAlso Not CanAddObject() Then" & vbCrLf)
    sb.Append("      Throw New System.Security.SecurityException(""User not authorized to add a " & m_Object.Name & """)" & vbCrLf)
    sb.Append("    End If" & vbCrLf)
    sb.Append(vbCrLf)

    sb.Append("    If Not CanEditObject() Then" & vbCrLf)
    sb.Append("      Throw New System.Security.SecurityException(""User not authorized to update a " & m_Object.Name & """)" & vbCrLf)
    sb.Append("    End If" & vbCrLf)
    sb.Append(vbCrLf)

    sb.Append("    Return MyBase.Save" & vbCrLf)
    sb.Append(vbCrLf)
    sb.Append("  End Function" & vbCrLf & vbCrLf)

    Return sb.ToString

  End Function

  Private Function NewObject(ByVal Std As Boolean, ByVal MarkAsChild As Boolean) As String

    Dim sb As StringBuilder = New StringBuilder

    If Std Then
      sb.Append("  Private Sub New()" & vbCrLf)
    Else
      sb.Append("  Private Sub New(ByVal dr As SafeDataReader)" & vbCrLf)
    End If

    If Not MarkAsChild Then
      If Std Then sb.Append("    ' require use of factory methods" & vbCrLf)
    Else
      sb.Append("    MarkAsChild()" & vbCrLf)
      If Std And m_Object.BOType = BOTypes.EditableChild Then
        sb.Append("    " & m_Object.PrimaryKey.MemberName & " = " & m_MemberPrefix & "LastID - 1" & vbCrLf)
        sb.Append("    " & m_MemberPrefix & "LastID = " & m_Object.PrimaryKey.Name & vbCrLf)
        sb.Append("    ValidationRules.CheckRules()" & vbCrLf)
      End If
    End If

    If Not Std Then sb.Append("    Fetch(dr)" & vbCrLf)

    sb.Append("  End Sub" & vbCrLf & vbCrLf)

    Return sb.ToString

  End Function

  Private Function Factory_NVL() As String

    Dim sb As StringBuilder = New StringBuilder

    sb.Append("  Private shared mList As " & m_Object.Name & vbCrLf & vbCrLf)

    sb.Append("  Public Shared Sub InvalidateCache()" & vbCrLf)
    sb.Append("    mList = Nothing" & vbCrLf)
    sb.Append("  End Sub" & vbCrLf & vbCrLf)

    Return sb.ToString

  End Function

#End Region ' Factory Methods Region

#Region " Data Access Region "

  Private Function DataAccess_CriteriaClass() As String

    Dim sb As StringBuilder = New StringBuilder

    sb.Append("  <Serializable()> _" & vbCrLf)
    sb.Append("  Private Class Criteria" & vbCrLf & vbCrLf)
    sb.Append("    Private " & m_Object.PrimaryKey.MemberName & " As " & m_Object.PrimaryKey.VBDataType & vbCrLf & vbCrLf)
    sb.Append("    Public ReadOnly Property " & m_Object.PrimaryKey.Name & "() As " & m_Object.PrimaryKey.VBDataType & vbCrLf)
    sb.Append("      Get" & vbCrLf)
    sb.Append("        Return " & m_Object.PrimaryKey.MemberName & vbCrLf)
    sb.Append("      End Get" & vbCrLf)
    sb.Append("    End Property" & vbCrLf & vbCrLf)
    sb.Append("    Public Sub New(ByVal " & m_Object.PrimaryKey.Name & " As " & m_Object.PrimaryKey.VBDataType & ")" & vbCrLf)
    sb.Append("      " & m_Object.PrimaryKey.MemberName & " = " & m_Object.PrimaryKey.Name & vbCrLf)
    sb.Append("    End Sub" & vbCrLf & vbCrLf)

    If m_Object.BOType = BOTypes.Switchable Then
      sb.Append("    Public Sub New()" & vbCrLf & vbCrLf)
      sb.Append("    End Sub" & vbCrLf & vbCrLf)
    End If

    sb.Append("  End Class" & vbCrLf & vbCrLf)

    If m_Object.BOType = BOTypes.Switchable Then
      sb.Append("  <Serializable()> _" & vbCrLf)
      sb.Append("  Private Class ChildCriteria" & vbCrLf & vbCrLf)
      sb.Append("  End Class" & vbCrLf & vbCrLf)
    End If

    Return sb.ToString

  End Function

  Private Function DataPortalCreate() As String

    Dim sb As StringBuilder = New StringBuilder

    If m_Object.BOType = BOTypes.EditableRoot Or m_Object.BOType = BOTypes.Switchable Then
      If m_RunLocalCreate Then
        sb.Append("  <RunLocal()> _" & vbCrLf)
      End If
    End If

    If m_Object.BOType = BOTypes.EditableRoot Or m_Object.BOType = BOTypes.Switchable Then
      sb.Append("  Private Overloads Sub DataPortal_Create(ByVal criteria As Criteria)" & vbCrLf & vbCrLf)
      If m_RunLocalCreate Then
        sb.Append("    ' TODO: load default values" & vbCrLf & vbCrLf)
      Else
        sb.Append("    ' TODO: load default values from database" & vbCrLf & vbCrLf)
      End If
      sb.Append("    ValidationRules.CheckRules()" & vbCrLf & vbCrLf)
      sb.Append("  End Sub" & vbCrLf & vbCrLf)

    ElseIf m_Object.BOType = BOTypes.EditableChild Then
      sb.Append("  Protected Overloads Sub DataPortal_Create(ByVal criteria As Criteria)" & vbCrLf & vbCrLf)
      sb.Append("    ' TODO: load default values, or remove method" & vbCrLf & vbCrLf)
      sb.Append("  End Sub" & vbCrLf & vbCrLf)
    End If

    If m_Object.BOType = BOTypes.Switchable Then
      sb.Append("  Private Overloads Sub DataPortal_Create(ByVal criteria As ChildCriteria)" & vbCrLf & vbCrLf)
      sb.Append("    MarkAsChild()" & vbCrLf)
      sb.Append("    " & m_Object.PrimaryKey.MemberName & " = " & m_MemberPrefix & "LastID - 1" & vbCrLf)
      sb.Append("    " & m_MemberPrefix & "LastID = " & m_Object.PrimaryKey.Name & vbCrLf)
      sb.Append("    ValidationRules.CheckRules()" & vbCrLf & vbCrLf)
      sb.Append("  End Sub" & vbCrLf & vbCrLf)
    End If

    Return sb.ToString

  End Function

  Private Function DataPortalFetch(Optional ByVal SwitchableAsChild As Boolean = False, Optional ByVal NVLParameters As String = "") As String

    Dim sb As StringBuilder = New StringBuilder

    If m_Object.BOType = BOTypes.EditableRoot Or m_Object.BOType = BOTypes.Switchable _
      Or m_Object.BOType = BOTypes.EditableRootCollection Or m_Object.BOType = BOTypes.ReadOnlyObject _
      Or m_Object.BOType = BOTypes.ReadOnlyCollection Or m_Object.BOType = BOTypes.NameValueList Then
      sb.Append("  Private Overloads Sub DataPortal_Fetch(ByVal criteria As Criteria)" & vbCrLf & vbCrLf)
      sb.Append("    Using cn As New SqlConnection(" & m_ConnectionString & ")" & vbCrLf)
      sb.Append("      cn.Open()" & vbCrLf)
      sb.Append("      Using cm As SqlCommand = cn.CreateCommand" & vbCrLf)
      sb.Append("        cm.CommandType = CommandType.StoredProcedure" & vbCrLf)

      If m_Object.BOType = BOTypes.NameValueList Then
        sb.Append("        cm.CommandText = """ & m_SPNVLPrefix & m_Object.Name & m_SPNVLSuffix & """" & vbCrLf)
      ElseIf m_Object.BOType = BOTypes.EditableRootCollection Or m_Object.BOType = BOTypes.ReadOnlyCollection Then
        sb.Append("        cm.CommandText = """ & m_SPListPrefix & m_Object.Name & m_SPListSuffix & """" & vbCrLf)
      Else
        sb.Append("        cm.CommandText = """ & m_SPSelectPrefix & m_Object.Name & m_SPSelectSuffix & """" & vbCrLf)
        sb.Append("        cm.Parameters.AddWithValue(""@" & m_Object.PrimaryKey.Name & """, criteria." & m_Object.PrimaryKey.Name & ")" & vbCrLf)
      End If
      sb.Append(vbCrLf)

      If m_Object.BOType = BOTypes.EditableRootCollection Or m_Object.BOType = BOTypes.ReadOnlyCollection _
        Or m_Object.BOType = BOTypes.NameValueList Then
        sb.Append("        RaiseListChangedEvents = False" & vbCrLf)
      End If

      sb.Append("        Using dr As New SafeDataReader(cm.ExecuteReader)" & vbCrLf)

    ElseIf m_Object.BOType = BOTypes.EditableChildCollection Then
      sb.Append("  Private Overloads Sub Fetch(ByVal dr As SafeDataReader)" & vbCrLf & vbCrLf)
      sb.Append("    RaiseListChangedEvents = False" & vbCrLf)
      sb.Append("    While dr.Read" & vbCrLf)
      sb.Append("      Add(" & m_ChildType & ".Get" & m_ChildType & "(dr))" & vbCrLf)
      sb.Append("    End While" & vbCrLf)
      sb.Append("    RaiseListChangedEvents = True" & vbCrLf)
      sb.Append("  End Sub" & vbCrLf & vbCrLf)

    ElseIf m_Object.BOType = BOTypes.EditableChild Or m_Object.BOType = BOTypes.ReadOnlyChild Then
      sb.Append("  Private Sub Fetch(ByVal dr As SafeDataReader)" & vbCrLf & vbCrLf)
      DataPortalFetchDoFetch(sb, 4)
      If m_Object.BOType = BOTypes.EditableChild Then
        sb.Append(vbCrLf)
        sb.Append("    MarkOld()" & vbCrLf & vbCrLf)
      End If
      sb.Append("  End Sub" & vbCrLf & vbCrLf)

    Else
      Debug.Assert(False, "Unhandled BOType")
    End If

    If m_Object.BOType = BOTypes.EditableRoot Or m_Object.BOType = BOTypes.ReadOnlyObject Then
      DataPortalFetchDoFetch(sb, 10)
    ElseIf (m_Object.BOType = BOTypes.Switchable And Not SwitchableAsChild) Then
      sb.Append("          dr.Read()" & vbCrLf)
      sb.Append("          DoFetch(dr)" & vbCrLf)
    End If

    If m_Object.BOType = BOTypes.EditableRootCollection Or m_Object.BOType = BOTypes.ReadOnlyCollection Or m_Object.BOType = BOTypes.NameValueList Then
      If m_Object.BOType = BOTypes.ReadOnlyCollection Or m_Object.BOType = BOTypes.NameValueList Then sb.Append("          IsReadOnly = False" & vbCrLf)

      sb.Append("          While dr.Read" & vbCrLf)

      If m_Object.BOType = BOTypes.NameValueList Then
        sb.Append("            Add(New NameValuePair( _" & vbCrLf)
        sb.Append("              " & NVLParameters & "))" & vbCrLf)
      Else
        If m_UseStructureForChild Then
          sb.Append("            With dr" & vbCrLf)
          sb.Append("              Dim obj As New " & m_ChildType & vbCrLf)
          For Each col As ColumnDef In m_Object.FetchColDefs
            sb.Append("              " & col.ReaderMethod.Replace(col.MemberName, "obj." & col.Name) & vbCrLf)
          Next
          sb.Append("              Add(obj)" & vbCrLf)
          sb.Append("            End With" & vbCrLf)
        Else
          sb.Append("            Add(" & m_ChildType & ".Get" & m_ChildType & "(dr))" & vbCrLf)
        End If
      End If
      sb.Append("          End While" & vbCrLf)

      If m_Object.BOType = BOTypes.ReadOnlyCollection Or m_Object.BOType = BOTypes.NameValueList Then sb.Append("          IsReadOnly = True" & vbCrLf)

      sb.Append("        End Using" & vbCrLf)
      sb.Append("        RaiseListChangedEvents = True" & vbCrLf & vbCrLf)
      sb.Append("      End Using" & vbCrLf)
      sb.Append("    End Using" & vbCrLf)
      sb.Append(vbCrLf)
      sb.Append("  End Sub" & vbCrLf & vbCrLf)
    End If

    If m_Object.BOType = BOTypes.EditableRoot Or (m_Object.BOType = BOTypes.Switchable And Not SwitchableAsChild) _
      Or m_Object.BOType = BOTypes.ReadOnlyObject Then
      sb.Append("        End Using" & vbCrLf)
      sb.Append("      End Using" & vbCrLf)
      sb.Append("    End Using" & vbCrLf)
      sb.Append(vbCrLf)
      sb.Append("  End Sub" & vbCrLf & vbCrLf)
    End If

    If (m_Object.BOType = BOTypes.Switchable And Not SwitchableAsChild) Then
      sb.Append("  Private Sub Fetch(ByVal dr As SafeDataReader)" & vbCrLf & vbCrLf)
      sb.Append("    MarkAsChild()" & vbCrLf)
      sb.Append("    DoFetch(dr)" & vbCrLf & vbCrLf)
      sb.Append("    MarkOld()" & vbCrLf)
      sb.Append("  End Sub" & vbCrLf & vbCrLf)

      sb.Append("  Private Sub DoFetch(ByVal dr As SafeDataReader)" & vbCrLf & vbCrLf)
      DataPortalFetchDoFetch(sb, 4)
      sb.Append("  End Sub" & vbCrLf & vbCrLf)
    End If

    Return sb.ToString

  End Function

  Private Sub DataPortalFetchDoFetch(ByRef sb As StringBuilder, ByVal LeadingSpaces As Integer)

    If m_Object.BOType = BOTypes.EditableRoot Or m_Object.BOType = BOTypes.ReadOnlyObject Then ' Or m_Object.BOType = BOTypes.Switchable Then
      sb.Append(Space(LeadingSpaces) & "dr.Read()" & vbCrLf)
    End If

    sb.Append(Space(LeadingSpaces) & "With dr" & vbCrLf)

    For Each col As ColumnDef In m_Object.ColDefs
      If Not col.IsWriteOnly Then
        sb.Append(Space(LeadingSpaces) & "  " & col.ReaderMethod & vbCrLf)
      End If
    Next

    If m_Object.HasChild Then
      sb.Append(vbCrLf)
      sb.Append(Space(LeadingSpaces) & "  ' load child objects" & vbCrLf)
      sb.Append(Space(LeadingSpaces) & "  .NextResult()" & vbCrLf)
      sb.Append(Space(LeadingSpaces) & "  " & m_MemberPrefix & m_ChildColType & " = " & m_ChildColType & ".Get" & m_ChildColType & "(dr)" & vbCrLf)
    End If

    sb.Append(Space(LeadingSpaces) & "End With" & vbCrLf & vbCrLf)

  End Sub

  Private Function DataPortalInsert(Optional ByVal SwitchableAsChild As Boolean = False) As String

    Dim sb As StringBuilder = New StringBuilder
    Dim indent As String = ""

    If m_Object.BOType = BOTypes.EditableRoot Or (m_Object.BOType = BOTypes.Switchable And Not SwitchableAsChild) Then
      If m_InsertTransationalType > TransactionalTypes.TransactSQL Then
        sb.Append("  <Transactional(TransactionalTypes." & m_InsertTransationalType.ToString & ")> _" & vbCrLf)
      End If

      sb.Append("  Protected Overrides Sub DataPortal_Insert()" & vbCrLf & vbCrLf)
      indent = "  "
    ElseIf m_Object.BOType = BOTypes.EditableChild Or (m_Object.BOType = BOTypes.Switchable And SwitchableAsChild) Then
      sb.Append("  Friend Sub Insert(ByVal parent As " & m_ParentType & ", ByVal cn As SqlConnection)" & vbCrLf & vbCrLf)
      sb.Append("    If Not Me.IsDirty Then Exit Sub" & vbCrLf & vbCrLf)
    Else
      Debug.Assert(False, "Unhandled BOType")
    End If

    If m_Object.BOType = BOTypes.EditableRoot Or (m_Object.BOType = BOTypes.Switchable And Not SwitchableAsChild) Then
      sb.Append("    Using cn As New SqlConnection(" & m_ConnectionString & ")" & vbCrLf)
      sb.Append("      cn.Open()" & vbCrLf)
    End If

    sb.Append(indent & "    Using cm As SqlCommand = cn.CreateCommand" & vbCrLf)
    sb.Append(indent & "      With cm" & vbCrLf)

    If m_Object.HasByteArray Then
      sb.Append(indent & "        Dim param As SqlParameter" & vbCrLf)
    End If

    sb.Append(indent & "        .CommandText = """ & m_SPInsertPrefix & m_Object.Name & m_SPInsertSuffix & """" & vbCrLf)
    sb.Append(indent & "        .CommandType = CommandType.StoredProcedure" & vbCrLf)

    If m_UseSP_ReturnValue Then
      sb.Append(indent & "        .Parameters.AddWithValue(""_ReturnValue_"", 0).Direction = ParameterDirection.ReturnValue" & vbCrLf)
    End If

    Dim blnHasOutputParam As Boolean
    For Each col As ColumnDef In m_Object.ColDefs
      If col.IsIdentity Then
        sb.Append(indent & "        .Parameters" & col.AddWithValue & ".Direction = ParameterDirection.Output" & vbCrLf)
      ElseIf col.IsTimeStamp Then
        sb.Append(indent & "        .Parameters" & col.AddWithValue & ".Direction = ParameterDirection.Output" & vbCrLf)
        blnHasOutputParam = True
      ElseIf col.IsByteArray Then
        sb.Append(indent & "        param = New SqlParameter(" & col.AddWithValue & ", SqlDbType.Binary)" & vbCrLf)
        sb.Append(indent & "        param.Value = " & col.TestAssign & "" & vbCrLf)
        sb.Append(indent & "        cm.Parameters.Add(param)" & vbCrLf)
      Else
        If SwitchableAsChild AndAlso col.IsForeignKeyMember Then
          sb.Append(indent & "        .Parameters" & ".AddWithValue(""@" & col.Name & """, parent." & col.Name & ")" & vbCrLf)
        Else
          If Not col.IsWriteOnly Then
            sb.Append(indent & "        .Parameters" & col.AddWithValue & vbCrLf)
          End If
        End If
        blnHasOutputParam = True
      End If
    Next
    sb.Append(vbCrLf)

    sb.Append(indent & "        .ExecuteNonQuery()" & vbCrLf)
    sb.Append(vbCrLf)

    If m_UseSP_ReturnValue Then
      sb.Append(indent & "        If CInt(.Parameters(0).Value) <> 0 Then" & vbCrLf)
      sb.Append(indent & "          Throw New DataException(""DataPortal_Insert returned with a non-zero value of "" & CInt(.Parameters(0).Value))" & vbCrLf)
      sb.Append(indent & "        End If" & vbCrLf & vbCrLf)
    End If

    If blnHasOutputParam Then
      For Each col As ColumnDef In m_Object.ColDefs
        If col.IsIdentity Then
          sb.Append(indent & "        " & col.MemberName & " = CInt(.Parameters(""@" & col.Name & """).Value)" & vbCrLf)
        End If
        If col.IsTimeStamp Then
          sb.Append(indent & "        " & col.MemberName & " = CType(.Parameters(""@" & col.Name & """).Value, Byte())" & vbCrLf)
        End If
      Next
      sb.Append(vbCrLf)
    End If

    sb.Append(indent & "      End With" & vbCrLf)

    sb.Append(indent & "    End Using" & vbCrLf)

    If m_Object.HasChild Then
      sb.Append(vbCrLf)
      sb.Append(indent & "    ' update child objects" & vbCrLf)
      sb.Append(indent & "    " & m_MemberPrefix & m_ChildColType & ".Update(Me, cn)" & vbCrLf & vbCrLf)
    End If

    If m_Object.BOType = BOTypes.EditableRoot Or (m_Object.BOType = BOTypes.Switchable And Not SwitchableAsChild) Then sb.Append("    End Using" & vbCrLf)

    sb.Append(vbCrLf)

    If m_Object.BOType = BOTypes.EditableChild Or (m_Object.BOType = BOTypes.Switchable And SwitchableAsChild) Then sb.Append("    MarkOld()" & vbCrLf & vbCrLf)

    sb.Append("  End Sub" & vbCrLf & vbCrLf)

    Return sb.ToString

  End Function

  Private Function DataPortalUpdate(Optional ByVal SwitchableAsChild As Boolean = False) As String

    Dim sb As StringBuilder = New StringBuilder
    Dim indent As String = ""

    If m_Object.BOType = BOTypes.EditableRoot Or (m_Object.BOType = BOTypes.Switchable And Not SwitchableAsChild) Then
      If m_UpdateTransationalType > TransactionalTypes.TransactSQL Then
        sb.Append("  <Transactional(TransactionalTypes." & m_UpdateTransationalType.ToString & ")> _" & vbCrLf)
      End If

      sb.Append("  Protected Overrides Sub DataPortal_Update()" & vbCrLf & vbCrLf)
      indent = "  "

    ElseIf m_Object.BOType = BOTypes.EditableChild Or (m_Object.BOType = BOTypes.Switchable And SwitchableAsChild) Then
      sb.Append("  Friend Sub Update(ByVal parent As " & m_ParentType & ", ByVal cn As SqlConnection)" & vbCrLf & vbCrLf)
      sb.Append("    If Me.IsDirty Then" & vbCrLf)
    End If

    If m_Object.BOType = BOTypes.EditableRoot Or (m_Object.BOType = BOTypes.Switchable And Not SwitchableAsChild) Then
      sb.Append("    Using cn As New SqlConnection(" & m_ConnectionString & ")" & vbCrLf)
      sb.Append("      cn.Open()" & vbCrLf)
      sb.Append("      If MyBase.IsDirty Then" & vbCrLf)
    End If

    sb.Append(indent & "      Using cm As SqlCommand = cn.CreateCommand" & vbCrLf)
    sb.Append(indent & "        With cm" & vbCrLf)

    If m_Object.HasByteArray Then
      sb.Append(indent & "          Dim param As SqlParameter" & vbCrLf)
    End If

    sb.Append(indent & "          .CommandText = """ & m_SPUpdatePrefix & m_Object.Name & m_SPUpdateSuffix & """" & vbCrLf)

    sb.Append(indent & "          .CommandType = CommandType.StoredProcedure" & vbCrLf)

    If m_UseSP_ReturnValue Then
      sb.Append(indent & "          .Parameters.AddWithValue(""_ReturnValue_"", 0).Direction = ParameterDirection.ReturnValue" & vbCrLf)
    End If

    Dim blnHasOutputParam As Boolean
    For Each col As ColumnDef In m_Object.ColDefs
      If col.IsTimeStamp Then
        sb.Append(indent & "          .Parameters" & col.AddWithValue & ".Direction = ParameterDirection.InputOutput" & vbCrLf)
        blnHasOutputParam = True
      ElseIf col.IsByteArray Then
        sb.Append(indent & "          param = New SqlParameter(" & col.AddWithValue & ", SqlDbType.Binary)" & vbCrLf)
        sb.Append(indent & "          param.Value = " & col.TestAssign & "" & vbCrLf)
        sb.Append(indent & "          cm.Parameters.Add(param)" & vbCrLf)
      Else
        If SwitchableAsChild AndAlso col.IsForeignKeyMember Then
          sb.Append(indent & "        .Parameters" & ".AddWithValue(""@" & col.Name & """, parent." & col.Name & ")" & vbCrLf)
        Else
          If Not col.IsWriteOnly Then
            sb.Append(indent & "          .Parameters" & col.AddWithValue & vbCrLf)
          End If
        End If
        blnHasOutputParam = True
      End If
    Next
    sb.Append(vbCrLf)

    sb.Append(indent & "          .ExecuteNonQuery()" & vbCrLf)
    sb.Append(vbCrLf)

    If m_UseSP_ReturnValue Then
      sb.Append(indent & "          If CInt(.Parameters(0).Value) <> 0 Then" & vbCrLf)
      sb.Append(indent & "            Throw New DataException(""DataPortal_Update returned with a non-zero value of "" & CInt(.Parameters(0).Value))" & vbCrLf)
      sb.Append(indent & "          End If" & vbCrLf)
    End If

    If blnHasOutputParam Then
      For Each col As ColumnDef In m_Object.ColDefs
        If (col.IsTimeStamp) Then
          sb.Append(indent & "          " & col.MemberName & " = CType(.Parameters(""@" & col.Name & """).Value, Byte())" & vbCrLf)
        End If
      Next
      sb.Append(vbCrLf)
    End If

    sb.Append(indent & "        End With" & vbCrLf)
    sb.Append(indent & "      End Using" & vbCrLf)

    If m_Object.BOType = BOTypes.EditableRoot Or (m_Object.BOType = BOTypes.Switchable And Not SwitchableAsChild) Then
      sb.Append("      End If" & vbCrLf)
    End If

    If m_Object.HasChild Then
      sb.Append(vbCrLf)
      sb.Append(indent & "    ' update child objects" & vbCrLf)
      sb.Append(indent & "    " & m_MemberPrefix & m_ChildColType & ".Update(Me, cn)" & vbCrLf & vbCrLf)
    End If

    If m_Object.BOType = BOTypes.EditableRoot Or (m_Object.BOType = BOTypes.Switchable And Not SwitchableAsChild) Then
      sb.Append("    End Using" & vbCrLf)
    Else
      sb.Append("    End If" & vbCrLf)
    End If

    sb.Append(vbCrLf)

    If m_Object.BOType = BOTypes.EditableChild Or (m_Object.BOType = BOTypes.Switchable And SwitchableAsChild) Then
      sb.Append("    MarkOld()" & vbCrLf & vbCrLf)
    End If

    sb.Append("  End Sub" & vbCrLf & vbCrLf)

    Return sb.ToString

  End Function

  Private Function DataAccess_Update() As String

    Dim sb As StringBuilder = New StringBuilder
    Dim indent As String = ""

    If m_Object.BOType = BOTypes.EditableRootCollection Then
      sb.Append("  Protected Overrides Sub DataPortal_Update()" & vbCrLf & vbCrLf)
      sb.Append("    Using cn As New SqlConnection(" & m_ConnectionString & ")" & vbCrLf)
      sb.Append("      cn.Open()" & vbCrLf & vbCrLf)
      indent = "  "
    ElseIf m_Object.BOType = BOTypes.EditableChildCollection Then
      sb.Append("  Friend Sub Update(ByVal parent As " & m_ParentType & ", ByVal cn As SqlConnection)" & vbCrLf & vbCrLf)
    End If

    Dim strTarget As String = ""
    If m_Object.BOType = BOTypes.EditableRootCollection Then
      strTarget = "Me, cn"
    ElseIf m_Object.BOType = BOTypes.EditableChildCollection Then
      strTarget = "parent, cn"
    End If

    sb.Append(indent & "    RaiseListChangedEvents = False" & vbCrLf)
    sb.Append(indent & "    For Each item As " & m_ChildType & " In DeletedList" & vbCrLf)
    sb.Append(indent & "      item.DeleteSelf(cn)" & vbCrLf)
    sb.Append(indent & "    Next" & vbCrLf)
    sb.Append(indent & "    DeletedList.Clear()" & vbCrLf & vbCrLf)

    sb.Append(indent & "    For Each item As " & m_ChildType & " In Me" & vbCrLf)
    sb.Append(indent & "      If item.IsNew Then" & vbCrLf)
    sb.Append(indent & "        item.Insert(" & strTarget & ")" & vbCrLf)
    sb.Append(indent & "      Else" & vbCrLf)
    sb.Append(indent & "        item.Update(" & strTarget & ")" & vbCrLf)
    sb.Append(indent & "      End If" & vbCrLf)
    sb.Append(indent & "    Next" & vbCrLf)
    sb.Append(indent & "    RaiseListChangedEvents = True" & vbCrLf & vbCrLf)

    If m_Object.BOType = BOTypes.EditableRootCollection Then
      sb.Append("    End Using" & vbCrLf & vbCrLf)
    End If

    sb.Append("  End Sub" & vbCrLf & vbCrLf)

    Return sb.ToString

  End Function

  Private Function DataPortalDeleteSelf() As String

    Dim sb As StringBuilder = New StringBuilder

    If m_DeleteTransationalType > TransactionalTypes.TransactSQL Then
      sb.Append("  <Transactional(TransactionalTypes." & m_DeleteTransationalType.ToString & ")> _" & vbCrLf)
    End If

    sb.Append("  Protected Overrides Sub DataPortal_DeleteSelf()" & vbCrLf)
    sb.Append(vbCrLf)
    sb.Append("    DataPortal_Delete(New Criteria(" & m_Object.PrimaryKey.MemberName & "))" & vbCrLf)
    sb.Append(vbCrLf)
    sb.Append("  End Sub" & vbCrLf & vbCrLf)

    Return sb.ToString

  End Function

  Private Function DataPortalDelete(Optional ByVal SwitchableAsChild As Boolean = False) As String

    Dim sb As StringBuilder = New StringBuilder
    Dim indent As String = ""

    If m_Object.BOType = BOTypes.EditableRoot Or (m_Object.BOType = BOTypes.Switchable And Not SwitchableAsChild) Then
      If m_DeleteTransationalType > TransactionalTypes.TransactSQL Then
        sb.Append("  <Transactional(TransactionalTypes." & m_DeleteTransationalType.ToString & ")> _" & vbCrLf)
      End If

      sb.Append("  Private Overloads Sub DataPortal_Delete(ByVal criteria As Criteria)" & vbCrLf & vbCrLf)
      indent = "  "

    ElseIf m_Object.BOType = BOTypes.EditableChild Or (m_Object.BOType = BOTypes.Switchable And SwitchableAsChild) Then
      sb.Append("  Friend Sub DeleteSelf(ByVal cn As SqlConnection)" & vbCrLf & vbCrLf)
    End If


    If m_Object.BOType = BOTypes.EditableRoot Or (m_Object.BOType = BOTypes.Switchable And Not SwitchableAsChild) Then
      sb.Append("    Using cn As New SqlConnection(" & m_ConnectionString & ")" & vbCrLf)
      sb.Append("      cn.Open()" & vbCrLf)
    End If

    sb.Append(indent & "    Using cm As SqlCommand = cn.CreateCommand" & vbCrLf)
    sb.Append(indent & "      With cm" & vbCrLf)
    sb.Append(indent & "        .Connection = cn" & vbCrLf)
    sb.Append(indent & "        .CommandType = CommandType.StoredProcedure" & vbCrLf)
    sb.Append(indent & "        .CommandText = """ & m_SPDeletePrefix & m_Object.Name & m_SPDeleteSuffix & """" & vbCrLf)

    If m_UseSP_ReturnValue Then
      sb.Append(indent & "        .Parameters.AddWithValue(""_ReturnValue_"", 0).Direction = ParameterDirection.ReturnValue" & vbCrLf)
    End If

    If m_Object.BOType = BOTypes.EditableChild Or (m_Object.BOType = BOTypes.Switchable And SwitchableAsChild) Then
      sb.Append(indent & "        .Parameters.AddWithValue(""@" & m_Object.PrimaryKey.Name & """, " & m_MemberPrefix & m_Object.PrimaryKey.Name & ")" & vbCrLf)
    Else
      sb.Append(indent & "        .Parameters.AddWithValue(""@" & m_Object.PrimaryKey.Name & """, criteria." & m_Object.PrimaryKey.Name & ")" & vbCrLf)
    End If
    sb.Append(indent & "        .ExecuteNonQuery()" & vbCrLf)

    If m_UseSP_ReturnValue Then
      sb.Append(indent & "        If CInt(.Parameters(0).Value) <> 0 Then" & vbCrLf)
      sb.Append(indent & "          Throw New DataException(""DataPortal_Update returned with a non-zero value of "" & CInt(.Parameters(0).Value))" & vbCrLf)
      sb.Append(indent & "        End If" & vbCrLf & vbCrLf)
    End If

    sb.Append(indent & "      End With" & vbCrLf)
    sb.Append(indent & "    End Using" & vbCrLf)

    If m_Object.BOType = BOTypes.EditableRoot Or (m_Object.BOType = BOTypes.Switchable And Not SwitchableAsChild) Then
      sb.Append("    End Using" & vbCrLf & vbCrLf)
    Else
      sb.Append(vbCrLf)
    End If

    If m_Object.BOType = BOTypes.EditableChild Or (m_Object.BOType = BOTypes.Switchable And SwitchableAsChild) Then sb.Append("    MarkNew()" & vbCrLf & vbCrLf)

    sb.Append("  End Sub" & vbCrLf & vbCrLf)

    Return sb.ToString

  End Function

  Private Function DataPortalInvoke() As String

    Dim sb As StringBuilder = New StringBuilder

    If m_ExecutionTime Then
      sb.Append("  Private Overloads Sub DataPortal_OnDataPortalInvoke(ByVal e As Csla.DataPortalEventArgs)" & vbCrLf)
      sb.Append("    " & m_MemberPrefix & "InitTime = DateTime.Now" & vbCrLf)
      sb.Append("  End Sub" & vbCrLf & vbCrLf)

      sb.Append("  Private Overloads Sub DataPortal_OnDataPortalInvokeComplete(ByVal e As Csla.DataPortalEventArgs)" & vbCrLf)
      sb.Append("    " & m_MemberPrefix & "ExecTime = DateTime.Now.Subtract(m_InitTime)" & vbCrLf)
      sb.Append("  End Sub" & vbCrLf & vbCrLf)
    End If

    Return sb.ToString

  End Function

#End Region ' Data Access Region

#Region " Stored Procedures "

  Private Function BuildStoredProcedures() As String

    Dim sb As StringBuilder = New StringBuilder

    If m_DataBaseName.Length > 0 Then
      sb.Append("USE " & m_DataBaseName & vbCrLf)
      sb.Append("GO" & vbCrLf)
    End If

    Select Case m_Object.BOType

      Case BOTypes.EditableRoot, BOTypes.Switchable
        sb.Append(SProc_Select)
        sb.Append(SProc_Insert)
        sb.Append(SProc_Update)
        sb.Append(SProc_Delete)
        sb.Append(SProc_Exists)

      Case BOTypes.EditableChild
        sb.Append(SProc_Insert)
        sb.Append(SProc_Update)
        sb.Append(SProc_Delete)
        sb.Append(SProc_Exists)

      Case BOTypes.EditableRootCollection
        sb.Append(SProc_List)

      Case BOTypes.ReadOnlyObject
        sb.Append(SProc_Select)
        sb.Append(SProc_Exists)

      Case BOTypes.ReadOnlyCollection
        sb.Append(SProc_List)

      Case BOTypes.NameValueList
        sb.Append(SProc_NVL)

      Case BOTypes.CommandObject
        Debug.Assert(False, "No sproc")

      Case Else
        Debug.Assert(False, "Unhandled BOType")

    End Select

    If m_OutputToFile Then OutputGeneratedObject(sb, m_Object.Name & "_SProcs.SQL")

    Return sb.ToString

  End Function

  Private Function SProc_Select() As String

    Dim sb As StringBuilder = New StringBuilder
    Dim dbo As String = m_DatabaseObjectOwner

    If dbo.Length = 0 Then dbo = "dbo."
    If Not dbo.EndsWith(".") Then dbo = dbo & "."

    Dim SProc As String = dbo & m_SPSelectPrefix & m_Object.Name & m_SPSelectSuffix

    If m_SProc_GenerateDROP Then
      sb.Append(vbCrLf)
      sb.Append("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'" & SProc & "') AND type in (N'P', N'PC'))" & vbCrLf)
      sb.Append("DROP PROCEDURE " & SProc & vbCrLf)
      sb.Append("GO" & vbCrLf)
    End If

    sb.Append("CREATE PROCEDURE " & SProc & vbCrLf)
    sb.Append(vbCrLf)

    sb.Append("    " & m_Object.PrimaryKey.SQLParam & vbCrLf & vbCrLf)

    If m_AddComments Then
      sb.Append(GetComments(CommentTypes.SProc, SProc) & vbCrLf)
    End If

    sb.Append("AS" & vbCrLf & vbCrLf)

    sb.Append("    SELECT" & vbCrLf)

    GetSelectColumns(m_Object.FetchColDefs, sb, 8)

    sb.Append(vbCrLf)
    sb.Append("    FROM " & dbo & m_Object.FetchDataSource & vbCrLf & vbCrLf)

    sb.Append("    WHERE " & m_Object.PrimaryKey.Name & " = @" & m_Object.PrimaryKey.Name & vbCrLf)

    If m_Object.HasChild Then
      sb.Append(vbCrLf)
      sb.Append("    -- Get child resultset" & vbCrLf)

      sb.Append("    SELECT" & vbCrLf)

      GetSelectColumns(m_Child.ColDefs, sb, 8)

      sb.Append(vbCrLf)
      sb.Append("    FROM " & dbo & m_Child.DataSource & vbCrLf & vbCrLf)

      Dim strFK As String = IIf(m_FK_to_Parent.Length = 0, m_Object.PrimaryKey.Name, m_FK_to_Parent).ToString
      sb.Append("    WHERE " & strFK & " = @" & m_Object.PrimaryKey.Name & vbCrLf)

    End If


    sb.Append("GO" & vbCrLf)

    sb.Append(GrantExecute(SProc))

    Return sb.ToString

  End Function

  Private Function SProc_Insert() As String

    Dim sb As StringBuilder = New StringBuilder
    Dim dbo As String = m_DatabaseObjectOwner
    Dim blnUseTrans As Boolean

    blnUseTrans = m_InsertTransationalType = TransactionalTypes.TransactSQL

    If dbo.Length = 0 Then dbo = "dbo."
    If Not dbo.EndsWith(".") Then dbo = dbo & "."

    Dim SProc As String = dbo & m_SPInsertPrefix & m_Object.Name & m_SPInsertSuffix

    If m_SProc_GenerateDROP Then
      sb.Append(vbCrLf)
      sb.Append("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'" & SProc & "') AND type in (N'P', N'PC'))" & vbCrLf)
      sb.Append("DROP PROCEDURE " & SProc & vbCrLf)
      sb.Append("GO" & vbCrLf)
    End If

    sb.Append("CREATE PROCEDURE " & SProc & vbCrLf)
    sb.Append(vbCrLf)
    GetInsertColumns(sb, 4, 1)
    sb.Append(vbCrLf)

    If m_AddComments Then
      sb.Append(GetComments(CommentTypes.SProc, SProc) & vbCrLf)
    End If

    sb.Append("AS" & vbCrLf)
    sb.Append(vbCrLf)

    If blnUseTrans Then sb.Append("  BEGIN TRAN" & vbCrLf & vbCrLf)
    sb.Append("    DECLARE @Error_Stat int, @Row_Count int" & vbCrLf & vbCrLf)

    sb.Append("    INSERT INTO " & dbo & m_Object.DataSource & vbCrLf)

    sb.Append("      (" & vbCrLf)
    GetInsertColumns(sb, 8, 2)
    sb.Append("      )" & vbCrLf)

    sb.Append("    VALUES" & vbCrLf)
    sb.Append("      (" & vbCrLf)
    GetInsertColumns(sb, 8, 3)
    sb.Append("      )" & vbCrLf)

    sb.Append(vbCrLf)
    sb.Append("    -- Store system function results to local vars" & vbCrLf)
    sb.Append("    SELECT @Error_Stat = @@ERROR, @Row_Count = @@ROWCOUNT" & vbCrLf)
    sb.Append(vbCrLf)

    sb.Append("    IF @Row_Count > 1" & vbCrLf)
    sb.Append("    BEGIN" & vbCrLf)
    If blnUseTrans Then sb.Append("      IF @@TRANCOUNT <> 0 ROLLBACK TRAN" & vbCrLf)
    sb.Append("      RAISERROR('Unexpected rowcount', 16, 1)" & vbCrLf)
    sb.Append("      RETURN -999" & vbCrLf)
    sb.Append("    END" & vbCrLf)
    sb.Append(vbCrLf)

    sb.Append("    IF @Error_Stat <> 0" & vbCrLf)
    sb.Append("    BEGIN" & vbCrLf)
    If blnUseTrans Then sb.Append("      IF @@TRANCOUNT <> 0 ROLLBACK TRAN" & vbCrLf)
    sb.Append("      RETURN @Error_Stat	" & vbCrLf)
    sb.Append("    END" & vbCrLf)
    sb.Append(vbCrLf)

    If m_Object.HasIdentityField And m_Object.HasTimeStampField Then
      sb.Append("    SET @" & m_Object.PrimaryKey.Name & " = SCOPE_IDENTITY()" & vbCrLf)
      sb.Append(vbCrLf)
      sb.Append("    SELECT @" & m_Object.TimeStamp & " = " & m_Object.TimeStamp & vbCrLf)
      sb.Append("    FROM " & dbo & m_Object.DataSource & vbCrLf)
      sb.Append("    WHERE " & m_Object.PrimaryKey.Name & " = @" & m_Object.PrimaryKey.Name & vbCrLf)

    ElseIf m_Object.HasIdentityField And Not m_Object.HasTimeStampField Then
      sb.Append(vbCrLf)
      sb.Append("    SET @" & m_Object.PrimaryKey.Name & " = SCOPE_IDENTITY()" & vbCrLf)

    ElseIf Not m_Object.HasIdentityField And m_Object.HasTimeStampField Then
      sb.Append(vbCrLf)
      sb.Append("    SELECT @" & m_Object.TimeStamp & " = " & m_Object.TimeStamp & vbCrLf)
      sb.Append("    FROM " & dbo & m_Object.DataSource & vbCrLf)
      sb.Append("    WHERE " & m_Object.PrimaryKey.Name & " = @" & m_Object.PrimaryKey.Name & vbCrLf)

    End If
    sb.Append(vbCrLf)

    If blnUseTrans Then sb.Append("  COMMIT TRAN" & vbCrLf & vbCrLf)
    sb.Append("RETURN 0" & vbCrLf)

    sb.Append(vbCrLf)
    sb.Append("GO" & vbCrLf)

    sb.Append(GrantExecute(SProc))

    Return sb.ToString

  End Function

  Private Function SProc_Update() As String

    Dim sb As StringBuilder = New StringBuilder
    Dim dbo As String = m_DatabaseObjectOwner
    Dim blnUseTrans As Boolean

    blnUseTrans = m_UpdateTransationalType = TransactionalTypes.TransactSQL

    If dbo.Length = 0 Then dbo = "dbo."
    If Not dbo.EndsWith(".") Then dbo = dbo & "."

    Dim SProc As String = dbo & m_SPUpdatePrefix & m_Object.Name & m_SPUpdateSuffix

    If m_SProc_GenerateDROP Then
      sb.Append(vbCrLf)
      sb.Append("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'" & SProc & "') AND type in (N'P', N'PC'))" & vbCrLf)
      sb.Append("DROP PROCEDURE " & SProc & vbCrLf)
      sb.Append("GO" & vbCrLf)
    End If

    sb.Append("CREATE PROCEDURE " & SProc & vbCrLf)
    sb.Append(vbCrLf)
    GetUpdateColumns(sb, 4, 1)
    sb.Append(vbCrLf)

    If m_AddComments Then
      sb.Append(GetComments(CommentTypes.SProc, SProc) & vbCrLf)
    End If

    sb.Append("AS" & vbCrLf)
    sb.Append(vbCrLf)

    If blnUseTrans Then sb.Append("  BEGIN TRAN" & vbCrLf & vbCrLf)
    sb.Append("    DECLARE @Error_Stat int, @Row_Count int" & vbCrLf & vbCrLf)

    sb.Append("    UPDATE " & dbo & m_Object.DataSource & vbCrLf)
    sb.Append(vbCrLf)

    sb.Append("    SET" & vbCrLf)
    GetUpdateColumns(sb, 8, 2)
    sb.Append(vbCrLf)

    sb.Append("    WHERE " & m_Object.PrimaryKey.Name & " = @" & m_Object.PrimaryKey.Name & vbCrLf)

    If m_Object.HasTimeStampField Then
      sb.Append("      AND " & m_Object.TimeStamp & " = @" & m_Object.TimeStamp & vbCrLf)
    End If

    sb.Append(vbCrLf)
    sb.Append("    -- Store system function results to local vars" & vbCrLf)
    sb.Append("    SELECT @Error_Stat = @@ERROR, @Row_Count = @@ROWCOUNT" & vbCrLf)
    sb.Append(vbCrLf)

    sb.Append("    IF @Row_Count = 0" & vbCrLf)
    sb.Append("    BEGIN" & vbCrLf)
    If m_Object.HasTimeStampField Then
      sb.Append("      IF EXISTS(SELECT * FROM " & dbo & m_Object.DataSource & vbCrLf)
      sb.Append("                WHERE " & m_Object.PrimaryKey.Name & " = @" & m_Object.PrimaryKey.Name & ")" & vbCrLf)
      sb.Append("      BEGIN" & vbCrLf)
      If blnUseTrans Then sb.Append("        IF @@TRANCOUNT <> 0 ROLLBACK TRAN" & vbCrLf)
      sb.Append("          RAISERROR(" & m_ConcurrencyErrMsg & ", 16, 1)" & vbCrLf)
      sb.Append("          RETURN -2" & vbCrLf)
      sb.Append("      END ELSE BEGIN" & vbCrLf)
      If blnUseTrans Then sb.Append("        IF @@TRANCOUNT <> 0 ROLLBACK TRAN" & vbCrLf)
      sb.Append("        RAISERROR('Row does not exist', 16, 1)" & vbCrLf)
      sb.Append("        RETURN -1" & vbCrLf)
      sb.Append("      END" & vbCrLf)
    Else
      If blnUseTrans Then sb.Append("        IF @@TRANCOUNT <> 0 ROLLBACK TRAN" & vbCrLf)
      sb.Append("        RAISERROR('Row does not exist', 16, 1)" & vbCrLf)
      sb.Append("        RETURN -1" & vbCrLf)
    End If
    sb.Append("    END" & vbCrLf)
    sb.Append(vbCrLf)

    sb.Append("    IF @Row_Count > 1" & vbCrLf)
    sb.Append("    BEGIN" & vbCrLf)
    If blnUseTrans Then sb.Append("      IF @@TRANCOUNT <> 0 ROLLBACK TRAN" & vbCrLf)
    sb.Append("      RAISERROR('Unexpected rowcount', 16, 1)" & vbCrLf)
    sb.Append("      RETURN -999" & vbCrLf)
    sb.Append("    END" & vbCrLf)
    sb.Append(vbCrLf)

    sb.Append("    IF @Error_Stat <> 0" & vbCrLf)
    sb.Append("    BEGIN" & vbCrLf)
    If blnUseTrans Then sb.Append("      IF @@TRANCOUNT <> 0 ROLLBACK TRAN" & vbCrLf)
    sb.Append("      RETURN @Error_Stat	" & vbCrLf)
    sb.Append("    END" & vbCrLf)
    sb.Append(vbCrLf)

    ' Refresh timestamp
    If m_Object.HasTimeStampField Then
      sb.Append("    SELECT @" & m_Object.TimeStamp & " = " & m_Object.TimeStamp & vbCrLf)
      sb.Append("    FROM " & dbo & m_Object.DataSource & vbCrLf)
      sb.Append("    WHERE " & m_Object.PrimaryKey.Name & " = @" & m_Object.PrimaryKey.Name & vbCrLf)
      sb.Append(vbCrLf)
    End If

    If blnUseTrans Then sb.Append("  COMMIT TRAN" & vbCrLf & vbCrLf)
    sb.Append("RETURN 0" & vbCrLf)

    sb.Append(vbCrLf)
    sb.Append("GO" & vbCrLf)

    sb.Append(GrantExecute(SProc))

    Return sb.ToString

  End Function

  Private Function SProc_Delete() As String

    Dim sb As StringBuilder = New StringBuilder
    Dim dbo As String = m_DatabaseObjectOwner
    Dim blnUseTrans As Boolean

    blnUseTrans = m_DeleteTransationalType = TransactionalTypes.TransactSQL

    If dbo.Length = 0 Then dbo = "dbo."
    If Not dbo.EndsWith(".") Then dbo = dbo & "."

    Dim SProc As String = dbo & m_SPDeletePrefix & m_Object.Name & m_SPDeleteSuffix

    If m_SProc_GenerateDROP Then
      sb.Append(vbCrLf)
      sb.Append("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'" & SProc & "') AND type in (N'P', N'PC'))" & vbCrLf)
      sb.Append("DROP PROCEDURE " & SProc & vbCrLf)
      sb.Append("GO" & vbCrLf)
    End If

    sb.Append("CREATE PROCEDURE " & SProc & vbCrLf)
    sb.Append(vbCrLf)

    sb.Append("    " & m_Object.PrimaryKey.SQLParam & vbCrLf)

    sb.Append(vbCrLf)
    sb.Append("AS" & vbCrLf)
    sb.Append(vbCrLf)

    If m_AddComments Then
      sb.Append(GetComments(CommentTypes.SProc, SProc) & vbCrLf)
    End If

    If blnUseTrans Then sb.Append("  BEGIN TRAN" & vbCrLf & vbCrLf)
    sb.Append("    DECLARE @Error_Stat int, @Row_Count int" & vbCrLf & vbCrLf)

    sb.Append("    DELETE " & dbo & m_Object.DataSource & vbCrLf)
    sb.Append(vbCrLf)

    sb.Append("    WHERE " & m_Object.PrimaryKey.Name & " = @" & m_Object.PrimaryKey.Name & vbCrLf)
    sb.Append(vbCrLf)

    sb.Append("    -- Store system function results to local vars" & vbCrLf)
    sb.Append("    SELECT @Error_Stat = @@ERROR, @Row_Count = @@ROWCOUNT" & vbCrLf)
    sb.Append(vbCrLf)

    sb.Append("    IF @Row_Count > 1" & vbCrLf)
    sb.Append("    BEGIN" & vbCrLf)
    If blnUseTrans Then sb.Append("      IF @@TRANCOUNT <> 0 ROLLBACK TRAN" & vbCrLf)
    sb.Append("      RAISERROR('Unexpected rowcount', 16, 1)" & vbCrLf)
    sb.Append("      RETURN -999" & vbCrLf)
    sb.Append("    END" & vbCrLf)
    sb.Append(vbCrLf)

    sb.Append("    IF @Error_Stat <> 0" & vbCrLf)
    sb.Append("    BEGIN" & vbCrLf)
    If blnUseTrans Then sb.Append("      IF @@TRANCOUNT <> 0 ROLLBACK TRAN" & vbCrLf)
    sb.Append("      RETURN @Error_Stat	" & vbCrLf)
    sb.Append("    END" & vbCrLf)
    sb.Append(vbCrLf)

    If blnUseTrans Then sb.Append("  COMMIT TRAN" & vbCrLf & vbCrLf)
    sb.Append("RETURN 0" & vbCrLf)

    sb.Append("GO" & vbCrLf)

    sb.Append(GrantExecute(SProc))

    Return sb.ToString

  End Function

  Private Function SProc_List() As String

    Dim sb As StringBuilder = New StringBuilder

    Dim dbo As String = m_DatabaseObjectOwner

    If dbo.Length = 0 Then dbo = "dbo."
    If Not dbo.EndsWith(".") Then dbo = dbo & "."

    Dim SProc As String = dbo & m_SPListPrefix & m_Object.Name & m_SPListSuffix

    If m_SProc_GenerateDROP Then
      sb.Append(vbCrLf)
      sb.Append("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'" & SProc & "') AND type in (N'P', N'PC'))" & vbCrLf)
      sb.Append("DROP PROCEDURE " & SProc & vbCrLf)
      sb.Append("GO" & vbCrLf)
    End If

    sb.Append("CREATE PROCEDURE " & SProc & vbCrLf)
    sb.Append(vbCrLf)
    sb.Append("AS" & vbCrLf)
    sb.Append(vbCrLf)

    If m_AddComments Then
      sb.Append(GetComments(CommentTypes.SProc, SProc) & vbCrLf)
    End If

    sb.Append("    SELECT" & vbCrLf)

    GetSelectColumns(m_Object.ColDefs, sb, 8)

    sb.Append(vbCrLf)
    sb.Append("    FROM " & dbo & m_Object.DataSource & vbCrLf)
    sb.Append(vbCrLf)

    sb.Append("GO" & vbCrLf)

    sb.Append(GrantExecute(SProc))

    Return sb.ToString

  End Function

  Private Function SProc_NVL() As String

    Dim sb As StringBuilder = New StringBuilder

    Dim dbo As String = m_DatabaseObjectOwner

    If dbo.Length = 0 Then dbo = "dbo."
    If Not dbo.EndsWith(".") Then dbo = dbo & "."

    Dim SProc As String = dbo & m_SPNVLPrefix & m_Object.Name & m_SPNVLSuffix

    If m_SProc_GenerateDROP Then
      sb.Append(vbCrLf)
      sb.Append("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'" & SProc & "') AND type in (N'P', N'PC'))" & vbCrLf)
      sb.Append("DROP PROCEDURE " & SProc & vbCrLf)
      sb.Append("GO" & vbCrLf)
    End If

    sb.Append("CREATE PROCEDURE " & SProc & vbCrLf)
    sb.Append(vbCrLf)
    sb.Append("AS" & vbCrLf)
    sb.Append(vbCrLf)

    If m_AddComments Then
      sb.Append(GetComments(CommentTypes.SProc, SProc) & vbCrLf)
    End If

    sb.Append("    SELECT" & vbCrLf)
    sb.Append("       " & m_Object.ColDefs(0).Name & "," & vbCrLf)
    sb.Append("       " & m_NVL_NameField & vbCrLf)

    sb.Append(vbCrLf)
    sb.Append("    FROM " & dbo & m_Object.DataSource & vbCrLf)
    sb.Append(vbCrLf)

    sb.Append("GO" & vbCrLf)

    sb.Append(GrantExecute(SProc))

    Return sb.ToString

  End Function

  Private Function SProc_Exists() As String

    Dim sb As StringBuilder = New StringBuilder

    If m_Implement_Exists And m_Object.BOType = BOTypes.EditableRoot OrElse _
      m_Object.BOType = BOTypes.Switchable OrElse _
      m_Object.BOType = BOTypes.ReadOnlyObject Then
      Dim dbo As String = m_DatabaseObjectOwner

      If dbo.Length = 0 Then dbo = "dbo."
      If Not dbo.EndsWith(".") Then dbo = dbo & "."

      Dim SProc As String = dbo & m_SPExistsPrefix & m_Object.Name & m_SPExistsSuffix

      If m_SProc_GenerateDROP Then
        sb.Append(vbCrLf)
        sb.Append("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'" & SProc & "') AND type in (N'P', N'PC'))" & vbCrLf)
        sb.Append("DROP PROCEDURE " & SProc & vbCrLf)
        sb.Append("GO" & vbCrLf)
      End If

      sb.Append("CREATE PROCEDURE " & SProc & vbCrLf)
      sb.Append(vbCrLf)

      sb.Append("    " & m_Object.PrimaryKey.SQLParam & vbCrLf)
      sb.Append(vbCrLf)

      If m_AddComments Then
        sb.Append(GetComments(CommentTypes.SProc, SProc) & vbCrLf)
      End If

      sb.Append("AS" & vbCrLf)
      sb.Append(vbCrLf)

      sb.Append("    SELECT COUNT(*)" & vbCrLf)
      sb.Append(vbCrLf)

      sb.Append("    FROM " & dbo & m_Object.DataSource & vbCrLf)
      sb.Append(vbCrLf)

      sb.Append("    WHERE " & m_Object.PrimaryKey.Name & " = @" & m_Object.PrimaryKey.Name & vbCrLf)
      sb.Append(vbCrLf)

      sb.Append("GO" & vbCrLf)

      sb.Append(GrantExecute(SProc))
    End If

    Return sb.ToString

  End Function

  Private Sub GetSelectColumns(ByVal Columns() As ColumnDef, ByRef sb As StringBuilder, ByVal LeadingSpaces As Integer)

    Dim cols(0) As String
    Dim i As Integer, j As Integer

    Dim column As ColumnDef
    For i = 0 To UBound(Columns)
      column = Columns(i)
      If Not column.IsWriteOnly Then
        ReDim Preserve cols(j)
        cols(j) = Space(LeadingSpaces) & column.Name
      End If
      j += 1
    Next

    For i = 0 To UBound(cols) - 1
      sb.Append(cols(i) & "," & vbCrLf)
    Next
    sb.Append(cols(UBound(cols)) & vbCrLf)

  End Sub

  Private Sub GetInsertColumns(ByRef sb As StringBuilder, ByVal LeadingSpaces As Integer, ByVal Part As Integer)

    Dim cols(0) As String
    Dim i As Integer, j As Integer

    If Part = 1 Then ' Parameters
      Dim column As ColumnDef
      For i = 0 To UBound(m_Object.ColDefs)
        column = m_Object.ColDefs(i)
        If Not column.IsWriteOnly Then
          ReDim Preserve cols(j)
          If column.IsIdentity Or column.IsTimeStamp Then
            cols(j) = Space(LeadingSpaces) & column.SQLParam & " OUTPUT"
          Else
            cols(j) = Space(LeadingSpaces) & column.SQLParam
          End If
          j += 1
        End If
      Next

    ElseIf Part = 2 Then ' Named columns
      Dim column As ColumnDef
      For i = 0 To UBound(m_Object.ColDefs)
        column = m_Object.ColDefs(i)
        If Not column.IsWriteOnly Then
          If Not column.IsIdentity AndAlso Not column.IsTimeStamp Then
            ReDim Preserve cols(j)
            cols(j) = Space(LeadingSpaces) & column.Name
            j += 1
          End If
        End If
      Next

    ElseIf Part = 3 Then ' Values
      Dim column As ColumnDef
      For i = 0 To UBound(m_Object.ColDefs)
        column = m_Object.ColDefs(i)
        If Not column.IsWriteOnly Then
          If Not column.IsIdentity AndAlso Not column.IsTimeStamp Then
            ReDim Preserve cols(j)
            cols(j) = Space(LeadingSpaces) & "@" & column.Name
            j += 1
          End If
        End If
      Next
    End If

    For i = 0 To UBound(cols) - 1
      sb.Append(cols(i) & "," & vbCrLf)
    Next
    sb.Append(cols(UBound(cols)) & vbCrLf)

  End Sub

  Private Sub GetUpdateColumns(ByRef sb As StringBuilder, ByVal LeadingSpaces As Integer, ByVal Part As Integer)

    Dim cols(0) As String
    Dim i As Integer, j As Integer

    If Part = 1 Then ' Parameters
      Dim column As ColumnDef
      For i = 0 To UBound(m_Object.ColDefs)
        column = m_Object.ColDefs(i)
        If Not column.IsWriteOnly Then
          ReDim Preserve cols(j)
          If column.IsIdentity Or column.IsTimeStamp Then
            cols(j) = Space(LeadingSpaces) & column.SQLParam & " OUTPUT"
          Else
            cols(j) = Space(LeadingSpaces) & column.SQLParam
          End If
          j += 1
        End If
      Next

    ElseIf Part = 2 Then ' Mapping
      Dim column As ColumnDef
      For i = 0 To UBound(m_Object.ColDefs)
        column = m_Object.ColDefs(i)
        If Not column.IsIdentity AndAlso Not column.IsTimeStamp AndAlso Not column.IsWriteOnly Then
          ReDim Preserve cols(j)
          cols(j) = Space(LeadingSpaces) & column.Name & " = @" & column.Name
          j += 1
        End If
      Next

    End If

    For i = 0 To UBound(cols) - 1
      sb.Append(cols(i) & "," & vbCrLf)
    Next
    sb.Append(cols(UBound(cols)) & vbCrLf)

  End Sub

  Private Function GrantExecute(ByVal SProc As String) As String

    Dim sb As StringBuilder = New StringBuilder
    Dim a() As String = Split(m_GrantExecute, ",")
    Dim i As Integer

    For i = 0 To UBound(a)
      If a(i).Length > 0 Then
        sb.Append("GRANT EXECUTE ON " & SProc & " TO " & a(i).Replace(Chr(34), "").Trim() & vbCrLf)
        sb.Append("GO" & vbCrLf)
      End If
    Next

    Return sb.ToString

  End Function

#End Region ' Stored Procedures

#Region " Unit Tests "

  Private Function BuildUnitTests() As String

    Dim sb As New StringBuilder

    If m_AddComments Then
      sb.Append(GetComments(CommentTypes.Generator, String.Empty))
    End If

    sb.Append("Imports Csla" & vbCrLf)
    sb.Append("Imports System.Guid" & vbCrLf)
    sb.Append("Imports NUnit.Framework" & vbCrLf)
    If m_TestImports.length > 0 Then
      sb.Append("Imports " & m_TestImports & vbCrLf)
    End If
    sb.Append(vbCrLf)

    ' Open tests
    sb.Append("<TestFixture()> _" & vbCrLf)
    sb.Append("Public Class " & m_Object.Name & "Tests" & vbCrLf & vbCrLf)

    ' Member vars
    Select Case m_Object.BOType
      Case BOTypes.EditableRoot, BOTypes.ReadOnlyObject, BOTypes.Switchable, BOTypes.EditableRootCollection
        sb.Append(UnitTest_MemberVars)
      Case BOTypes.NameValueList, BOTypes.CommandObject, BOTypes.ReadOnlyCollection
      Case Else
        Debug.Assert(False, m_Object.BOType.ToString & " not supported")
    End Select

    Select Case m_Object.BOType
      Case BOTypes.EditableRoot, BOTypes.Switchable, BOTypes.EditableRootCollection
        sb.Append("  Private IsTestRowPresent As Boolean" & vbCrLf & vbCrLf)
    End Select

    ' BasicTests - Main entry point
    sb.Append("  Public Sub BasicTests(ByVal Iterations As Integer)" & vbCrLf & vbCrLf)
    sb.Append("    For i As Integer = 1 To Iterations" & vbCrLf)
    Select Case m_Object.BOType
      Case BOTypes.EditableRoot, BOTypes.Switchable, BOTypes.EditableRootCollection
        sb.Append("      Test1_New" & m_Object.Name & "()" & vbCrLf)
        sb.Append("      Test2_Update" & m_Object.Name & "()" & vbCrLf)
        sb.Append("      Test3_Get" & m_Object.Name & "()" & vbCrLf)
        sb.Append("      Test4_Delete" & m_Object.Name & "()" & vbCrLf)

      Case BOTypes.ReadOnlyObject
        sb.Append("      Test_Get" & m_Object.Name & "()" & vbCrLf)

      Case BOTypes.NameValueList, BOTypes.CommandObject, BOTypes.ReadOnlyCollection
        sb.Append("      Test1_" & m_Object.Name & "()" & vbCrLf)

      Case Else
        Debug.Assert(False, m_Object.BOType.ToString & " not supported")
    End Select
    sb.Append("    Next" & vbCrLf & vbCrLf)
    sb.Append("  End Sub" & vbCrLf & vbCrLf)

    ' Unit Tests
    Select Case m_Object.BOType
      Case BOTypes.EditableRoot, BOTypes.Switchable, BOTypes.EditableRootCollection
        sb.Append(UnitTest_Insert)
        sb.Append(UnitTest_Update)
        sb.Append(UnitTest_Get(False))
        sb.Append(UnitTest_Delete)

      Case BOTypes.ReadOnlyObject
        sb.Append(UnitTest_Get(True))

      Case BOTypes.NameValueList, BOTypes.ReadOnlyCollection
        sb.Append(UnitTest_NVL_ROC)

      Case BOTypes.CommandObject
        sb.Append(UnitTest_CommandObject)

      Case Else
        Debug.Assert(False, m_Object.BOType.ToString & " not supported")
    End Select

    ' Close tests
    sb.Append("  <TestFixtureTearDown()> _" & vbCrLf)
    sb.Append("  Public Sub RunAfterAllTests()" & vbCrLf & vbCrLf)

    Select Case m_Object.BOType
      Case BOTypes.EditableRoot, BOTypes.Switchable, BOTypes.EditableRootCollection
        'If m_Object.PrimaryKey.VBDataType = "Guid" Then
        sb.Append("    If IsTestRowPresent Then Test4_Delete" & m_Object.Name & "()" & vbCrLf & vbCrLf)
        'Else
        'sb.Append("    If " & m_Object.PrimaryKey.MemberName & " > 0 Then Test4_Delete" & m_Object.Name & "()" & vbCrLf & vbCrLf)
        'End If
        sb.Append("  End Sub" & vbCrLf & vbCrLf)
        sb.Append(UnitTest_Helper(False))
      Case BOTypes.ReadOnlyObject
        sb.Append("  End Sub" & vbCrLf & vbCrLf)
        sb.Append(UnitTest_Helper(True))
      Case BOTypes.NameValueList, BOTypes.CommandObject, BOTypes.ReadOnlyCollection
        sb.Append("  End Sub" & vbCrLf & vbCrLf)
        sb.Append(UnitTest_Helper(True))
      Case Else
        Debug.Assert(False, m_Object.BOType.ToString & " not supported")
    End Select

    sb.Append("End Class" & vbCrLf)

    If m_OutputToFile Then OutputGeneratedObject(sb, m_Object.Name & "_Tests.vb")

    Return sb.ToString

  End Function

  Private Function UnitTest_MemberVars() As String

    Dim sb As New StringBuilder

    If m_Object.BOType = BOTypes.ReadOnlyObject Then
      sb.Append("  Private " & m_Object.PrimaryKey.MemberName & " As " & m_Object.PrimaryKey.VBDataType)
      sb.Append(" ' <-- Test_Get" & m_Object.Name & " requires a user supplied key value" & vbCrLf & vbCrLf)
    Else
      For Each column As ColumnDef In m_Object.FetchColDefs
        If Not column.IsReadOnly And Not column.IsWriteOnly Then
          If column.IsTimeStamp Then
            'sb.Append("  Private " & column.MemberName & "(" & column.Size - 1 & ")" & " As " & column.TestInit & vbCrLf)
          Else
            If column.IsSmartDate Then
              sb.Append("  Private " & column.MemberName & " As " & column.Declaration & vbCrLf)
            Else
              sb.Append("  Private " & CStr(IIf(column.IsByteArray, column.TestAssign & "()", column.MemberName)) & " As " & column.TestInit & vbCrLf)
            End If
            'End If
          End If
        Else
          If column.IsIdentity Then
            sb.Append("  Private " & column.MemberName & " As " & column.VBDataType & vbCrLf)
          End If
        End If
      Next
      sb.Append(vbCrLf)
    End If

    Return sb.ToString

  End Function

  Private Function UnitTest_Insert() As String

    Dim sb As New StringBuilder

    sb.Append("  <Test()> _" & vbCrLf)
    sb.Append("  Public Sub Test1_New" & m_Object.Name & "()" & vbCrLf & vbCrLf)

    sb.Append("    ' Clean up if other test created its own test data" & vbCrLf)
    sb.Append("    If IsTestRowPresent Then Test4_Delete" & m_Object.Name & "()" & vbCrLf & vbCrLf)

    sb.Append("    LogIn()" & vbCrLf & vbCrLf)

    sb.Append("    ' Create object" & vbCrLf)
    If m_Object.BOType = BOTypes.EditableRootCollection Then
      sb.Append("    Dim objCol As " & m_Object.Name & " = " & m_Object.Name & ".New" & m_Object.Name & "" & vbCrLf)
      sb.Append("    Dim obj As " & m_ChildType & " = objCol.Add" & vbCrLf & vbCrLf)
    Else
      sb.Append("    Dim obj As " & m_Object.Name & " = " & m_Object.Name & ".New" & m_Object.Name & "" & vbCrLf & vbCrLf)
    End If

    sb.Append("    ' Make assertions" & vbCrLf)
    sb.Append("    Assert.IsNotNull(obj)" & vbCrLf)
    sb.Append("    Assert.AreEqual(False, obj.IsValid, GetBrokenRules(obj))" & vbCrLf & vbCrLf)

    sb.Append("    ' Populate object" & vbCrLf)
    For Each column As ColumnDef In m_Object.ColDefs
      If Not column.IsIdentity AndAlso Not column.IsTimeStamp AndAlso Not column.IsWriteOnly Then
        sb.Append("    obj." & column.Name & " = " & column.TestAssign & vbCrLf)
      End If
    Next
    sb.Append(vbCrLf)

    If m_Object.HasChild Then
      sb.Append("    Dim objChild As " & m_ChildType & vbCrLf)

      For i As Integer = 1 To 2
        sb.Append("    ' Add child number " & i.ToString & vbCrLf)
        sb.Append("    objChild = obj." & m_ChildColName & ".Add" & vbCrLf)
        For Each col As ColumnDef In m_Child.ColDefs
          If Not col.IsIdentity AndAlso Not col.IsTimeStamp AndAlso Not col.IsForeignKeyMember AndAlso Not col.IsPrimaryKeyMember Then
            sb.Append("    objChild." & col.Name & Replace(col.TestInit, col.VBDataType & " =", " =") & vbCrLf)
          End If
        Next
      Next
      sb.Append(vbCrLf)
    End If

    sb.Append("    ' Make assertions" & vbCrLf)

    sb.Append("    Assert.AreEqual(True, obj.IsNew)" & vbCrLf)
    sb.Append("    Assert.AreEqual(False, obj.IsDeleted)" & vbCrLf)
    sb.Append("    Assert.AreEqual(True, obj.IsDirty)" & vbCrLf)
    sb.Append("    Assert.AreEqual(True, obj.IsValid, GetBrokenRules(obj))" & vbCrLf & vbCrLf)

    sb.Append("    ' Persist object" & vbCrLf)

    If m_Object.BOType = BOTypes.EditableRootCollection Then
      sb.Append("    objCol.Save()" & vbCrLf & vbCrLf)
    Else
      sb.Append("    obj.Save()" & vbCrLf & vbCrLf)
    End If

    If m_ExecutionTime Then
      sb.Append("    ' Make assertions" & vbCrLf)
      sb.Append("    Assert.IsTrue(obj.ExecutionTime.Milliseconds < " & m_TestExecuteTime & ", """ & m_Object.Name & ".New" & m_Object.Name)
      sb.Append(" exceution time exceeded the "" & _" & vbCrLf)
      sb.Append("      ""expected number of milliseconds."" & vbCrLf & ""  Execution Time: "" & obj.ExecutionTime.TotalMilliseconds.ToString)" & vbCrLf)
      sb.Append("    Console.WriteLine(""" & m_Object.Name & ".New" & m_Object.Name & " Execution Time: "" & obj.ExecutionTime.TotalMilliseconds.ToString)" & vbCrLf & vbCrLf)

    Else
      sb.Append(vbCrLf)
    End If

    sb.Append("    ' Save object's primary key for possible" & vbCrLf)
    sb.Append("    ' use with other tests in this class" & vbCrLf)

    sb.Append("    " & m_Object.PrimaryKey.MemberName & " = obj." & m_Object.PrimaryKey.Name & "" & vbCrLf)
    sb.Append("    IsTestRowPresent = True" & vbCrLf & vbCrLf)

    sb.Append("    LogOut()" & vbCrLf & vbCrLf)

    sb.Append("  End Sub" & vbCrLf & vbCrLf)

    Return sb.ToString

  End Function

  Private Function UnitTest_Get(ByVal IsReadOnly As Boolean) As String

    Dim sb As New StringBuilder

    sb.Append("  <Test()> _" & vbCrLf)

    If IsReadOnly Then
      sb.Append("  Public Sub Test_Get" & m_Object.Name & "()" & vbCrLf & vbCrLf)
    Else
      sb.Append("  Public Sub Test3_Get" & m_Object.Name & "()" & vbCrLf & vbCrLf)
      sb.Append("    Dim doDelete As Boolean" & vbCrLf)
      sb.Append("    If Not IsTestRowPresent Then" & vbCrLf)
      sb.Append("      Test1_New" & m_Object.Name & "()" & vbCrLf)
      sb.Append("      doDelete = True" & vbCrLf)
      sb.Append("    End If" & vbCrLf & vbCrLf)
    End If

    sb.Append("    LogIn()" & vbCrLf & vbCrLf)

    sb.Append("    ' Get object" & vbCrLf)
    If m_Object.BOType = BOTypes.EditableRootCollection Then
      sb.Append("    Dim objCol As " & m_Object.Name & " = " & m_Object.Name & ".Get" & m_Object.Name & "()" & vbCrLf)
      sb.Append("    Dim obj As " & m_ChildType & " = objCol.ItemByID(" & m_Object.PrimaryKey.MemberName & ")" & vbCrLf & vbCrLf)
    Else
      If m_Implement_Exists Then
        sb.Append("    ' Exists" & vbCrLf)
        sb.Append("    Assert.AreEqual(True, " & m_Object.Name & ".Exists(" & m_Object.PrimaryKey.MemberName & "), """ & m_Object.Name & " with " & m_Object.PrimaryKey.Name & " of "" & " & m_Object.PrimaryKey.MemberName & ".ToString & "" not found"")" & vbCrLf & vbCrLf)
      End If

      sb.Append("    Dim obj As " & m_Object.Name & " = " & m_Object.Name & ".Get" & m_Object.Name & "(" & m_Object.PrimaryKey.MemberName & ")" & vbCrLf & vbCrLf)
    End If

    If m_ExecutionTime Then
      sb.Append("    ' Make assertions" & vbCrLf)
      sb.Append("    Assert.IsTrue(obj.ExecutionTime.Milliseconds < " & m_TestExecuteTime & ", """ & m_Object.Name & ".Get" & m_Object.Name)
      sb.Append(" exceution time exceeded the "" & _" & vbCrLf)
      sb.Append("      ""expected number of milliseconds."" & vbCrLf & ""  Execution Time: "" & obj.ExecutionTime.TotalMilliseconds.ToString)" & vbCrLf)
      sb.Append("    Console.WriteLine(""" & m_Object.Name & ".Get" & m_Object.Name & " Execution Time: "" & obj.ExecutionTime.TotalMilliseconds.ToString)" & vbCrLf & vbCrLf)
    Else
      sb.Append(vbCrLf)
    End If

    If IsReadOnly Then
      sb.Append("    ' Make assertions" & vbCrLf)
      For Each column As ColumnDef In m_Object.ColDefs
        If Not column.AllowDBNull Then
          sb.Append("    Assert.IsNotEmpty(obj." & column.Name & ".ToString, """ & column.Name & " is empty"")" & vbCrLf)
        End If
      Next

      sb.Append(vbCrLf)
      sb.Append("    LogOut()" & vbCrLf & vbCrLf)

    Else
      sb.Append("    ' Make assertions" & vbCrLf)
      sb.Append("    Assert.AreEqual(False, obj.IsNew)" & vbCrLf)
      sb.Append("    Assert.AreEqual(False, obj.IsDeleted)" & vbCrLf)
      sb.Append("    Assert.AreEqual(False, obj.IsDirty)" & vbCrLf)
      sb.Append("    Assert.AreEqual(True, obj.IsValid)" & vbCrLf)
      If m_Object.HasChild Then
        sb.Append("    Assert.AreEqual(True, obj." & m_ChildColName & ".Count = 2)" & vbCrLf)
      End If
      sb.Append(vbCrLf)

      sb.Append("    ' Make assertions - Compare persisted values" & vbCrLf)
      For Each column As ColumnDef In m_Object.ColDefs
        If Not column.IsIdentity AndAlso Not column.IsTimeStamp AndAlso Not column.IsPrimaryKeyMember Then
          If column.VBDataType = "Integer" Then
          ElseIf column.VBDataType = "String" Then
            If column.IsSmartDate Then
              sb.Append("    Assert.AreEqual(True, obj." & column.Name & " = " & column.MemberName & ".ToString, """ & column.Name & " values do not match"")" & vbCrLf)
            Else
              sb.Append("    Assert.AreEqual(True, obj." & column.Name & " = " & column.MemberName & ", """ & column.Name & " values do not match"")" & vbCrLf)
            End If
          ElseIf column.IsByteArray Then
            sb.Append("    Assert.AreEqual(True, BitConverter.ToString(obj." & column.Name & ", 0, " & column.Size & ") = BitConverter.ToString(" & column.TestAssign & "), """ & column.Name & " values do not match"")" & vbCrLf)
          Else
            sb.Append("    Assert.AreEqual(True, obj." & column.Name & " = " & column.MemberName & ", """ & column.Name & " values do not match"")" & vbCrLf)
          End If
        End If
      Next
      sb.Append(vbCrLf)

      sb.Append("    LogOut()" & vbCrLf & vbCrLf)

      sb.Append("    ' Clean up if test created its own test data" & vbCrLf)
      sb.Append("    If doDelete Then Test4_Delete" & m_Object.Name & "()" & vbCrLf & vbCrLf)

    End If

    sb.Append("  End Sub" & vbCrLf & vbCrLf)

    Return sb.ToString

  End Function

  Private Function UnitTest_Update() As String

    Dim sb As New StringBuilder

    sb.Append("  <Test()> _" & vbCrLf)
    sb.Append("Public Sub Test2_Update" & m_Object.Name & "()" & vbCrLf & vbCrLf)

    sb.Append("    Dim doDelete As Boolean" & vbCrLf)
    sb.Append("    If Not IsTestRowPresent Then" & vbCrLf)
    sb.Append("      Test1_New" & m_Object.Name & "()" & vbCrLf)
    sb.Append("      doDelete = True" & vbCrLf)
    sb.Append("    End If" & vbCrLf & vbCrLf)

    sb.Append("    LogIn()" & vbCrLf & vbCrLf)

    sb.Append("    ' Get object" & vbCrLf)
    If m_Object.BOType = BOTypes.EditableRootCollection Then
      sb.Append("    Dim objCol As " & m_Object.Name & " = " & m_Object.Name & ".Get" & m_Object.Name & "()" & vbCrLf)
      sb.Append("    Dim obj As " & m_ChildType & " = objCol.ItemByID(" & m_Object.PrimaryKey.MemberName & ")" & vbCrLf & vbCrLf)
    Else
      If m_Implement_Exists Then
        sb.Append("    ' Exists" & vbCrLf)
        sb.Append("    Assert.AreEqual(True, " & m_Object.Name & ".Exists(" & m_Object.PrimaryKey.MemberName & "), """ & m_Object.Name & " with " & m_Object.PrimaryKey.Name & " of "" & " & m_Object.PrimaryKey.MemberName & ".ToString & "" not found"")" & vbCrLf & vbCrLf)
      End If

      sb.Append("    Dim obj As " & m_Object.Name & " = " & m_Object.Name & ".Get" & m_Object.Name & "(" & m_Object.PrimaryKey.MemberName & ")" & vbCrLf & vbCrLf)
    End If

    sb.Append("    ' Snapshot state" & vbCrLf)
    sb.Append("    obj.BeginEdit()" & vbCrLf & vbCrLf)

    If m_Object.HasChild Then
      sb.Append("    ' Make assertions" & vbCrLf)
      sb.Append("    Assert.IsTrue(obj." & m_ChildColName & ".Count = 2, ""obj." & m_ChildColName & ".Count = "" & obj." & m_ChildColName & ".Count)" & vbCrLf & vbCrLf)

      sb.Append("    Dim objChild As " & m_ChildType & vbCrLf)
      sb.Append("    ' Add child number 3" & vbCrLf)
      sb.Append("    objChild = obj." & m_ChildColName & ".Add" & vbCrLf)
      For Each col As ColumnDef In m_Child.ColDefs
        If Not col.IsIdentity AndAlso Not col.IsTimeStamp AndAlso Not col.IsForeignKeyMember AndAlso Not col.IsPrimaryKeyMember Then
          sb.Append("    objChild." & col.Name & Replace(col.TestInit, col.VBDataType & " =", " =") & vbCrLf)
        End If
      Next
      sb.Append(vbCrLf)

      sb.Append("    ' Update child number 1" & vbCrLf)
      sb.Append("    objChild = obj." & m_ChildColName & "(0)" & vbCrLf)
      For Each col As ColumnDef In m_Child.ColDefs
        If Not col.IsIdentity AndAlso Not col.IsTimeStamp AndAlso Not col.IsForeignKeyMember AndAlso Not col.IsPrimaryKeyMember Then
          sb.Append("    objChild." & col.Name & Replace(col.TestInit, col.VBDataType & " =", " =") & vbCrLf)
        End If
      Next
      sb.Append(vbCrLf)

      sb.Append("    ' Delete child number 2" & vbCrLf)
      sb.Append("    obj." & m_ChildColName & ".Remove(obj." & m_ChildColName & "(1))" & vbCrLf)
      sb.Append(vbCrLf)
    End If

    sb.Append("    ' Update member vars" & vbCrLf)
    For Each column As ColumnDef In m_Object.FetchColDefs
      If Not column.IsIdentity AndAlso Not column.IsTimeStamp AndAlso Not column.IsReadOnly _
        AndAlso Not column.IsPrimaryKeyMember AndAlso Not column.IsWriteOnly Then
        If column.IsByteArray Then
          sb.Append("    " & column.TestAssign & " = System.Text.Encoding.UTF8.GetBytes(GetRandomString(" & column.Size & "))" & vbCrLf)
        Else
          sb.Append("    " & column.MemberName & " = " & column.TestModify & vbCrLf)
        End If
      End If
    Next
    sb.Append(vbCrLf)

    sb.Append("    ' Update object" & vbCrLf)
    For Each column As ColumnDef In m_Object.ColDefs
      If Not column.IsIdentity AndAlso Not column.IsTimeStamp AndAlso Not column.IsReadOnly _
        AndAlso Not column.IsPrimaryKeyMember AndAlso Not column.IsWriteOnly Then
        sb.Append("    obj." & column.Name & " = " & column.TestAssign & vbCrLf)
      End If
    Next
    sb.Append(vbCrLf)

    sb.Append("    ' Make assertions" & vbCrLf)
    sb.Append("    Assert.AreEqual(False, obj.IsNew)" & vbCrLf)
    sb.Append("    Assert.AreEqual(False, obj.IsDeleted)" & vbCrLf)
    sb.Append("    Assert.AreEqual(True, obj.IsDirty)" & vbCrLf)
    sb.Append("    Assert.AreEqual(True, obj.IsValid, GetBrokenRules(obj))" & vbCrLf & vbCrLf)

    sb.Append("    ' Accept recent changes to state" & vbCrLf)
    sb.Append("    obj.ApplyEdit()" & vbCrLf & vbCrLf)

    sb.Append("    ' Persist object" & vbCrLf)
    If m_Object.BOType = BOTypes.EditableRootCollection Then
      sb.Append("    objCol.Save()" & vbCrLf & vbCrLf)
    Else
      sb.Append("    obj.Save()" & vbCrLf & vbCrLf)
    End If

    If m_ExecutionTime Then
      sb.Append("    ' Make assertions" & vbCrLf)
      sb.Append("    Assert.IsTrue(obj.ExecutionTime.Milliseconds < " & m_TestExecuteTime & ", """ & m_Object.Name & ".Save" & m_Object.Name)
      sb.Append(" exceution time exceeded the "" & _" & vbCrLf)
      sb.Append("      ""expected number of milliseconds."" & vbCrLf & ""  Execution Time: "" & obj.ExecutionTime.TotalMilliseconds.ToString)" & vbCrLf)
      sb.Append("    Console.WriteLine(""" & m_Object.Name & ".Save (Update)" & m_Object.Name & " Execution Time: "" & obj.ExecutionTime.TotalMilliseconds.ToString)" & vbCrLf & vbCrLf)
    Else
      sb.Append(vbCrLf)
    End If

    sb.Append("    LogOut()" & vbCrLf & vbCrLf)

    sb.Append("    ' Clean up if test created its own test data" & vbCrLf)
    sb.Append("    If doDelete Then Test4_Delete" & m_Object.Name & "()" & vbCrLf & vbCrLf)

    sb.Append("  End Sub" & vbCrLf & vbCrLf)

    Return sb.ToString

  End Function

  Private Function UnitTest_Delete() As String

    Dim sb As New StringBuilder

    sb.Append("  <Test()> _" & vbCrLf)
    sb.Append("  Public Sub Test4_Delete" & m_Object.Name & "()" & vbCrLf & vbCrLf)

    sb.Append("    If Not IsTestRowPresent Then Test1_New" & m_Object.Name & "()" & vbCrLf & vbCrLf)

    sb.Append("    LogIn()" & vbCrLf & vbCrLf)

    sb.Append("    ' Get object" & vbCrLf)
    If m_Object.BOType = BOTypes.EditableRootCollection Then
      sb.Append("    Dim objCol As " & m_Object.Name & " = " & m_Object.Name & ".Get" & m_Object.Name & "()" & vbCrLf & vbCrLf)
    Else
      If m_Implement_Exists Then
        sb.Append("    ' Exists" & vbCrLf)
        sb.Append("    Assert.AreEqual(True, " & m_Object.Name & ".Exists(" & m_Object.PrimaryKey.MemberName & "), """ & m_Object.Name & " with " & m_Object.PrimaryKey.Name & " of "" & " & m_Object.PrimaryKey.MemberName & ".ToString & "" not found"")" & vbCrLf & vbCrLf)
      End If

      sb.Append("    Dim obj As " & m_Object.Name & " = " & m_Object.Name & ".Get" & m_Object.Name & "(" & m_Object.PrimaryKey.MemberName & ")" & vbCrLf & vbCrLf)
    End If

    If m_Object.HasChild Then
      sb.Append("    ' Delete object's children" & vbCrLf)
      sb.Append("    Dim objChild As " & m_ChildType & vbCrLf)
      sb.Append("    For i As Integer = obj." & m_ChildColName & ".Count - 1 To 0 Step -1" & vbCrLf)
      sb.Append("      objChild = obj." & m_ChildColName & ".Item(i)" & vbCrLf)
      sb.Append("      obj." & m_ChildColName & ".Remove(objChild)" & vbCrLf)
      sb.Append("    Next" & vbCrLf & vbCrLf)
      sb.Append("    ' Make assertions" & vbCrLf)
      sb.Append("    Assert.AreEqual(True, obj." & m_ChildColName & ".Count = 0)" & vbCrLf & vbCrLf)
      sb.Append("    ' Save edited object" & vbCrLf)
      sb.Append("    obj.Save()" & vbCrLf & vbCrLf)
    End If

    sb.Append("    ' Delete object" & vbCrLf)
    If m_Object.BOType = BOTypes.EditableRootCollection Then
      sb.Append("    objCol.Remove(" & m_Object.PrimaryKey.MemberName & ")" & vbCrLf)
      sb.Append("    objCol.Save()" & vbCrLf & vbCrLf)
    Else
      sb.Append("    obj.Delete()" & vbCrLf)
      sb.Append("    obj.Save()" & vbCrLf & vbCrLf)

      If m_Implement_Exists Then
        sb.Append("    ' Exists" & vbCrLf)
        sb.Append("    Assert.AreEqual(False, " & m_Object.Name & ".Exists(" & m_Object.PrimaryKey.MemberName & "), """ & m_Object.Name & " with " & m_Object.PrimaryKey.Name & " of "" & " & m_Object.PrimaryKey.MemberName & ".ToString & "" not deleted"")" & vbCrLf & vbCrLf)
      End If
    End If

    If m_ExecutionTime Then
      sb.Append("    ' Make assertions" & vbCrLf)
      sb.Append("    Assert.IsTrue(obj.ExecutionTime.Milliseconds < " & m_TestExecuteTime & ", """ & m_Object.Name & ".Save" & m_Object.Name)
      sb.Append(" exceution time exceeded the "" & _" & vbCrLf)
      sb.Append("      ""expected number of milliseconds."" & vbCrLf & ""  Execution Time: "" & obj.ExecutionTime.TotalMilliseconds.ToString)" & vbCrLf)
      sb.Append("    Console.WriteLine(""" & m_Object.Name & ".Save (Delete)" & m_Object.Name & " Execution Time: "" & obj.ExecutionTime.TotalMilliseconds.ToString)" & vbCrLf & vbCrLf)

    Else
      sb.Append(vbCrLf)
    End If

    sb.Append("    LogOut()" & vbCrLf & vbCrLf)

    sb.Append("    ' Reset member var" & vbCrLf)
    If m_Object.PrimaryKey.VBDataType = "Guid" Then
      sb.Append("    " & m_Object.PrimaryKey.MemberName & " = Guid.Empty" & vbCrLf)
    Else
      sb.Append("    " & m_Object.PrimaryKey.MemberName & " = 0" & vbCrLf)
    End If

    sb.Append("    IsTestRowPresent = False" & vbCrLf & vbCrLf)

    sb.Append("  End Sub" & vbCrLf & vbCrLf)

    Return sb.ToString

  End Function

  Private Function UnitTest_NVL_ROC() As String

    Dim sb As New StringBuilder

    sb.Append("  <Test()> _" & vbCrLf)
    sb.Append("  Public Sub Test1_" & m_Object.Name & "()" & vbCrLf & vbCrLf)

    sb.Append("    LogIn()" & vbCrLf & vbCrLf)

    sb.Append("    ' Create object" & vbCrLf)
    sb.Append("    Dim obj As " & m_Object.Name & " = " & m_Object.Name & ".Get" & m_Object.Name & "()" & vbCrLf & vbCrLf)

    sb.Append("    ' Make assertions" & vbCrLf)
    sb.Append("    Assert.IsFalse(obj.Count = 0, """ & m_Object.Name & " is empty"")" & vbCrLf)
    sb.Append("    Assert.IsTrue(obj.Count > 1, """ & m_Object.Name & " has less than the expected minimum number of items"")" & vbCrLf)
    sb.Append("    Assert.IsTrue(obj.Count < 101, """ & m_Object.Name & " has more than the expected minimum number of items"")" & vbCrLf)

    If m_ExecutionTime Then
      sb.Append("    Assert.IsTrue(obj.ExecutionTime.Milliseconds < " & m_TestExecuteTime & ", """ & m_Object.Name & ".Get" & m_Object.Name)
      sb.Append(" exceution time exceeded the "" & _" & vbCrLf)
      sb.Append("      ""expected number of milliseconds."" & vbCrLf & ""  Execution Time: "" & obj.ExecutionTime.TotalMilliseconds.ToString)" & vbCrLf)
      sb.Append("    Console.WriteLine(""" & m_Object.Name & ".Get" & m_Object.Name & " Execution Time: "" & obj.ExecutionTime.TotalMilliseconds.ToString)" & vbCrLf & vbCrLf)

    Else
      sb.Append(vbCrLf)
    End If

    sb.Append("    LogOut()" & vbCrLf & vbCrLf)

    sb.Append("  End Sub" & vbCrLf & vbCrLf)

    Return sb.ToString

  End Function

  Private Function UnitTest_CommandObject() As String

    Dim sb As New StringBuilder

    sb.Append("  <Test()> _" & vbCrLf)
    sb.Append("  Public Sub Test1_" & m_Object.Name & "()" & vbCrLf & vbCrLf)

    sb.Append("    LogIn()" & vbCrLf & vbCrLf)

    sb.Append("    ' Make assertions" & vbCrLf)
    sb.Append("    Assert.IsTrue(" & m_Object.Name & "." & m_CommandMethod & ", """ & m_CommandMethod & " returned unexpected results"")" & vbCrLf & vbCrLf)

    sb.Append("    LogOut()" & vbCrLf & vbCrLf)

    sb.Append("  End Sub" & vbCrLf & vbCrLf)

    Return sb.ToString

  End Function

  Private Function UnitTest_Helper(ByVal Basic As Boolean) As String

    Dim sb As New StringBuilder

    sb.Append(DoRegion("Helper code", True))

    sb.Append("  Private Sub LogIn()" & vbCrLf)
    sb.Append("    ' TODO: Call login method" & vbCrLf)
    sb.Append("  End Sub" & vbCrLf & vbCrLf)

    sb.Append("  Private Sub LogOut()" & vbCrLf)
    sb.Append("    ' TODO: Call logout method" & vbCrLf)
    sb.Append("  End Sub" & vbCrLf & vbCrLf)

    If Not Basic Then
      sb.Append("  Private Function GetRandomString(ByVal Length As Integer) As String" & vbCrLf & vbCrLf)
      sb.Append("    Dim r As New Random" & vbCrLf)
      sb.Append("    Dim sb As New System.Text.StringBuilder" & vbCrLf)
      sb.Append("    If Length > 4000 Then Length = 4000" & vbCrLf & vbCrLf)
      sb.Append("    For i As Integer = 0 To Length - 1" & vbCrLf)
      sb.Append("      sb.Append(Chr(CInt((26 * r.NextDouble()) + 65)))" & vbCrLf)
      sb.Append("    Next" & vbCrLf & vbCrLf)
      sb.Append("    Return sb.ToString" & vbCrLf & vbCrLf)
      sb.Append("  End Function" & vbCrLf & vbCrLf)

      If m_Object.BOType = BOTypes.EditableRootCollection Then
        sb.Append("  Private Function GetBrokenRules(ByVal obj As " & m_ChildType & ") As String" & vbCrLf & vbCrLf)
      Else
        sb.Append("  Private Function GetBrokenRules(ByVal obj As " & m_Object.Name & ") As String" & vbCrLf & vbCrLf)
      End If

      sb.Append("    Dim sb As New System.Text.StringBuilder" & vbCrLf & vbCrLf)
      sb.Append("    For Each br As Csla.Validation.BrokenRule In obj.BrokenRulesCollection" & vbCrLf)
      sb.Append("      sb.Append(br.Description & vbCrLf)" & vbCrLf)
      sb.Append("    Next" & vbCrLf & vbCrLf)
      sb.Append("    Return sb.ToString" & vbCrLf & vbCrLf)
      sb.Append("  End Function" & vbCrLf & vbCrLf)
    End If

    sb.Append(DoRegion("Helper code", False))

    Return sb.ToString

  End Function

#End Region ' Unit Tests

#Region " Helper Code "

  Private Sub MapSchema()

    If Not m_ObjectTable Is Nothing Then
      If m_ObjectName.Length > 0 Then
        ReDim m_Object.ColDefs(m_ObjectTable.Columns.Count - 1)
        MapSchemaColumns(m_ObjectTable, m_Object.ColDefs)
        m_Object.DataSource = m_ObjectTable.Name
        m_DataBaseName = m_ObjectTable.Database.Name
      Else
        Throw New Exception("Object name must be specified")
      End If

    Else
      If m_Object.BOType = BOTypes.NameValueList Or m_Object.BOType = BOTypes.ReadOnlyCollection Or _
        m_Object.BOType = BOTypes.ReadOnlyObject Or m_Object.BOType = BOTypes.CommandObject Then
        If Not m_ObjectFetchDataSource Is Nothing Then
          ReDim m_Object.ColDefs(m_ObjectFetchDataSource.Columns.Count - 1)
          MapSchemaColumns(m_ObjectFetchDataSource, m_Object.ColDefs)
          m_Object.DataSource = m_ObjectFetchDataSource.Name
          m_DataBaseName = m_ObjectFetchDataSource.Database.Name
        End If
      Else
        Throw New Exception("You must specify a Table")
      End If
    End If


    If Not m_ObjectFetchDataSource Is Nothing Then
      If m_ObjectFetchDataSource.GetType.ToString = "SchemaExplorer.ViewSchema" Then
        ReDim m_Object.FetchColDefs(m_ObjectFetchDataSource.Columns.Count - 1)
        MapSchemaColumns(m_ObjectFetchDataSource, m_Object.FetchColDefs, m_Object.ColDefs)
        m_Object.FetchDataSource = m_ObjectFetchDataSource.Name
      Else
        m_Object.FetchColDefs = m_Object.ColDefs
        Debug.Assert(False, "Unhandled data source type: " & m_ObjectFetchDataSource.GetType.ToString)
      End If
    ElseIf Not m_Object.ColDefs Is Nothing Then
      m_Object.FetchDataSource = m_ObjectTable.Name
      m_Object.FetchColDefs = m_Object.ColDefs
    Else
      Throw New Exception("Must have either an object datasource or object fetch datasource")
    End If

    If Not m_ChildTable Is Nothing Then
      If m_ChildColName.Length > 0 AndAlso m_ChildColType.Length > 0 Then
        m_Child = New BusinessObject

        ReDim m_Child.ColDefs(m_ChildTable.Columns.Count - 1)
        MapSchemaColumns(m_ChildTable, m_Child.ColDefs)

        m_Child.DataSource = m_ChildTable.Name
        m_Child.Name = m_ChildColType

        m_Object.HasChild = True

      Else
        Throw New Exception("Child collection type must be specified")
      End If
    End If

    If m_Object.ColDefs Is Nothing Then
      Debug.Assert(False, "Should never reach this")
      m_Object.ColDefs = m_Object.FetchColDefs
    End If

    ' Locate the PrimaryKey/Identity field for easy retrieval (if exists)
    m_Object.PrimaryKey.MemberName = ""
    For Each Column As ColumnDef In m_Object.ColDefs
      If Column.IsPrimaryKeyMember AndAlso m_Object.PrimaryKey.MemberName.Length = 0 Then
        m_Object.PrimaryKey.MemberName = Column.MemberName
        m_Object.PrimaryKey.Name = Column.Name
        m_Object.PrimaryKey.VBDataType = Column.VBDataType
        m_Object.PrimaryKey.SQLParam = Column.SQLParam
        m_Object.HasIdentityField = Column.IsIdentity
        Exit For
      End If
    Next

    ' If we can't find a primary key (probably because the source is a view) 
    ' then promote the first column as the PK
    If m_Object.PrimaryKey.MemberName = "" Then
      m_Object.PrimaryKey.MemberName = m_Object.ColDefs(0).MemberName
      m_Object.PrimaryKey.Name = m_Object.ColDefs(0).Name
      m_Object.PrimaryKey.VBDataType = m_Object.ColDefs(0).VBDataType
      m_Object.PrimaryKey.SQLParam = m_Object.ColDefs(0).SQLParam
      m_Object.HasIdentityField = False
    End If

    ' Locate the Timestamp field for easy retrieval (if exists)
    For Each Column As ColumnDef In m_Object.ColDefs
      If Column.IsTimeStamp Then
        m_Object.HasTimeStampField = True
        m_Object.TimeStamp = Column.Name
        Exit For
      End If
    Next

    ' Set the IsIdentity flag on the fetchcol collection
    If m_Object.HasIdentityField Then
      For Each Column As ColumnDef In m_Object.ColDefs
        Dim b As Boolean
        If Column.IsIdentity Then
          For i As Integer = 0 To UBound(m_Object.FetchColDefs)
            If Column.Name = m_Object.FetchColDefs(0).Name Then
              m_Object.FetchColDefs(i).IsIdentity = True
              b = True
              Exit For
            End If
            If b Then Exit For
          Next
        End If
      Next
    End If

  End Sub

  Private Sub MapSchemaColumns(ByVal ObjectTable As SchemaExplorer.TableSchema, _
    ByVal ColumnDefs() As ColumnDef)

    Dim col As ColumnSchema
    Dim i As Integer

    For Each col In ObjectTable.PrimaryKey.MemberColumns
      Dim schCol As New ColumnDef
      With schCol
        .AllowDBNull = col.AllowDBNull
        .DataType = col.DataType.ToString
        .IsForeignKeyMember = col.IsForeignKeyMember
        .IsIdentity = CBool(col.ExtendedProperties.Item("CS_IsIdentity").Value)
        .IsPrimaryKeyMember = col.IsPrimaryKeyMember
        .IsReadOnly = .IsIdentity OrElse .IsTimeStamp
        .Name = col.Name
        .NativeType = col.NativeType
        If m_CamelCaseMemberVars Then
          .MemberName = m_MemberPrefix & .Name.Substring(0, 1).ToUpper & LCase(.Name.Substring(1))
        Else
          .MemberName = m_MemberPrefix & .Name
        End If
        .Precision = col.Precision
        .Scale = col.Scale
        .Size = col.Size
        .IsMax = CBool(.Size = -1)
        DataTypeStrings(schCol, col.NativeType)
        .SQLParam = "@" & .Name & " " & .SQLDataType
      End With
      ColumnDefs(i) = schCol
      i += 1
    Next

    For Each col In ObjectTable.NonPrimaryKeyColumns
      Dim schCol As New ColumnDef
      With schCol
        .AllowDBNull = col.AllowDBNull
        .DataType = col.DataType.ToString
        .IsForeignKeyMember = col.IsForeignKeyMember
        .IsIdentity = CBool(col.ExtendedProperties.Item("CS_IsIdentity").Value)
        .IsPrimaryKeyMember = col.IsPrimaryKeyMember
        .Name = col.Name
        .NativeType = col.NativeType
        If m_CamelCaseMemberVars Then
          .MemberName = m_MemberPrefix & .Name.Substring(0, 1).ToUpper & LCase(.Name.Substring(1))
        Else
          .MemberName = m_MemberPrefix & .Name
        End If
        .Precision = col.Precision
        .Scale = col.Scale
        .Size = col.Size
        .IsMax = CBool(.Size = -1)
        DataTypeStrings(schCol, col.NativeType)
        .SQLParam = "@" & .Name & " " & .SQLDataType
        .IsReadOnly = .IsIdentity OrElse .IsTimeStamp
      End With
      ColumnDefs(i) = schCol
      i += 1
    Next

  End Sub

  Private Sub MapSchemaColumns(ByVal ObjectView As SchemaExplorer.ViewSchema, _
    ByVal FetchCols() As ColumnDef, ByVal Cols() As ColumnDef)

    Dim vwCol As ViewColumnSchema
    Dim i As Integer

    For Each vwCol In ObjectView.Columns
      Dim schCol As New ColumnDef
      With schCol
        .AllowDBNull = vwCol.AllowDBNull
        .DataType = vwCol.DataType.ToString
        .IsForeignKeyMember = False
        .IsPrimaryKeyMember = IsPrimaryKey_Member(vwCol.Name, Cols)
        .IsReadOnly = IsReadOnlyMember(vwCol.Name, Cols)
        .Name = vwCol.Name
        .NativeType = vwCol.NativeType
        If m_CamelCaseMemberVars Then
          .MemberName = m_MemberPrefix & .Name.Substring(0, 1).ToUpper & LCase(.Name.Substring(1))
        Else
          .MemberName = m_MemberPrefix & .Name
        End If
        .Precision = vwCol.Precision
        .Scale = vwCol.Scale
        .Size = vwCol.Size
        .IsMax = CBool(.Size = -1)
        DataTypeStrings(schCol, vwCol.NativeType)
        .SQLParam = "@" & .Name & " " & .SQLDataType
      End With
      FetchCols(i) = schCol
      i += 1
    Next

    CopyMissingColumnDefs(FetchCols, Cols)

  End Sub

  Private Sub MapSchemaColumns(ByVal ObjectView As SchemaExplorer.ViewSchema, _
      ByVal ColumnDefs() As ColumnDef)

    Dim col As ViewColumnSchema
    Dim i As Integer

    For Each col In ObjectView.Columns
      Dim schCol As New ColumnDef
      With schCol
        .AllowDBNull = col.AllowDBNull
        .DataType = col.DataType.ToString
        .IsForeignKeyMember = False
        .IsIdentity = False
        .IsPrimaryKeyMember = False
        .IsReadOnly = .IsIdentity OrElse .IsTimeStamp
        .Name = col.Name
        .NativeType = col.NativeType
        If m_CamelCaseMemberVars Then
          .MemberName = m_MemberPrefix & .Name.Substring(0, 1).ToUpper & LCase(.Name.Substring(1))
        Else
          .MemberName = m_MemberPrefix & .Name
        End If
        .Precision = col.Precision
        .Scale = col.Scale
        .Size = col.Size
        .IsMax = CBool(.Size = -1)
        DataTypeStrings(schCol, col.NativeType)
        .SQLParam = "@" & .Name & " " & .SQLDataType
      End With
      ColumnDefs(i) = schCol
      i += 1
    Next

  End Sub

  Private Sub DataTypeStrings(ByRef Col As ColumnDef, ByVal NativeType As String)

    Select Case NativeType

      'Case "sql_variant"  ' Not supported

      Case "binary", "varbinary"
        Col.SQLDataType = NativeType & "(" & Col.Size & ")"
        Col.VBDataType = "Byte"
        Col.Declaration = Col.VBDataType
        Col.ReaderMethod = ".GetBytes(""" & Col.Name & """, 0, " & Col.MemberName & ", 0, " & Col.Size & ")"
        Col.TestAssign = Col.MemberName
        Col.MemberName = Col.MemberName & "(" & Col.Size & ")"
        Col.AddWithValue = """@" & Col.Name & """"
        Col.TestInit = Col.VBDataType & " = System.Text.Encoding.UTF8.GetBytes(GetRandomString(" & Col.Size & "))"
        Col.TestModify = Col.MemberName
        Col.IsByteArray = True
        m_Object.HasByteArray = True

      Case "bigint"
        Col.SQLDataType = "bigint"
        Col.VBDataType = "Long"
        Col.Declaration = Col.VBDataType
        Col.ReaderMethod = Col.MemberName & " = .GetInt64(""" & Col.Name & """)"
        Col.AddWithValue = ".AddWithValue(""@" & Col.Name & """, " & Col.MemberName & ")"
        Col.TestAssign = Col.MemberName
        Col.TestInit = Col.VBDataType & " = " & Col.VBDataType & ".MaxValue"
        Col.TestModify = Col.MemberName & " - 1"

      Case "bit"
        Col.SQLDataType = "bit"
        Col.VBDataType = "Boolean"
        Col.Declaration = Col.VBDataType
        Col.ReaderMethod = Col.MemberName & " = .GetBoolean(""" & Col.Name & """)"
        Col.AddWithValue = ".AddWithValue(""@" & Col.Name & """, " & Col.MemberName & ")"
        Col.TestAssign = Col.MemberName
        Col.TestInit = Col.VBDataType & " = True"
        Col.TestModify = "Not " & Col.MemberName

      Case "char"
        Col.SQLDataType = "char(" & Col.Size & ")"
        Col.VBDataType = "String"
        Col.Declaration = Col.VBDataType
        Col.IsString = True
        Col.ReaderMethod = Col.MemberName & " = .GetString(""" & Col.Name & """)"
        Col.AddWithValue = ".AddWithValue(""@" & Col.Name & """, " & Col.MemberName & ")"
        Col.TestAssign = Col.MemberName
        Col.TestInit = Col.VBDataType & " = GetRandomString(" & Col.Size & ")"
        Col.TestModify = "GetRandomString(" & Col.Size & ")"

      Case "datetime", "smalldatetime"
        Col.SQLDataType = NativeType
        If m_UseSmartDate Then
          Col.VBDataType = "String"
          Col.IsSmartDate = True
          If Col.AllowDBNull Then
            Col.Declaration = "New SmartDate(False)"
            Col.TestAssign = Col.MemberName & ".ToString"
            Col.TestInit = " = New SmartDate(Date.Today).ToString"
            Col.TestModify = "New SmartDate(" & Col.MemberName & ".Add(System.TimeSpan.FromDays(-1)))"
          Else
            Col.Declaration = "New SmartDate(Date.Today)"
            Col.TestAssign = Col.MemberName & ".ToString"
            Col.TestInit = " = New SmartDate(Date.Today).ToString"
            Col.TestModify = "New SmartDate(" & Col.MemberName & ".Add(System.TimeSpan.FromDays(-1)))"
          End If
          Col.ReaderMethod = Col.MemberName & " = .GetSmartDate(""" & Col.Name & """)"
          Col.AddWithValue = ".AddWithValue(""@" & Col.Name & """, " & Col.MemberName & ".DBValue)"
        Else
          Col.VBDataType = "Date"
          Col.Declaration = Col.VBDataType
          Col.ReaderMethod = Col.MemberName & " = .GetDateTime(""" & Col.Name & """)"
          Col.AddWithValue = ".AddWithValue(""@" & Col.Name & """, " & Col.MemberName & ")"
          Col.TestAssign = Col.MemberName
          Col.TestInit = "Date = Date.Today"
          Col.TestModify = Col.MemberName & ".Date.Add(New System.TimeSpan(-1, 0, 0))"
        End If

      Case "decimal", "numeric"
        Col.SQLDataType = NativeType & "(" & Col.Precision & ", " & Col.Scale & ")"
        Col.VBDataType = "Decimal"
        Col.Declaration = Col.VBDataType
        Col.ReaderMethod = Col.MemberName & " = .GetDecimal(""" & Col.Name & """)"
        Col.AddWithValue = ".AddWithValue(""@" & Col.Name & """, " & Col.MemberName & ")"
        Col.TestAssign = Col.MemberName
        If Col.Scale > 0 Then
          Col.TestInit = Col.VBDataType & " = " & StrDup(Col.Precision - Col.Scale, "9") & "." & StrDup(Col.Scale, "9") & "D"
        Else
          Col.TestInit = Col.VBDataType & " = " & StrDup(Col.Precision - Col.Scale, "9") & "D"
        End If
        Col.TestModify = Col.MemberName & " - 1D"

      Case "float"
        Col.SQLDataType = "float"
        Col.VBDataType = "Double"
        Col.Declaration = Col.VBDataType
        Col.ReaderMethod = Col.MemberName & " = .GetDouble(""" & Col.Name & """)"
        Col.AddWithValue = ".AddWithValue(""@" & Col.Name & """, " & Col.MemberName & ")"
        Col.TestAssign = Col.MemberName
        Col.TestInit = Col.VBDataType & " = " & Col.VBDataType & ".MaxValue"
        Col.TestModify = Col.MemberName & " * 0.00001D"

      Case "image"
        Col.SQLDataType = "image"
        Col.VBDataType = "Byte"
        Col.Declaration = Col.VBDataType
        Col.ReaderMethod = ".GetBytes(""" & Col.Name & """, 0, " & Col.MemberName & ", 0, " & Col.Size & ")"
        Col.TestAssign = Col.MemberName
        Col.MemberName = Col.MemberName & "(" & Col.Size & ")"
        Col.AddWithValue = """@" & Col.Name & """"
        Col.TestInit = Col.VBDataType & " = System.Text.Encoding.UTF8.GetBytes(GetRandomString(" & Col.Size & "))"
        Col.TestModify = Col.MemberName
        Col.IsByteArray = True
        m_Object.HasByteArray = True

      Case "int"
        Col.SQLDataType = "int"
        Col.VBDataType = "Integer"
        Col.Declaration = Col.VBDataType
        Col.ReaderMethod = Col.MemberName & " = .GetInt32(""" & Col.Name & """)"
        Col.AddWithValue = ".AddWithValue(""@" & Col.Name & """, " & Col.MemberName & ")"
        Col.TestAssign = Col.MemberName
        Col.TestInit = Col.VBDataType & " = Integer.MaxValue"
        Col.TestModify = Col.MemberName & " - 1"

      Case "money"
        Col.SQLDataType = "money"
        Col.VBDataType = "Decimal"
        Col.Declaration = Col.VBDataType
        Col.ReaderMethod = Col.MemberName & " = .GetDecimal(""" & Col.Name & """)"
        Col.AddWithValue = ".AddWithValue(""@" & Col.Name & """, " & Col.MemberName & ")"
        Col.TestAssign = Col.MemberName
        Col.TestInit = Col.VBDataType & " = 922337203685477.5807D"
        Col.TestModify = Col.MemberName & " - 1"

      Case "nchar"
        Col.SQLDataType = "nchar(" & Col.Size & ")"
        Col.VBDataType = "String"
        Col.Declaration = Col.VBDataType & " = """""
        Col.IsString = True
        Col.ReaderMethod = Col.MemberName & " = .GetString(""" & Col.Name & """)"
        Col.AddWithValue = ".AddWithValue(""@" & Col.Name & """, " & Col.MemberName & ")"
        Col.TestAssign = Col.MemberName
        Col.TestInit = Col.VBDataType & " = GetRandomString(" & Col.Size & ")"
        Col.TestModify = "GetRandomString(" & Col.Size & ")"

      Case "ntext", "text"
        Col.SQLDataType = NativeType
        Col.VBDataType = "String"
        Col.Declaration = Col.VBDataType & " = """""
        Col.IsString = True
        Col.ReaderMethod = Col.MemberName & " = .GetString(""" & Col.Name & """)"
        Col.AddWithValue = ".AddWithValue(""@" & Col.Name & """, " & Col.MemberName & ")"
        Col.TestAssign = Col.MemberName
        Col.TestInit = Col.VBDataType & " = GetRandomString(" & m_MaxEquals & ")"
        Col.TestModify = "GetRandomString(" & m_MaxEquals & ")"
        Col.IsMax = True

      Case "nvarchar", "varchar"
        If Col.IsMax Then
          Col.SQLDataType = NativeType & "(MAX)"
        Else
          Col.SQLDataType = NativeType & "(" & Col.Size & ")"
        End If
        Col.VBDataType = "String"
        Col.Declaration = Col.VBDataType & " = """""
        Col.IsString = True
        Col.ReaderMethod = Col.MemberName & " = .GetString(""" & Col.Name & """)"
        Col.AddWithValue = ".AddWithValue(""@" & Col.Name & """, " & Col.MemberName & ")"
        Col.TestAssign = Col.MemberName
        If Col.IsMax Then
          Col.TestInit = Col.VBDataType & " = GetRandomString(" & m_MaxEquals & ")"
          Col.TestModify = "GetRandomString(" & m_MaxEquals & ")"
        Else
          Col.TestInit = Col.VBDataType & " = GetRandomString(" & Col.Size & ")"
          Col.TestModify = "GetRandomString(" & Col.Size & ")"
        End If

      Case "real"
        Col.SQLDataType = "real"
        Col.VBDataType = "Single"
        Col.Declaration = Col.VBDataType
        Col.ReaderMethod = Col.MemberName & " = .GetFloat(""" & Col.Name & """)"
        Col.AddWithValue = ".AddWithValue(""@" & Col.Name & """, " & Col.MemberName & ")"
        Col.TestAssign = Col.MemberName
        Col.TestInit = Col.VBDataType & " = " & Col.VBDataType & ".MaxValue"
        Col.TestModify = Col.MemberName & " * 0.00001D"

      Case "smallint"
        Col.SQLDataType = "smallint"
        Col.VBDataType = "Short"
        Col.Declaration = Col.VBDataType
        Col.ReaderMethod = Col.MemberName & " = .GetInt16(""" & Col.Name & """)"
        Col.AddWithValue = ".AddWithValue(""@" & Col.Name & """, " & Col.MemberName & ")"
        Col.TestAssign = Col.MemberName
        Col.TestInit = Col.VBDataType & " = " & Col.VBDataType & ".MaxValue"
        Col.TestModify = Col.MemberName & " - 1S"

      Case "smallmoney"
        Col.SQLDataType = "smallmoney"
        Col.VBDataType = "Decimal"
        Col.Declaration = Col.VBDataType
        Col.ReaderMethod = Col.MemberName & " = .GetDecimal(""" & Col.Name & """)"
        Col.AddWithValue = ".AddWithValue(""@" & Col.Name & """, " & Col.MemberName & ")"
        Col.TestAssign = Col.MemberName
        Col.TestInit = Col.VBDataType & " = " & Col.VBDataType & ".MaxValue"
        Col.TestInit = Col.VBDataType & " = 214748.3647D"
        Col.TestModify = Col.MemberName & " - 1"

      Case "timestamp"
        Col.IsReadOnly = True
        Col.SQLDataType = "timestamp"
        Col.IsTimeStamp = True
        Col.VBDataType = "Byte"
        Col.Declaration = Col.VBDataType
        Col.Size = 8 ' <--- something going on with timestamp datatype.  can't read the size from codesmith.  hardcode the bastard.
        Col.ReaderMethod = ".GetBytes(""" & Col.Name & """, 0, " & Col.MemberName & ", 0, " & Col.Size & ")"
        Col.AddWithValue = ".AddWithValue(""@" & Col.Name & """, " & Col.MemberName & ")"
        Col.TestAssign = "-" & Col.MemberName
        Col.TestInit = "Byte"
        Col.TestModify = Col.MemberName
        Col.IsByteArray = True

      Case "tinyint"
        Col.SQLDataType = "tinyint"
        Col.VBDataType = "Byte"
        Col.Declaration = Col.VBDataType
        Col.ReaderMethod = Col.MemberName & " = .GetByte(""" & Col.Name & """)"
        Col.AddWithValue = ".AddWithValue(""@" & Col.Name & """, " & Col.MemberName & ")"
        Col.TestAssign = Col.MemberName
        Col.TestInit = Col.VBDataType & " = " & Col.VBDataType & ".MaxValue"
        Col.TestModify = "CByte(IIf(" & Col.MemberName & " = Byte.MinValue, Byte.MaxValue, Byte.MinValue))"

      Case "uniqueidentifier"
        Col.SQLDataType = "uniqueIdentifier"
        Col.VBDataType = "Guid"
        Col.Declaration = Col.VBDataType & " = Guid.NewGuid"
        Col.IsObject = True
        Col.ReaderMethod = Col.MemberName & " = .GetGuid(""" & Col.Name & """)"
        Col.AddWithValue = ".AddWithValue(""@" & Col.Name & """, " & Col.MemberName & ")"
        Col.TestAssign = Col.MemberName
        Col.TestInit = Col.VBDataType & " = Guid.NewGuid"
        Col.TestModify = "Guid.NewGuid"

      Case "xml"
        Col.SQLDataType = "XML"
        Col.VBDataType = "String"
        Col.Declaration = Col.VBDataType & " = """""
        Col.IsString = True
        Col.ReaderMethod = Col.MemberName & " = .GetString(""" & Col.Name & """)"
        Col.AddWithValue = ".AddWithValue(""@" & Col.Name & """, " & Col.MemberName & ")"
        Col.TestAssign = Col.MemberName
        Col.TestInit = Col.VBDataType & " = ""<x data="""""" & GetRandomString(10) & """""" />"""
        Col.TestModify = """<x data="""""" & GetRandomString(10) & """""" />"""

      Case Else
        Debug.Assert(False, "Unhandled datatype: " & NativeType)

    End Select

    If m_FK_to_Parent.Length > 0 AndAlso Col.Name = m_FK_to_Parent Then
      If m_Object.BOType = BOTypes.Switchable Then
      Else
        Col.AddWithValue = ".AddWithValue(""@" & Col.Name & """, parent." & Col.Name & ")"
      End If
      m_Object.ForeignKey.MemberName = Col.MemberName
      m_Object.ForeignKey.Name = Col.Name
      m_Object.ForeignKey.VBDataType = Col.VBDataType
      Col.IsForeignKeyMember = True
    End If

  End Sub

  Private Function IsPrimaryKey_Member(ByVal Name As String, ByRef Cols() As ColumnDef) As Boolean

    Dim b As Boolean = False

    If Not IsNothing(Cols) Then
      For Each col As ColumnDef In Cols
        If Name = col.Name Then
          b = col.IsPrimaryKeyMember
          Exit For
        End If
      Next
    End If

    Return b

  End Function

  Private Function IsForeignKey_Member(ByVal Name As String, ByRef Cols() As ColumnDef) As Boolean

    Dim b As Boolean = False
    Dim ref As ColumnDef

    For i As Integer = 0 To UBound(Cols)
      ref = Cols(i)
      If Name = ref.Name Then
        b = ref.IsPrimaryKeyMember
        Exit For
      End If
    Next

    Return b

  End Function

  Private Function IsReadOnlyMember(ByVal Name As String, ByRef Cols() As ColumnDef) As Boolean

    Dim b As Boolean = False

    If Not IsNothing(Cols) Then
      For Each col As ColumnDef In Cols
        If Name = col.Name And Not col.IsReadOnly Then
          b = True
          Exit For
        End If
      Next
    End If

    Return Not b

  End Function

  Private Sub CopyMissingColumnDefs(ByVal vwCols() As ColumnDef, ByRef tblCols() As ColumnDef)

    Dim b As Boolean = False
    Dim col As ColumnDef
    Dim fetchcol As ColumnDef

    For i As Integer = 0 To UBound(tblCols)
      col = tblCols(i)
      For j As Integer = 0 To UBound(vwCols)
        fetchcol = vwCols(j)
        If col.Name = fetchcol.Name Then
          b = True
          Exit For
        End If
      Next

      If Not b Then
        ReDim Preserve m_Object.FetchColDefs(UBound(m_Object.FetchColDefs) + 1)
        m_Object.FetchColDefs(UBound(m_Object.FetchColDefs)) = tblCols(i)
        m_Object.FetchColDefs(UBound(m_Object.FetchColDefs)).IsWriteOnly = True
        tblCols(i).IsWriteOnly = True
      End If
      b = False
    Next

  End Sub

  Private Sub OutputGeneratedObject(ByVal sb As StringBuilder, ByVal OutputFileName As String)

    Dim strOutputDir As String = ""

    strOutputDir = m_OutputDirectory

    If Not strOutputDir.EndsWith("\") Then strOutputDir = m_OutputDirectory & "\"

    If m_TodaySubFolder Then
      strOutputDir = strOutputDir & Now.ToString("yyyyMMdd") & "\"
    End If

    If Not Directory.Exists(strOutputDir) Then
      Directory.CreateDirectory(strOutputDir)
    End If

    If Not strOutputDir.EndsWith("\") Then strOutputDir = strOutputDir & "\"

    Dim strFileName As String = ""

    If OutputFileName.Length = 0 Then
      strFileName = m_Object.Name & ".vb"
    Else
      strFileName = OutputFileName
      If Not strFileName.EndsWith(".vb") AndAlso Not strFileName.EndsWith(".SQL") Then
        strFileName = strFileName & ".vb"
      End If
    End If

    Dim fs As New FileStream(strOutputDir & "\" & strFileName, FileMode.Create)
    Dim sw As New StreamWriter(fs)
    sw.Write(sb.ToString)
    sw.Close()

  End Sub

  Private Function DoClass(ByVal ClassName As String, ByVal AsBegin As Boolean) As String

    Dim s As String = ""

    If AsBegin Then
      s = "<Serializable()> _" & vbCrLf & "Public Class " & ClassName & vbCrLf
    Else
      s = "End Class" & vbCrLf
    End If

    Return s

  End Function

  Private Function DoRegion(ByVal RegionName As String, ByVal AsBegin As Boolean) As String

    Dim s As String = ""

    If AsBegin Then
      s = "#Region "" " & RegionName & " """ & vbCrLf & vbCrLf
    Else
      s = "#End Region ' " & RegionName & vbCrLf & vbCrLf
    End If

    Return s

  End Function

  Private Function DoNamespace(ByVal AsBegin As Boolean, ByVal BOName As String) As String

    Dim sb As New System.Text.StringBuilder

    If m_NameSpace.Length > 0 Then
      If AsBegin Then
        sb.Append("Namespace " & m_NameSpace & vbCrLf & vbCrLf)
        If m_AddComments Then sb.Append(GetComments(CommentTypes.XML, BOName))
      Else
        sb.Append("End Namespace" & vbCrLf & vbCrLf)
      End If

    Else
      If AsBegin Then
        If m_AddComments Then sb.Append(GetComments(CommentTypes.XML, BOName))
      End If
    End If

    Return sb.ToString

  End Function

  Private Function GetComments(ByVal CommentType As CommentTypes, ByVal ObjectName As String) As String

    Dim sb As New System.Text.StringBuilder

    If CommentType = CommentTypes.XML Then
      sb.Append("''' <summary>" & vbCrLf)
      sb.Append("''' " & ObjectName & " based on the CSLA.Net framework." & vbCrLf)
      sb.Append("''' Visit http://www.lhotka.net for more info." & vbCrLf)
      sb.Append("''' </summary>" & vbCrLf)
      sb.Append("''' <remarks>" & vbCrLf)
      sb.Append("''' Auto-generated from database entity: " & m_Object.DataSource & vbCrLf)
      sb.Append("''' </remarks>" & vbCrLf)
    ElseIf CommentType = CommentTypes.Generator Then
      sb.Append("'*******************************************************" & vbCrLf)
      sb.Append("' CSLA.Net Framework v2.0.2" & vbCrLf)
      sb.Append("' Generation Tool: CSLA Express v0.1.0" & vbCrLf)
      sb.Append("' Author: " & Environment.UserDomainName & "\" & Environment.UserName & vbCrLf)
      sb.Append("' Creation Date: " & Date.Now.ToString & vbCrLf)
      sb.Append("' Modifications" & vbCrLf)
      sb.Append("'*******************************************************" & vbCrLf)
      sb.Append("'" & vbCrLf)
      sb.Append("'*******************************************************" & vbCrLf & vbCrLf)
    ElseIf CommentType = CommentTypes.SProc Then
      sb.Append("/***************************************************************************" & vbCrLf)
      sb.Append(" * Db Object: " & ObjectName & vbCrLf)
      sb.Append(" * Author: " & Environment.UserDomainName & "\" & Environment.UserName & vbCrLf)
      sb.Append(" * Creation Date: " & Date.Now.ToString & vbCrLf)
      sb.Append(" * Summary: Created to support CSLA Business Object - " & m_Object.Name & vbCrLf)
      sb.Append(" * Example:" & vbCrLf)
      sb.Append(" * Modifications" & vbCrLf)
      sb.Append(" ***************************************************************************" & vbCrLf)
      sb.Append(" * " & vbCrLf)
      sb.Append(" ***************************************************************************/" & vbCrLf)

    Else
      Debug.Assert(False, "Unhandled CommentType: " & CommentType.ToString)
    End If

    Return sb.ToString

  End Function

  Public Function CompileTemplate(ByVal TemplateFile As String) As CodeTemplate

    Dim compiler As New CodeTemplateCompiler(TemplateFile)
    Dim template As CodeTemplate

    compiler.Compile()

    If compiler.Errors.Count = 0 Then
      template = compiler.CreateInstance()
    Else
      template = Nothing
      Throw New Exception("Could not compile " & TemplateFile)
    End If

    Return template

  End Function

#End Region

End Class
