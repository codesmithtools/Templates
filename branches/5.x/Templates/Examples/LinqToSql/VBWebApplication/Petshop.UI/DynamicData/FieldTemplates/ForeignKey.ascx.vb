Imports System.Web.DynamicData

Partial Class ForeignKeyField
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
    
    Protected Function GetDisplayString() As String
        Return FormatFieldValue(ForeignKeyColumn.ParentTable.GetDisplayString(FieldValue))
    End Function
    
    Protected Function GetNavigateUrl() As String
        If Not AllowNavigation Then
            Return Nothing
        End If
        If String.IsNullOrEmpty(NavigateUrl) Then
            Return ForeignKeyPath
        Else
            Return BuildForeignKeyPath(NavigateUrl)
        End If
    End Function


End Class
