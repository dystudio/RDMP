--Version:2.7.0.1
--Description: Adds the MappedDataPath (nullable) property to the External Database Server; needed for Release of MDF extractions
if not exists (select 1 from sys.columns where name = 'MappedDataPath' and OBJECT_NAME(object_id) = 'ExternalDatabaseServer')
  begin
	alter table ExternalDatabaseServer add MappedDataPath varchar(1000) null
  end
  GO
