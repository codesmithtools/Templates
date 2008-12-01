
    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category("Validation Rules"), _
  Description("Auto-generate ValidationRules for the business object.")> _
  Public Property ImplementValidation() As Boolean
    Get
      Return m_ImplementValidation
    End Get
    Set(ByVal Value As Boolean)
      m_ImplementValidation = Value
    End Set
  End Property
