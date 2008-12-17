
    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category(" General Options - Business Object"), _
  Description("The prefix for the member variables.")> _
  Public Property MemberPrefix() As String
    Get
      Return m_MemberPrefix
    End Get
    Set(ByVal Value As String)
      m_MemberPrefix = Value
    End Set
  End Property

    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category(" General Options - Business Object"), _
  Description("Namespace.")> _
  Public Property ObjectNameSpace() As String
    Get
      Return m_NameSpace
    End Get
    Set(ByVal Value As String)
      m_NameSpace = Value
    End Set
  End Property

    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category(" General Options - Business Object"), _
  Description("Camel-case member variables.")> _
  Public Property CamelCaseMemberVars() As Boolean
    Get
      Return m_CamelCaseMemberVars
    End Get
    Set(ByVal Value As Boolean)
      m_CamelCaseMemberVars = Value
    End Set
  End Property

    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category(" General Options - Business Object"), _
  Description("Use SmartDate instead of Date.")> _
  Public Property UseSmartDate() As Boolean
    Get
      Return m_UseSmartDate
    End Get
    Set(ByVal Value As Boolean)
      m_UseSmartDate = Value
    End Set
  End Property
 
'    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
'  <Category(" General Options - Business Object"), _
'  Description("Business object reacts to non-zero return values from stored procedures.")> _
'  Public Property UseSP_ReturnValue() As Boolean
'    Get
'      Return m_UseSP_ReturnValue
'    End Get
'    Set(ByVal Value As Boolean)
'      m_UseSP_ReturnValue = Value
'    End Set
'  End Property

    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category(" General Options - Business Object"), _
  Description("Generate Business Object Class.")> _
  Public Property Generate_Class() As Boolean
    Get
      Return m_GenClass
    End Get
    Set(ByVal Value As Boolean)
      m_GenClass = Value
    End Set
  End Property

'    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
'  <Category(" General Options - Business Object"), _
'  Description("Create an Exists method within the Business Object.")> _
'  Public Property Implement_Exists() As Boolean
'    Get
'      Return m_Implement_Exists
'    End Get
'    Set(ByVal Value As Boolean)
'      m_Implement_Exists = Value
'    End Set
'  End Property

    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category(" General Options - Business Object"), _
  Description("Allow anonymous access when no access controls exist.")> _
  Public Property AnonymousAccess() As Boolean
    Get
      Return m_AnonymousAccess
    End Get
    Set(ByVal Value As Boolean)
      m_AnonymousAccess = Value
    End Set
  End Property

    ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category(" General Options - Business Object"), _
  Description("Add comments to generated code.")> _
  Public Property AddComments() As Boolean
    Get
      Return m_AddComments
    End Get
    Set(ByVal Value As Boolean)
      m_AddComments = Value
    End Set
  End Property
