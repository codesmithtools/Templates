
    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category("NameValueList"), _
  Description("Database column to use as the 'Name' in the NVL.")> _
  Public Property NVL_Name_Column() As String
    Get
      Return m_NVL_NameField
    End Get
    Set(ByVal Value As String)
      m_NVL_NameField = Value
    End Set
  End Property
