
    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category(" General Options - Database"), _
  Description("Database Connection String.")> _
  Public Property ConnectionString() As String
    Get
      Return m_ConnectionString
    End Get
    Set(ByVal Value As String)
      m_ConnectionString = Value
    End Set
  End Property

    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category(" General Options - Database"), _
  Description("Database owner of generated SProcs (optional).")> _
  Public Property DatabaseObjectOwner() As String
    Get
      Return m_DatabaseObjectOwner
    End Get
    Set(ByVal Value As String)
      m_DatabaseObjectOwner = Value
    End Set
  End Property

    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category(" General Options - Database"), _
  Description("Comma-separated list of database Users to grant execute permission.")> _
  Public Property GrantExecute() As String
    Get
      Return m_GrantExecute
    End Get
    Set(ByVal Value As String)
      m_GrantExecute = Value
    End Set
  End Property

    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category(" General Options - Database"), _
  Description("Generate database DROP statements for stored procedures.")> _
  Public Property Use_DROP_Statements() As Boolean
    Get
      Return m_SProc_GenerateDROP
    End Get
    Set(ByVal Value As Boolean)
      m_SProc_GenerateDROP = Value
    End Set
  End Property
