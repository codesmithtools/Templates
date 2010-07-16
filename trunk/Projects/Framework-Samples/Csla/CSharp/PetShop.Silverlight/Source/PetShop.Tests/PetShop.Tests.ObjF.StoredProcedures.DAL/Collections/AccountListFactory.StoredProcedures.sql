--region Drop Existing Procedures

SET NUMERIC_ROUNDABORT OFF
GO
SET ANSI_PADDING, ANSI_WARNINGS, CONCAT_NULL_YIELDS_NULL, ARITHABORT, QUOTED_IDENTIFIER, ANSI_NULLS ON
GO
IF EXISTS (SELECT * FROM tempdb..sysobjects WHERE id=OBJECT_ID('tempdb..#tmpErrors')) DROP TABLE #tmpErrors
GO
CREATE TABLE #tmpErrors (Error int)
GO
SET XACT_ABORT ON
GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO
BEGIN TRANSACTION
GO
PRINT N'Dropping CSLA_Account_Insert'
GO
IF EXISTS(SELECT 1 FROM fn_listextendedproperty (NULL, 'SCHEMA', 'dbo', 'PROCEDURE', 'CSLA_Account_Insert', default, default) WHERE name = 'CustomProcedure' and value = '1')
BEGIN
    RAISERROR ('The procedure CSLA_Account_Insert has an Extended Property "CustomProcedure" which means is has been customized. Please review and remove the property if you wish to drop the procedure.',16,1)
    INSERT INTO #tmpErrors (Error) SELECT 1
END
GO

IF OBJECT_ID(N'CSLA_Account_Insert') IS NOT NULL
	DROP PROCEDURE CSLA_Account_Insert

GO
IF @@ERROR!=0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

IF EXISTS (SELECT * FROM #tmpErrors) ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT>0 BEGIN
PRINT 'The stored procedure drop has succeeded'
COMMIT TRANSACTION
END
ELSE PRINT 'The stored procedure drop has failed'
GO

DROP TABLE #tmpErrors
GO
SET NUMERIC_ROUNDABORT OFF
GO
SET ANSI_PADDING, ANSI_WARNINGS, CONCAT_NULL_YIELDS_NULL, ARITHABORT, QUOTED_IDENTIFIER, ANSI_NULLS ON
GO
IF EXISTS (SELECT * FROM tempdb..sysobjects WHERE id=OBJECT_ID('tempdb..#tmpErrors')) DROP TABLE #tmpErrors
GO
CREATE TABLE #tmpErrors (Error int)
GO
SET XACT_ABORT ON
GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO
BEGIN TRANSACTION
GO
PRINT N'Dropping CSLA_Account_Update'
GO
IF EXISTS(SELECT 1 FROM fn_listextendedproperty (NULL, 'SCHEMA', 'dbo', 'PROCEDURE', 'CSLA_Account_Update', default, default) WHERE name = 'CustomProcedure' and value = '1')
BEGIN
    RAISERROR ('The procedure CSLA_Account_Update has an Extended Property "CustomProcedure" which means is has been customized. Please review and remove the property if you wish to drop the procedure.',16,1)
    INSERT INTO #tmpErrors (Error) SELECT 1
END
GO

IF OBJECT_ID(N'CSLA_Account_Update') IS NOT NULL
	DROP PROCEDURE CSLA_Account_Update

GO
IF @@ERROR!=0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

IF EXISTS (SELECT * FROM #tmpErrors) ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT>0 BEGIN
PRINT 'The stored procedure drop has succeeded'
COMMIT TRANSACTION
END
ELSE PRINT 'The stored procedure drop has failed'
GO

DROP TABLE #tmpErrors
GO
SET NUMERIC_ROUNDABORT OFF
GO
SET ANSI_PADDING, ANSI_WARNINGS, CONCAT_NULL_YIELDS_NULL, ARITHABORT, QUOTED_IDENTIFIER, ANSI_NULLS ON
GO
IF EXISTS (SELECT * FROM tempdb..sysobjects WHERE id=OBJECT_ID('tempdb..#tmpErrors')) DROP TABLE #tmpErrors
GO
CREATE TABLE #tmpErrors (Error int)
GO
SET XACT_ABORT ON
GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO
BEGIN TRANSACTION
GO
PRINT N'Dropping CSLA_Account_Delete'
GO
IF EXISTS(SELECT 1 FROM fn_listextendedproperty (NULL, 'SCHEMA', 'dbo', 'PROCEDURE', 'CSLA_Account_Delete', default, default) WHERE name = 'CustomProcedure' and value = '1')
BEGIN
    RAISERROR ('The procedure CSLA_Account_Delete has an Extended Property "CustomProcedure" which means is has been customized. Please review and remove the property if you wish to drop the procedure.',16,1)
    INSERT INTO #tmpErrors (Error) SELECT 1
END
GO

IF OBJECT_ID(N'CSLA_Account_Delete') IS NOT NULL
	DROP PROCEDURE CSLA_Account_Delete

GO
IF @@ERROR!=0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

IF EXISTS (SELECT * FROM #tmpErrors) ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT>0 BEGIN
PRINT 'The stored procedure drop has succeeded'
COMMIT TRANSACTION
END
ELSE PRINT 'The stored procedure drop has failed'
GO

DROP TABLE #tmpErrors
GO
SET NUMERIC_ROUNDABORT OFF
GO
SET ANSI_PADDING, ANSI_WARNINGS, CONCAT_NULL_YIELDS_NULL, ARITHABORT, QUOTED_IDENTIFIER, ANSI_NULLS ON
GO
IF EXISTS (SELECT * FROM tempdb..sysobjects WHERE id=OBJECT_ID('tempdb..#tmpErrors')) DROP TABLE #tmpErrors
GO
CREATE TABLE #tmpErrors (Error int)
GO
SET XACT_ABORT ON
GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO
BEGIN TRANSACTION
GO
PRINT N'Dropping CSLA_Account_Select'
GO
IF EXISTS(SELECT 1 FROM fn_listextendedproperty (NULL, 'SCHEMA', 'dbo', 'PROCEDURE', 'CSLA_Account_Select', default, default) WHERE name = 'CustomProcedure' and value = '1')
BEGIN
    RAISERROR ('The procedure CSLA_Account_Select has an Extended Property "CustomProcedure" which means is has been customized. Please review and remove the property if you wish to drop the procedure.',16,1)
    INSERT INTO #tmpErrors (Error) SELECT 1
END
GO

IF OBJECT_ID(N'CSLA_Account_Select') IS NOT NULL
	DROP PROCEDURE CSLA_Account_Select

GO
IF @@ERROR!=0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

IF EXISTS (SELECT * FROM #tmpErrors) ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT>0 BEGIN
PRINT 'The stored procedure drop has succeeded'
COMMIT TRANSACTION
END
ELSE PRINT 'The stored procedure drop has failed'
GO

DROP TABLE #tmpErrors
GO
--endregion

GO


--region [dbo].[CSLA_Account_Insert]

------------------------------------------------------------------------------------------------------------------------
-- Generated By:   Blake Niemyjski using CodeSmith: v5.2.2, CSLA Templates: v3.0.0.1817, CSLA Framework: v4.0.0
-- Procedure Name: [dbo].[CSLA_Account_Insert]
------------------------------------------------------------------------------------------------------------------------

SET NUMERIC_ROUNDABORT OFF
GO
SET ANSI_PADDING, ANSI_WARNINGS, CONCAT_NULL_YIELDS_NULL, ARITHABORT, QUOTED_IDENTIFIER, ANSI_NULLS ON
GO
IF EXISTS (SELECT * FROM tempdb..sysobjects WHERE id=OBJECT_ID('tempdb..#tmpErrors')) DROP TABLE #tmpErrors
GO
CREATE TABLE #tmpErrors (Error int)
GO
SET XACT_ABORT ON
GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO
BEGIN TRANSACTION
GO

PRINT N'Creating [dbo].[CSLA_Account_Insert]'
GO

IF EXISTS(SELECT 1 FROM fn_listextendedproperty (NULL, 'SCHEMA', 'dbo', 'PROCEDURE', 'CSLA_Account_Insert', default, default) WHERE name = 'CustomProcedure' and value = '1')
    BEGIN
        RAISERROR ('The procedure [dbo].[CSLA_Account_Insert] has an Extended Property "CustomProcedure" which means is has been customized. Please review and remove the property if you wish to create the stored procedure.',16,1)
        INSERT INTO #tmpErrors (Error) SELECT 1
    END
GO

CREATE PROCEDURE [dbo].[CSLA_Account_Insert]
	@p_UniqueID int,
	@p_Email varchar(80),
	@p_FirstName varchar(80),
	@p_LastName varchar(80),
	@p_Address1 varchar(80),
	@p_Address2 varchar(80),
	@p_City varchar(80),
	@p_State varchar(80),
	@p_Zip varchar(20),
	@p_Country varchar(20),
	@p_Phone varchar(20),
	@p_AccountId int OUTPUT
AS

INSERT INTO [dbo].[Account] (
	[UniqueID],
	[Email],
	[FirstName],
	[LastName],
	[Address1],
	[Address2],
	[City],
	[State],
	[Zip],
	[Country],
	[Phone]    
) VALUES (
	@p_UniqueID,
	@p_Email,
	@p_FirstName,
	@p_LastName,
	@p_Address1,
	@p_Address2,
	@p_City,
	@p_State,
	@p_Zip,
	@p_Country,
	@p_Phone)

SET @p_AccountId = SCOPE_IDENTITY()



GO
IF @@ERROR!=0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

IF EXISTS (SELECT * FROM #tmpErrors) ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT>0 BEGIN
PRINT 'Stored procedure creation succeeded.'
COMMIT TRANSACTION
END
ELSE PRINT 'Stored procedure creation failed.'
GO
DROP TABLE #tmpErrors
GO

--endregion

GO


--region [dbo].[CSLA_Account_Update]

------------------------------------------------------------------------------------------------------------------------
-- Generated By:   Blake Niemyjski using CodeSmith: v5.2.2, CSLA Templates: v3.0.0.1817, CSLA Framework: v4.0.0
-- Procedure Name: [dbo].[CSLA_Account_Update]
------------------------------------------------------------------------------------------------------------------------

SET NUMERIC_ROUNDABORT OFF
GO
SET ANSI_PADDING, ANSI_WARNINGS, CONCAT_NULL_YIELDS_NULL, ARITHABORT, QUOTED_IDENTIFIER, ANSI_NULLS ON
GO
IF EXISTS (SELECT * FROM tempdb..sysobjects WHERE id=OBJECT_ID('tempdb..#tmpErrors')) DROP TABLE #tmpErrors
GO
CREATE TABLE #tmpErrors (Error int)
GO
SET XACT_ABORT ON
GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO
BEGIN TRANSACTION
GO

PRINT N'Creating [dbo].[CSLA_Account_Update]'
GO

IF EXISTS(SELECT 1 FROM fn_listextendedproperty (NULL, 'SCHEMA', 'dbo', 'PROCEDURE', 'CSLA_Account_Update', default, default) WHERE name = 'CustomProcedure' and value = '1')
    BEGIN
        RAISERROR ('The procedure [dbo].[CSLA_Account_Update] has an Extended Property "CustomProcedure" which means is has been customized. Please review and remove the property if you wish to create the stored procedure.',16,1)
        INSERT INTO #tmpErrors (Error) SELECT 1
    END
GO

CREATE PROCEDURE [dbo].[CSLA_Account_Update]
	@p_AccountId int,
	@p_UniqueID int,
	@p_Email varchar(80),
	@p_FirstName varchar(80),
	@p_LastName varchar(80),
	@p_Address1 varchar(80),
	@p_Address2 varchar(80),
	@p_City varchar(80),
	@p_State varchar(80),
	@p_Zip varchar(20),
	@p_Country varchar(20),
	@p_Phone varchar(20)
AS

UPDATE [dbo].[Account] SET
	[UniqueID] = @p_UniqueID,
	[Email] = @p_Email,
	[FirstName] = @p_FirstName,
	[LastName] = @p_LastName,
	[Address1] = @p_Address1,
	[Address2] = @p_Address2,
	[City] = @p_City,
	[State] = @p_State,
	[Zip] = @p_Zip,
	[Country] = @p_Country,
	[Phone] = @p_Phone
WHERE
	[AccountId] = @p_AccountId


GO
IF @@ERROR!=0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

IF EXISTS (SELECT * FROM #tmpErrors) ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT>0 BEGIN
PRINT 'Stored procedure creation succeeded.'
COMMIT TRANSACTION
END
ELSE PRINT 'Stored procedure creation failed.'
GO
DROP TABLE #tmpErrors
GO

--endregion

GO

--region [dbo].[CSLA_Account_Delete]

------------------------------------------------------------------------------------------------------------------------
-- Generated By:   Blake Niemyjski using CodeSmith: v5.2.2, CSLA Templates: v3.0.0.1817, CSLA Framework: v4.0.0
-- Procedure Name: [dbo].[CSLA_Account_Delete]
------------------------------------------------------------------------------------------------------------------------

SET NUMERIC_ROUNDABORT OFF
GO
SET ANSI_PADDING, ANSI_WARNINGS, CONCAT_NULL_YIELDS_NULL, ARITHABORT, QUOTED_IDENTIFIER, ANSI_NULLS ON
GO
IF EXISTS (SELECT * FROM tempdb..sysobjects WHERE id=OBJECT_ID('tempdb..#tmpErrors')) DROP TABLE #tmpErrors
GO
CREATE TABLE #tmpErrors (Error int)
GO
SET XACT_ABORT ON
GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO
BEGIN TRANSACTION
GO

PRINT N'Creating [dbo].[CSLA_Account_Delete]'
GO

IF EXISTS(SELECT 1 FROM fn_listextendedproperty (NULL, 'SCHEMA', 'dbo', 'PROCEDURE', 'CSLA_Account_Delete', default, default) WHERE name = 'CustomProcedure' and value = '1')
    BEGIN
        RAISERROR ('The procedure [dbo].[CSLA_Account_Delete] has an Extended Property "CustomProcedure" which means is has been customized. Please review and remove the property if you wish to create the stored procedure.',16,1)
        INSERT INTO #tmpErrors (Error) SELECT 1
    END
GO

CREATE PROCEDURE [dbo].[CSLA_Account_Delete]
	@p_AccountId int
AS

DELETE FROM
    [dbo].[Account]
WHERE
	[AccountId] = @p_AccountId

GO
IF @@ERROR!=0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

IF EXISTS (SELECT * FROM #tmpErrors) ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT>0 BEGIN
PRINT 'Stored procedure creation succeeded.'
COMMIT TRANSACTION
END
ELSE PRINT 'Stored procedure creation failed.'
GO
DROP TABLE #tmpErrors
GO

--endregion

GO

--region [dbo].[CSLA_Account_Select]

------------------------------------------------------------------------------------------------------------------------
-- Generated By:   Blake Niemyjski using CodeSmith: v5.2.2, CSLA Templates: v3.0.0.1817, CSLA Framework: v4.0.0
-- Procedure Name: [dbo].[CSLA_Account_Select]
------------------------------------------------------------------------------------------------------------------------

SET NUMERIC_ROUNDABORT OFF
GO
SET ANSI_PADDING, ANSI_WARNINGS, CONCAT_NULL_YIELDS_NULL, ARITHABORT, QUOTED_IDENTIFIER, ANSI_NULLS ON
GO
IF EXISTS (SELECT * FROM tempdb..sysobjects WHERE id=OBJECT_ID('tempdb..#tmpErrors')) DROP TABLE #tmpErrors
GO
CREATE TABLE #tmpErrors (Error int)
GO
SET XACT_ABORT ON
GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO
BEGIN TRANSACTION
GO

PRINT N'Creating [dbo].[CSLA_Account_Select]'
GO

IF EXISTS(SELECT 1 FROM fn_listextendedproperty (NULL, 'SCHEMA', 'dbo', 'PROCEDURE', 'CSLA_Account_Select', default, default) WHERE name = 'CustomProcedure' and value = '1')
    BEGIN
        RAISERROR ('The procedure [dbo].[CSLA_Account_Select] has an Extended Property "CustomProcedure" which means is has been customized. Please review and remove the property if you wish to create the stored procedure.',16,1)
        INSERT INTO #tmpErrors (Error) SELECT 1
    END
GO

CREATE PROCEDURE [dbo].[CSLA_Account_Select]
	@p_AccountId int = NULL,
	@p_UniqueID int = NULL,
	@p_Email varchar(80) = NULL,
	@p_FirstName varchar(80) = NULL,
	@p_LastName varchar(80) = NULL,
	@p_Address1 varchar(80) = NULL,
	@p_Address2 varchar(80) = NULL,
	@p_Address2HasValue BIT = 0,
	@p_City varchar(80) = NULL,
	@p_State varchar(80) = NULL,
	@p_Zip varchar(20) = NULL,
	@p_Country varchar(20) = NULL,
	@p_Phone varchar(20) = NULL,
	@p_PhoneHasValue BIT = 0
AS

SET TRANSACTION ISOLATION LEVEL READ COMMITTED

SELECT
	[AccountId],
	[UniqueID],
	[Email],
	[FirstName],
	[LastName],
	[Address1],
	[Address2],
	[City],
	[State],
	[Zip],
	[Country],
	[Phone]
FROM
    [dbo].[Account]
WHERE
	([AccountId] = @p_AccountId OR @p_AccountId IS NULL)
	AND ([UniqueID] = @p_UniqueID OR @p_UniqueID IS NULL)
	AND ([Email] = @p_Email OR @p_Email IS NULL)
	AND ([FirstName] = @p_FirstName OR @p_FirstName IS NULL)
	AND ([LastName] = @p_LastName OR @p_LastName IS NULL)
	AND ([Address1] = @p_Address1 OR @p_Address1 IS NULL)
	AND ([Address2] = @p_Address2 OR (@p_Address2 IS NULL AND @p_Address2HasValue = 0))
	AND ([City] = @p_City OR @p_City IS NULL)
	AND ([State] = @p_State OR @p_State IS NULL)
	AND ([Zip] = @p_Zip OR @p_Zip IS NULL)
	AND ([Country] = @p_Country OR @p_Country IS NULL)
	AND ([Phone] = @p_Phone OR (@p_Phone IS NULL AND @p_PhoneHasValue = 0))

GO
IF @@ERROR!=0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

IF EXISTS (SELECT * FROM #tmpErrors) ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT>0 BEGIN
PRINT 'Stored procedure creation succeeded.'
COMMIT TRANSACTION
END
ELSE PRINT 'Stored procedure creation failed.'
GO
DROP TABLE #tmpErrors
GO

--endregion

GO

