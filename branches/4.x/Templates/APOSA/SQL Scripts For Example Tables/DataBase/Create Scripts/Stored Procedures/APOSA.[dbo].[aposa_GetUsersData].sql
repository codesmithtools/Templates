
/****** Object:  Procedure [dbo].[aposa_GetUsersData]    Script Date: 04/17/2007 10:00:51 AM ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[aposa_GetUsersData]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[aposa_GetUsersData]
GO

/*********************************************************************************************
 
 Created By: This code was generated by APOSA CodeSmith Domain Object Template.
 Date:    04/17/2007
 Time:    10:00 AM
 Version: 4.0.0.0

 Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.

 Procedure Name: [dbo].[aposa_GetUsersData]
 
 Description: User Table
 
 Parameters: 
		@IsActive bit

       
--region RevisionLog
***************************************  Revision Log  ***************************************
 
Version   Date        Revised By       Description / WO#
--------  ----------  ---------------  -------------------------------------------------------
   
**********************************************************************************************/
--endregion
CREATE PROCEDURE [dbo].[aposa_GetUsersData]
	@IsActive bit


AS

SET NOCOUNT ON

SELECT
		[UserID] AS 'UserID',
		[UserName] AS 'UserName',
		[DateCreated] AS 'DateCreated',
		[IsActive] AS 'IsActive',
		[Random Column 1] AS 'RandomColumn1',
		[Random_Column2] AS 'RandomColumn2'

FROM
	[dbo].[User]
WHERE
		[IsActive] = @IsActive

--region [dbo].[aposa_GetUsersData]
------------------------------------------------------------------------------------------------------------------------");
-- Generated By:   cl1 using CodeSmith 4.0.0.0
-- Template:       SelectStoredProcedure.cst
-- Procedure Name: [dbo].[aposa_GetUsersData]
-- Date Generated: Tuesday, April 17, 2007
------------------------------------------------------------------------------------------------------------------------");
--endregion

