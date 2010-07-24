Imports System.Windows
Imports System.Windows.Controls

Namespace PetShop.UI.Silverlight
    Public Class MainPageModel
        Inherits FrameworkElement
        Public Shared ReadOnly CurrentControlProperty As DependencyProperty = DependencyProperty.Register("CurrentControl", GetType(UserControl), GetType(MainPageModel), New PropertyMetadata(Nothing))

        Public Property CurrentControl() As UserControl
            Get
                Return DirectCast(GetValue(CurrentControlProperty), UserControl)
            End Get
            Set(ByVal value As UserControl)
                SetValue(CurrentControlProperty, value)
            End Set
        End Property

        Private Shared _main As MainPageModel
        Public Sub New()
            _main = Me
            CurrentControl = New CategoryListPage()
        End Sub

        Public Shared Sub ShowForm(ByVal form As UserControl)
            _main.CurrentControl = form
        End Sub
    End Class
End Namespace