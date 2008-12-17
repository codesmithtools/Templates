
    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category("Stored Procedures"), _
  Description("Prefix to the Exists stored procedure name (optional).")> _
  Public Property ExistsPrefix() As String
    Get
      Return m_SPExistsPrefix
    End Get
    Set(ByVal Value As String)
      m_SPExistsPrefix = Value
    End Set
  End Property

    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category("Stored Procedures"), _
  Description("Suffix to the Exists stored procedure name (optional).")> _
  Public Property ExistsSuffix() As String
    Get
      Return m_SPExistsSuffix
    End Get
    Set(ByVal Value As String)
      m_SPExistsSUFfix = Value
    End Set
  End Property
