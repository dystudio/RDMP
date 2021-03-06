--Version:2.6.0.1
--Description: Creates the RemoteRDMP Table and Adds a Name to the AutomationServiceSlot
if not exists (select 1 from sys.tables where name = 'ObjectImport')
begin

CREATE TABLE [dbo].[ObjectImport](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SharingUID] [varchar](36) NOT NULL,
	[LocalObjectID] [int] NOT NULL,
	[LocalTypeName] [varchar](500) NOT NULL,
	[RepositoryTypeName] [varchar](500) NOT NULL,
 CONSTRAINT [PK_ObjectImports] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[ObjectExport](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ObjectTypeName] [varchar](500) NOT NULL,
	[ObjectID] [int] NOT NULL,
	[SharingUID] [varchar](36) NOT NULL,
	[RepositoryTypeName] [varchar](500) NOT NULL,
 CONSTRAINT [PK_ObjectShares] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

end

/****** Object:  Index [ix_YouCanExportEachObjectOnlyOnce]    Script Date: 09/11/2017 10:31:36 ******/
if not exists (select 1 from sys.indexes where name ='ix_YouCanExportEachObjectOnlyOnce')
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [ix_YouCanExportEachObjectOnlyOnce] ON [dbo].[ObjectExport]
    (
	    [ObjectTypeName] ASC,
	    [ObjectID] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
END

/****** Object:  Index [ix_YouCanImportEachObjectOnlyOnce]    Script Date: 09/11/2017 10:31:36 ******/
if not exists (select 1 from sys.indexes where name ='ix_YouCanImportEachObjectOnlyOnce')
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [ix_YouCanImportEachObjectOnlyOnce] ON [dbo].[ObjectImport]
    (
	    [SharingUID] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
END
