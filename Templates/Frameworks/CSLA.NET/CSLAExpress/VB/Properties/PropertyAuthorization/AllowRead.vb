
    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category("Authorization Rules - Properties Level"), _
  Description("Comma-separated list of the roles allowed to read this business object's properties.")> _
  Public Property AllowRead() As String
    Get
      Return m_AllowRead
    End Get
    Set(ByVal Value As String)
      m_AllowRead = Value
    End Set
  End Property
