Imports System.Data.Services
Imports System.Linq
Imports System.ServiceModel.Web

Public Class PetshopDataService
    ' TODO: replace Petshop.Data.PetshopDataContext with your data class name
    Inherits DataService(Of Petshop.Data.PetshopDataContext)

    ' This method is called only once to initialize service-wide policies.
    Public Shared Sub InitializeService(ByVal config As IDataServiceConfiguration)
        ' TODO: set rules to indicate which entity sets and service operations are visible, updatable, etc.
        ' Examples:
        ' config.SetEntitySetAccessRule("MyEntityset", EntitySetRights.AllRead)
        ' config.SetServiceOperationAccessRule("MyServiceOperation", ServiceOperationRights.All)
    End Sub

End Class
