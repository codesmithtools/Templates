
    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category(" Business Object"), _
  Description("The Business Object's Child Type.")> _
  Public Property ChildType() As String
    Get
      Return m_ChildType
    End Get
    Set(ByVal Value As String)
      m_ChildType = Value
    End Set
  End Property

    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category(" Business Object"), _
  Description("The Business Object's Child Collection Type.")> _
  Public Property ChildCollectionType() As String
    Get
      Return m_ChildColType
    End Get
    Set(ByVal Value As String)
      m_ChildColType = Value
    End Set
  End Property

    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category(" Business Object"), _
  Description("The Business Object's Child Collection Name.")> _
  Public Property ChildCollectionName() As String
    Get
      Return m_ChildColName
    End Get
    Set(ByVal Value As String)
      m_ChildColName = Value
    End Set
  End Property

  ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category(" Business Object"), _
  Description("Table that the Business Object Child is based on.")> _
  Public Property ChildTable() As SchemaExplorer.TableSchema
    Get
      Return m_ChildTable
    End Get
    Set(ByVal Value As SchemaExplorer.TableSchema)
      m_ChildTable = Value
    End Set
  End Property
