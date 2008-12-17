Imports System

Namespace Northwind.DAL
	''' <summary>
	''' This object represents the properties and methods of a Employee.
	''' </summary>
	Public Class Employee
		Private _employeeID As Int32
		Private _lastName As String
		Private _firstName As String
		Private _title As String
		Private _titleOfCourtesy As String
		Private _birthDate As DateTime
		Private _hireDate As DateTime
		Private _address As String
		Private _city As String
		Private _region As String
		Private _postalCode As String
		Private _country As String
		Private _homePhone As String
		Private _extension As String
		Private _photo As Byte[]
		Private _notes As String
		Private _reportsTo As Int32
		Private _photoPath As String

		Public Sub New()
		End Sub

		#region Custom - Methods
		' Insert custom methods in here so that they are preserved during re-generation.
#End Region
	End Class
End Namespace

