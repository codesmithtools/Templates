
    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category("Authorization Rules - Object Level"), _
  Description("Comma-separated list of the roles that can Execute this business object.")> _
  Public Property CanExecuteObject() As String
    Get
      Return m_CanExecuteObject
    End Get
    Set(ByVal Value As String)
      m_CanExecuteObject = Value
    End Set
  End Property
