
    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category("Command Object"), _
  Description("The command object's shared method name.")> _
  Public Property SharedMethodName() As String
    Get
      Return m_CommandMethod
    End Get
    Set(ByVal Value As String)
      m_CommandMethod = Value
    End Set
  End Property
