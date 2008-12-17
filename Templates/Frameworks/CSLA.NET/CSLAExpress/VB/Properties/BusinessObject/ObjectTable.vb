
  ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category(" Business Object"), _
  Description("Table that the Business Object is based on.")> _
  Public Property ObjectTable() As SchemaExplorer.TableSchema
    Get
      Return m_ObjectTable
    End Get
    Set(ByVal Value As SchemaExplorer.TableSchema)
      m_ObjectTable = Value
    End Set
  End Property
