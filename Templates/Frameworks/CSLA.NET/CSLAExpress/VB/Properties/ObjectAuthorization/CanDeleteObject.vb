
    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category("Authorization Rules - Object Level"), _
  Description("Comma-separated list of the roles that can delete the business object.")> _
  Public Property CanDeleteObject() As String
    Get
      Return m_CanDeleteObject
    End Get
    Set(ByVal Value As String)
      m_CanDeleteObject = Value
    End Set
  End Property
