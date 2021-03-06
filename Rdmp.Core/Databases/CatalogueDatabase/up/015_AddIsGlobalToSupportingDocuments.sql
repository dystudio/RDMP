--Version:1.12.0.0
--Description:Adds extra field IsGlobal to SupportingDocument which is not nullable bit of which all existing data becomes 0 (not global by default)
if not exists (select 1 from sys.columns where name = 'IsGlobal' and object_id = (select object_id from sys.tables where name = 'SupportingDocument'))
begin
alter table SupportingDocument add IsGlobal bit null
end


IF 'YES' = (SELECT IS_NULLABLE FROM INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'SupportingDocument' and COLUMN_NAME = 'IsGlobal')
BEGIN
EXEC('update SupportingDocument set IsGlobal = 0 where IsGlobal is null')
ALTER TABLE SupportingDocument alter column IsGlobal bit not null 
alter table SupportingDocument add constraint df_SupportingDocumentIsGlobal default 0 for IsGlobal

END