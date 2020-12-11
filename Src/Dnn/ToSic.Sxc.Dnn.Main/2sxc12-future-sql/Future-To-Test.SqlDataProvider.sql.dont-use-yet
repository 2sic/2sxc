SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Apps]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ToSIC_EAV_Apps](
	[AppID] [int] IDENTITY(1,1) NOT NULL,
	[ZoneID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_ToSIC_EAV_Apps] PRIMARY KEY CLUSTERED
(
	[AppID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
SET IDENTITY_INSERT [dbo].[ToSIC_EAV_Apps] ON
INSERT [dbo].[ToSIC_EAV_Apps] ([AppID], [ZoneID], [Name]) VALUES (1, 1, N'Default')
SET IDENTITY_INSERT [dbo].[ToSIC_EAV_Apps] OFF
END
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_AssignmentObjectTypes]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ToSIC_EAV_AssignmentObjectTypes](
	[AssignmentObjectTypeID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_ToSIC_EAV_AssignmentObjectTypes] PRIMARY KEY CLUSTERED
(
	[AssignmentObjectTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
SET IDENTITY_INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ON
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (1, N'Default', N'Default')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (2, N'EAV Field Properties', N'EAV Field Properties')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (3, N'App', N'App')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (4, N'Entity', N'For Permissions, Data Pipelines with Pipeline Parts and Configurations')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (5, N'ContentType', N'Metadata for ContentTypes')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (6, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (7, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (8, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (9, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (10, N'CmsObject', N'References to CMS objects like files and pages')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (11, N'News?', N'News?')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (12, N'Contacts?', N'Contacts?')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (13, N'2SexyContent', N'2Sexy Contents data for the 2Sexy Content Module')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (14, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (15, N'2SexyContent-Template', N'2Sexy Content Template')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (16, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (17, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (18, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (19, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (20, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (21, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (22, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (23, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (24, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (25, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (26, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (27, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (28, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (29, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (30, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (31, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (32, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (33, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (34, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (35, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (36, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (37, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (38, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (39, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (40, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (41, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (42, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (43, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (44, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (45, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (46, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (47, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (48, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (49, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (50, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (51, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (52, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (53, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (54, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (55, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (56, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (57, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (58, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (59, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (60, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (61, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (62, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (63, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (64, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (65, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (66, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (67, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (68, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (69, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (70, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (71, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (72, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (73, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (74, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (75, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (76, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (77, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (78, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (79, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (80, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (81, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (82, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (83, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (84, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (85, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (86, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (87, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (88, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (89, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (90, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (91, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (92, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (93, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (94, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (95, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (96, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (97, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (98, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (99, N'Reserved', N'Reserved')
INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID], [Name], [Description]) VALUES (100, N'Reserved', N'Reserved')
SET IDENTITY_INSERT [dbo].[ToSIC_EAV_AssignmentObjectTypes] OFF
END
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_AttributeGroups]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ToSIC_EAV_AttributeGroups](
	[AttributeGroupID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[SortOrder] [int] NOT NULL,
	[AttributeSetID] [int] NOT NULL,
 CONSTRAINT [PK_ToSIC_EAV_AttributeGroups] PRIMARY KEY CLUSTERED
(
	[AttributeGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
SET IDENTITY_INSERT [dbo].[ToSIC_EAV_AttributeGroups] ON
INSERT [dbo].[ToSIC_EAV_AttributeGroups] ([AttributeGroupID], [Name], [SortOrder], [AttributeSetID]) VALUES (1, N'Default', 0, 1)
SET IDENTITY_INSERT [dbo].[ToSIC_EAV_AttributeGroups] OFF
END
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Attributes]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ToSIC_EAV_Attributes](
	[AttributeID] [int] IDENTITY(1,1) NOT NULL,
	[StaticName] [nvarchar](50) NOT NULL,
	[Type] [nvarchar](50) NOT NULL,
	[ChangeLogCreated] [int] NOT NULL,
	[ChangeLogDeleted] [int] NULL,
 CONSTRAINT [PK_ToSIC_EAV_Attributes] PRIMARY KEY CLUSTERED
(
	[AttributeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_AttributeSets]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ToSIC_EAV_AttributeSets](
	[AttributeSetID] [int] IDENTITY(1,1) NOT NULL,
	[StaticName] [nvarchar](150) NULL,
	[Name] [nvarchar](150) NULL,
	[Scope] [nvarchar](50) NULL,
	[Description] [nvarchar](max) NOT NULL,
	[ChangeLogCreated] [int] NOT NULL,
	[ChangeLogDeleted] [int] NULL,
	[AppID] [int] NOT NULL,
	[UsesConfigurationOfAttributeSet] [int] NULL,
	[AlwaysShareConfiguration] [bit] NOT NULL,
	[Json] [nvarchar](max) NULL,
 CONSTRAINT [PK_ToSIC_EAV_AttributeSets] PRIMARY KEY CLUSTERED
(
	[AttributeSetID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
SET IDENTITY_INSERT [dbo].[ToSIC_EAV_AttributeSets] ON
INSERT [dbo].[ToSIC_EAV_AttributeSets] ([AttributeSetID], [StaticName], [Name], [Scope], [Description], [ChangeLogCreated], [ChangeLogDeleted], [AppID], [UsesConfigurationOfAttributeSet], [AlwaysShareConfiguration], [Json]) VALUES (1, N'Default', N'Default (built in)', N'2SexyContent-System', N'Default', 1, NULL, 1, NULL, 1, NULL)
SET IDENTITY_INSERT [dbo].[ToSIC_EAV_AttributeSets] OFF
END
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_AttributesInSets]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ToSIC_EAV_AttributesInSets](
	[AttributeID] [int] NOT NULL,
	[AttributeSetID] [int] NOT NULL,
	[AttributeGroupID] [int] NOT NULL,
	[SortOrder] [int] NOT NULL,
	[IsTitle] [bit] NOT NULL,
 CONSTRAINT [PK_ToSIC_EAV_AttributesInSets] PRIMARY KEY CLUSTERED
(
	[AttributeID] ASC,
	[AttributeSetID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_AttributeTypes]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ToSIC_EAV_AttributeTypes](
	[Type] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_ToSIC_EAV_AttributeTypes] PRIMARY KEY CLUSTERED
(
	[Type] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
INSERT [dbo].[ToSIC_EAV_AttributeTypes] ([Type]) VALUES (N'Boolean')
INSERT [dbo].[ToSIC_EAV_AttributeTypes] ([Type]) VALUES (N'Custom')
INSERT [dbo].[ToSIC_EAV_AttributeTypes] ([Type]) VALUES (N'DateTime')
INSERT [dbo].[ToSIC_EAV_AttributeTypes] ([Type]) VALUES (N'Empty')
INSERT [dbo].[ToSIC_EAV_AttributeTypes] ([Type]) VALUES (N'Entity')
INSERT [dbo].[ToSIC_EAV_AttributeTypes] ([Type]) VALUES (N'Hyperlink')
INSERT [dbo].[ToSIC_EAV_AttributeTypes] ([Type]) VALUES (N'Number')
INSERT [dbo].[ToSIC_EAV_AttributeTypes] ([Type]) VALUES (N'String')
END
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_ChangeLog]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ToSIC_EAV_ChangeLog](
	[ChangeID] [int] IDENTITY(1,1) NOT NULL,
	[Timestamp] [datetime] NOT NULL,
	[User] [nvarchar](255) NULL,
 CONSTRAINT [PK_ToSIC_EAV_ChangeLog] PRIMARY KEY CLUSTERED
(
	[ChangeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
SET IDENTITY_INSERT [dbo].[ToSIC_EAV_ChangeLog] ON
INSERT [dbo].[ToSIC_EAV_ChangeLog] ([ChangeID], [Timestamp], [User]) VALUES (1, CAST(N'2012-05-02T08:31:35.297' AS DateTime), NULL)
INSERT [dbo].[ToSIC_EAV_ChangeLog] ([ChangeID], [Timestamp], [User]) VALUES (100, CAST(N'2020-10-20T00:00:00.000' AS DateTime), NULL)
SET IDENTITY_INSERT [dbo].[ToSIC_EAV_ChangeLog] OFF
END
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_ContextInfo]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ToSIC_EAV_ContextInfo](
	[ContextInfo] [varbinary](128) NOT NULL,
	[ChangeID] [nvarchar](128) NOT NULL,
	[UpdatedAt] [datetime] NOT NULL,
 CONSTRAINT [PK_ToSIC_EAV_ContextInfo] PRIMARY KEY CLUSTERED
(
	[ContextInfo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
INSERT [dbo].[ToSIC_EAV_ContextInfo] ([ContextInfo], [ChangeID], [UpdatedAt]) VALUES (0x7398907A91C4BC4BBEEF94F97F76996E00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000, N'99', CAST(N'2020-09-14T08:29:21.173' AS DateTime))
END
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_DataTimeline]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ToSIC_EAV_DataTimeline](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SourceTable] [nvarchar](250) NOT NULL,
	[SourceID] [int] NULL,
	[SourceGuid] [uniqueidentifier] NULL,
	[SourceTextKey] [nvarchar](250) NULL,
	[Operation] [nchar](1) NOT NULL,
	[SysCreatedDate] [datetime] NOT NULL,
	[SysLogID] [int] NULL,
	[NewData] [xml] NOT NULL,
	[Json] [nvarchar](max) NULL,
 CONSTRAINT [PK_ToSIC_EAV_DataTimeline] PRIMARY KEY CLUSTERED
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Dimensions]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ToSIC_EAV_Dimensions](
	[DimensionID] [int] IDENTITY(1,1) NOT NULL,
	[Parent] [int] NULL,
	[Name] [nvarchar](100) NOT NULL,
	[SystemKey] [nvarchar](100) NULL,
	[ExternalKey] [nvarchar](100) NULL,
	[Active] [bit] NOT NULL,
	[ZoneID] [int] NOT NULL,
 CONSTRAINT [PK_ToSIC_EAV_Dimensions] PRIMARY KEY CLUSTERED
(
	[DimensionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
SET IDENTITY_INSERT [dbo].[ToSIC_EAV_Dimensions] ON
INSERT [dbo].[ToSIC_EAV_Dimensions] ([DimensionID], [Parent], [Name], [SystemKey], [ExternalKey], [Active], [ZoneID]) VALUES (1, NULL, N'Culture Root', N'Culture', NULL, 1, 1)
SET IDENTITY_INSERT [dbo].[ToSIC_EAV_Dimensions] OFF
END
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Entities]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ToSIC_EAV_Entities](
	[EntityID] [int] IDENTITY(1,1) NOT NULL,
	[EntityGUID] [uniqueidentifier] NOT NULL,
	[AttributeSetID] [int] NOT NULL,
	[ConfigurationSet] [int] NULL,
	[AssignmentObjectTypeID] [int] NOT NULL,
	[KeyNumber] [int] NULL,
	[KeyGuid] [uniqueidentifier] NULL,
	[KeyString] [nvarchar](100) NULL,
	[SortOrder] [int] NOT NULL,
	[ChangeLogCreated] [int] NOT NULL,
	[ChangeLogDeleted] [int] NULL,
	[IsPublished] [bit] NOT NULL,
	[PublishedEntityId] [int] NULL,
	[ChangeLogModified] [int] NOT NULL,
	[Owner] [nvarchar](250) NULL,
	[Json] [nvarchar](max) NULL,
	[Version] [int] NOT NULL,
	[AppId] [int] NOT NULL,
	[ContentType] [nvarchar](250) NULL,
 CONSTRAINT [PK_ToSIC_EAV_Entities] PRIMARY KEY CLUSTERED
(
	[EntityID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_EntityRelationships]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ToSIC_EAV_EntityRelationships](
	[AttributeID] [int] NOT NULL,
	[ParentEntityID] [int] NOT NULL,
	[ChildEntityID] [int] NULL,
	[SortOrder] [int] NOT NULL,
 CONSTRAINT [PK_ToSIC_EAV_EntityRelationships] PRIMARY KEY CLUSTERED
(
	[AttributeID] ASC,
	[ParentEntityID] ASC,
	[SortOrder] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Values]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ToSIC_EAV_Values](
	[ValueID] [int] IDENTITY(1,1) NOT NULL,
	[EntityID] [int] NOT NULL,
	[AttributeID] [int] NOT NULL,
	[Value] [nvarchar](max) NOT NULL,
	[ChangeLogCreated] [int] NOT NULL,
	[ChangeLogDeleted] [int] NULL,
	[ChangeLogModified] [int] NULL,
 CONSTRAINT [PK_ToSIC_EAV_Values] PRIMARY KEY CLUSTERED
(
	[ValueID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_ValuesDimensions]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ToSIC_EAV_ValuesDimensions](
	[ValueID] [int] NOT NULL,
	[DimensionID] [int] NOT NULL,
	[ReadOnly] [bit] NOT NULL,
 CONSTRAINT [PK_ToSIC_EAV_ValuesDimensions] PRIMARY KEY CLUSTERED
(
	[ValueID] ASC,
	[DimensionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Zones]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ToSIC_EAV_Zones](
	[ZoneID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_ToSIC_EAV_Zones] PRIMARY KEY CLUSTERED
(
	[ZoneID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
SET IDENTITY_INSERT [dbo].[ToSIC_EAV_Zones] ON
INSERT [dbo].[ToSIC_EAV_Zones] ([ZoneID], [Name]) VALUES (1, N'Default')
SET IDENTITY_INSERT [dbo].[ToSIC_EAV_Zones] OFF
END
GO


SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Apps]') AND name = N'ToSIC_EAV_Apps_PreventDuplicates')
ALTER TABLE [dbo].[ToSIC_EAV_Apps] ADD  CONSTRAINT [ToSIC_EAV_Apps_PreventDuplicates] UNIQUE NONCLUSTERED
(
	[Name] ASC,
	[ZoneID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_AssignmentObjectTypes]') AND name = N'IX_ToSIC_EAV_AssignmentObjectTypes')
CREATE NONCLUSTERED INDEX [IX_ToSIC_EAV_AssignmentObjectTypes] ON [dbo].[ToSIC_EAV_AssignmentObjectTypes]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Entities]') AND name = N'IX_KeyNumber')
CREATE NONCLUSTERED INDEX [IX_KeyNumber] ON [dbo].[ToSIC_EAV_Entities]
(
	[KeyNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Values]') AND name = N'IX_EAV_Values1')
CREATE NONCLUSTERED INDEX [IX_EAV_Values1] ON [dbo].[ToSIC_EAV_Values]
(
	[AttributeID] ASC,
	[EntityID] ASC,
	[ChangeLogDeleted] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Values]') AND name = N'IX_EAV_Values2')
CREATE NONCLUSTERED INDEX [IX_EAV_Values2] ON [dbo].[ToSIC_EAV_Values]
(
	[EntityID] ASC,
	[ChangeLogDeleted] ASC,
	[AttributeID] ASC,
	[ValueID] ASC
)
INCLUDE([Value],[ChangeLogCreated]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_ToSIC_EAV_AttributeSets_StaticName]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ToSIC_EAV_AttributeSets] ADD  CONSTRAINT [DF_ToSIC_EAV_AttributeSets_StaticName]  DEFAULT (newid()) FOR [StaticName]
END
GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_ToSIC_EAV_AttributeSets_AlwaysShareConfiguration]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ToSIC_EAV_AttributeSets] ADD  CONSTRAINT [DF_ToSIC_EAV_AttributeSets_AlwaysShareConfiguration]  DEFAULT ((0)) FOR [AlwaysShareConfiguration]
END
GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_ToSIC_EAV_AttributesInSets_IsTitle]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ToSIC_EAV_AttributesInSets] ADD  CONSTRAINT [DF_ToSIC_EAV_AttributesInSets_IsTitle]  DEFAULT ((0)) FOR [IsTitle]
END
GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_ToSIC_EAV_ChangeLog_Timestamp]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ToSIC_EAV_ChangeLog] ADD  CONSTRAINT [DF_ToSIC_EAV_ChangeLog_Timestamp]  DEFAULT (getdate()) FOR [Timestamp]
END
GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_DataTimeline_Operation]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ToSIC_EAV_DataTimeline] ADD  CONSTRAINT [DF_DataTimeline_Operation]  DEFAULT (N'I') FOR [Operation]
END
GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_ToSIC_EAV_Dimensions_Active]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ToSIC_EAV_Dimensions] ADD  CONSTRAINT [DF_ToSIC_EAV_Dimensions_Active]  DEFAULT ((1)) FOR [Active]
END
GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_ToSIC_EAV_Entities_EntityGUID]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ToSIC_EAV_Entities] ADD  CONSTRAINT [DF_ToSIC_EAV_Entities_EntityGUID]  DEFAULT (newid()) FOR [EntityGUID]
END
GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_ToSIC_EAV_Entities_IsPublished]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ToSIC_EAV_Entities] ADD  CONSTRAINT [DF_ToSIC_EAV_Entities_IsPublished]  DEFAULT ((1)) FOR [IsPublished]
END
GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_ToSIC_EAV_ValuesDimensions_ReadOnly]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ToSIC_EAV_ValuesDimensions] ADD  CONSTRAINT [DF_ToSIC_EAV_ValuesDimensions_ReadOnly]  DEFAULT ((0)) FOR [ReadOnly]
END
GO


IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_Apps_ToSIC_EAV_Zones]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Apps]'))
ALTER TABLE [dbo].[ToSIC_EAV_Apps]  WITH CHECK ADD  CONSTRAINT [FK_ToSIC_EAV_Apps_ToSIC_EAV_Zones] FOREIGN KEY([ZoneID])
REFERENCES [dbo].[ToSIC_EAV_Zones] ([ZoneID])
GO


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_Apps_ToSIC_EAV_Zones]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Apps]'))
ALTER TABLE [dbo].[ToSIC_EAV_Apps] CHECK CONSTRAINT [FK_ToSIC_EAV_Apps_ToSIC_EAV_Zones]
GO


IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_AttributeGroups_ToSIC_EAV_AttributeSets]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_AttributeGroups]'))
ALTER TABLE [dbo].[ToSIC_EAV_AttributeGroups]  WITH CHECK ADD  CONSTRAINT [FK_ToSIC_EAV_AttributeGroups_ToSIC_EAV_AttributeSets] FOREIGN KEY([AttributeSetID])
REFERENCES [dbo].[ToSIC_EAV_AttributeSets] ([AttributeSetID])
GO


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_AttributeGroups_ToSIC_EAV_AttributeSets]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_AttributeGroups]'))
ALTER TABLE [dbo].[ToSIC_EAV_AttributeGroups] CHECK CONSTRAINT [FK_ToSIC_EAV_AttributeGroups_ToSIC_EAV_AttributeSets]
GO


IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_Attributes_ToSIC_EAV_ChangeLogCreated]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Attributes]'))
ALTER TABLE [dbo].[ToSIC_EAV_Attributes]  WITH CHECK ADD  CONSTRAINT [FK_ToSIC_EAV_Attributes_ToSIC_EAV_ChangeLogCreated] FOREIGN KEY([ChangeLogCreated])
REFERENCES [dbo].[ToSIC_EAV_ChangeLog] ([ChangeID])
GO


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_Attributes_ToSIC_EAV_ChangeLogCreated]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Attributes]'))
ALTER TABLE [dbo].[ToSIC_EAV_Attributes] CHECK CONSTRAINT [FK_ToSIC_EAV_Attributes_ToSIC_EAV_ChangeLogCreated]
GO


IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_Attributes_ToSIC_EAV_ChangeLogDeleted]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Attributes]'))
ALTER TABLE [dbo].[ToSIC_EAV_Attributes]  WITH CHECK ADD  CONSTRAINT [FK_ToSIC_EAV_Attributes_ToSIC_EAV_ChangeLogDeleted] FOREIGN KEY([ChangeLogDeleted])
REFERENCES [dbo].[ToSIC_EAV_ChangeLog] ([ChangeID])
GO


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_Attributes_ToSIC_EAV_ChangeLogDeleted]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Attributes]'))
ALTER TABLE [dbo].[ToSIC_EAV_Attributes] CHECK CONSTRAINT [FK_ToSIC_EAV_Attributes_ToSIC_EAV_ChangeLogDeleted]
GO


IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_Attributes_ToSIC_EAV_Types]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Attributes]'))
ALTER TABLE [dbo].[ToSIC_EAV_Attributes]  WITH CHECK ADD  CONSTRAINT [FK_ToSIC_EAV_Attributes_ToSIC_EAV_Types] FOREIGN KEY([Type])
REFERENCES [dbo].[ToSIC_EAV_AttributeTypes] ([Type])
GO


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_Attributes_ToSIC_EAV_Types]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Attributes]'))
ALTER TABLE [dbo].[ToSIC_EAV_Attributes] CHECK CONSTRAINT [FK_ToSIC_EAV_Attributes_ToSIC_EAV_Types]
GO


IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_AttributeSets_ToSIC_EAV_Apps]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_AttributeSets]'))
ALTER TABLE [dbo].[ToSIC_EAV_AttributeSets]  WITH CHECK ADD  CONSTRAINT [FK_ToSIC_EAV_AttributeSets_ToSIC_EAV_Apps] FOREIGN KEY([AppID])
REFERENCES [dbo].[ToSIC_EAV_Apps] ([AppID])
GO


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_AttributeSets_ToSIC_EAV_Apps]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_AttributeSets]'))
ALTER TABLE [dbo].[ToSIC_EAV_AttributeSets] CHECK CONSTRAINT [FK_ToSIC_EAV_AttributeSets_ToSIC_EAV_Apps]
GO


IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_AttributeSets_ToSIC_EAV_AttributeSets]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_AttributeSets]'))
ALTER TABLE [dbo].[ToSIC_EAV_AttributeSets]  WITH CHECK ADD  CONSTRAINT [FK_ToSIC_EAV_AttributeSets_ToSIC_EAV_AttributeSets] FOREIGN KEY([UsesConfigurationOfAttributeSet])
REFERENCES [dbo].[ToSIC_EAV_AttributeSets] ([AttributeSetID])
GO


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_AttributeSets_ToSIC_EAV_AttributeSets]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_AttributeSets]'))
ALTER TABLE [dbo].[ToSIC_EAV_AttributeSets] CHECK CONSTRAINT [FK_ToSIC_EAV_AttributeSets_ToSIC_EAV_AttributeSets]
GO


IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_AttributeSets_ToSIC_EAV_ChangeLogCreated]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_AttributeSets]'))
ALTER TABLE [dbo].[ToSIC_EAV_AttributeSets]  WITH CHECK ADD  CONSTRAINT [FK_ToSIC_EAV_AttributeSets_ToSIC_EAV_ChangeLogCreated] FOREIGN KEY([ChangeLogCreated])
REFERENCES [dbo].[ToSIC_EAV_ChangeLog] ([ChangeID])
GO


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_AttributeSets_ToSIC_EAV_ChangeLogCreated]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_AttributeSets]'))
ALTER TABLE [dbo].[ToSIC_EAV_AttributeSets] CHECK CONSTRAINT [FK_ToSIC_EAV_AttributeSets_ToSIC_EAV_ChangeLogCreated]
GO


IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_AttributeSets_ToSIC_EAV_ChangeLogDeleted]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_AttributeSets]'))
ALTER TABLE [dbo].[ToSIC_EAV_AttributeSets]  WITH CHECK ADD  CONSTRAINT [FK_ToSIC_EAV_AttributeSets_ToSIC_EAV_ChangeLogDeleted] FOREIGN KEY([ChangeLogDeleted])
REFERENCES [dbo].[ToSIC_EAV_ChangeLog] ([ChangeID])
GO


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_AttributeSets_ToSIC_EAV_ChangeLogDeleted]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_AttributeSets]'))
ALTER TABLE [dbo].[ToSIC_EAV_AttributeSets] CHECK CONSTRAINT [FK_ToSIC_EAV_AttributeSets_ToSIC_EAV_ChangeLogDeleted]
GO


IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_AttributesInSets_ToSIC_EAV_AttributeGroups]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_AttributesInSets]'))
ALTER TABLE [dbo].[ToSIC_EAV_AttributesInSets]  WITH CHECK ADD  CONSTRAINT [FK_ToSIC_EAV_AttributesInSets_ToSIC_EAV_AttributeGroups] FOREIGN KEY([AttributeGroupID])
REFERENCES [dbo].[ToSIC_EAV_AttributeGroups] ([AttributeGroupID])
GO


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_AttributesInSets_ToSIC_EAV_AttributeGroups]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_AttributesInSets]'))
ALTER TABLE [dbo].[ToSIC_EAV_AttributesInSets] CHECK CONSTRAINT [FK_ToSIC_EAV_AttributesInSets_ToSIC_EAV_AttributeGroups]
GO


IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_AttributesInSets_ToSIC_EAV_Attributes]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_AttributesInSets]'))
ALTER TABLE [dbo].[ToSIC_EAV_AttributesInSets]  WITH CHECK ADD  CONSTRAINT [FK_ToSIC_EAV_AttributesInSets_ToSIC_EAV_Attributes] FOREIGN KEY([AttributeID])
REFERENCES [dbo].[ToSIC_EAV_Attributes] ([AttributeID])
ON DELETE CASCADE
GO


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_AttributesInSets_ToSIC_EAV_Attributes]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_AttributesInSets]'))
ALTER TABLE [dbo].[ToSIC_EAV_AttributesInSets] CHECK CONSTRAINT [FK_ToSIC_EAV_AttributesInSets_ToSIC_EAV_Attributes]
GO


IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_AttributesInSets_ToSIC_EAV_AttributeSets]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_AttributesInSets]'))
ALTER TABLE [dbo].[ToSIC_EAV_AttributesInSets]  WITH CHECK ADD  CONSTRAINT [FK_ToSIC_EAV_AttributesInSets_ToSIC_EAV_AttributeSets] FOREIGN KEY([AttributeSetID])
REFERENCES [dbo].[ToSIC_EAV_AttributeSets] ([AttributeSetID])
GO


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_AttributesInSets_ToSIC_EAV_AttributeSets]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_AttributesInSets]'))
ALTER TABLE [dbo].[ToSIC_EAV_AttributesInSets] CHECK CONSTRAINT [FK_ToSIC_EAV_AttributesInSets_ToSIC_EAV_AttributeSets]
GO


IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_Dimensions_ToSIC_EAV_Dimensions1]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Dimensions]'))
ALTER TABLE [dbo].[ToSIC_EAV_Dimensions]  WITH CHECK ADD  CONSTRAINT [FK_ToSIC_EAV_Dimensions_ToSIC_EAV_Dimensions1] FOREIGN KEY([Parent])
REFERENCES [dbo].[ToSIC_EAV_Dimensions] ([DimensionID])
GO


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_Dimensions_ToSIC_EAV_Dimensions1]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Dimensions]'))
ALTER TABLE [dbo].[ToSIC_EAV_Dimensions] CHECK CONSTRAINT [FK_ToSIC_EAV_Dimensions_ToSIC_EAV_Dimensions1]
GO


IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_Dimensions_ToSIC_EAV_Zones]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Dimensions]'))
ALTER TABLE [dbo].[ToSIC_EAV_Dimensions]  WITH CHECK ADD  CONSTRAINT [FK_ToSIC_EAV_Dimensions_ToSIC_EAV_Zones] FOREIGN KEY([ZoneID])
REFERENCES [dbo].[ToSIC_EAV_Zones] ([ZoneID])
GO


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_Dimensions_ToSIC_EAV_Zones]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Dimensions]'))
ALTER TABLE [dbo].[ToSIC_EAV_Dimensions] CHECK CONSTRAINT [FK_ToSIC_EAV_Dimensions_ToSIC_EAV_Zones]
GO


IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_Entities_ToSIC_EAV_Apps]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Entities]'))
ALTER TABLE [dbo].[ToSIC_EAV_Entities]  WITH CHECK ADD  CONSTRAINT [FK_ToSIC_EAV_Entities_ToSIC_EAV_Apps] FOREIGN KEY([AppId])
REFERENCES [dbo].[ToSIC_EAV_Apps] ([AppID])
GO


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_Entities_ToSIC_EAV_Apps]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Entities]'))
ALTER TABLE [dbo].[ToSIC_EAV_Entities] CHECK CONSTRAINT [FK_ToSIC_EAV_Entities_ToSIC_EAV_Apps]
GO


IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_Entities_ToSIC_EAV_AssignmentObjectTypes]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Entities]'))
ALTER TABLE [dbo].[ToSIC_EAV_Entities]  WITH CHECK ADD  CONSTRAINT [FK_ToSIC_EAV_Entities_ToSIC_EAV_AssignmentObjectTypes] FOREIGN KEY([AssignmentObjectTypeID])
REFERENCES [dbo].[ToSIC_EAV_AssignmentObjectTypes] ([AssignmentObjectTypeID])
GO


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_Entities_ToSIC_EAV_AssignmentObjectTypes]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Entities]'))
ALTER TABLE [dbo].[ToSIC_EAV_Entities] CHECK CONSTRAINT [FK_ToSIC_EAV_Entities_ToSIC_EAV_AssignmentObjectTypes]
GO


IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_Entities_ToSIC_EAV_AttributeSets]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Entities]'))
ALTER TABLE [dbo].[ToSIC_EAV_Entities]  WITH CHECK ADD  CONSTRAINT [FK_ToSIC_EAV_Entities_ToSIC_EAV_AttributeSets] FOREIGN KEY([AttributeSetID])
REFERENCES [dbo].[ToSIC_EAV_AttributeSets] ([AttributeSetID])
GO


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_Entities_ToSIC_EAV_AttributeSets]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Entities]'))
ALTER TABLE [dbo].[ToSIC_EAV_Entities] CHECK CONSTRAINT [FK_ToSIC_EAV_Entities_ToSIC_EAV_AttributeSets]
GO


IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_Entities_ToSIC_EAV_ChangeLog_Modified]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Entities]'))
ALTER TABLE [dbo].[ToSIC_EAV_Entities]  WITH CHECK ADD  CONSTRAINT [FK_ToSIC_EAV_Entities_ToSIC_EAV_ChangeLog_Modified] FOREIGN KEY([ChangeLogModified])
REFERENCES [dbo].[ToSIC_EAV_ChangeLog] ([ChangeID])
GO


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_Entities_ToSIC_EAV_ChangeLog_Modified]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Entities]'))
ALTER TABLE [dbo].[ToSIC_EAV_Entities] CHECK CONSTRAINT [FK_ToSIC_EAV_Entities_ToSIC_EAV_ChangeLog_Modified]
GO


IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_Entities_ToSIC_EAV_ChangeLogCreated]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Entities]'))
ALTER TABLE [dbo].[ToSIC_EAV_Entities]  WITH CHECK ADD  CONSTRAINT [FK_ToSIC_EAV_Entities_ToSIC_EAV_ChangeLogCreated] FOREIGN KEY([ChangeLogCreated])
REFERENCES [dbo].[ToSIC_EAV_ChangeLog] ([ChangeID])
GO


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_Entities_ToSIC_EAV_ChangeLogCreated]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Entities]'))
ALTER TABLE [dbo].[ToSIC_EAV_Entities] CHECK CONSTRAINT [FK_ToSIC_EAV_Entities_ToSIC_EAV_ChangeLogCreated]
GO


IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_Entities_ToSIC_EAV_ChangeLogDeleted]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Entities]'))
ALTER TABLE [dbo].[ToSIC_EAV_Entities]  WITH CHECK ADD  CONSTRAINT [FK_ToSIC_EAV_Entities_ToSIC_EAV_ChangeLogDeleted] FOREIGN KEY([ChangeLogDeleted])
REFERENCES [dbo].[ToSIC_EAV_ChangeLog] ([ChangeID])
GO


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_Entities_ToSIC_EAV_ChangeLogDeleted]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Entities]'))
ALTER TABLE [dbo].[ToSIC_EAV_Entities] CHECK CONSTRAINT [FK_ToSIC_EAV_Entities_ToSIC_EAV_ChangeLogDeleted]
GO


IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_Entities_ToSIC_EAV_Entities]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Entities]'))
ALTER TABLE [dbo].[ToSIC_EAV_Entities]  WITH CHECK ADD  CONSTRAINT [FK_ToSIC_EAV_Entities_ToSIC_EAV_Entities] FOREIGN KEY([ConfigurationSet])
REFERENCES [dbo].[ToSIC_EAV_Entities] ([EntityID])
GO


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_Entities_ToSIC_EAV_Entities]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Entities]'))
ALTER TABLE [dbo].[ToSIC_EAV_Entities] CHECK CONSTRAINT [FK_ToSIC_EAV_Entities_ToSIC_EAV_Entities]
GO


IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_EntityRelationships_ToSIC_EAV_Attributes]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_EntityRelationships]'))
ALTER TABLE [dbo].[ToSIC_EAV_EntityRelationships]  WITH CHECK ADD  CONSTRAINT [FK_ToSIC_EAV_EntityRelationships_ToSIC_EAV_Attributes] FOREIGN KEY([AttributeID])
REFERENCES [dbo].[ToSIC_EAV_Attributes] ([AttributeID])
GO


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_EntityRelationships_ToSIC_EAV_Attributes]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_EntityRelationships]'))
ALTER TABLE [dbo].[ToSIC_EAV_EntityRelationships] CHECK CONSTRAINT [FK_ToSIC_EAV_EntityRelationships_ToSIC_EAV_Attributes]
GO


IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_EntityRelationships_ToSIC_EAV_ChildEntities]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_EntityRelationships]'))
ALTER TABLE [dbo].[ToSIC_EAV_EntityRelationships]  WITH CHECK ADD  CONSTRAINT [FK_ToSIC_EAV_EntityRelationships_ToSIC_EAV_ChildEntities] FOREIGN KEY([ChildEntityID])
REFERENCES [dbo].[ToSIC_EAV_Entities] ([EntityID])
GO


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_EntityRelationships_ToSIC_EAV_ChildEntities]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_EntityRelationships]'))
ALTER TABLE [dbo].[ToSIC_EAV_EntityRelationships] CHECK CONSTRAINT [FK_ToSIC_EAV_EntityRelationships_ToSIC_EAV_ChildEntities]
GO


IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_EntityRelationships_ToSIC_EAV_ParentEntities]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_EntityRelationships]'))
ALTER TABLE [dbo].[ToSIC_EAV_EntityRelationships]  WITH CHECK ADD  CONSTRAINT [FK_ToSIC_EAV_EntityRelationships_ToSIC_EAV_ParentEntities] FOREIGN KEY([ParentEntityID])
REFERENCES [dbo].[ToSIC_EAV_Entities] ([EntityID])
GO


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_EntityRelationships_ToSIC_EAV_ParentEntities]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_EntityRelationships]'))
ALTER TABLE [dbo].[ToSIC_EAV_EntityRelationships] CHECK CONSTRAINT [FK_ToSIC_EAV_EntityRelationships_ToSIC_EAV_ParentEntities]
GO


IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_Values_ToSIC_EAV_Attributes]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Values]'))
ALTER TABLE [dbo].[ToSIC_EAV_Values]  WITH CHECK ADD  CONSTRAINT [FK_ToSIC_EAV_Values_ToSIC_EAV_Attributes] FOREIGN KEY([AttributeID])
REFERENCES [dbo].[ToSIC_EAV_Attributes] ([AttributeID])
ON DELETE CASCADE
GO


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_Values_ToSIC_EAV_Attributes]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Values]'))
ALTER TABLE [dbo].[ToSIC_EAV_Values] CHECK CONSTRAINT [FK_ToSIC_EAV_Values_ToSIC_EAV_Attributes]
GO


IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_Values_ToSIC_EAV_ChangeLogCreated]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Values]'))
ALTER TABLE [dbo].[ToSIC_EAV_Values]  WITH CHECK ADD  CONSTRAINT [FK_ToSIC_EAV_Values_ToSIC_EAV_ChangeLogCreated] FOREIGN KEY([ChangeLogCreated])
REFERENCES [dbo].[ToSIC_EAV_ChangeLog] ([ChangeID])
GO


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_Values_ToSIC_EAV_ChangeLogCreated]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Values]'))
ALTER TABLE [dbo].[ToSIC_EAV_Values] CHECK CONSTRAINT [FK_ToSIC_EAV_Values_ToSIC_EAV_ChangeLogCreated]
GO


IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_Values_ToSIC_EAV_ChangeLogDeleted]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Values]'))
ALTER TABLE [dbo].[ToSIC_EAV_Values]  WITH CHECK ADD  CONSTRAINT [FK_ToSIC_EAV_Values_ToSIC_EAV_ChangeLogDeleted] FOREIGN KEY([ChangeLogDeleted])
REFERENCES [dbo].[ToSIC_EAV_ChangeLog] ([ChangeID])
GO


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_Values_ToSIC_EAV_ChangeLogDeleted]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Values]'))
ALTER TABLE [dbo].[ToSIC_EAV_Values] CHECK CONSTRAINT [FK_ToSIC_EAV_Values_ToSIC_EAV_ChangeLogDeleted]
GO


IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_Values_ToSIC_EAV_ChangeLogModified]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Values]'))
ALTER TABLE [dbo].[ToSIC_EAV_Values]  WITH CHECK ADD  CONSTRAINT [FK_ToSIC_EAV_Values_ToSIC_EAV_ChangeLogModified] FOREIGN KEY([ChangeLogModified])
REFERENCES [dbo].[ToSIC_EAV_ChangeLog] ([ChangeID])
GO


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_Values_ToSIC_EAV_ChangeLogModified]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Values]'))
ALTER TABLE [dbo].[ToSIC_EAV_Values] CHECK CONSTRAINT [FK_ToSIC_EAV_Values_ToSIC_EAV_ChangeLogModified]
GO


IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_Values_ToSIC_EAV_Entities]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Values]'))
ALTER TABLE [dbo].[ToSIC_EAV_Values]  WITH CHECK ADD  CONSTRAINT [FK_ToSIC_EAV_Values_ToSIC_EAV_Entities] FOREIGN KEY([EntityID])
REFERENCES [dbo].[ToSIC_EAV_Entities] ([EntityID])
GO


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_Values_ToSIC_EAV_Entities]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_Values]'))
ALTER TABLE [dbo].[ToSIC_EAV_Values] CHECK CONSTRAINT [FK_ToSIC_EAV_Values_ToSIC_EAV_Entities]
GO


IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_ValuesDimensions_ToSIC_EAV_Dimensions]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_ValuesDimensions]'))
ALTER TABLE [dbo].[ToSIC_EAV_ValuesDimensions]  WITH CHECK ADD  CONSTRAINT [FK_ToSIC_EAV_ValuesDimensions_ToSIC_EAV_Dimensions] FOREIGN KEY([DimensionID])
REFERENCES [dbo].[ToSIC_EAV_Dimensions] ([DimensionID])
GO


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_ValuesDimensions_ToSIC_EAV_Dimensions]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_ValuesDimensions]'))
ALTER TABLE [dbo].[ToSIC_EAV_ValuesDimensions] CHECK CONSTRAINT [FK_ToSIC_EAV_ValuesDimensions_ToSIC_EAV_Dimensions]
GO


IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_ValuesDimensions_ToSIC_EAV_Values]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_ValuesDimensions]'))
ALTER TABLE [dbo].[ToSIC_EAV_ValuesDimensions]  WITH CHECK ADD  CONSTRAINT [FK_ToSIC_EAV_ValuesDimensions_ToSIC_EAV_Values] FOREIGN KEY([ValueID])
REFERENCES [dbo].[ToSIC_EAV_Values] ([ValueID])
GO


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ToSIC_EAV_ValuesDimensions_ToSIC_EAV_Values]') AND parent_object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_ValuesDimensions]'))
ALTER TABLE [dbo].[ToSIC_EAV_ValuesDimensions] CHECK CONSTRAINT [FK_ToSIC_EAV_ValuesDimensions_ToSIC_EAV_Values]
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_ChangeLogAdd]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[ToSIC_EAV_ChangeLogAdd] AS'
END
GO

ALTER PROCEDURE [dbo].[ToSIC_EAV_ChangeLogAdd]
	-- Add the parameters for the stored procedure here
	@User nvarchar(255) = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;

	-- Insert statements for procedure here
	INSERT INTO [dbo].[ToSIC_EAV_ChangeLog] ([Timestamp] ,[User])
	VALUES (GetDate(), @user)

	DECLARE @ChangeID int
	SET @ChangeID = scope_identity()
	EXEC ToSIC_EAV_ChangeLogSet @ChangeID

	SELECT *
	FROM [dbo].[ToSIC_EAV_ChangeLog]
	WHERE [ChangeID] = @ChangeID
END
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_ChangeLogGet]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[ToSIC_EAV_ChangeLogGet] AS'
END
GO

ALTER PROCEDURE [dbo].[ToSIC_EAV_ChangeLogGet]
AS
	SET NOCOUNT ON
	DECLARE @ContextInfo varbinary(128)
	SELECT @ContextInfo = CONTEXT_INFO()

	DECLARE @ChangeID int
	SET @ChangeID = 0

	SELECT @ChangeID = [ChangeID]
	FROM [dbo].[ToSIC_EAV_ContextInfo]
	WHERE [ContextInfo] = @ContextInfo

	RETURN @ChangeID

GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_ChangeLogSet]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[ToSIC_EAV_ChangeLogSet] AS'
END
GO


ALTER PROCEDURE [dbo].[ToSIC_EAV_ChangeLogSet]
  @ChangeID int
AS

SET NOCOUNT ON

-- Remove all context items older than an 5 minutes ago
DELETE FROM [dbo].[ToSIC_EAV_ContextInfo] WHERE [UpdatedAt] < DATEADD(mi, -5, GETUTCDATE())

IF SERVERPROPERTY('edition') <> 'SQL Azure' OR CAST(SERVERPROPERTY('ProductVersion') AS CHAR(2)) >= '12'
BEGIN
	DECLARE @b varbinary(128)
	SET @b = CONVERT(varbinary(128),newid())
	EXEC sp_executesql @statement=N'SET CONTEXT_INFO @b',@params=N'@b varbinary(128)',@b=@b
	print @b
END

DECLARE @ContextInfo varbinary(128)
SELECT @ContextInfo = CONTEXT_INFO()

IF EXISTS (SELECT * FROM [dbo].[ToSIC_EAV_ContextInfo] WHERE [ContextInfo] = @ContextInfo)
	UPDATE [dbo].[ToSIC_EAV_ContextInfo]
	SET
		[ChangeID] = @ChangeID,
		[UpdatedAt] = GETUTCDATE()
	WHERE
		ContextInfo = @ContextInfo
ELSE
	INSERT INTO [dbo].[ToSIC_EAV_ContextInfo] ([ContextInfo], [ChangeID], [UpdatedAt]) VALUES (@ContextInfo, @ChangeID, GETUTCDATE());


GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_DeleteApp]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[ToSIC_EAV_DeleteApp] AS'
END
GO


-- =============================================
-- Author:		Benjamin Gemperle
-- Create date: 2014-02-26
-- Description:	Delete an App in the 2sic EAV System
-- =============================================
ALTER PROCEDURE [dbo].[ToSIC_EAV_DeleteApp]
	@AppId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Delete Value-Dimensions
	DELETE FROM ToSIC_EAV_ValuesDimensions
	FROM            ToSIC_EAV_Values INNER JOIN
							 ToSIC_EAV_Entities ON ToSIC_EAV_Values.EntityID = ToSIC_EAV_Entities.EntityID INNER JOIN
							 ToSIC_EAV_AttributeSets ON ToSIC_EAV_Entities.AttributeSetID = ToSIC_EAV_AttributeSets.AttributeSetID INNER JOIN
							 ToSIC_EAV_ValuesDimensions ON ToSIC_EAV_Values.ValueID = ToSIC_EAV_ValuesDimensions.ValueID
	WHERE        (ToSIC_EAV_AttributeSets.AppID = @AppID)

	-- Delete Values
	DELETE FROM ToSIC_EAV_Values
	FROM            ToSIC_EAV_Values INNER JOIN
							 ToSIC_EAV_Entities ON ToSIC_EAV_Values.EntityID = ToSIC_EAV_Entities.EntityID INNER JOIN
							 ToSIC_EAV_AttributeSets ON ToSIC_EAV_Entities.AttributeSetID = ToSIC_EAV_AttributeSets.AttributeSetID
	WHERE        (ToSIC_EAV_AttributeSets.AppID = @AppID)

	-- Delete Parent-EntityRelationships
	DELETE FROM ToSIC_EAV_EntityRelationships
	FROM            ToSIC_EAV_Entities INNER JOIN
							 ToSIC_EAV_AttributeSets ON ToSIC_EAV_Entities.AttributeSetID = ToSIC_EAV_AttributeSets.AttributeSetID INNER JOIN
							 ToSIC_EAV_EntityRelationships ON ToSIC_EAV_Entities.EntityID = ToSIC_EAV_EntityRelationships.ParentEntityID
	WHERE        (ToSIC_EAV_AttributeSets.AppID = @AppID)

	-- Delete Child-EntityRelationships
	DELETE FROM ToSIC_EAV_EntityRelationships
	FROM            ToSIC_EAV_Entities INNER JOIN
							 ToSIC_EAV_AttributeSets ON ToSIC_EAV_Entities.AttributeSetID = ToSIC_EAV_AttributeSets.AttributeSetID INNER JOIN
							 ToSIC_EAV_EntityRelationships ON ToSIC_EAV_Entities.EntityID = ToSIC_EAV_EntityRelationships.ChildEntityID
	WHERE        (ToSIC_EAV_AttributeSets.AppID = @AppID)

	-- Delete Entities
	DELETE FROM ToSIC_EAV_Entities
	FROM            ToSIC_EAV_Entities INNER JOIN
							 ToSIC_EAV_AttributeSets ON ToSIC_EAV_Entities.AttributeSetID = ToSIC_EAV_AttributeSets.AttributeSetID
	WHERE        (ToSIC_EAV_AttributeSets.AppID = @AppId)

	-- Delete Attributes
	DELETE FROM ToSIC_EAV_Attributes
	FROM            ToSIC_EAV_Attributes INNER JOIN
							 ToSIC_EAV_AttributesInSets ON ToSIC_EAV_Attributes.AttributeID = ToSIC_EAV_AttributesInSets.AttributeID INNER JOIN
							 ToSIC_EAV_AttributeSets ON ToSIC_EAV_AttributesInSets.AttributeSetID = ToSIC_EAV_AttributeSets.AttributeSetID
	WHERE        (ToSIC_EAV_AttributeSets.AppID = @AppID)


	-- Delete Attributes not in use anywhere (Attribute not in any Set, no Values/Related Entities)
	DELETE FROM ToSIC_EAV_Attributes
	FROM            ToSIC_EAV_Attributes LEFT OUTER JOIN
							 ToSIC_EAV_AttributesInSets ON ToSIC_EAV_Attributes.AttributeID = ToSIC_EAV_AttributesInSets.AttributeID LEFT OUTER JOIN
							 ToSIC_EAV_EntityRelationships ON ToSIC_EAV_Attributes.AttributeID = ToSIC_EAV_EntityRelationships.AttributeID LEFT OUTER JOIN
							 ToSIC_EAV_Values ON ToSIC_EAV_Attributes.AttributeID = ToSIC_EAV_Values.AttributeID
	WHERE        (ToSIC_EAV_Values.ValueID IS NULL) AND (ToSIC_EAV_EntityRelationships.AttributeID IS NULL) AND (ToSIC_EAV_AttributesInSets.AttributeID IS NULL)

	-- Delete Attribute-In-Sets
	DELETE FROM ToSIC_EAV_AttributesInSets
	FROM            ToSIC_EAV_AttributeSets INNER JOIN
							 ToSIC_EAV_AttributesInSets ON ToSIC_EAV_AttributeSets.AttributeSetID = ToSIC_EAV_AttributesInSets.AttributeSetID
	WHERE        (ToSIC_EAV_AttributeSets.AppID = @AppId)

	-- Delete AttributeSets
	DELETE FROM ToSIC_EAV_AttributeSets WHERE AppID = @AppId

	-- Delete App
	DELETE FROM ToSIC_EAV_Apps WHERE AppID = @AppId


END

GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ToSIC_EAV_LogToTimeline]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[ToSIC_EAV_LogToTimeline] AS'
END
GO

ALTER PROCEDURE [dbo].[ToSIC_EAV_LogToTimeline]
	-- Add the parameters for the stored procedure here
	@table nvarchar(250) = '',
	@sourceID int = null,
	@sourceGuid uniqueidentifier = null,
	@sourceTextKey nvarchar(250) = null,
	@operation nchar(1),
	@sysCreated datetime = null,
	@sysChangeLogId int = null,
	@newData xml

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	Set @operation = Lower(@operation)	-- convert D-->d, U-->u, I-->i

	if @operation = 'd'
		Begin
			Select @newData = '<d/>'
		End

	-- Insert statements for procedure here
	INSERT INTO [ToSIC_EAV_DataTimeline]
		   ([SourceTable]
		   ,[SourceID]
		   ,[SourceGuid]
		   ,[SourceTextKey]
		   ,[Operation]
		   ,[SysCreatedDate]
		   ,[SysLogID]
		   ,[NewData])
	 VALUES
		   (@table
		   ,@sourceID
		   ,@sourceGuid
		   ,@sourceTextKey
		   ,@operation
		   ,@sysCreated
		   ,@sysChangeLogId
		   ,@newData)
END

GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[AutoLogAllChangesToTimeline_EntityRelationships]'))
EXEC dbo.sp_executesql @statement = N'
-- =============================================
-- Author:		Daniel Mettler
-- Create date: 2013-01-28
-- Description:	Automatically log all changes to the DataTimeline
-- =============================================
CREATE TRIGGER [dbo].[AutoLogAllChangesToTimeline_EntityRelationships]
   ON  [dbo].[ToSIC_EAV_EntityRelationships]
   AFTER INSERT,DELETE,UPDATE
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Insert statements for trigger here
	Declare @table nvarchar(250)
		,@rowID int
		,@rowGuid uniqueidentifier
		,@rowTextKey nvarchar(250)
		,@operation nchar(1)
		,@sysCreated datetime
		,@sysLogID int
		,@newData xml

	EXEC @sysLogId = [dbo].[ToSIC_EAV_ChangeLogGet]
	IF @sysLogId = 0
	BEGIN
		RAISERROR (''ChangeLogID is not set'', 0, 1)
		RETURN
	END

	-- Automatically get the table name where this trigger is attached
	Select @table = OBJECT_NAME(parent_id) FROM sys.triggers WHERE object_id=@@PROCID
	Select @sysCreated = GetDate()

	-- Find out if insert, update or delete
	-- Note: here you would adapt things to our table if you re-use this trigger
	-- 1. Ensure you use a valid field in both IF EXISTS queries (the SysCreated might not exist everywhere)
	-- 2. Ensure you get the right keys (this example uses @rowID, but you could also use @rowGuid, @rowTextKey)
	-- 3. if you have a logid, also set the @sysLogId
	-- Note: don''t know how to get the LogID in there when deleting...
	IF EXISTS (SELECT * FROM Inserted)
		BEGIN
			Select @rowID = ParentEntityID From inserted
			Select @newData = (Select * From Inserted For XML Auto)
			Set @operation = ''I''
			IF EXISTS (SELECT * FROM deleted)
				Begin
					SET @operation = ''U''
				End
		END
	ELSE
		BEGIN
			Select @rowID = ParentEntityID From deleted
			SET @operation = ''D''
		END

	-- Add the stuff...
	Exec dbo.ToSIC_EAV_LogToTimeline @table, @rowID, @rowGuid, @rowTextKey, @operation, @sysCreated, @sysLogId, @newData
END
'
GO
ALTER TABLE [dbo].[ToSIC_EAV_EntityRelationships] ENABLE TRIGGER [AutoLogAllChangesToTimeline_EntityRelationships]
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[AutoLogAllChangesToTimeline_Values]'))
EXEC dbo.sp_executesql @statement = N'
-- =============================================
-- Author:		Daniel Mettler
-- Create date: 2013-01-28
-- Description:	Automatically log all changes to the DataTimeline
-- =============================================
CREATE TRIGGER [dbo].[AutoLogAllChangesToTimeline_Values]
   ON  [dbo].[ToSIC_EAV_Values]
   AFTER INSERT,DELETE,UPDATE
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Insert statements for trigger here
	Declare @table nvarchar(250)
		,@rowID int
		,@rowGuid uniqueidentifier
		,@rowTextKey nvarchar(250)
		,@operation nchar(1)
		,@sysCreated datetime
		,@sysLogID int
		,@newData xml

	EXEC @sysLogId = [dbo].[ToSIC_EAV_ChangeLogGet]
	IF @sysLogId = 0
	BEGIN
		RAISERROR (''ChangeLogID is not set'', 0, 1)
		RETURN
	END

	-- Automatically get the table name where this trigger is attached
	Select @table = OBJECT_NAME(parent_id) FROM sys.triggers WHERE object_id=@@PROCID
	Select @sysCreated = GetDate()

	-- Find out if insert, update or delete
	-- Note: here you would adapt things to our table if you re-use this trigger
	-- 1. Ensure you use a valid field in both IF EXISTS queries (the SysCreated might not exist everywhere)
	-- 2. Ensure you get the right keys (this example uses @rowID, but you could also use @rowGuid, @rowTextKey)
	-- 3. if you have a logid, also set the @sysLogId
	-- Note: don''t know how to get the LogID in there when deleting...
	IF EXISTS (SELECT * FROM Inserted)
		BEGIN
			Select @rowID = ValueID From inserted
			Select @newData = (Select * From Inserted For XML Auto)
			Set @operation = ''I''
			IF EXISTS (SELECT * FROM deleted)
				Begin
					SET @operation = ''U''
				End
		END
	ELSE
		BEGIN
			Select @rowID = ValueID From deleted
			SET @operation = ''D''
		END

	-- Add the stuff...
	Exec dbo.ToSIC_EAV_LogToTimeline @table, @rowID, @rowGuid, @rowTextKey, @operation, @sysCreated, @sysLogId, @newData
END
'
GO
ALTER TABLE [dbo].[ToSIC_EAV_Values] ENABLE TRIGGER [AutoLogAllChangesToTimeline_Values]
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[AutoLogAllChangesToTimeline_ValuesDimensions]'))
EXEC dbo.sp_executesql @statement = N'
-- =============================================
-- Author:		Daniel Mettler
-- Create date: 2013-01-28
-- Description:	Automatically log all changes to the DataTimeline
-- =============================================
CREATE TRIGGER [dbo].[AutoLogAllChangesToTimeline_ValuesDimensions]
   ON  [dbo].[ToSIC_EAV_ValuesDimensions]
   AFTER INSERT,DELETE,UPDATE
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Insert statements for trigger here
	Declare @table nvarchar(250)
		,@rowID int
		,@rowGuid uniqueidentifier
		,@rowTextKey nvarchar(250)
		,@operation nchar(1)
		,@sysCreated datetime
		,@sysLogID int
		,@newData xml

	EXEC @sysLogId = [dbo].[ToSIC_EAV_ChangeLogGet]
	IF @sysLogId = 0
	BEGIN
		RAISERROR (''ChangeLogID is not set'', 0, 1)
		RETURN
	END

	-- Automatically get the table name where this trigger is attached
	Select @table = OBJECT_NAME(parent_id) FROM sys.triggers WHERE object_id=@@PROCID
	Select @sysCreated = GetDate()

	-- Find out if insert, update or delete
	-- Note: here you would adapt things to our table if you re-use this trigger
	-- 1. Ensure you use a valid field in both IF EXISTS queries (the SysCreated might not exist everywhere)
	-- 2. Ensure you get the right keys (this example uses @rowID, but you could also use @rowGuid, @rowTextKey)
	-- 3. if you have a logid, also set the @sysLogId
	-- Note: don''t know how to get the LogID in there when deleting...
	IF EXISTS (SELECT * FROM Inserted)
		BEGIN
			Select @rowID = ValueID From inserted
			Select @newData = (Select * From Inserted For XML Auto)
			Set @operation = ''I''
			IF EXISTS (SELECT * FROM deleted)
				Begin
					SET @operation = ''U''
				End
		END
	ELSE
		BEGIN
			Select @rowID = ValueID From deleted
			SET @operation = ''D''
		END

	-- Add the stuff...
	Exec dbo.ToSIC_EAV_LogToTimeline @table, @rowID, @rowGuid, @rowTextKey, @operation, @sysCreated, @sysLogId, @newData
END
'
GO
ALTER TABLE [dbo].[ToSIC_EAV_ValuesDimensions] ENABLE TRIGGER [AutoLogAllChangesToTimeline_ValuesDimensions]
GO
