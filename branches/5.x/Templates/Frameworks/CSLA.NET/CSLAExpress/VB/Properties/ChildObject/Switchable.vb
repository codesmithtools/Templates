
    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category(" Business Object - Switchable"), _
  Description("The name of the Switchable Business Object.")> _
  Public Property ObjectName() As String
    Get
      Return m_ObjectName
    End Get
    Set(ByVal Value As String)
      m_ObjectName = Value
    End Set
  End Property

    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category(" Business Object - Switchable"), _
  Description("The Switchable Business Object's Parent Type.")> _
  Public Property ParentType() As String
    Get
      Return m_ParentType
    End Get
    Set(ByVal Value As String)
      m_ParentType = Value
    End Set
  End Property

    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category(" Business Object - Switchable"), _
  Description("The Switchable Business Object's foreign key to the parent.")> _
  Public Property FK_to_Parent() As String
    Get
      Return m_FK_to_Parent
    End Get
    Set(ByVal Value As String)
      m_FK_to_Parent = Value
    End Set
  End Property

  ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category(" Business Object - Switchable"), _
  Description("Table that the Switchable Business Object is based on.")> _
  Public Property ObjectTable() As SchemaExplorer.TableSchema
    Get
      Return m_ObjectTable
    End Get
    Set(ByVal Value As SchemaExplorer.TableSchema)
      m_ObjectTable = Value
    End Set
  End Property