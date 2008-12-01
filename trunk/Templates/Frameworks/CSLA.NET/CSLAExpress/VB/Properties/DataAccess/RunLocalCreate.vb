
    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category("Data Access"), _
  Description("The Business Object does not get default values from the server.")> _
  Public Property RunLocalCreate() As Boolean
    Get
      Return m_RunLocalCreate
    End Get
    Set(ByVal Value As Boolean)
      m_RunLocalCreate = Value
    End Set
  End Property
