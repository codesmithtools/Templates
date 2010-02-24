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
PRINT N'Dropping [dbo].[CSLA_Supplier_Insert]'
GO
IF EXISTS(SELECT 1 FROM fn_listextendedproperty (NULL, 'SCHEMA', 'dbo', 'PROCEDURE', '[dbo].[CSLA_Supplier_Insert]', default, default) WHERE name = 'CustomProcedure' and value = '1')
BEGIN
    RAISERROR ('The procedure [dbo].[CSLA_Supplier_Insert] has an Extended Property "CustomProcedure" which means is has been customised. Please review and remove the property if you wish to drop the procedure.',16,1)
    INSERT INTO #tmpErrors (Error) SELECT 1
END
GO

IF OBJECT_ID(N'[dbo].[CSLA_Supplier_Insert]') IS NOT NULL
	DROP PROCEDURE [dbo].[CSLA_Supplier_Insert]

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
PRINT N'Dropping [dbo].[CSLA_Supplier_Update]'
GO
IF EXISTS(SELECT 1 FROM fn_listextendedproperty (NULL, 'SCHEMA', 'dbo', 'PROCEDURE', '[dbo].[CSLA_Supplier_Update]', default, default) WHERE name = 'CustomProcedure' and value = '1')
BEGIN
    RAISERROR ('The procedure [dbo].[CSLA_Supplier_Update] has an Extended Property "CustomProcedure" which means is has been customised. Please review and remove the property if you wish to drop the procedure.',16,1)
    INSERT INTO #tmpErrors (Error) SELECT 1
END
GO

IF OBJECT_ID(N'[dbo].[CSLA_Supplier_Update]') IS NOT NULL
	DROP PROCEDURE [dbo].[CSLA_Supplier_Update]

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
PRINT N'Dropping [dbo].[CSLA_Supplier_Delete]'
GO
IF EXISTS(SELECT 1 FROM fn_listextendedproperty (NULL, 'SCHEMA', 'dbo', 'PROCEDURE', '[dbo].[CSLA_Supplier_Delete]', default, default) WHERE name = 'CustomProcedure' and value = '1')
BEGIN
    RAISERROR ('The procedure [dbo].[CSLA_Supplier_Delete] has an Extended Property "CustomProcedure" which means is has been customised. Please review and remove the property if you wish to drop the procedure.',16,1)
    INSERT INTO #tmpErrors (Error) SELECT 1
END
GO

IF OBJECT_ID(N'[dbo].[CSLA_Supplier_Delete]') IS NOT NULL
	DROP PROCEDURE [dbo].[CSLA_Supplier_Delete]

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
PRINT N'Dropping [dbo].[CSLA_Supplier_Select]'
GO
IF EXISTS(SELECT 1 FROM fn_listextendedproperty (NULL, 'SCHEMA', 'dbo', 'PROCEDURE', '[dbo].[CSLA_Supplier_Select]', default, default) WHERE name = 'CustomProcedure' and value = '1')
BEGIN
    RAISERROR ('The procedure [dbo].[CSLA_Supplier_Select] has an Extended Property "CustomProcedure" which means is has been customised. Please review and remove the property if you wish to drop the procedure.',16,1)
    INSERT INTO #tmpErrors (Error) SELECT 1
END
GO

IF OBJECT_ID(N'[dbo].[CSLA_Supplier_Select]') IS NOT NULL
	DROP PROCEDURE [dbo].[CSLA_Supplier_Select]

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

--region [dbo].[CSLA_Supplier_Insert]

------------------------------------------------------------------------------------------------------------------------
-- Generated By:   Blake Niemyjski using CodeSmith: v5.2.1, CSLA Templates: v2.0.0.1440, CSLA Framework: v3.8.2
-- Procedure Name: [dbo].[CSLA_Supplier_Insert]
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

PRINT N'Creating [dbo].[CSLA_Supplier_Insert]'
GO

IF EXISTS(SELECT 1 FROM fn_listextendedproperty (NULL, 'SCHEMA', 'dbo', 'PROCEDURE', '[dbo].[CSLA_Supplier_Insert]', default, default) WHERE name = 'CustomProcedure' and value = '1')
    BEGIN
        RAISERROR ('The procedure [dbo].[CSLA_Supplier_Insert] has an Extended Property "CustomProcedure" which means is has been customised. Please review and remove the property if you wish to create the stored procedure.',16,1)
        INSERT INTO #tmpErrors (Error) SELECT 1
    END
GO


CREATE PROCEDURE [dbo].[CSLA_Supplier_Insert]
	@p_SuppId int,
	@p_Name varchar(80),
	@p_Status varchar(2),
	@p_Addr1 varchar(80),
	@p_Addr2 varchar(80),
	@p_City varchar(80),
	@p_State varchar(80),
	@p_Zip varchar(5),
	@p_Phone varchar(40)
AS

INSERT INTO [dbo].[Supplier] (
	[SuppId],
	[Name],
	[Status],
	[Addr1],
	[Addr2],
	[City],
	[State],
	[Zip],
	[Phone]) VALUES (
	@p_SuppId,
	@p_Name,
	@p_Status,
	@p_Addr1,
	@p_Addr2,
	@p_City,
	@p_State,
	@p_Zip,
	@p_Phone)



IF @@ERROR!=0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

IF EXISTS (SELECT * FROM #tmpErrors) ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT>0 BEGIN
PRINT 'Stored procedure creation succedded was succeeded.'
COMMIT TRANSACTION
END
ELSE PRINT 'Stored procedure creation failed.'
GO
DROP TABLE #tmpErrors
GO

--endregion

GO

--region [dbo].[CSLA_Supplier_Update]

------------------------------------------------------------------------------------------------------------------------
-- Generated By:   Blake Niemyjski using CodeSmith: v5.2.1, CSLA Templates: v2.0.0.1440, CSLA Framework: v3.8.2
-- Procedure Name: [dbo].[CSLA_Supplier_Update]
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

PRINT N'Creating [dbo].[CSLA_Supplier_Update]'
GO

IF EXISTS(SELECT 1 FROM fn_listextendedproperty (NULL, 'SCHEMA', 'dbo', 'PROCEDURE', '[dbo].[CSLA_Supplier_Update]', default, default) WHERE name = 'CustomProcedure' and value = '1')
    BEGIN
        RAISERROR ('The procedure [dbo].[CSLA_Supplier_Update] has an Extended Property "CustomProcedure" which means is has been customised. Please review and remove the property if you wish to create the stored procedure.',16,1)
        INSERT INTO #tmpErrors (Error) SELECT 1
    END
GO

CREATE PROCEDURE [dbo].[CSLA_Supplier_Update]
	@p_SuppId int,
	@p_OriginalSuppId int,
	@p_Name varchar(80),
	@p_Status varchar(2),
	@p_Addr1 varchar(80),
	@p_Addr2 varchar(80),
	@p_City varchar(80),
	@p_State varchar(80),
	@p_Zip varchar(5),
	@p_Phone varchar(40)
AS

UPDATE [dbo].[Supplier] SET
	[SuppId] = @p_SuppId,
	[Name] = @p_Name,
	[Status] = @p_Status,
	[Addr1] = @p_Addr1,
	[Addr2] = @p_Addr2,
	[City] = @p_City,
	[State] = @p_State,
	[Zip] = @p_Zip,
	[Phone] = @p_Phone
WHERE
	[SuppId] = @p_OriginalSuppId


IF @@ERROR!=0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

IF EXISTS (SELECT * FROM #tmpErrors) ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT>0 BEGIN
PRINT 'Stored procedure creation succedded was succeeded.'
COMMIT TRANSACTION
END
ELSE PRINT 'Stored procedure creation failed.'
GO
DROP TABLE #tmpErrors
GO


--endregion

GO

--region [dbo].[CSLA_Supplier_Delete]

------------------------------------------------------------------------------------------------------------------------
-- Generated By:   Blake Niemyjski using CodeSmith: v5.2.1, CSLA Templates: v2.0.0.1440, CSLA Framework: v3.8.2
-- Procedure Name: [dbo].[CSLA_Supplier_Delete]
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

PRINT N'Creating [dbo].[CSLA_Supplier_Delete]'
GO

IF EXISTS(SELECT 1 FROM fn_listextendedproperty (NULL, 'SCHEMA', 'dbo', 'PROCEDURE', '[dbo].[CSLA_Supplier_Delete]', default, default) WHERE name = 'CustomProcedure' and value = '1')
    BEGIN
        RAISERROR ('The procedure [dbo].[CSLA_Supplier_Delete] has an Extended Property "CustomProcedure" which means is has been customised. Please review and remove the property if you wish to create the stored procedure.',16,1)
        INSERT INTO #tmpErrors (Error) SELECT 1
    END
GO

CREATE PROCEDURE [dbo].[CSLA_Supplier_Delete]
	@p_SuppId int
AS

DELETE FROM
    [dbo].[Supplier]
WHERE
	[SuppId] = @p_SuppId

IF @@ERROR!=0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

IF EXISTS (SELECT * FROM #tmpErrors) ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT>0 BEGIN
PRINT 'Stored procedure creation succedded was succeeded.'
COMMIT TRANSACTION
END
ELSE PRINT 'Stored procedure creation failed.'
GO
DROP TABLE #tmpErrors
GO


--endregion

GO

--region [dbo].[CSLA_Supplier_Select]

------------------------------------------------------------------------------------------------------------------------
-- Generated By:   Blake Niemyjski using CodeSmith: v5.2.1, CSLA Templates: v2.0.0.1440, CSLA Framework: v3.8.2
-- Procedure Name: [dbo].[CSLA_Supplier_Select]
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

PRINT N'Creating [dbo].[CSLA_Supplier_Select]'
GO

IF EXISTS(SELECT 1 FROM fn_listextendedproperty (NULL, 'SCHEMA', 'dbo', 'PROCEDURE', '[dbo].[CSLA_Supplier_Select]', default, default) WHERE name = 'CustomProcedure' and value = '1')
    BEGIN
        RAISERROR ('The procedure [dbo].[CSLA_Supplier_Select] has an Extended Property "CustomProcedure" which means is has been customised. Please review and remove the property if you wish to create the stored procedure.',16,1)
        INSERT INTO #tmpErrors (Error) SELECT 1
    END
GO

CREATE PROCEDURE [dbo].[CSLA_Supplier_Select]
	@p_SuppId int = NULL,
	@p_Name varchar(80) = NULL,
	@p_NameHasValue BIT = 0,
	@p_Status varchar(2) = NULL,
	@p_Addr1 varchar(80) = NULL,
	@p_Addr1HasValue BIT = 0,
	@p_Addr2 varchar(80) = NULL,
	@p_Addr2HasValue BIT = 0,
	@p_City varchar(80) = NULL,
	@p_CityHasValue BIT = 0,
	@p_State varchar(80) = NULL,
	@p_StateHasValue BIT = 0,
	@p_Zip varchar(5) = NULL,
	@p_ZipHasValue BIT = 0,
	@p_Phone varchar(40) = NULL,
	@p_PhoneHasValue BIT = 0
AS

SET TRANSACTION ISOLATION LEVEL READ COMMITTED

SELECT
	[SuppId],
	[Name],
	[Status],
	[Addr1],
	[Addr2],
	[City],
	[State],
	[Zip],
	[Phone]
FROM
    [dbo].[Supplier]
WHERE
	([SuppId] = @p_SuppId OR @p_SuppId IS NULL)
	AND ([Name] = @p_Name OR (@p_Name IS NULL AND @p_NameHasValue = 0))
	AND ([Status] = @p_Status OR @p_Status IS NULL)
	AND ([Addr1] = @p_Addr1 OR (@p_Addr1 IS NULL AND @p_Addr1HasValue = 0))
	AND ([Addr2] = @p_Addr2 OR (@p_Addr2 IS NULL AND @p_Addr2HasValue = 0))
	AND ([City] = @p_City OR (@p_City IS NULL AND @p_CityHasValue = 0))
	AND ([State] = @p_State OR (@p_State IS NULL AND @p_StateHasValue = 0))
	AND ([Zip] = @p_Zip OR (@p_Zip IS NULL AND @p_ZipHasValue = 0))
	AND ([Phone] = @p_Phone OR (@p_Phone IS NULL AND @p_PhoneHasValue = 0))

IF @@ERROR!=0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

IF EXISTS (SELECT * FROM #tmpErrors) ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT>0 BEGIN
PRINT 'Stored procedure creation succedded was succeeded.'
COMMIT TRANSACTION
END
ELSE PRINT 'Stored procedure creation failed.'
GO
DROP TABLE #tmpErrors
GO


--endregion

GO

