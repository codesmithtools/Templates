
  ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category("Business Methods"), _
  Description("Can Read Properties of the Business Object.")> _
  Public Property CanReadProperty() As PropAuthMethods
    Get
      Return m_CanReadPropMethods
    End Get
    Set(ByVal Value As PropAuthMethods)
      m_CanReadPropMethods = Value
    End Set
  End Property

  ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category("Business Methods"), _
  Description("Can Write Properties of the Business Object.")> _
  Public Property CanWriteProperty() As PropAuthMethods
    Get
      Return m_CanWritePropMethods
    End Get
    Set(ByVal Value As PropAuthMethods)
      m_CanWritePropMethods = Value
    End Set
  End Property

