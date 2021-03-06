--Version:2.11.0.1
--Description: Changes unique constraint to include Plugin.Name AND Plugin.Version

if exists( select 1 from sys.indexes where name = 'ix_PluginNamesMustBeUnique' and object_id = object_id('Plugin'))
begin
	DROP INDEX [ix_PluginNamesMustBeUnique] ON [dbo].[Plugin]
end
if exists( select 1 from sys.indexes where name = 'ix_PluginNameAndVersionMustBeUnique' and object_id = object_id('Plugin'))
begin
	DROP INDEX [ix_PluginNameAndVersionMustBeUnique] ON [dbo].[Plugin]
end

UPDATE [dbo].[Plugin] 
	SET PluginVersion = '1.0.0.0' 
	WHERE PluginVersion IS NULL

ALTER TABLE [dbo].[Plugin]
	ALTER COLUMN PluginVersion VARCHAR(50) NOT NULL
GO

CREATE UNIQUE NONCLUSTERED INDEX [ix_PluginNameAndVersionMustBeUnique] ON [dbo].[Plugin]
(
	[Name] ASC,
	[PluginVersion] ASC
)
GO
