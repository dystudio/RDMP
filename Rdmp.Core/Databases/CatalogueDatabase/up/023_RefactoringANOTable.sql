--Version:1.17.0.0
--Description: Removes Configuration from the ANO database and instead stores this information in the Catalogue
exec sp_rename 'ANOTable.Name','TableName','COLUMN'

alter table ANOTable add NumberOfIntegersToUseInAnonymousRepresentation int not null    CONSTRAINT DF_NumberOfIntegersToUseInAnonymousRepresentation DEFAULT 1
alter table ANOTable add NumberOfCharactersToUseInAnonymousRepresentation int not null  CONSTRAINT DF_NumberOfCharactersToUseInAnonymousRepresentation DEFAULT 1
alter table ANOTable add Suffix varchar(10) null

CREATE UNIQUE NONCLUSTERED INDEX [idx_ANOTableNamesMustBeUnique] ON [dbo].[ANOTable]
(
	[TableName] ASC
)

