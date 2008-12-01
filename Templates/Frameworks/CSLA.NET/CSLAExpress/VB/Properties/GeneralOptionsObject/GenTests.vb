
    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category(" General Options - Business Object"), _
  Description("Generate Business Object Test(s).")> _
  Public Property Generate_Tests() As Boolean
    Get
      Return m_GenTests
    End Get
    Set(ByVal Value As Boolean)
      m_GenTests = Value
    End Set
  End Property
