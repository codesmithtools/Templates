Option Strict On
Option Explicit On
Option Compare Text

Imports SchemaExplorer
Imports CSLAExpress20.CSLAExpress20.BOTypes

Module Main

  Public DataBase As DatabaseSchema

  Sub Main()
    ' *************************************
    ' YOU MUST COMMENT OUT LINE 20 OF
    ' THE CSLA CLASS FILE BEFORE DEVELOPING
    ' WITHIN VISUAL STUDIO...
    ' *************************************
    DataBase = New DatabaseSchema(New SqlSchemaProvider(), _
      "Data Source=SQLServer;Initial Catalog=DBName;Integrated Security=true;")

    Dim CSLAExpress As New CSLAExpress20

    With CSLAExpress
      '.SetEnv(EditableRoot, DataBase.Tables("tblEmployees"), DataBase.Views("vw_Employees"), Nothing, Nothing) ' DataBase.Tables("tblEmployees"), Nothing)
      '.SetEnv(EditableRoot, DataBase.Tables("tblEmployees"), Nothing, Nothing, Nothing) ' DataBase.Tables("tblEmployees"), Nothing)

      '.SetEnv(EditableRoot, DataBase.Tables("tblTypical"), Nothing, Nothing, Nothing) ' DataBase.Tables("tblEmployees"), Nothing)
      ''.SetEnv(EditableRoot, DataBase.Tables("Integer_Autos"), Nothing, DataBase.Tables("Integer_Parts"), Nothing) ' DataBase.Tables("tblEmployees"), Nothing)
      ''.SetEnv(EditableRoot, DataBase.Tables("GUID_Autos"), Nothing, DataBase.Tables("GUID_Parts"), Nothing) ' DataBase.Tables("tblEmployees"), Nothing)
      ''.SetEnv(EditableRoot, DataBase.Tables("tblPositions"), Nothing, DataBase.Tables("tblEmployees"), Nothing) ' DataBase.Tables("tblEmployees"), Nothing)
      '.SetEnv(EditableRoot, DataBase.Tables("tblPositions"), Nothing, Nothing, Nothing)
      '.SetEnv(EditableRoot, DataBase.Tables("tblPositions"), DataBase.Views("vw_Positions"), Nothing, Nothing)
      '.Generate("EditableRoot")

      '.SetEnv(EditableChildCollection, Nothing, Nothing, Nothing, Nothing)
      '.SetEnv(EditableChildCollection, DataBase.Tables("GUID_Parts"), Nothing, Nothing, Nothing)
      '.SetEnv(EditableChildCollection, DataBase.Tables("Integer_Parts"), Nothing, Nothing, Nothing)
      '.SetEnv(EditableChildCollection, DataBase.Tables("tblEmployees"), Nothing, Nothing, Nothing)
      '.Generate("EditableChildCollection")

      '.SetEnv(EditableChild, DataBase.Tables("tblEmployees"), Nothing, Nothing, Nothing)
      '.SetEnv(EditableChild, DataBase.Tables("GUID_Parts"), Nothing, Nothing, Nothing)
      '.SetEnv(EditableChild, DataBase.Tables("Integer_Parts"), Nothing, Nothing, Nothing)
      '.SetEnv(EditableChild, DataBase.Tables("tblPositions"), Nothing, Nothing, Nothing)
      '.Generate("EditableChild")

      '.SetEnv(ReadOnlyChild, DataBase.Tables("tblPositions"), Nothing, Nothing, Nothing)
      '.Generate("ReadOnlyChild")

      '.SetEnv(Switchable, DataBase.Tables("tblEmployees"), Nothing, Nothing, Nothing)
      '.Generate("Switchable")

      '.SetEnv(EditableRootCollection, DataBase.Tables("tblPositions"), Nothing, Nothing, Nothing)
      '.Generate("EditableRootCollection")

      .SetEnv(ReadOnlyObject, DataBase.Tables("tblPositions"), Nothing, Nothing, Nothing)
      .Generate("ReadOnlyObject")

      '.SetEnv(ReadOnlyCollection, Nothing, DataBase.Views("vw_Projects"), Nothing, Nothing)
      '.SetEnv(ReadOnlyCollection, Nothing, DataBase.Views("vw_Positions"), Nothing, Nothing)
      '      .SetEnv(ReadOnlyCollection, DataBase.Tables("tblPositions"), Nothing, Nothing, Nothing)
      '.Generate("ReadOnlyCollection")

      '.SetEnv(NameValueList, DataBase.Tables("tblEmployees"), Nothing, Nothing, Nothing)
      '.Generate("NameValueList")

      '.SetEnv(CommandObject, Nothing, Nothing, Nothing, Nothing)
      '.Generate("CommandObject")

    End With

  End Sub

End Module
