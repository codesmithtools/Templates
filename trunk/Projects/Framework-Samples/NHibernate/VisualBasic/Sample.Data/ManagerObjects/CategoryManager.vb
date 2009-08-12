Imports System
Imports System.Collections.Generic
Imports System.Text

Imports NHibernate
Imports Sample.Data.Generated.BusinessObjects
Imports Sample.Data.Generated.Base

Namespace Sample.Data.Generated.ManagerObjects
	'Public Partial Interface ICategoryManager
	'	Inherits IManagerBase(Of Category, System.String)
	'End Interface

	Partial Class CategoryManager
		Inherits ManagerBase(Of Category, System.String)
		Implements ICategoryManager
	End Class
End Namespace
