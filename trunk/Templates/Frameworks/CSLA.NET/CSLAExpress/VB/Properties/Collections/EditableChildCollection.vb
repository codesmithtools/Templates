
    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category(" Business Object - Editable Child Collection"), _
  Description("The name of the Editable Child Collection.")> _
  Public Property CollectionName() As String
    Get
      Return m_ObjectName
    End Get
    Set(ByVal Value As String)
      m_ObjectName = Value
    End Set
  End Property

    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category(" Business Object - Editable Child Collection"), _
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
  <Category(" Business Object - Editable Child Collection"), _
  Description("The parent type containing this collection.")> _
  Public Property ParentType() As String
    Get
      Return m_ParentType
    End Get
    Set(ByVal Value As String)
      m_ParentType = Value
    End Set
  End Property

  ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category(" Business Object - Editable Child Collection"), _
  Description("Table that the Business Object Child is based on.")> _
  Public Property ChildTable() As SchemaExplorer.TableSchema
    Get
      Return m_ObjectTable
    End Get
    Set(ByVal Value As SchemaExplorer.TableSchema)
      m_ObjectTable = Value
    End Set
  End Property

    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category(" Business Object - Editable Child Collection"), _
  Description("Collection members are Switchable objects.")> _
  Public Property SwitchableChild() As Boolean
    Get
      Return m_SwitchableChildMembers
    End Get
    Set(ByVal Value As Boolean)
      m_SwitchableChildMembers = Value
    End Set
  End Property

