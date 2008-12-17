
  ' The variable referenced in this property is defined in the CSLAHelper20.vb file
  <Category("Business Methods"), _
  Description("Business Object properties changed....")> _
  Public Property PropertyHasChanged() As PropHasChangedMethods
    Get
      Return m_PropHasChangedMethods
    End Get
    Set(ByVal Value As PropHasChangedMethods)
      m_PropHasChangedMethods = Value
    End Set
  End Property
