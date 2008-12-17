
    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category(" Business Object"), _
  Description("The name of the Business Object.")> _
  Public Property ObjectName() As String
    Get
      Return m_ObjectName
    End Get
    Set(ByVal Value As String)
      m_ObjectName = Value
    End Set
  End Property
