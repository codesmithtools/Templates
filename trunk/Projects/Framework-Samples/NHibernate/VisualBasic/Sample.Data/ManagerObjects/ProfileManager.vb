Imports System
Imports System.Collections.Generic
Imports System.Text

Imports NHibernate
Imports Sample.Data.Generated.BusinessObjects
Imports Sample.Data.Generated.Base

Namespace Sample.Data.Generated.ManagerObjects
	'Public Partial Interface IProfileManager
	'	Inherits IManagerBase(Of Profile, System.Int32)
	'End Interface

	Partial Class ProfileManager
		Inherits ManagerBase(Of Profile, System.Int32)
		Implements IProfileManager
	End Class
End Namespace
