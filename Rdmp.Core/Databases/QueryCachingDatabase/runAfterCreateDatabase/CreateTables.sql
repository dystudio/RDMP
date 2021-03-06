--Version:1.0.0.0
--Description: Initial creation script
CREATE TABLE [dbo].[CachedAggregateConfigurationResults](
	[Committer] [varchar](500) NOT NULL,
	[Date] [datetime] NOT NULL,
	[AggregateConfiguration_ID] [nchar](10) NOT NULL,
	[SqlExecuted] [varchar](max) NOT NULL,
	[Operation] [varchar](50) NOT NULL,
	[TableName] [varchar](500) NOT NULL,
 CONSTRAINT [PK_CachedAggregateConfigurationResults] PRIMARY KEY CLUSTERED 
(
	[AggregateConfiguration_ID] ASC,
	[Operation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[CachedAggregateConfigurationResults] ADD  CONSTRAINT [DF_CachedAggregateConfigurationResults_Date]  DEFAULT (getdate()) FOR [Date]
GO
