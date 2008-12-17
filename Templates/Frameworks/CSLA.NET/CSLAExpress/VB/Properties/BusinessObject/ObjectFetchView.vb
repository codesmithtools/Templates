
  ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category(" Business Object"), _
  Description("View that the Business Object Fetch is based on.")> _
  Public Property ObjectFetchView() As SchemaExplorer.ViewSchema
    Get
      Return m_ObjectFetchDataSource
    End Get
    Set(ByVal Value As SchemaExplorer.ViewSchema)
      m_ObjectFetchDataSource = Value
    End Set
  End Property
