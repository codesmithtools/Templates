Imports System
Imports System.Collections.Generic
Imports System.Text

Imports NHibernate
Imports Sample.Data.Generated.BusinessObjects
Imports Sample.Data.Generated.Base

Namespace Sample.Data.Generated.ManagerObjects
	'Public Partial Interface ISupplierManager
	'	Inherits IManagerBase(Of Supplier, System.Int32)
	'End Interface

	Partial Class SupplierManager
		Inherits ManagerBase(Of Supplier, System.Int32)
		Implements ISupplierManager
	End Class
End Namespace
