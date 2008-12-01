
    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category(" Business Object Combo"), _
  Description("The name of the Read Only Root Collection.")> _
  Public Property CollectionName() As String
    Get
      Return m_ObjectName
    End Get
    Set(ByVal Value As String)
      m_ObjectName = Value
    End Set
  End Property

    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category(" Business Object Combo"), _
  Description("The child type contained in collection.")> _
  Public Property ChildType() As String
    Get
      Return m_ChildType
    End Get
    Set(ByVal Value As String)
      m_ChildType = Value
    End Set
  End Property

  ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category(" Business Object Combo"), _
  Description("Table that the Business Object Child is based on.")> _
  Public Property ObjectTable() As SchemaExplorer.TableSchema
    Get
      Return m_ObjectTable
    End Get
    Set(ByVal Value As SchemaExplorer.TableSchema)
      m_ObjectTable = Value
    End Set
  End Property
