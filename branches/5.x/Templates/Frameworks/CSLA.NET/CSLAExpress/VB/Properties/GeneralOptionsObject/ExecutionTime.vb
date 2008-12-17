
    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category(" General Options - Business Object"), _
  Description("Add plumbing for dataportal execution elapsed time.")> _
  Public Property ExecutionTime() As Boolean
    Get
      Return m_ExecutionTime
    End Get
    Set(ByVal Value As Boolean)
      m_ExecutionTime = Value
    End Set
  End Property
