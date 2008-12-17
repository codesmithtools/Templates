Imports System.Web.DynamicData

Partial Class ChildrenField
    Inherits System.Web.DynamicData.FieldTemplateUserControl

        
    Private _allowNavigation As Boolean = true
    
    Private _navigateUrl As String
    
    Public Property NavigateUrl As String
        Get
            Return _navigateUrl
        End Get
        Set
            _navigateUrl = value
        End Set
    End Property
    
    Public Property AllowNavigation As Boolean
        Get
            Return _allowNavigation
        End Get
        Set
            _allowNavigation = value
        End Set
    End Property
    
    Public Overrides ReadOnly Property DataControl As Control
        Get
            Return HyperLink1
        End Get
    End Property
    
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
        HyperLink1.Text = ("View " + ChildrenColumn.ChildTable.DisplayName)
    End Sub
    
    Protected Function GetChildrenPath() As String
        If Not AllowNavigation Then
            Return Nothing
        End If
        If String.IsNullOrEmpty(NavigateUrl) Then
            Return ChildrenPath
        Else
            Return BuildChildrenPath(NavigateUrl)
        End If
    End Function


End Class
