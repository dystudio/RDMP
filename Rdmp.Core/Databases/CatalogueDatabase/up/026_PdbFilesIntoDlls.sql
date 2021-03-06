--Version:1.20.0.0
--Description: Adds Pdb field to LoadModuleAssembly to allow for better error reporting in plugins, also adds UploadDate and DllFileVersion

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE name = 'Pdb')
BEGIN
ALTER TABLE LoadModuleAssembly add [Pdb] [varbinary](max)
ALTER TABLE LoadModuleAssembly add UploadDate datetime CONSTRAINT DF_LoadModuleAssembly_UploadDate DEFAULT GetDate()
ALTER TABLE LoadModuleAssembly add DllFileVersion varchar(50)

END
GO

IF (SELECT columnproperty(object_id('LoadModuleAssembly'), 'UploadDate', 'AllowsNull')) = 1
BEGIN
	UPDATE LoadModuleAssembly set UploadDate = GETDATE()
END
GO

IF (SELECT columnproperty(object_id('LoadModuleAssembly'), 'UploadDate', 'AllowsNull')) = 1
BEGIN
	alter table LoadModuleAssembly alter column UploadDate datetime not null
END
GO
