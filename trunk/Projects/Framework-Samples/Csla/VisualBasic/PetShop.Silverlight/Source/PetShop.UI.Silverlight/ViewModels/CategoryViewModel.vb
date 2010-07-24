Imports System.Net
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents
Imports System.Windows.Ink
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Shapes

Imports Csla.Xaml

Imports PetShop.Business

Public Class CategoryViewModel
    Inherits ViewModel(Of Category)
    ''' <summary>
    ''' Gets or sets the Test object.
    ''' </summary>
    Public Shared ReadOnly TestProperty As DependencyProperty = DependencyProperty.Register("Test", GetType(Object), GetType(CategoryViewModel), New PropertyMetadata(Sub(o, e)
                                                                                                                                                                          DirectCast(o, CategoryViewModel).Model = DirectCast(e.NewValue, Category)
                                                                                                                                                                      End Sub))

    ''' <summary>
    ''' Gets or sets the Test object.
    ''' </summary>
    Public Property Test() As Object
        Get
            Return GetValue(TestProperty)
        End Get
        Set(ByVal value As Object)
            SetValue(TestProperty, value)
        End Set
    End Property
End Class