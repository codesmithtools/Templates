Imports System.ComponentModel
Imports CodeSmith.Engine

Public Class SampleCodeBehindClass
	Inherits CodeTemplate
	Private _propertyFromCodeBehind As Boolean = False

	<Category("Options")> _
	<Description("This property is inherited from the code behind file.")> _
	Public Property PropertyFromCodeBehind() As Boolean
		Get
			Return _propertyFromCodeBehind
		End Get
		Set
			_propertyFromCodeBehind = value
		End Set
	End Property

	Public Function GetSomething() As String
		Return "Something"
	End Function
End Class
