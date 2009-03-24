Imports System
Imports System.Collections.Generic
Imports System.Text

Imports NHibernate
Imports NHibVbSample.Generated.BusinessObjects
Imports NHibVbSample.Generated.Base

Namespace NHibVbSample.Generated.ManagerObjects
	'Public Partial Interface ICategoryManager
	'	Inherits IManagerBase(Of Category, System.String)
	'End Interface

	Partial Class CategoryManager
		Inherits ManagerBase(Of Category, System.String)
		Implements ICategoryManager
	End Class
End Namespace
