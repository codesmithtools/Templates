
    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category(" General Options - Business Object"), _
  Description("Generate Business Object Stored Procedure(s).")> _
  Public Property Generate_SProcs() As Boolean
    Get
      Return m_GenSProcs
    End Get
    Set(ByVal Value As Boolean)
      m_GenSProcs = Value
    End Set
  End Property
