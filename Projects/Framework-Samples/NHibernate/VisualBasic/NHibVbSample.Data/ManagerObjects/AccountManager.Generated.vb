Imports System
Imports System.Collections.Generic
Imports System.Text

Imports NHibernate
Imports NHibVbSample.Generated.BusinessObjects
Imports NHibVbSample.Generated.Base

Namespace NHibVbSample.Generated.ManagerObjects
	Public Interface IAccountManager
		Inherits IManagerBase(Of Account, System.Int32)
		
		' Get Methods
		Function GetByUniqueID(ByVal profile As System.Int32) As IList(Of Account)
		
	End Interface

	Partial Class AccountManager
		Inherits ManagerBase(Of Account, System.Int32)
		Implements IAccountManager
#region "Constructors"

		Public Sub New()
			MyBase.New()
		End Sub
		Public Sub New(ByVal session As INHibernateSession)
			MyBase.New(session)
		End Sub
#End Region

#region "Get Methods"

		Public Function GetByUniqueID(ByVal profile As System.Int32) As IList(Of Account) Implements IAccountManager.GetByUniqueID
            Dim criteria As ICriteria = Session.GetISession().CreateCriteria(GetType(Account))
			
			
			Dim profileCriteria As ICriteria = criteria.CreateCriteria("Profile")
            profileCriteria.Add(NHibernate.Criterion.Expression.Eq("Id", profile))
						
            return criteria.List(Of Account)()
        End Function
		
#End Region
	End Class
End Namespace
