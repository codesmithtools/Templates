Imports System
Imports System.Collections
Imports System.Collections.Generic

Imports Sample.Data.Generated.Base

Namespace Sample.Data.Generated.BusinessObjects
	Public Partial Class Category
		Inherits BusinessBase(Of System.String)

		#region "Declarations"

		Private _name As String = Nothing
		Private _descn As String = Nothing
		
		
		Private _products As IList(Of Product) = New List(Of Product)()
				
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
		
		Public Overridable Property Products() As IList(Of Product)
			Get
				Return _products
			End Get
			Set
				OnProductsChanging()
				_products = value
				OnProductsChanged()
			End Set
		End Property
		Partial Private Sub OnProductsChanging()
		End Sub
		Partial Private Sub OnProductsChanged()
		End Sub
		
        #End Region

	End Class
End Namespace
