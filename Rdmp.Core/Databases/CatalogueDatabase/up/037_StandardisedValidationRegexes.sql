--Version:1.31.0.0
--Description: Allows you to define agency wide Regex validation concepts such as StudyBarcode [0-3]\s[0-9] and then reference them in Validation by concept name
if not exists (select 1 from sys.tables where name ='StandardRegex') 
begin
	CREATE TABLE [dbo].[StandardRegex](
		[ID] [int] IDENTITY(1,1) NOT NULL,
		[ConceptName] [varchar](500) NOT NULL,
		[Regex] [varchar](5000) NOT NULL,
		[Description] [varchar](5000) NULL,
	 CONSTRAINT [PK_StandardRegex] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
	) ON [PRIMARY]
end

if not exists (select 1 from sys.indexes where name = 'ix_ConceptNamesMustBeUnique') 
begin

CREATE UNIQUE NONCLUSTERED INDEX [ix_ConceptNamesMustBeUnique] ON [dbo].[StandardRegex]
(
	[ConceptName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

end