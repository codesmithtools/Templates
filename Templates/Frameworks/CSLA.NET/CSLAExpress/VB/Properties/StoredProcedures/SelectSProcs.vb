
    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category("Stored Procedures"), _
  Description("Prefix to the Select stored procedure name (optional).")> _
  Public Property SelectPrefix() As String
    Get
      Return m_SPSelectPrefix
    End Get
    Set(ByVal Value As String)
      m_SPSelectPrefix = Value
    End Set
  End Property

    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category("Stored Procedures"), _
  Description("Suffix to the Select stored procedure name (optional).")> _
  Public Property SelectSuffix() As String
    Get
      Return m_SPSelectSuffix
    End Get
    Set(ByVal Value As String)
      m_SPSelectSUFfix = Value
    End Set
  End Property
