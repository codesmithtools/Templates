
Imports System
Imports System.ComponentModel
Imports System.Runtime.Serialization

Namespace Petshop.Data
    ''' <summary>
    ''' A base class for Linq entities that implements notification events.
    ''' </summary>
    <Serializable> _
    <DataContract( IsReference:=True )> _
    Public Partial MustInherit Class LinqEntityBase 
       Implements INotifyPropertyChanging
       Implements INotifyPropertyChanged
        ''' <summary>
        ''' Initializes a new instance of the <see cref="LinqEntityBase"/> class.
        ''' </summary>
        Protected Sub New()
        End Sub

        #Region "Notification Events"

        ''' <summary>
        ''' Implements a PropertyChanged event.
        ''' </summary>
        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        ''' <summary>
        ''' Raise the PropertyChanged event for a specific property.
        ''' </summary>
        ''' <param name="propertyName">Name of the property that has changed.</param>
        <EditorBrowsable(EditorBrowsableState.Advanced)> _
        Protected Sub OnPropertyChanged(ByVal propertyName As String)
            RaiseEvent    PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
        End Sub


        ''' <summary>
        ''' Implements a PropertyChanging event.
        ''' </summary>
        Public Event PropertyChanging As PropertyChangingEventHandler Implements INotifyPropertyChanging.PropertyChanging

        ''' <summary>
        ''' Raise the PropertyChanging event for a specific property.
        ''' </summary>
        ''' <param name="propertyName">Name of the property that is changing.</param>
        <EditorBrowsable(EditorBrowsableState.Advanced)> _
        Protected Sub OnPropertyChanging(ByVal propertyName As String)
            RaiseEvent PropertyChanging(Me, New PropertyChangingEventArgs(propertyName))
        End Sub
        #End Region
    End Class
End Namespace