Public Class PreserveRegionsSample

#Region "Custom Region 1"

    ' This is my custom code that I want to preserve.
    ' I can make changes to it and my changes will
    ' not be overwritten.

#End Region


    Public Sub SomeGeneratedMethod()

        ' This section and all other non-custom code
        ' regions will be overwritten during each
        ' template execution.
        ' Current Date: 11/3/2008 9:24:28 AM

    End Sub


#Region "Custom Region 2"

    ' The contents of this region will also be preserved
    ' during generation.

#End Region

End Class
