
    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category(" Business Object - Read Only Root Collection"), _
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
  <Category(" Business Object - Read Only Root Collection"), _
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
  <Category(" Business Object - Read Only Root Collection"), _
  Description("Collection uses an internal public structure to represent child members.")> _
  Public Property UseStructureForChild() As Boolean
    Get
      Return m_UseStructureForChild
    End Get
    Set(ByVal Value As Boolean)
      m_UseStructureForChild = Value
    End Set
  End Property

