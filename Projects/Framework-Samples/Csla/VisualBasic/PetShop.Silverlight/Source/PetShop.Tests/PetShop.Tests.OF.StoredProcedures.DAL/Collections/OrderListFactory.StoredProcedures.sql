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
PRINT N'Dropping CSLA_Order_Insert'
GO
IF EXISTS(SELECT 1 FROM fn_listextendedproperty (NULL, 'SCHEMA', 'dbo', 'PROCEDURE', 'CSLA_Order_Insert', default, default) WHERE name = 'CustomProcedure' and value = '1')
BEGIN
    RAISERROR ('The procedure CSLA_Order_Insert has an Extended Property "CustomProcedure" which means is has been customised. Please review and remove the property if you wish to drop the procedure.',16,1)
    INSERT INTO #tmpErrors (Error) SELECT 1
END
GO

IF OBJECT_ID(N'CSLA_Order_Insert') IS NOT NULL
	DROP PROCEDURE CSLA_Order_Insert

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
PRINT N'Dropping CSLA_Order_Update'
GO
IF EXISTS(SELECT 1 FROM fn_listextendedproperty (NULL, 'SCHEMA', 'dbo', 'PROCEDURE', 'CSLA_Order_Update', default, default) WHERE name = 'CustomProcedure' and value = '1')
BEGIN
    RAISERROR ('The procedure CSLA_Order_Update has an Extended Property "CustomProcedure" which means is has been customised. Please review and remove the property if you wish to drop the procedure.',16,1)
    INSERT INTO #tmpErrors (Error) SELECT 1
END
GO

IF OBJECT_ID(N'CSLA_Order_Update') IS NOT NULL
	DROP PROCEDURE CSLA_Order_Update

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
PRINT N'Dropping CSLA_Order_Delete'
GO
IF EXISTS(SELECT 1 FROM fn_listextendedproperty (NULL, 'SCHEMA', 'dbo', 'PROCEDURE', 'CSLA_Order_Delete', default, default) WHERE name = 'CustomProcedure' and value = '1')
BEGIN
    RAISERROR ('The procedure CSLA_Order_Delete has an Extended Property "CustomProcedure" which means is has been customised. Please review and remove the property if you wish to drop the procedure.',16,1)
    INSERT INTO #tmpErrors (Error) SELECT 1
END
GO

IF OBJECT_ID(N'CSLA_Order_Delete') IS NOT NULL
	DROP PROCEDURE CSLA_Order_Delete

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
PRINT N'Dropping CSLA_Order_Select'
GO
IF EXISTS(SELECT 1 FROM fn_listextendedproperty (NULL, 'SCHEMA', 'dbo', 'PROCEDURE', 'CSLA_Order_Select', default, default) WHERE name = 'CustomProcedure' and value = '1')
BEGIN
    RAISERROR ('The procedure CSLA_Order_Select has an Extended Property "CustomProcedure" which means is has been customised. Please review and remove the property if you wish to drop the procedure.',16,1)
    INSERT INTO #tmpErrors (Error) SELECT 1
END
GO

IF OBJECT_ID(N'CSLA_Order_Select') IS NOT NULL
	DROP PROCEDURE CSLA_Order_Select

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

--region [dbo].[CSLA_Order_Insert]

------------------------------------------------------------------------------------------------------------------------
-- Generated By:   Blake Niemyjski using CodeSmith: v5.2.3, CSLA Templates: v3.0.1.1934, CSLA Framework: v3.8.4
-- Procedure Name: [dbo].[CSLA_Order_Insert]
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

PRINT N'Creating [dbo].[CSLA_Order_Insert]'
GO

IF EXISTS(SELECT 1 FROM fn_listextendedproperty (NULL, 'SCHEMA', 'dbo', 'PROCEDURE', 'CSLA_Order_Insert', default, default) WHERE name = 'CustomProcedure' and value = '1')
    BEGIN
        RAISERROR ('The procedure [dbo].[CSLA_Order_Insert] has an Extended Property "CustomProcedure" which means is has been customised. Please review and remove the property if you wish to create the stored procedure.',16,1)
        INSERT INTO #tmpErrors (Error) SELECT 1
    END
GO


CREATE PROCEDURE [dbo].[CSLA_Order_Insert]
	@p_UserId varchar(20),
	@p_OrderDate datetime,
	@p_ShipAddr1 varchar(80),
	@p_ShipAddr2 varchar(80),
	@p_ShipCity varchar(80),
	@p_ShipState varchar(80),
	@p_ShipZip varchar(20),
	@p_ShipCountry varchar(20),
	@p_BillAddr1 varchar(80),
	@p_BillAddr2 varchar(80),
	@p_BillCity varchar(80),
	@p_BillState varchar(80),
	@p_BillZip varchar(20),
	@p_BillCountry varchar(20),
	@p_Courier varchar(80),
	@p_TotalPrice decimal(10, 2),
	@p_BillToFirstName varchar(80),
	@p_BillToLastName varchar(80),
	@p_ShipToFirstName varchar(80),
	@p_ShipToLastName varchar(80),
	@p_AuthorizationNumber int,
	@p_Locale varchar(20),
	@p_OrderId int OUTPUT
AS

INSERT INTO [dbo].[Orders] (
	[UserId],
	[OrderDate],
	[ShipAddr1],
	[ShipAddr2],
	[ShipCity],
	[ShipState],
	[ShipZip],
	[ShipCountry],
	[BillAddr1],
	[BillAddr2],
	[BillCity],
	[BillState],
	[BillZip],
	[BillCountry],
	[Courier],
	[TotalPrice],
	[BillToFirstName],
	[BillToLastName],
	[ShipToFirstName],
	[ShipToLastName],
	[AuthorizationNumber],
	[Locale]
) VALUES (
	@p_UserId,
	@p_OrderDate,
	@p_ShipAddr1,
	@p_ShipAddr2,
	@p_ShipCity,
	@p_ShipState,
	@p_ShipZip,
	@p_ShipCountry,
	@p_BillAddr1,
	@p_BillAddr2,
	@p_BillCity,
	@p_BillState,
	@p_BillZip,
	@p_BillCountry,
	@p_Courier,
	@p_TotalPrice,
	@p_BillToFirstName,
	@p_BillToLastName,
	@p_ShipToFirstName,
	@p_ShipToLastName,
	@p_AuthorizationNumber,
	@p_Locale)

SET @p_OrderId = SCOPE_IDENTITY()



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

--region [dbo].[CSLA_Order_Update]

------------------------------------------------------------------------------------------------------------------------
-- Generated By:   Blake Niemyjski using CodeSmith: v5.2.3, CSLA Templates: v3.0.1.1934, CSLA Framework: v3.8.4
-- Procedure Name: [dbo].[CSLA_Order_Update]
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

PRINT N'Creating [dbo].[CSLA_Order_Update]'
GO

IF EXISTS(SELECT 1 FROM fn_listextendedproperty (NULL, 'SCHEMA', 'dbo', 'PROCEDURE', 'CSLA_Order_Update', default, default) WHERE name = 'CustomProcedure' and value = '1')
    BEGIN
        RAISERROR ('The procedure [dbo].[CSLA_Order_Update] has an Extended Property "CustomProcedure" which means is has been customized. Please review and remove the property if you wish to create the stored procedure.',16,1)
        INSERT INTO #tmpErrors (Error) SELECT 1
    END
GO

CREATE PROCEDURE [dbo].[CSLA_Order_Update]
	@p_OrderId int,
	@p_UserId varchar(20),
	@p_OrderDate datetime,
	@p_ShipAddr1 varchar(80),
	@p_ShipAddr2 varchar(80),
	@p_ShipCity varchar(80),
	@p_ShipState varchar(80),
	@p_ShipZip varchar(20),
	@p_ShipCountry varchar(20),
	@p_BillAddr1 varchar(80),
	@p_BillAddr2 varchar(80),
	@p_BillCity varchar(80),
	@p_BillState varchar(80),
	@p_BillZip varchar(20),
	@p_BillCountry varchar(20),
	@p_Courier varchar(80),
	@p_TotalPrice decimal(10, 2),
	@p_BillToFirstName varchar(80),
	@p_BillToLastName varchar(80),
	@p_ShipToFirstName varchar(80),
	@p_ShipToLastName varchar(80),
	@p_AuthorizationNumber int,
	@p_Locale varchar(20)
AS

UPDATE [dbo].[Orders] SET
	[UserId] = @p_UserId,
	[OrderDate] = @p_OrderDate,
	[ShipAddr1] = @p_ShipAddr1,
	[ShipAddr2] = @p_ShipAddr2,
	[ShipCity] = @p_ShipCity,
	[ShipState] = @p_ShipState,
	[ShipZip] = @p_ShipZip,
	[ShipCountry] = @p_ShipCountry,
	[BillAddr1] = @p_BillAddr1,
	[BillAddr2] = @p_BillAddr2,
	[BillCity] = @p_BillCity,
	[BillState] = @p_BillState,
	[BillZip] = @p_BillZip,
	[BillCountry] = @p_BillCountry,
	[Courier] = @p_Courier,
	[TotalPrice] = @p_TotalPrice,
	[BillToFirstName] = @p_BillToFirstName,
	[BillToLastName] = @p_BillToLastName,
	[ShipToFirstName] = @p_ShipToFirstName,
	[ShipToLastName] = @p_ShipToLastName,
	[AuthorizationNumber] = @p_AuthorizationNumber,
	[Locale] = @p_Locale
WHERE
	[OrderId] = @p_OrderId


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

--region [dbo].[CSLA_Order_Delete]

------------------------------------------------------------------------------------------------------------------------
-- Generated By:   Blake Niemyjski using CodeSmith: v5.2.3, CSLA Templates: v3.0.1.1934, CSLA Framework: v3.8.4
-- Procedure Name: [dbo].[CSLA_Order_Delete]
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

PRINT N'Creating [dbo].[CSLA_Order_Delete]'
GO

IF EXISTS(SELECT 1 FROM fn_listextendedproperty (NULL, 'SCHEMA', 'dbo', 'PROCEDURE', 'CSLA_Order_Delete', default, default) WHERE name = 'CustomProcedure' and value = '1')
    BEGIN
        RAISERROR ('The procedure [dbo].[CSLA_Order_Delete] has an Extended Property "CustomProcedure" which means is has been customised. Please review and remove the property if you wish to create the stored procedure.',16,1)
        INSERT INTO #tmpErrors (Error) SELECT 1
    END
GO

CREATE PROCEDURE [dbo].[CSLA_Order_Delete]
	@p_OrderId int
AS

DELETE FROM
    [dbo].[Orders]
WHERE
	[OrderId] = @p_OrderId

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

--region [dbo].[CSLA_Order_Select]

------------------------------------------------------------------------------------------------------------------------
-- Generated By:   Blake Niemyjski using CodeSmith: v5.2.3, CSLA Templates: v3.0.1.1934, CSLA Framework: v3.8.4
-- Procedure Name: [dbo].[CSLA_Order_Select]
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

PRINT N'Creating [dbo].[CSLA_Order_Select]'
GO

IF EXISTS(SELECT 1 FROM fn_listextendedproperty (NULL, 'SCHEMA', 'dbo', 'PROCEDURE', 'CSLA_Order_Select', default, default) WHERE name = 'CustomProcedure' and value = '1')
    BEGIN
        RAISERROR ('The procedure [dbo].[CSLA_Order_Select] has an Extended Property "CustomProcedure" which means is has been customised. Please review and remove the property if you wish to create the stored procedure.',16,1)
        INSERT INTO #tmpErrors (Error) SELECT 1
    END
GO

CREATE PROCEDURE [dbo].[CSLA_Order_Select]
	@p_OrderId int = NULL,
	@p_UserId varchar(20) = NULL,
	@p_OrderDate datetime = NULL,
	@p_ShipAddr1 varchar(80) = NULL,
	@p_ShipAddr2 varchar(80) = NULL,
	@p_ShipAddr2HasValue BIT = 0,
	@p_ShipCity varchar(80) = NULL,
	@p_ShipState varchar(80) = NULL,
	@p_ShipZip varchar(20) = NULL,
	@p_ShipCountry varchar(20) = NULL,
	@p_BillAddr1 varchar(80) = NULL,
	@p_BillAddr2 varchar(80) = NULL,
	@p_BillAddr2HasValue BIT = 0,
	@p_BillCity varchar(80) = NULL,
	@p_BillState varchar(80) = NULL,
	@p_BillZip varchar(20) = NULL,
	@p_BillCountry varchar(20) = NULL,
	@p_Courier varchar(80) = NULL,
	@p_TotalPrice decimal(10, 2) = NULL,
	@p_BillToFirstName varchar(80) = NULL,
	@p_BillToLastName varchar(80) = NULL,
	@p_ShipToFirstName varchar(80) = NULL,
	@p_ShipToLastName varchar(80) = NULL,
	@p_AuthorizationNumber int = NULL,
	@p_Locale varchar(20) = NULL
AS

SET TRANSACTION ISOLATION LEVEL READ COMMITTED

SELECT
	[OrderId],
	[UserId],
	[OrderDate],
	[ShipAddr1],
	[ShipAddr2],
	[ShipCity],
	[ShipState],
	[ShipZip],
	[ShipCountry],
	[BillAddr1],
	[BillAddr2],
	[BillCity],
	[BillState],
	[BillZip],
	[BillCountry],
	[Courier],
	[TotalPrice],
	[BillToFirstName],
	[BillToLastName],
	[ShipToFirstName],
	[ShipToLastName],
	[AuthorizationNumber],
	[Locale]
FROM
    [dbo].[Orders]
WHERE
	([OrderId] = @p_OrderId OR @p_OrderId IS NULL)
	AND ([UserId] = @p_UserId OR @p_UserId IS NULL)
	AND ([OrderDate] = @p_OrderDate OR @p_OrderDate IS NULL)
	AND ([ShipAddr1] = @p_ShipAddr1 OR @p_ShipAddr1 IS NULL)
	AND ([ShipAddr2] = @p_ShipAddr2 OR (@p_ShipAddr2 IS NULL AND @p_ShipAddr2HasValue = 0))
	AND ([ShipCity] = @p_ShipCity OR @p_ShipCity IS NULL)
	AND ([ShipState] = @p_ShipState OR @p_ShipState IS NULL)
	AND ([ShipZip] = @p_ShipZip OR @p_ShipZip IS NULL)
	AND ([ShipCountry] = @p_ShipCountry OR @p_ShipCountry IS NULL)
	AND ([BillAddr1] = @p_BillAddr1 OR @p_BillAddr1 IS NULL)
	AND ([BillAddr2] = @p_BillAddr2 OR (@p_BillAddr2 IS NULL AND @p_BillAddr2HasValue = 0))
	AND ([BillCity] = @p_BillCity OR @p_BillCity IS NULL)
	AND ([BillState] = @p_BillState OR @p_BillState IS NULL)
	AND ([BillZip] = @p_BillZip OR @p_BillZip IS NULL)
	AND ([BillCountry] = @p_BillCountry OR @p_BillCountry IS NULL)
	AND ([Courier] = @p_Courier OR @p_Courier IS NULL)
	AND ([TotalPrice] = @p_TotalPrice OR @p_TotalPrice IS NULL)
	AND ([BillToFirstName] = @p_BillToFirstName OR @p_BillToFirstName IS NULL)
	AND ([BillToLastName] = @p_BillToLastName OR @p_BillToLastName IS NULL)
	AND ([ShipToFirstName] = @p_ShipToFirstName OR @p_ShipToFirstName IS NULL)
	AND ([ShipToLastName] = @p_ShipToLastName OR @p_ShipToLastName IS NULL)
	AND ([AuthorizationNumber] = @p_AuthorizationNumber OR @p_AuthorizationNumber IS NULL)
	AND ([Locale] = @p_Locale OR @p_Locale IS NULL)

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

