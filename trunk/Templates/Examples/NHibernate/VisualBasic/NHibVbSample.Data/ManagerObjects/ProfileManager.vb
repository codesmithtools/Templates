Imports System
Imports System.Collections.Generic
Imports System.Text

Imports NHibernate
Imports NHibVbSample.Generated.BusinessObjects
Imports NHibVbSample.Generated.Base

Namespace NHibVbSample.Generated.ManagerObjects
	'Public Partial Interface IProfileManager
	'	Inherits IManagerBase(Of Profile, System.Int32)
	'End Interface

	Partial Class ProfileManager
		Inherits ManagerBase(Of Profile, System.Int32)
		Implements IProfileManager
	End Class
End Namespace
