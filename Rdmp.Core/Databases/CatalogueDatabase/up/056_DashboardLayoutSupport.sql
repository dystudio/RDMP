--Version:2.1.0.1
--Description: Support for dynamic user created control collections which exist within a single tab
if not exists (select 1 from sys.tables where name = 'DashboardLayout')
begin
CREATE TABLE [dbo].[DashboardLayout](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](1000) NOT NULL,
	[Username] [varchar](500) NOT NULL,
	[Created] [datetime] NOT NULL CONSTRAINT [DF_DashboardLayout_Created]  DEFAULT (getdate()),
	
 CONSTRAINT [PK_DashboardLayout] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

end
GO

if not exists( select 1 from sys.tables where name = 'DashboardControl')
begin

CREATE TABLE [dbo].[DashboardControl](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DashboardLayout_ID] [int] NOT NULL,
	[ControlType] [varchar](1000) NOT NULL,
	[X] [int] NOT NULL,
	[Y] [int] NOT NULL,
	[Width] [int] NOT NULL,
	[Height] [int] NOT NULL,
	PersistenceString varchar(max) NULL,
	 CONSTRAINT [FK_DashboardControl_DashboardLayout] FOREIGN KEY([DashboardLayout_ID])
REFERENCES [dbo].[DashboardLayout] ([ID]) ON DELETE CASCADE,
 CONSTRAINT [PK_DashboardControl] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

end
GO

if not exists( select 1 from sys.tables where name = 'DashboardObjectUse')
begin
CREATE TABLE [dbo].[DashboardObjectUse](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DashboardControl_ID] [int] NOT NULL,
	[TypeName] [varchar](500) NOT NULL,
	[ObjectID] [int] NOT NULL,
	[RepositoryTypeName] [varchar](500) NOT NULL,
	 CONSTRAINT [FK_DashboardObjectUsage_DashboardControl] FOREIGN KEY([DashboardControl_ID])
REFERENCES [dbo].[DashboardControl] ([ID]) ON DELETE CASCADE,
 CONSTRAINT [PK_DashboardObjectUse] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
end

GO
if not exists ( select 1 from sys.indexes where name = 'ix_DashboardControlObjectUseNoDuplicatesAllowed')
begin 
CREATE UNIQUE NONCLUSTERED INDEX [ix_DashboardControlObjectUseNoDuplicatesAllowed] ON [dbo].[DashboardObjectUse]
(
	[DashboardControl_ID] ASC,
	[TypeName] ASC,
	[ObjectID] ASC
)
end