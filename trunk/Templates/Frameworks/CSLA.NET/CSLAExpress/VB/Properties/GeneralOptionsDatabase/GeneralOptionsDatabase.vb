
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

    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category(" General Options - Database"), _
  Description("Currency error message to return from stored procedure when a concurrency event occurs.")> _
  Public Property ConcurrencyErrMsg() As String
    Get
      Return m_ConcurrencyErrMsg
    End Get
    Set(ByVal Value As String)
      m_ConcurrencyErrMsg = Value
    End Set
  End Property

    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category(" General Options - Database"), _
  Description("Select Transactional Support Type for Deletion")> _
  Public Property TransationalType_Delete() As TransactionalTypes
    Get
      Return m_DeleteTransationalType
    End Get
    Set(ByVal Value As TransactionalTypes)
      m_DeleteTransationalType = Value
    End Set
  End Property

    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category(" General Options - Database"), _
  Description("Select Transactional Support Type for Insert")> _
  Public Property TransationalType_Insert() As TransactionalTypes
    Get
      Return m_InsertTransationalType
    End Get
    Set(ByVal Value As TransactionalTypes)
      m_InsertTransationalType = Value
    End Set
  End Property

    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category(" General Options - Database"), _
  Description("Select Transactional Support Type for Update")> _
  Public Property TransationalType_Update() As TransactionalTypes
    Get
      Return m_UpdateTransationalType
    End Get
    Set(ByVal Value As TransactionalTypes)
      m_UpdateTransationalType = Value
    End Set
  End Property

