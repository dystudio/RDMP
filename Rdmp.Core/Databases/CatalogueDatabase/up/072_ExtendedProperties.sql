--Version:4.2.0
--Description: Adds IgnoreTrigger to LoadMetadata, IgnoreInLoads to ColumnInfo and the ExtendedProperty class
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='ExtendedProperty')
	CREATE TABLE ExtendedProperty 
	(
		[ID] [int] IDENTITY(1,1) NOT NULL,
		
		--what it is set on
		ReferencedObjectType varchar(500),
		ReferencedObjectID int,
		ReferencedObjectRepositoryType varchar(500),

		--the value being stored
		[Name] [varchar](500) NOT NULL,
		[Value] [varchar](max) NULL,
		[Type] [varchar](500) NOT NULL,
		[Description] [varchar](1000) NULL,
 CONSTRAINT [PK_ExtendedProperty] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	)

	
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='LoadMetadata' AND COLUMN_NAME='IgnoreTrigger')
	ALTER TABLE LoadMetadata                                     ADD IgnoreTrigger bit

	
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='ColumnInfo' AND COLUMN_NAME='IgnoreInLoads')
	ALTER TABLE ColumnInfo                                     ADD IgnoreInLoads bit