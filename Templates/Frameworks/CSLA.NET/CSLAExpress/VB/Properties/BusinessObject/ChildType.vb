
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
