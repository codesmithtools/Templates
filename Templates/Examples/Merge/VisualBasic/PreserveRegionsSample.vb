Public Class PreserveRegionsSample

#Region "Custom Region 1"
	
	' This is a place holder for your custom code.
	' It must exist so that CodeSmith knows where
	' to put the custom code that will be parsed
	' from the target source file.
	' The region name is used to match up the regions
	' and determine where each region of custom code
	' should be inserted into the merge result.
	
#End Region


    Public Sub SomeGeneratedMethod()

        ' This section and all other non-custom code
        ' regions will be overwritten during each
        ' template execution.
        ' Current Date: 7/27/2009 2:02:07 PM

    End Sub


#Region "Custom Region 2"

    ' The contents will also be preserved
    ' during generation.

#End Region

End Class
