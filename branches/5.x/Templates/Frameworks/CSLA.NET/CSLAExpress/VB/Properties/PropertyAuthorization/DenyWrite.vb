
    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category("Authorization Rules - Properties Level"), _
  Description("Comma-separated list of the roles not allowed to write this business object's properties.")> _
  Public Property DenyWrite() As String
    Get
      Return m_DenyWrite
    End Get
    Set(ByVal Value As String)
      m_DenyWrite = Value
    End Set
  End Property
