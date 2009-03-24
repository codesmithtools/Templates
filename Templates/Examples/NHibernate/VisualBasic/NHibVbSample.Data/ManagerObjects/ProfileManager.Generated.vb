Imports System
Imports System.Collections.Generic
Imports System.Text

Imports NHibernate
Imports NHibVbSample.Generated.BusinessObjects
Imports NHibVbSample.Generated.Base

Namespace NHibVbSample.Generated.ManagerObjects
	Public Interface IProfileManager
		Inherits IManagerBase(Of Profile, System.Int32)
		
		' Get Methods
		Function GetByUsernameApplicationName(ByVal username As System.String, ByVal applicationName As System.String) As IList(Of Profile)
		
	End Interface

	Partial Class ProfileManager
		Inherits ManagerBase(Of Profile, System.Int32)
		Implements IProfileManager
#region "Constructors"

		Public Sub New()
			MyBase.New()
		End Sub
		Public Sub New(ByVal session As INHibernateSession)
			MyBase.New(session)
		End Sub
#End Region

#region "Get Methods"

		Public Function GetByUsernameApplicationName(ByVal username As System.String, ByVal applicationName As System.String) As IList(Of Profile) Implements IProfileManager.GetByUsernameApplicationName
            Dim criteria As ICriteria = Session.GetISession().CreateCriteria(GetType(Profile))
			
			
			criteria.Add(NHibernate.Criterion.Expression.Eq("Username", username))
			
			criteria.Add(NHibernate.Criterion.Expression.Eq("ApplicationName", applicationName))
						
            return criteria.List(Of Profile)()
        End Function
		
#End Region
	End Class
End Namespace
