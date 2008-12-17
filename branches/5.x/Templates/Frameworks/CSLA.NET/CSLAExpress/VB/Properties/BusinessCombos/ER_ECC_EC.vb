
  ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category(" Business Object Combo"), _
  Description("Table that the Business Object is based on.")> _
  Public Property RootObjectTable() As SchemaExplorer.TableSchema
    Get
      Return m_ObjectTable
    End Get
    Set(ByVal Value As SchemaExplorer.TableSchema)
      m_ObjectTable = Value
    End Set
  End Property

  ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category(" Business Object Combo"), _
  Description("Table that the Business Object Child is based on.")> _
  Public Property ChildTable() As SchemaExplorer.TableSchema
    Get
      Return m_ChildTable
    End Get
    Set(ByVal Value As SchemaExplorer.TableSchema)
      m_ChildTable = Value
    End Set
  End Property

    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category(" Business Object Combo"), _
  Description("The name of the Business Object.")> _
  Public Property RootObjectName() As String
    Get
      Return m_ObjectName
    End Get
    Set(ByVal Value As String)
      m_ObjectName = Value
    End Set
  End Property

    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category(" Business Object Combo"), _
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
  <Category(" Business Object Combo"), _
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
  <Category(" Business Object Combo"), _
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
  <Category(" Business Object Combo"), _
  Description("The Business Object Child's foreign key to the parent.")> _
  Public Property FK_to_Parent() As String
    Get
      Return m_FK_to_Parent
    End Get
    Set(ByVal Value As String)
      m_FK_to_Parent = Value
    End Set
  End Property
