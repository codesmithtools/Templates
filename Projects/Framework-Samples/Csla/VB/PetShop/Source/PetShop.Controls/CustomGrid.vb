Imports System.Web.UI.WebControls
Imports System.Web.UI
Imports System.Text.RegularExpressions

Public Class CustomGrid
    Inherits Repeater

    'Static constants
    Protected Const HTML1 As String = "<table cellpadding=0 cellspacing=0><tr><td colspan=2>"
    Protected Const HTML2 As String = "</td></tr><tr><td class=paging align=left>"
    Protected Const HTML3 As String = "</td><td align=right class=paging>"
    Protected Const HTML4 As String = "</td></tr></table>"
    Private Shared ReadOnly RX As New Regex("^&page=\d+", RegexOptions.Compiled)
    Private Const LINK_PREV As String = "<a href=?page={0}>&#060;&nbsp;Previous</a>"
    Private Const LINK_MORE As String = "<a href=?page={0}>More&nbsp;&#062;</a>"
    Private Const KEY_PAGE As String = "page"
    Private Const COMMA As String = "?"
    Private Const AMP As String = "&"

    Protected _emptyText As String
    Private _dataSource As IList
    Private _pageSize As Integer = 10
    Private _currentPageIndex As Integer
    Private _itemCount As Integer

    Public Overrides Property DataSource() As Object
        Set(ByVal value As Object)
            'This try catch block is to avoid issues with the VS.NET designer
            'The designer will try and bind a datasource which does not derive from ILIST
            Try
                _dataSource = DirectCast(value, IList)
                _itemCount = DataSource.Count
            Catch
                _dataSource = Nothing
                _itemCount = 0
            End Try
        End Set
        Get
            Return _dataSource
        End Get
    End Property

    Public Property PageSize() As Integer
        Get
            Return _pageSize
        End Get
        Set(ByVal value As Integer)
            _pageSize = value
        End Set
    End Property

    Protected ReadOnly Property PageCount() As Integer
        Get
            Return (_itemCount - 1) / _pageSize
        End Get
    End Property

    Protected Overridable Property ItemCount() As Integer
        Get
            Return _itemCount
        End Get
        Set(ByVal value As Integer)
            _itemCount = value
        End Set
    End Property

    Public Overridable Property CurrentPageIndex() As Integer
        Get
            Return _currentPageIndex
        End Get
        Set(ByVal value As Integer)
            _currentPageIndex = value
        End Set
    End Property

    Public WriteOnly Property EmptyText() As String
        Set(ByVal value As String)
            _emptyText = value
        End Set
    End Property

    Public Sub SetPage(ByVal index As Integer)
        OnPageIndexChanged(New DataGridPageChangedEventArgs(Nothing, index))
    End Sub

    Protected Overrides Sub OnLoad(ByVal e As EventArgs)
        If Visible Then
            Dim page As String = Context.Request(KEY_PAGE)
            Dim index As Integer = If((page <> Nothing), Integer.Parse(page), 0)
            SetPage(index)
        End If
    End Sub

    ''' <summary>
    ''' Overriden method to control how the page is rendered
    ''' </summary>
    ''' <param name="writer"></param>
    Protected Overrides Sub Render(ByVal writer As HtmlTextWriter)

        'Check there is some data attached
        If _itemCount = 0 Then
            writer.Write(_emptyText)
            Return
        End If

        'Mask the query
        Dim query As String = Context.Request.Url.Query.Replace(COMMA, AMP)
        query = RX.Replace(query, String.Empty)

        ' Write out the first part of the control, the table header
        writer.Write(HTML1)

        ' Call the inherited method
        MyBase.Render(writer)

        ' Write out a table row closure
        writer.Write(HTML2)

        'Determin whether next and previous buttons are required
        'Previous button?
        If _currentPageIndex > 0 Then
            writer.Write(String.Format(LINK_PREV, (_currentPageIndex - 1).ToString() + query))
        End If

        'Close the table data tag
        writer.Write(HTML3)

        'Next button?
        If CurrentPageIndex < PageCount Then
            writer.Write(String.Format(LINK_MORE, (_currentPageIndex + 1).ToString() + query))
        End If

        'Close the table
        writer.Write(HTML4)
    End Sub

    Protected Overrides Sub OnDataBinding(ByVal e As EventArgs)

        'Work out which items we want to render to the page
        Dim start As Integer = _currentPageIndex * _pageSize
        Dim size As Integer = Math.Min(_pageSize, _itemCount - start)

        Dim page As IList = New ArrayList()

        'Add the relevant items from the datasource
        Dim i As Integer = 0
        While i < size
            page.Add(DataSource(start + i))
            System.Math.Max(System.Threading.Interlocked.Increment(i), i - 1)
        End While

        'set the base objects datasource
        DataSource = page
        MyBase.OnDataBinding(e)

    End Sub

    Public Event PageIndexChanged As DataGridPageChangedEventHandler

    Protected Overridable Sub OnPageIndexChanged(ByVal e As DataGridPageChangedEventArgs)
        RaiseEvent PageIndexChanged(Me, e)
    End Sub

End Class
