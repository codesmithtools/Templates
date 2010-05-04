
USE PetShop;
GO

IF OBJECT_ID ( 'dbo.GetAllAccounts', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.GetAllAccounts;
GO

CREATE PROCEDURE dbo.GetAllAccounts

--region Custom Parameters

--endregion

AS

--region Custom Preprocessing Logic

--endregion

    SELECT * FROM dbo.Account
    
--region Custom Postprocessing Logic

--endregion

GO