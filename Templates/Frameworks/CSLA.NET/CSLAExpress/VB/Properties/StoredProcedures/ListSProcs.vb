
    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category("Stored Procedures"), _
  Description("Prefix to the List stored procedure name (optional).")> _
  Public Property ListPrefix() As String
    Get
      Return m_SPListPrefix
    End Get
    Set(ByVal Value As String)
      m_SPListPrefix = Value
    End Set
  End Property

    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category("Stored Procedures"), _
  Description("Suffix to the List stored procedure name (optional).")> _
  Public Property ListSuffix() As String
    Get
      Return m_SPListSuffix
    End Get
    Set(ByVal Value As String)
      m_SPListSUFfix = Value
    End Set
  End Property
