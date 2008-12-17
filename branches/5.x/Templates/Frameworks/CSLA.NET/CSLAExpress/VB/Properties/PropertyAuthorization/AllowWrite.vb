
    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category("Authorization Rules - Properties Level"), _
  Description("Comma-separated list of the roles allowed to write this business object's properties.")> _
  Public Property AllowWrite() As String
    Get
      Return m_AllowWrite
    End Get
    Set(ByVal Value As String)
      m_AllowWrite = Value
    End Set
  End Property
