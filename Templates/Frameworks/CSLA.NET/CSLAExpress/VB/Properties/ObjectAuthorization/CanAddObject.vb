
    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category("Authorization Rules - Object Level"), _
  Description("Comma-separated list of the roles that can Add a new business object.")> _
  Public Property CanAddObject() As String
    Get
      Return m_CanAddObject
    End Get
    Set(ByVal Value As String)
      m_CanAddObject = Value
    End Set
  End Property
