Imports System
Imports System.Collections
Imports System.Collections.Generic

Imports NHibVbSample.Generated.Base

Namespace NHibVbSample.Generated.BusinessObjects
	Public Partial Class Inventory
		Inherits BusinessBase(Of System.String)

		#region "Declarations"

		Private _qty As Integer = Nothing
		
		
				
        #End Region

        #region "Constructors"

        Public Sub New()
		End Sub

        #End Region

        #region "Methods"

		Public Overloads Overrides Function GetHashCode() As Integer
			Dim sb As New System.Text.StringBuilder()

			sb.Append(Me.[GetType]().FullName)
			
			sb.Append(_qty)
			
			Return sb.ToString().GetHashCode()
		End Function

        #End Region

        #region "Properties"

		Public Overridable Property Qty() As Integer
			Get
				Return _qty
			End Get
			Set
				OnQtyChanging()
				_qty = value
				OnQtyChanged()
			End Set
		End Property
		Partial Private Sub OnQtyChanging()
		End Sub
		Partial Private Sub OnQtyChanged()
		End Sub
		
        #End Region

	End Class
End Namespace
