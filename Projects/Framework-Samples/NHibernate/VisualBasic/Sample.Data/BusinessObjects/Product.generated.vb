Imports System
Imports System.Collections
Imports System.Collections.Generic

Imports Sample.Data.Generated.Base

Namespace Sample.Data.Generated.BusinessObjects
	Public Partial Class Product
		Inherits BusinessBase(Of System.String)

		#region "Declarations"

		Private _name As String = Nothing
		Private _descn As String = Nothing
		Private _image As String = Nothing
		
		Private _category As Category = Nothing
		
		Private _items As IList(Of Item) = New List(Of Item)()
				
        #End Region

        #region "Constructors"

        Public Sub New()
		End Sub

        #End Region

        #region "Methods"

		Public Overloads Overrides Function GetHashCode() As Integer
			Dim sb As New System.Text.StringBuilder()

			sb.Append(Me.[GetType]().FullName)
			
			sb.Append(_name)
			sb.Append(_descn)
			sb.Append(_image)
			
			Return sb.ToString().GetHashCode()
		End Function

        #End Region

        #region "Properties"

		Public Overridable Property Name() As String
			Get
				Return _name
			End Get
			Set
				OnNameChanging()
				_name = value
				OnNameChanged()
			End Set
		End Property
		Partial Private Sub OnNameChanging()
		End Sub
		Partial Private Sub OnNameChanged()
		End Sub
		Public Overridable Property Descn() As String
			Get
				Return _descn
			End Get
			Set
				OnDescnChanging()
				_descn = value
				OnDescnChanged()
			End Set
		End Property
		Partial Private Sub OnDescnChanging()
		End Sub
		Partial Private Sub OnDescnChanged()
		End Sub
		Public Overridable Property Image() As String
			Get
				Return _image
			End Get
			Set
				OnImageChanging()
				_image = value
				OnImageChanged()
			End Set
		End Property
		Partial Private Sub OnImageChanging()
		End Sub
		Partial Private Sub OnImageChanged()
		End Sub
		
		Public Overridable Property Category() As Category
			Get
				Return _category
			End Get
			Set
				OnCategoryChanging()
				_category = value
				OnCategoryChanged()
			End Set
		End Property
		Partial Private Sub OnCategoryChanging()
		End Sub
		Partial Private Sub OnCategoryChanged()
		End Sub
		
		Public Overridable Property Items() As IList(Of Item)
			Get
				Return _items
			End Get
			Set
				OnItemsChanging()
				_items = value
				OnItemsChanged()
			End Set
		End Property
		Partial Private Sub OnItemsChanging()
		End Sub
		Partial Private Sub OnItemsChanged()
		End Sub
		
        #End Region

	End Class
End Namespace
