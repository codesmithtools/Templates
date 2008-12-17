
    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category("Authorization Rules - Object Level"), _
  Description("Comma-separated list of the roles that can Get this business object.")> _
  Public Property CanGetObject() As String
    Get
      Return m_CanGetObject
    End Get
    Set(ByVal Value As String)
      m_CanGetObject = Value
    End Set
  End Property
