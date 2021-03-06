--Version:1.34.0.0
--Description: Gets rid of override functionality and ensures that if you have anonymisation configured then it happens! you cant just turn it off!
if exists (select 1 from sys.columns where name = 'EnablePrimaryKeyDuplicationResolution')
begin
	alter table LoadMetadata drop column RawDatabaseServer 
	alter table LoadMetadata drop column StagingDatabaseServer
	alter table LoadMetadata drop column LiveDatabaseServer

	alter table LoadMetadata drop constraint DF_LoadMetadata_EnableAnonymisation
	alter table LoadMetadata drop column EnableAnonymisation

	alter table LoadMetadata drop column OverrideLoggingServer 

	alter table LoadMetadata drop constraint DF_LoadMetadata_SkipLookups	
	alter table LoadMetadata drop column EnableLookupPopulation 

	alter table LoadMetadata drop constraint DF_LoadMetadata_EnablePrimaryKeyDuplicationResolution
	alter table LoadMetadata drop column EnablePrimaryKeyDuplicationResolution 

	alter table LoadMetadata drop constraint DF_LoadMetadata_CacheFilenameDateFormat
	alter table LoadMetadata drop column CacheFilenameDateFormat 
end

