--Version:1.18.0.0
--Description: Adds CacheFetchFailure table for logging when cache requests from a caching source fail

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'CacheFetchFailure')
BEGIN
CREATE TABLE [dbo].[CacheFetchFailure](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CacheProgress_ID] [int] NOT NULL,
	[FetchRequestStart] [datetime] NOT NULL,
	[FetchRequestEnd] [datetime] NOT NULL,
	[ExceptionText] [varchar](max) NULL,
	[LastAttempt] [datetime] NOT NULL,
	[ResolvedOn] [datetime] NULL,
 CONSTRAINT [PK_CacheFetchFailure] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)
)
END

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_CacheFetchFailure_CacheProgress')
BEGIN
	ALTER TABLE [dbo].[CacheFetchFailure]  WITH CHECK ADD  CONSTRAINT [FK_CacheFetchFailure_CacheProgress] FOREIGN KEY([CacheProgress_ID])
	REFERENCES [dbo].[CacheProgress] ([ID])
	ON DELETE CASCADE

	ALTER TABLE [dbo].[CacheFetchFailure] CHECK CONSTRAINT [FK_CacheFetchFailure_CacheProgress]
END