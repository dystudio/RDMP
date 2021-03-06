--Version:2.9.0.1
--Description: Removes support for automation
if exists (select 1 from sys.tables where name ='AutomationServiceSlot')
begin
    drop table AutomateablePipeline
	drop table AutomationLockedCatalogues
	drop table AutomationJob
    drop table AutomationServiceException
	drop table AutomationServiceSlot
	drop table LoadPeriodically
end
  GO


if exists (select 1 from sys.columns where name like '%Locked%')
begin
  alter table LoadProgress drop constraint DF_LoadSchedule_CachingInProgress
  alter table LoadProgress drop constraint DF_LoadProgress_AllowAutomation  
  alter table LoadProgress drop column LockedBecauseRunning
  alter table LoadProgress drop column LockHeldBy
  alter table LoadProgress drop column AllowAutomation
  alter table PermissionWindow drop constraint DF_PermissionWindow_IsLocked
  alter table PermissionWindow drop column LockedBecauseRunning
  alter table PermissionWindow drop column LockHeldBy
end

UPDATE ProcessTask set Path = 'LoadModules.Generic.Attachers.RemoteTableAttacher' WHERE Path = 'LoadModules.Generic.Attachers.RemoteSqlServerTableAttacher'
UPDATE ProcessTask set Path = 'LoadModules.Generic.Attachers.RemoteTableAttacher' WHERE Path = 'LoadModules.Generic.Attachers.RemoteMySqlTableAttacher'
UPDATE ProcessTask set Path = 'LoadModules.Generic.Attachers.RemoteTableAttacher' WHERE Path = 'LoadModules.Generic.Attachers.RemoteOracleTableAttacher'

--External Database Server now gets explicit DatabaseType
if not exists (select OBJECT_NAME(object_id),* from sys.columns where name ='DatabaseType' and  OBJECT_NAME(object_id) = 'ExternalDatabaseServer')
begin
	alter table ExternalDatabaseServer add DatabaseType varchar(100) null
end
GO

UPDATE ExternalDatabaseServer set DatabaseType = 'MicrosoftSQLServer' where DatabaseType is null
GO

if exists (select 1 from sys.columns where name = 'DatabaseType' and OBJECT_NAME(object_id) = 'ExternalDatabaseServer' and is_nullable = 1)
  begin
    alter table ExternalDatabaseServer alter column DatabaseType varchar(100) not null
end

-- ColumnInfo support for IsAutoIncrement
if not exists (select OBJECT_NAME(object_id),* from sys.columns where name ='IsAutoIncrement' and  OBJECT_NAME(object_id) = 'ColumnInfo')
begin
	alter table ColumnInfo add IsAutoIncrement bit NOT NULL CONSTRAINT DF_IsAutoIncrement Default 0 with values
end
GO


-- ColumnInfo support for Collation
if not exists (select OBJECT_NAME(object_id),* from sys.columns where name ='Collation' and  OBJECT_NAME(object_id) = 'ColumnInfo')
begin
	alter table ColumnInfo add Collation varchar(100) NULL
end
GO



if not exists (select 1 from sys.tables where name = 'ConnectionStringKeyword')
begin
CREATE TABLE [ConnectionStringKeyword](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DatabaseType] [varchar](50) NOT NULL,
	[Name] [varchar](500) NOT NULL,
	[Value] [varchar](1000) NULL,
 CONSTRAINT [PK_ConnectionStringKeyword] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
) 
) 
end
GO