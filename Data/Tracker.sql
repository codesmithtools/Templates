Use Tracker

/****** Object:  Table [dbo].[User]    Script Date: 09/29/2009 14:44:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[User](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmailAddress] [nvarchar](250) NOT NULL,
	[FirstName] [nvarchar](200) NULL,
	[LastName] [nvarchar](200) NULL,
	[Avatar] [varbinary](max) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
	[PasswordHash] [char](86) NOT NULL,
	[PasswordSalt] [char](5) NOT NULL,
	[Comment] [text] NULL,
	[IsApproved] [bit] NOT NULL,
	[LastLoginDate] [datetime] NULL,
	[LastActivityDate] [datetime] NOT NULL,
	[LastPasswordChangeDate] [datetime] NULL,
	[AvatarType] [nvarchar](150) NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_User] ON [dbo].[User] 
(
	[EmailAddress] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Guid]    Script Date: 09/29/2009 15:04:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Guid](
	[Id] [uniqueidentifier] NOT NULL,
	[AlternateId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Guid] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Status]    Script Date: 09/29/2009 14:44:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Status](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](150) NULL,
	[Order] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_Status] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Role]    Script Date: 09/29/2009 14:44:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](150) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Priority]    Script Date: 09/29/2009 14:44:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Priority](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Order] [int] NOT NULL,
	[Description] [nvarchar](200) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_Priority] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  UserDefinedFunction [dbo].[GetOne]    Script Date: 09/29/2009 14:44:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[GetOne]
(
	@param int
)
RETURNS int
AS
BEGIN
	-- Return the result of the function
	RETURN 1

END
GO
/****** Object:  Table [dbo].[Task]    Script Date: 09/29/2009 14:44:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Task](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StatusId] [int] NOT NULL,
	[PriorityId] [int] NULL,
	[CreatedId] [int] NOT NULL,
	[Summary] [nvarchar](255) NOT NULL,
	[Details] [nvarchar](2000) NULL,
	[StartDate] [datetime] NULL,
	[DueDate] [datetime] NULL,
	[CompleteDate] [datetime] NULL,
	[AssignedId] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
	[LastModifiedBy] [nvarchar](50) NULL,
 CONSTRAINT [PK_Task] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Task] ON [dbo].[Task] 
(
	[AssignedId] ASC,
	[StatusId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserRole]    Script Date: 09/29/2009 14:44:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRole](
	[UserId] [int] NOT NULL,
	[RoleId] [int] NOT NULL,
 CONSTRAINT [PK_UserRole] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TaskExtended]    Script Date: 09/29/2009 14:44:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TaskExtended](
	[TaskId] [int] NOT NULL,
	[Browser] [nvarchar](200) NULL,
	[OS] [nvarchar](150) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_TaskExtended] PRIMARY KEY CLUSTERED 
(
	[TaskId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Audit]    Script Date: 09/29/2009 14:44:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Audit](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Date] [datetime] NOT NULL,
	[UserId] [int] NULL,
	[TaskId] [int] NULL,
	[Content] [varchar](max) NOT NULL,
	[Username] [nvarchar](50) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
	[myxml] [xml] NULL,
 CONSTRAINT [PK_Audit] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [dbo].[GetUsersWithRoles]    Script Date: 09/29/2009 14:44:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetUsersWithRoles]
AS
BEGIN
	SET NOCOUNT ON;

	Select * From [User]
	Select * From UserRole
END
GO
/****** Object:  StoredProcedure [dbo].[RolesForUser]    Script Date: 05/27/2011 18:47:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[RolesForUser]
	@UserId int
AS
BEGIN
	SET NOCOUNT ON;

	select * from [Role] r
	join [UserRole]	ur on r.Id = ur.RoleId
	where ur.UserId = UserId
END
GO
/****** Object:  Default [DF__User__CreatedDat__1920BF5C]    Script Date: 09/29/2009 14:44:09 ******/
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF__User__CreatedDat__1920BF5C]  DEFAULT (getdate()) FOR [CreatedDate]
GO
/****** Object:  Default [DF__User__ModifiedDa__1A14E395]    Script Date: 09/29/2009 14:44:09 ******/
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF__User__ModifiedDa__1A14E395]  DEFAULT (getdate()) FOR [ModifiedDate]
GO
/****** Object:  Default [DF__User__PasswordHa__3A81B327]    Script Date: 09/29/2009 14:44:09 ******/
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF__User__PasswordHa__3A81B327]  DEFAULT ('') FOR [PasswordHash]
GO
/****** Object:  Default [DF__User__PasswordSa__3B75D760]    Script Date: 09/29/2009 14:44:09 ******/
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF__User__PasswordSa__3B75D760]  DEFAULT ('') FOR [PasswordSalt]
GO
/****** Object:  Default [DF__User__IsApproved__3C69FB99]    Script Date: 09/29/2009 14:44:09 ******/
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF__User__IsApproved__3C69FB99]  DEFAULT ((1)) FOR [IsApproved]
GO
/****** Object:  Default [DF__User__LastLoginD__3D5E1FD2]    Script Date: 09/29/2009 14:44:09 ******/
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF__User__LastLoginD__3D5E1FD2]  DEFAULT (getdate()) FOR [LastLoginDate]
GO
/****** Object:  Default [DF__User__LastActivi__3E52440B]    Script Date: 09/29/2009 14:44:09 ******/
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF__User__LastActivi__3E52440B]  DEFAULT (getdate()) FOR [LastActivityDate]
GO
/****** Object:  Default [DF__Status__CreatedD__0EA330E9]    Script Date: 09/29/2009 14:44:09 ******/
ALTER TABLE [dbo].[Status] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
/****** Object:  Default [DF__Status__Modified__0F975522]    Script Date: 09/29/2009 14:44:09 ******/
ALTER TABLE [dbo].[Status] ADD  DEFAULT (getdate()) FOR [ModifiedDate]
GO
/****** Object:  Default [DF__Role__CreatedDat__0CBAE877]    Script Date: 09/29/2009 14:44:09 ******/
ALTER TABLE [dbo].[Role] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
/****** Object:  Default [DF__Role__ModifiedDa__0DAF0CB0]    Script Date: 09/29/2009 14:44:09 ******/
ALTER TABLE [dbo].[Role] ADD  DEFAULT (getdate()) FOR [ModifiedDate]
GO
/****** Object:  Default [DF__Priority__Create__0AD2A005]    Script Date: 09/29/2009 14:44:09 ******/
ALTER TABLE [dbo].[Priority] ADD  CONSTRAINT [DF__Priority__Create__0AD2A005]  DEFAULT (getdate()) FOR [CreatedDate]
GO
/****** Object:  Default [DF__Priority__Modifi__0BC6C43E]    Script Date: 09/29/2009 14:44:09 ******/
ALTER TABLE [dbo].[Priority] ADD  CONSTRAINT [DF__Priority__Modifi__0BC6C43E]  DEFAULT (getdate()) FOR [ModifiedDate]
GO
/****** Object:  Default [DF__Task__CreatedDat__145C0A3F]    Script Date: 09/29/2009 14:44:10 ******/
ALTER TABLE [dbo].[Task] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
/****** Object:  Default [DF__Task__ModifiedDa__15502E78]    Script Date: 09/29/2009 14:44:10 ******/
ALTER TABLE [dbo].[Task] ADD  DEFAULT (getdate()) FOR [ModifiedDate]
GO
/****** Object:  Default [DF__TaskExten__Creat__173876EA]    Script Date: 09/29/2009 14:44:10 ******/
ALTER TABLE [dbo].[TaskExtended] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
/****** Object:  Default [DF__TaskExten__Modif__182C9B23]    Script Date: 09/29/2009 14:44:10 ******/
ALTER TABLE [dbo].[TaskExtended] ADD  DEFAULT (getdate()) FOR [ModifiedDate]
GO
/****** Object:  Default [DF_Audit_CreatedDate]    Script Date: 09/29/2009 14:44:10 ******/
ALTER TABLE [dbo].[Audit] ADD  CONSTRAINT [DF_Audit_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
/****** Object:  ForeignKey [FK_Task_Priority]    Script Date: 09/29/2009 14:44:10 ******/
ALTER TABLE [dbo].[Task]  WITH NOCHECK ADD  CONSTRAINT [FK_Task_Priority] FOREIGN KEY([PriorityId])
REFERENCES [dbo].[Priority] ([Id])
GO
ALTER TABLE [dbo].[Task] CHECK CONSTRAINT [FK_Task_Priority]
GO
/****** Object:  ForeignKey [FK_Task_Status]    Script Date: 09/29/2009 14:44:10 ******/
ALTER TABLE [dbo].[Task]  WITH NOCHECK ADD  CONSTRAINT [FK_Task_Status] FOREIGN KEY([StatusId])
REFERENCES [dbo].[Status] ([Id])
GO
ALTER TABLE [dbo].[Task] CHECK CONSTRAINT [FK_Task_Status]
GO
/****** Object:  ForeignKey [FK_Task_User_Assigned]    Script Date: 09/29/2009 14:44:10 ******/
ALTER TABLE [dbo].[Task]  WITH NOCHECK ADD  CONSTRAINT [FK_Task_User_Assigned] FOREIGN KEY([AssignedId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Task] CHECK CONSTRAINT [FK_Task_User_Assigned]
GO
/****** Object:  ForeignKey [FK_Task_User_Created]    Script Date: 09/29/2009 14:44:10 ******/
ALTER TABLE [dbo].[Task]  WITH NOCHECK ADD  CONSTRAINT [FK_Task_User_Created] FOREIGN KEY([CreatedId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Task] CHECK CONSTRAINT [FK_Task_User_Created]
GO
/****** Object:  ForeignKey [FK_UserRole_Role]    Script Date: 09/29/2009 14:44:10 ******/
ALTER TABLE [dbo].[UserRole]  WITH NOCHECK ADD  CONSTRAINT [FK_UserRole_Role] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Role] ([Id])
GO
ALTER TABLE [dbo].[UserRole] CHECK CONSTRAINT [FK_UserRole_Role]
GO
/****** Object:  ForeignKey [FK_UserRole_User]    Script Date: 09/29/2009 14:44:10 ******/
ALTER TABLE [dbo].[UserRole]  WITH NOCHECK ADD  CONSTRAINT [FK_UserRole_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[UserRole] CHECK CONSTRAINT [FK_UserRole_User]
GO
/****** Object:  ForeignKey [FK_TaskExtended_Task]    Script Date: 09/29/2009 14:44:10 ******/
ALTER TABLE [dbo].[TaskExtended]  WITH CHECK ADD  CONSTRAINT [FK_TaskExtended_Task] FOREIGN KEY([TaskId])
REFERENCES [dbo].[Task] ([Id])
GO
ALTER TABLE [dbo].[TaskExtended] CHECK CONSTRAINT [FK_TaskExtended_Task]
GO
/****** Object:  ForeignKey [FK_Audit_Task]    Script Date: 09/29/2009 14:44:10 ******/
ALTER TABLE [dbo].[Audit]  WITH CHECK ADD  CONSTRAINT [FK_Audit_Task] FOREIGN KEY([TaskId])
REFERENCES [dbo].[Task] ([Id])
GO
ALTER TABLE [dbo].[Audit] CHECK CONSTRAINT [FK_Audit_Task]
GO
/****** Object:  ForeignKey [FK_Audit_User]    Script Date: 09/29/2009 14:44:10 ******/
ALTER TABLE [dbo].[Audit]  WITH CHECK ADD  CONSTRAINT [FK_Audit_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Audit] CHECK CONSTRAINT [FK_Audit_User]
GO


/****** Object:  Table [dbo].[Self]    Script Date: 06/21/2011 23:58:18 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Self](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MySelfId] [int] NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Self] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Self]  WITH CHECK ADD  CONSTRAINT [FK_Self_Self] FOREIGN KEY([MySelfId])
REFERENCES [dbo].[Self] ([Id])
GO

ALTER TABLE [dbo].[Self] CHECK CONSTRAINT [FK_Self_Self]
GO


/****** Object:  View [dbo].[TaskDetail]    Script Date: 05/31/2011 18:47:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[TaskDetail]
AS
SELECT     dbo.Task.Id, dbo.Task.Summary, dbo.Priority.Name AS Priority, dbo.Status.Name AS Status, ua.EmailAddress AS Assigned, uc.EmailAddress AS Created
FROM         dbo.Task INNER JOIN
                      dbo.Status ON dbo.Status.Id = dbo.Task.StatusId INNER JOIN
                      dbo.Priority ON dbo.Priority.Id = dbo.Task.PriorityId INNER JOIN
                      dbo.[User] AS ua ON dbo.Task.AssignedId = ua.Id INNER JOIN
                      dbo.[User] AS uc ON dbo.Task.CreatedId = uc.Id

GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "Priority"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 204
               Right = 198
            End
            DisplayFlags = 280
            TopColumn = 3
         End
         Begin Table = "Status"
            Begin Extent = 
               Top = 110
               Left = 240
               Bottom = 303
               Right = 400
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Task"
            Begin Extent = 
               Top = 0
               Left = 427
               Bottom = 260
               Right = 748
            End
            DisplayFlags = 280
            TopColumn = 1
         End
         Begin Table = "ua"
            Begin Extent = 
               Top = 67
               Left = 876
               Bottom = 186
               Right = 1091
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "uc"
            Begin Extent = 
               Top = 6
               Left = 1129
               Bottom = 125
               Right = 1344
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TaskDetail'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TaskDetail'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TaskDetail'
GO

/** Insert Roles **/

Set Identity_Insert [Role] On 
GO
INSERT INTO [Role] ([Id],[Name])
VALUES(1, 'Admin')

INSERT INTO [Role] ([Id],[Name])
VALUES(2, 'Manager')

INSERT INTO [Role] ([Id],[Name])
VALUES(3, 'Newb')

INSERT INTO [Role] ([Id],[Name])
VALUES(4, 'Nobody')

INSERT INTO [Role] ([Id],[Name])
VALUES(5, 'Power User')
GO
Set Identity_Insert [Role] Off
GO

/** Insert Roles **/

/** Insert Statuses **/

Set Identity_Insert [Status] On 
GO
INSERT INTO [Status] ([Id],[Name], [Order])
VALUES(1, 'Not Started', 1)

INSERT INTO [Status] ([Id],[Name], [Order])
VALUES(2, 'In Progress', 2)

INSERT INTO [Status] ([Id],[Name], [Order])
VALUES(3, 'Completed', 3)

INSERT INTO [Status] ([Id],[Name], [Order])
VALUES(4, 'Waiting on someone else', 4)

INSERT INTO [Status] ([Id],[Name], [Order])
VALUES(5, 'Deferred', 5)

INSERT INTO [Status] ([Id],[Name], [Order])
VALUES(6, 'Done', 6)
GO
Set Identity_Insert [Status] Off
GO

/** Insert Statuses **/

/** Insert Priorities **/

INSERT INTO [Priority] ([Id],[Name], [Order], [Description])
VALUES(1, 'High', 1, 'A High Priority')

INSERT INTO [Priority] ([Id],[Name], [Order], [Description])
VALUES(2, 'Normal', 2, 'A Normal Priority')

INSERT INTO [Priority] ([Id],[Name], [Order], [Description])
VALUES(3, 'Low', 3, 'A Low Priority')

/** Insert Priorities **/

/** Insert Users **/

SET IDENTITY_INSERT [dbo].[User] ON
INSERT [dbo].[User] ([Id], [EmailAddress], [FirstName], [LastName], [Avatar], [CreatedDate], [ModifiedDate], [PasswordHash], [PasswordSalt], [Comment], [IsApproved], [LastLoginDate], [LastActivityDate], [LastPasswordChangeDate]) 
VALUES (1, N'william.adama@battlestar.com', N'William', N'Adama', NULL, CAST(0x00009C010124E143 AS DateTime), CAST(0x00009C010124E143 AS DateTime), N'1+v5rvSXnyX7tvwTKfM+aq+s0hDmNXsduGZfq8sQv1ggPnGlQdDdBdbUP0bUmbMbiU40PvRQWKRAy5QUd1xrAA', N'?#nkY', NULL, 1, NULL, CAST(0x00009C010124E143 AS DateTime), NULL)

INSERT [dbo].[User] ([Id], [EmailAddress], [FirstName], [LastName], [Avatar], [CreatedDate], [ModifiedDate], [PasswordHash], [PasswordSalt], [Comment], [IsApproved], [LastLoginDate], [LastActivityDate], [LastPasswordChangeDate]) 
VALUES (2, N'laura.roslin@battlestar.com', N'Laura', N'Roslin', NULL, CAST(0x00009C0101250FD3 AS DateTime), CAST(0x00009C0101250FD3 AS DateTime), N'Sx/jwRHFW/CQpO0E6G8d+jo344AmAKfX+C48a0iAZyMrb4sE8VoDuyZorbhbLZAX9f4UZk67O7eCjk854OrYSg', N'Ph)6;', NULL, 1, NULL, CAST(0x00009C0101250FD3 AS DateTime), NULL)

INSERT [dbo].[User] ([Id], [EmailAddress], [FirstName], [LastName], [Avatar], [CreatedDate], [ModifiedDate], [PasswordHash], [PasswordSalt], [Comment], [IsApproved], [LastLoginDate], [LastActivityDate], [LastPasswordChangeDate]) 
VALUES (3, N'kara.thrace@battlestar.com', N'Kara', N'Thrace', NULL, CAST(0x00009C0101254251 AS DateTime), CAST(0x00009C0101254251 AS DateTime), N'5KhtS4ax7G1aGkq97w02ooVZMmJp8bcySEKMSxruXu/Z/wRKoNAxMlkjRVY1yLavrC3GRYQZjy5b6JW8cR5EWg', N'!]@2/', NULL, 1, NULL, CAST(0x00009C0101254251 AS DateTime), NULL)

INSERT [dbo].[User] ([Id], [EmailAddress], [FirstName], [LastName], [Avatar], [CreatedDate], [ModifiedDate], [PasswordHash], [PasswordSalt], [Comment], [IsApproved], [LastLoginDate], [LastActivityDate], [LastPasswordChangeDate]) 
VALUES (4, N'lee.adama@battlestar.com', N'Lee', N'Adama', NULL, CAST(0x00009C0101255886 AS DateTime), CAST(0x00009C0101255886 AS DateTime), N'IrK8OhI2n4Ev3YA4y5kP7vy+n2CffX2MgcONbAh6/kZpNZYBYoYyrMhqdYztehL0NAIdvcInQ6zOjMplq+zWQA', N'e@_a{', NULL, 1, NULL, CAST(0x00009C0101255886 AS DateTime), NULL)

INSERT [dbo].[User] ([Id], [EmailAddress], [FirstName], [LastName], [Avatar], [CreatedDate], [ModifiedDate], [PasswordHash], [PasswordSalt], [Comment], [IsApproved], [LastLoginDate], [LastActivityDate], [LastPasswordChangeDate]) 
VALUES (5, N'gaius.baltar@battlestar.com', N'Gaius', N'Baltar', NULL, CAST(0x00009C010125748A AS DateTime), CAST(0x00009C010125748A AS DateTime), N'7tfajMaEerDNVgi6Oi6UJ6JxsUXZ0u4zQeUrZQxnaXJQ2f2vd9AzBR4sVOaH7LZtCjQopMzlQ38QqNEnpK0B/g', N'_qpA2', NULL, 1, NULL, CAST(0x00009C010125748A AS DateTime), NULL)

INSERT [dbo].[User] ([Id], [EmailAddress], [FirstName], [LastName], [Avatar], [CreatedDate], [ModifiedDate], [PasswordHash], [PasswordSalt], [Comment], [IsApproved], [LastLoginDate], [LastActivityDate], [LastPasswordChangeDate]) 
VALUES (6, N'saul.tigh@battlestar.com', N'Saul', N'Tigh', NULL, CAST(0x00009C010125BA8F AS DateTime), CAST(0x00009C010125BA8F AS DateTime), N'wzkR89zRXe7hND1jqAP9xgupYJBvEZcjsfUe3TaU45kxRajjjS9u0fOTLK+uglzk67EGochJdeoikWs7hxMNRA', N'h]-zG', NULL, 1, NULL, CAST(0x00009C010125BA8F AS DateTime), NULL)
SET IDENTITY_INSERT [dbo].[User] OFF

/** Insert Users **/

SET IDENTITY_INSERT [dbo].[Task] ON

INSERT [dbo].[Task] ([Id], [StatusId], [PriorityId], [CreatedId], [Summary], [Details], [StartDate], [DueDate], [CompleteDate], [AssignedId], [CreatedDate], [ModifiedDate], [LastModifiedBy]) 
VALUES (1, 1, 1, 2, N'Make it to Earth', N'Find and make it to earth while avoiding the cylons.', NULL, NULL, NULL, 1, CAST(0x00009CE300B5D15E AS DateTime), CAST(0x00009CE300B5D15E AS DateTime), N'laura.roslin@battlestar.com')

SET IDENTITY_INSERT [dbo].[Task] OFF
