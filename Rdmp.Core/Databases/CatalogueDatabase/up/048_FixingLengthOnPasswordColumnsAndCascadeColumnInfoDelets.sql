--Version:1.42.0.1
--Description: Fixes the length on Data Access Credentials
alter table DataAccessCredentials alter column Password varchar(max) null

if exists (select 1 from sys.foreign_keys where name = 'FK_ColumnInfo_CatalogueItem_ColumnInfo' and delete_referential_action_desc = 'SET_NULL')
begin
ALTER TABLE [dbo].[ColumnInfo_CatalogueItem]  drop constraint [FK_ColumnInfo_CatalogueItem_ColumnInfo]

ALTER TABLE [dbo].[ColumnInfo_CatalogueItem]  ADD CONSTRAINT [FK_ColumnInfo_CatalogueItem_ColumnInfo] FOREIGN KEY([ColumnInfo_ID])
REFERENCES [dbo].[ColumnInfo] ([ID])
ON DELETE CASCADE
end
