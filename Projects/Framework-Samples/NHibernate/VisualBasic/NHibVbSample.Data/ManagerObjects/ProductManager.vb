Imports System
Imports System.Collections.Generic
Imports System.Text

Imports NHibernate
Imports NHibVbSample.Generated.BusinessObjects
Imports NHibVbSample.Generated.Base

Namespace NHibVbSample.Generated.ManagerObjects
	'Public Partial Interface IProductManager
	'	Inherits IManagerBase(Of Product, System.String)
	'End Interface

	Partial Class ProductManager
		Inherits ManagerBase(Of Product, System.String)
		Implements IProductManager
	End Class
End Namespace
