﻿public class PreserveRegionsSample
{

#region "Custom Region 1"
	
	// This is a place holder for your custom code.
	// It must exist so that CodeSmith knows where
	// to put the custom code that will be parsed
	// from the target source file.
	// The region name is used to match up the regions
	// and determine where each region of custom code
	// should be inserted into the merge result.
	
#endregion


    public void SomeGeneratedMethod()
	{

        // This section and all other non-custom code
        // regions will be overwritten during each
        // template execution.
        // Current Date: Wednesday, November 27, 2013
	}


#region "Custom Region 2"

    // The contents of this region will also be preserved
    // during generation.

#endregion

}
