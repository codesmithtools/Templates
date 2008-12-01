
    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category("Stored Procedures"), _
  Description("Prefix to the NameValueList stored procedure name (optional).")> _
  Public Property NameValueListPrefix() As String
    Get
      Return m_SPNVLPrefix
    End Get
    Set(ByVal Value As String)
      m_SPNVLPrefix = Value
    End Set
  End Property

    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category("Stored Procedures"), _
  Description("Suffix to the NameValueList stored procedure name (optional).")> _
  Public Property NameValueListSuffix() As String
    Get
      Return m_SPNVLSuffix
    End Get
    Set(ByVal Value As String)
      m_SPNVLSuffix = Value
    End Set
  End Property
