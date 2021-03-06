--Version:2.10.0.1
--Description: Fixes naming for all objects that reference another object in Catalogue database

--Favourite
if(not exists (select 1 from sys.columns where OBJECT_NAME(object_id) = 'Favourite' and name ='ReferencedObjectID'))
	EXEC sp_rename 'Favourite.ObjectID', 'ReferencedObjectID', 'COLUMN'; 

if(not exists (select 1 from sys.columns where OBJECT_NAME(object_id) = 'Favourite' and name ='ReferencedObjectRepositoryType'))
	EXEC sp_rename 'Favourite.RepositoryTypeName', 'ReferencedObjectRepositoryType', 'COLUMN'; 

if(not exists (select 1 from sys.columns where OBJECT_NAME(object_id) = 'Favourite' and name ='ReferencedObjectType'))
	EXEC sp_rename 'Favourite.TypeName', 'ReferencedObjectType', 'COLUMN'; 



--AnyTableSqlParameter
if(not exists (select 1 from sys.columns where OBJECT_NAME(object_id) = 'AnyTableSqlParameter' and name ='ReferencedObjectID'))
	EXEC sp_rename 'AnyTableSqlParameter.Parent_ID', 'ReferencedObjectID', 'COLUMN'; 

/*Col does not exist yet so create it and update it to CatalogueRepository then make it not nullable*/
if(not exists (select 1 from sys.columns where OBJECT_NAME(object_id) = 'AnyTableSqlParameter' and name ='ReferencedObjectRepositoryType'))
	alter table AnyTableSqlParameter add ReferencedObjectRepositoryType varchar(500) null

GO

update AnyTableSqlParameter set ReferencedObjectRepositoryType = 'CatalogueRepository' where ReferencedObjectRepositoryType is null

if(exists (select 1 from sys.columns where OBJECT_NAME(object_id) = 'AnyTableSqlParameter' and name ='ReferencedObjectRepositoryType' and is_nullable=1))
	alter table AnyTableSqlParameter alter column ReferencedObjectRepositoryType varchar(500) not null

if(not exists (select 1 from sys.columns where OBJECT_NAME(object_id) = 'AnyTableSqlParameter' and name ='ReferencedObjectType'))
	EXEC sp_rename 'AnyTableSqlParameter.ParentTable', 'ReferencedObjectType', 'COLUMN'; 

--ObjectExport
if(not exists (select 1 from sys.columns where OBJECT_NAME(object_id) = 'ObjectExport' and name ='ReferencedObjectID'))
	EXEC sp_rename 'ObjectExport.ObjectID', 'ReferencedObjectID', 'COLUMN'; 
if(not exists (select 1 from sys.columns where OBJECT_NAME(object_id) = 'ObjectExport' and name ='ReferencedObjectRepositoryType'))
	EXEC sp_rename 'ObjectExport.RepositoryTypeName', 'ReferencedObjectRepositoryType', 'COLUMN'; 
if(not exists (select 1 from sys.columns where OBJECT_NAME(object_id) = 'ObjectExport' and name ='ReferencedObjectType'))
	EXEC sp_rename 'ObjectExport.ObjectTypeName', 'ReferencedObjectType', 'COLUMN'; 
	
--ObjectImport
if(not exists (select 1 from sys.columns where OBJECT_NAME(object_id) = 'ObjectImport' and name ='ReferencedObjectID'))
	EXEC sp_rename 'ObjectImport.LocalObjectID', 'ReferencedObjectID', 'COLUMN'; 
if(not exists (select 1 from sys.columns where OBJECT_NAME(object_id) = 'ObjectImport' and name ='ReferencedObjectRepositoryType'))
	EXEC sp_rename 'ObjectImport.RepositoryTypeName', 'ReferencedObjectRepositoryType', 'COLUMN'; 
if(not exists (select 1 from sys.columns where OBJECT_NAME(object_id) = 'ObjectImport' and name ='ReferencedObjectType'))
	EXEC sp_rename 'ObjectImport.LocalTypeName', 'ReferencedObjectType', 'COLUMN'; 

--DashboardObjectUse
if(not exists (select 1 from sys.columns where OBJECT_NAME(object_id) = 'DashboardObjectUse' and name ='ReferencedObjectID'))
	EXEC sp_rename 'DashboardObjectUse.ObjectID', 'ReferencedObjectID', 'COLUMN'; 
if(not exists (select 1 from sys.columns where OBJECT_NAME(object_id) = 'DashboardObjectUse' and name ='ReferencedObjectRepositoryType'))
	EXEC sp_rename 'DashboardObjectUse.RepositoryTypeName', 'ReferencedObjectRepositoryType', 'COLUMN'; 
if(not exists (select 1 from sys.columns where OBJECT_NAME(object_id) = 'DashboardObjectUse' and name ='ReferencedObjectType'))
	EXEC sp_rename 'DashboardObjectUse.TypeName', 'ReferencedObjectType', 'COLUMN'; 

		
if not exists (select 1 from sys.tables where name = 'WindowLayout')
BEGIN

CREATE TABLE [dbo].[WindowLayout](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](1000) NOT NULL,
	[LayoutData] [varchar](max) NOT NULL,
 CONSTRAINT [PK_Layout] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)
)

END

if not exists (select 1 from sys.columns where OBJECT_NAME(object_id) = 'CohortIdentificationConfiguration' and name ='ClonedFrom_ID')
	alter table CohortIdentificationConfiguration add ClonedFrom_ID int null
	
if not exists (select 1 from sys.columns where OBJECT_NAME(object_id) = 'LoadMetadata' and name ='OverrideRAWServer_ID')
	alter table LoadMetadata add OverrideRAWServer_ID int null

if not exists (select 1 from sys.foreign_keys where name ='FK_OverrideRAWServer_ID')
	ALTER TABLE LoadMetadata ADD  CONSTRAINT [FK_OverrideRAWServer_ID] FOREIGN KEY([OverrideRAWServer_ID]) REFERENCES [ExternalDatabaseServer] ([ID])

if not exists (select 1 from sys.columns where OBJECT_NAME(object_id) = 'TableInfo' and name ='Schema')
	alter table TableInfo add [Schema] varchar(500) null