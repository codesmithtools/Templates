
    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category("Authorization Rules - Properties Level"), _
  Description("Comma-separated list of the roles not allowed to read this business object's properties.")> _
  Public Property DenyRead() As String
    Get
      Return m_DenyRead
    End Get
    Set(ByVal Value As String)
      m_DenyRead = Value
    End Set
  End Property
