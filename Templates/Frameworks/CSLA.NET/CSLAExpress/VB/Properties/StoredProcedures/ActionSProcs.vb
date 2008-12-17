
    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category("Stored Procedures"), _
  Description("Prefix to the Delete stored procedure name (optional).")> _
  Public Property DeletePrefix() As String
    Get
      Return m_SPDeletePrefix
    End Get
    Set(ByVal Value As String)
      m_SPDeletePrefix = Value
    End Set
  End Property

    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category("Stored Procedures"), _
  Description("Suffix to the Delete stored procedure name (optional).")> _
  Public Property DeleteSuffix() As String
    Get
      Return m_SPDeleteSuffix
    End Get
    Set(ByVal Value As String)
      m_SPDeleteSUFfix = Value
    End Set
  End Property


    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category("Stored Procedures"), _
  Description("Prefix to the Insert stored procedure name (optional).")> _
  Public Property InsertPrefix() As String
    Get
      Return m_SPInsertPrefix
    End Get
    Set(ByVal Value As String)
      m_SPInsertPrefix = Value
    End Set
  End Property

    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category("Stored Procedures"), _
  Description("Suffix to the Insert stored procedure name (optional).")> _
  Public Property InsertSuffix() As String
    Get
      Return m_SPInsertSuffix
    End Get
    Set(ByVal Value As String)
      m_SPInsertSUFfix = Value
    End Set
  End Property

    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category("Stored Procedures"), _
  Description("Prefix to the Update stored procedure name (optional).")> _
  Public Property UpdatePrefix() As String
    Get
      Return m_SPUpdatePrefix
    End Get
    Set(ByVal Value As String)
      m_SPUpdatePrefix = Value
    End Set
  End Property

    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category("Stored Procedures"), _
  Description("Suffix to the Update stored procedure name (optional).")> _
  Public Property UpdateSuffix() As String
    Get
      Return m_SPUpdateSuffix
    End Get
    Set(ByVal Value As String)
      m_SPUpdateSUFfix = Value
    End Set
  End Property

