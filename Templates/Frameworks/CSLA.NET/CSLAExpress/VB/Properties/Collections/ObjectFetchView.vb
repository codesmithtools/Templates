'  ' The variable referenced in this property is defined in the CSLAHelper20.vb file
'  <Category(" Business Object - Read Only Root Collection"), _
'  Description("Table that the Business Object Child is based on.")> _
'  Public Property ObjectTable() As SchemaExplorer.TableSchema
'    Get
'      Return m_ObjectTable
'    End Get
'    Set(ByVal Value As SchemaExplorer.TableSchema)
'      m_ObjectTable = Value
'    End Set
'  End Property

  ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category(" Business Object - Read Only Root Collection"), _
  Description("View that the Business Object Child is based on.")> _
  Public Property ObjectFetchView() As SchemaExplorer.ViewSchema
    Get
      Return m_ObjectFetchDataSource
    End Get
    Set(ByVal Value As SchemaExplorer.ViewSchema)
      m_ObjectFetchDataSource = Value
    End Set
  End Property
