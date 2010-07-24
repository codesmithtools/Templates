Imports System.Linq
Imports System.Windows
Imports System.Windows.Controls

Imports Csla.Xaml

Imports PetShop.Business

Namespace PetShop.UI.Silverlight
    Public Class CategoryListViewModel
        Inherits ViewModel(Of CategoryList)
#Region "Constructor"

        Public Sub New()
            If System.ComponentModel.DesignerProperties.GetIsInDesignMode(Me) Then
                Return
            End If

			Me.PropertyChanged += Function(o, e) Do
            If e.PropertyName = "Error" AndAlso [Error] IsNot Nothing Then
                MessageBox.Show([Error].ToString(), "Data error", MessageBoxButton.OK)
            End If
        End Sub

			Load()
		End Sub

#End Region

#Region "Methods"

        Public Sub Load()
            BeginRefresh("GetAllAsync")
        End Sub

        Public Sub LoadByID(ByVal id As String)
            BeginRefresh("GetByCategoryIdAsync", id)
        End Sub

        Public Sub ShowItem(ByVal sender As Object, ByVal e As ExecuteEventArgs)
            SelectedData = DirectCast(e.MethodParameter, Category)
        End Sub

        Public Sub ProcessItemsTrigger(ByVal sender As Object, ByVal e As ExecuteEventArgs)
            ' copy selected items into known list type
            Dim selection = DirectCast(e.MethodParameter, System.Collections.IEnumerable).Cast(Of Category)().ToList()

            ' display detail form
            Dim form = New CategoryDetailPage()
            Dim vm = TryCast(form.Resources("ViewModel"), CategoryDetailModel)
            If vm IsNot Nothing Then
                vm.SelectedItems = selection
            End If

            MainPageModel.ShowForm(form)
        End Sub

        Public Sub ProcessItemsExecute(ByVal sender As Object, ByVal e As ExecuteEventArgs)
            ' copy selected items to known list type
            Dim listBox = TryCast(e.TriggerSource.Tag, ListBox)

            If listBox IsNot Nothing Then
                Dim selection = listBox.SelectedItems.Cast(Of Category)().ToList()

                ' process selection
                Dim form = New CategoryDetailPage()
                Dim vm = TryCast(form.Resources("ViewModel"), CategoryDetailModel)
                If vm IsNot Nothing Then
                    vm.SelectedItems = selection
                End If

                MainPageModel.ShowForm(form)
            End If
        End Sub

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets or sets the SelectedData object.
        ''' </summary>
        Public Shared ReadOnly SelectedDataProperty As DependencyProperty = DependencyProperty.Register("SelectedData", GetType(Category), GetType(CategoryListViewModel), New PropertyMetadata(Nothing))

        ''' <summary>
        ''' Gets or sets the SelectedData object.
        ''' </summary>
        Public Property SelectedData() As Category
            Get
                Return DirectCast(GetValue(SelectedDataProperty), Category)
            End Get
            Set(ByVal value As Category)
                SetValue(SelectedDataProperty, value)
                OnPropertyChanged("SelectedData")
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the SelectedItems object.
        ''' </summary>
        Public Shared ReadOnly SelectedItemsProperty As DependencyProperty = DependencyProperty.Register("SelectedItems", GetType(System.Collections.ObjectModel.ObservableCollection(Of Object)), GetType(CategoryListViewModel), New PropertyMetadata(Nothing))

        ''' <summary>
        ''' Gets or sets the SelectedItems object.
        ''' </summary>
        Public Property SelectedItems() As System.Collections.ObjectModel.ObservableCollection(Of Object)
            Get
                Return DirectCast(GetValue(SelectedItemsProperty), System.Collections.ObjectModel.ObservableCollection(Of Object))
            End Get
            Set(ByVal value As System.Collections.ObjectModel.ObservableCollection(Of Object))
                SetValue(SelectedItemsProperty, value)
                OnPropertyChanged("SelectedItems")
            End Set
        End Property


        Public Shared ReadOnly FilterProperty As DependencyProperty = DependencyProperty.Register("Filter", GetType(String), GetType(CategoryDetailModel), Nothing)
        Public Property Filter() As String
            Get
                Return DirectCast(GetValue(FilterProperty), String)
            End Get
            Set(ByVal value As String)
                SetValue(FilterProperty, value)
            End Set
        End Property

#End Region
    End Class
End Namespace