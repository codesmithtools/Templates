
    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category(" Business Object - Parent"), _
  Description("The name of the Business Object's Parent member (optional).")> _
  Public Property ParentName() As String
    Get
      Return m_ParentName
    End Get
    Set(ByVal Value As String)
      m_ParentName = Value
    End Set
  End Property

    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category(" Business Object - Parent"), _
  Description("The Business Object's Parent type (optional).")> _
  Public Property ParentType() As String
    Get
      Return m_ParentType
    End Get
    Set(ByVal Value As String)
      m_ParentType = Value
    End Set
  End Property
