Imports System
Imports System.Collections
Imports System.Configuration
Imports System.Data
Imports System.Linq
Imports System.Web
Imports System.Web.Security
Imports System.Web.SessionState
Imports System.Xml.Linq
Imports System.Web.Routing
Imports System.Web.DynamicData
Imports Petshop.Data.Petshop.Data


Public Class Global_asax
	Inherits System.Web.HttpApplication

    Public Shared Sub RegisterRoutes(ByVal routes As RouteCollection)
	Dim model As New MetaModel

        model.RegisterContext(GetType(Petshop.Data.PetshopDataContext), New ContextConfiguration() With {.ScaffoldAllTables = False})

        routes.Add(New DynamicDataRoute("{table}/{action}.aspx") With { _
    		.Constraints = New RouteValueDictionary(New With {.Action = "List|Details|Edit|Insert"}), _
    		.Model = model})
    End Sub

    Private Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
	RegisterRoutes(RouteTable.Routes)
	End Sub

End Class
