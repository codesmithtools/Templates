Imports System.Collections.Generic
Imports System.Net
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents
Imports System.Windows.Ink
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Shapes

Imports PetShop.Business

Public Class CategoryDetailModel
    Inherits FrameworkElement
    Public Shared ReadOnly SelectedItemsProperty As DependencyProperty = DependencyProperty.Register("SelectedItems", GetType(List(Of Category)), GetType(CategoryDetailModel), Nothing)
    Public Property SelectedItems() As List(Of Category)
        Get
            Return DirectCast(GetValue(SelectedItemsProperty), List(Of Category))
        End Get
        Set(ByVal value As List(Of Category))
            SetValue(SelectedItemsProperty, value)
        End Set
    End Property

    Public Sub Home()
        MainPageModel.ShowForm(New CategoryListPage())
    End Sub
End Class