
    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category("Authorization Rules - Object Level"), _
  Description("Comma-separated list of the roles that can edit the business object.")> _
  Public Property CanEditObject() As String
    Get
      Return m_CanEditObject
    End Get
    Set(ByVal Value As String)
      m_CanEditObject = Value
    End Set
  End Property
