Imports System.Web.DynamicData

Partial Class FilterUserControl
    Inherits System.Web.DynamicData.FilterUserControlBase

        
    Public Overrides ReadOnly Property SelectedValue As String
        Get
            Return DropDownList1.SelectedValue
        End Get
    End Property
    
    Public Event SelectedIndexChanged As EventHandler
    
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs)
        If Not Page.IsPostBack Then
            PopulateListControl(DropDownList1)
            ' Set the initial value if there is one
            If Not String.IsNullOrEmpty(InitialValue) Then
                DropDownList1.SelectedValue = InitialValue
            End If
        End If
    End Sub


End Class
