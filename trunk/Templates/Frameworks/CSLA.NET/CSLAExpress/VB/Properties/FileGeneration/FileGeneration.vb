
'  ' The variable referenced in this property is defined in the CSLAHelper20.vb file
'  <Category(" File Generation"), _
'  EditorAttribute(GetType(System.Windows.Forms.Design.FolderNameEditor), _
'  GetType(System.Drawing.Design.UITypeEditor)), _
'  Description("Directory for the generated files.")> _
'  Public Property OutputDirectory() As String
'    Get
'      Return m_OutputDirectory
'    End Get
'    Set(ByVal Value As String)
'      m_OutputDirectory = Value
'    End Set
'  End Property

    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category(" File Generation"), _
  Description("Directory for the generated files.")> _
  Public Property OutputDirectory() As String
    Get
      Return m_OutputDirectory
    End Get
    Set(ByVal Value As String)
      m_OutputDirectory = Value
    End Set
  End Property

    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category(" File Generation"), _
  Description("Output generated code to file.")> _
  Public Property OutputToFile() As Boolean
    Get
      Return m_OutputToFile
    End Get
    Set(ByVal Value As Boolean)
      m_OutputToFile = Value
    End Set
  End Property

    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category(" File Generation"), _
  Description("Create subfolders with date.")> _
  Public Property DateAsSubFolder() As Boolean
    Get
      Return m_TodaySubFolder
    End Get
    Set(ByVal Value As Boolean)
      m_TodaySubFolder = Value
    End Set
  End Property
