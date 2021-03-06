--Version:4.0.3
--Description: Adds RowVer column to all tables

DECLARE @dbname varchar(100)
set @dbname=quotename(db_name())
 
if not exists( SELECT * FROM sys.change_tracking_databases WHERE database_id=DB_ID(db_name()))
	exec('alter database '+@dbname+' SET CHANGE_TRACKING = ON  (CHANGE_RETENTION = 2 DAYS, AUTO_CLEANUP = ON) ');
	
IF NOT EXISTS (SELECT 1 FROM sys.change_tracking_tables WHERE object_id = OBJECT_ID('SupplementalExtractionResults')) 
	ALTER TABLE SupplementalExtractionResults							ENABLE CHANGE_TRACKING 
IF NOT EXISTS (SELECT 1 FROM sys.change_tracking_tables WHERE object_id = OBJECT_ID('ConfigurationProperties')) 
	ALTER TABLE ConfigurationProperties									ENABLE CHANGE_TRACKING 
IF NOT EXISTS (SELECT 1 FROM sys.change_tracking_tables WHERE object_id = OBJECT_ID('CumulativeExtractionResults')) 
	ALTER TABLE CumulativeExtractionResults								ENABLE CHANGE_TRACKING 
IF NOT EXISTS (SELECT 1 FROM sys.change_tracking_tables WHERE object_id = OBJECT_ID('DataUser')) 
	ALTER TABLE DataUser												ENABLE CHANGE_TRACKING 
IF NOT EXISTS (SELECT 1 FROM sys.change_tracking_tables WHERE object_id = OBJECT_ID('DeployedExtractionFilter')) 
	ALTER TABLE DeployedExtractionFilter								ENABLE CHANGE_TRACKING 
IF NOT EXISTS (SELECT 1 FROM sys.change_tracking_tables WHERE object_id = OBJECT_ID('DeployedExtractionFilterParameter')) 
	ALTER TABLE DeployedExtractionFilterParameter						ENABLE CHANGE_TRACKING 
IF NOT EXISTS (SELECT 1 FROM sys.change_tracking_tables WHERE object_id = OBJECT_ID('ExternalCohortTable')) 
	ALTER TABLE ExternalCohortTable										ENABLE CHANGE_TRACKING 
IF NOT EXISTS (SELECT 1 FROM sys.change_tracking_tables WHERE object_id = OBJECT_ID('ExtractableCohort')) 
	ALTER TABLE ExtractableCohort										ENABLE CHANGE_TRACKING 
IF NOT EXISTS (SELECT 1 FROM sys.change_tracking_tables WHERE object_id = OBJECT_ID('ExtractableColumn')) 
	ALTER TABLE ExtractableColumn										ENABLE CHANGE_TRACKING 
IF NOT EXISTS (SELECT 1 FROM sys.change_tracking_tables WHERE object_id = OBJECT_ID('ExtractableDataSet')) 
	ALTER TABLE ExtractableDataSet										ENABLE CHANGE_TRACKING 
IF NOT EXISTS (SELECT 1 FROM sys.change_tracking_tables WHERE object_id = OBJECT_ID('ExtractionConfiguration')) 
	ALTER TABLE ExtractionConfiguration									ENABLE CHANGE_TRACKING 
IF NOT EXISTS (SELECT 1 FROM sys.change_tracking_tables WHERE object_id = OBJECT_ID('FilterContainer')) 
	ALTER TABLE FilterContainer											ENABLE CHANGE_TRACKING 
IF NOT EXISTS (SELECT 1 FROM sys.change_tracking_tables WHERE object_id = OBJECT_ID('GlobalExtractionFilterParameter')) 
	ALTER TABLE GlobalExtractionFilterParameter							ENABLE CHANGE_TRACKING 
IF NOT EXISTS (SELECT 1 FROM sys.change_tracking_tables WHERE object_id = OBJECT_ID('Project')) 
	ALTER TABLE Project													ENABLE CHANGE_TRACKING 
IF NOT EXISTS (SELECT 1 FROM sys.change_tracking_tables WHERE object_id = OBJECT_ID('Project_DataUser')) 
	ALTER TABLE Project_DataUser										ENABLE CHANGE_TRACKING 
IF NOT EXISTS (SELECT 1 FROM sys.change_tracking_tables WHERE object_id = OBJECT_ID('ReleaseLog')) 
	ALTER TABLE ReleaseLog												ENABLE CHANGE_TRACKING 
IF NOT EXISTS (SELECT 1 FROM sys.change_tracking_tables WHERE object_id = OBJECT_ID('SelectedDataSets')) 
	ALTER TABLE SelectedDataSets										ENABLE CHANGE_TRACKING 
IF NOT EXISTS (SELECT 1 FROM sys.change_tracking_tables WHERE object_id = OBJECT_ID('ExtractableDataSetPackage')) 
	ALTER TABLE ExtractableDataSetPackage								ENABLE CHANGE_TRACKING 
IF NOT EXISTS (SELECT 1 FROM sys.change_tracking_tables WHERE object_id = OBJECT_ID('ExtractableDataSetPackage_ExtractableDataSet')) 
	ALTER TABLE ExtractableDataSetPackage_ExtractableDataSet			ENABLE CHANGE_TRACKING 
IF NOT EXISTS (SELECT 1 FROM sys.change_tracking_tables WHERE object_id = OBJECT_ID('ProjectCohortIdentificationConfigurationAssociation')) 
	ALTER TABLE ProjectCohortIdentificationConfigurationAssociation		ENABLE CHANGE_TRACKING 
IF NOT EXISTS (SELECT 1 FROM sys.change_tracking_tables WHERE object_id = OBJECT_ID('SelectedDataSetsForcedJoin')) 
	ALTER TABLE SelectedDataSetsForcedJoin								ENABLE CHANGE_TRACKING 

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='SupplementalExtractionResults' AND COLUMN_NAME='RowVer')
	ALTER TABLE SupplementalExtractionResults						ADD RowVer rowversion
	
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='ConfigurationProperties' AND COLUMN_NAME='RowVer')
	ALTER TABLE ConfigurationProperties								ADD RowVer rowversion
	
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='CumulativeExtractionResults' AND COLUMN_NAME='RowVer')
	ALTER TABLE CumulativeExtractionResults							ADD RowVer rowversion
	
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='DataUser' AND COLUMN_NAME='RowVer')
	ALTER TABLE DataUser											ADD RowVer rowversion

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='DeployedExtractionFilter' AND COLUMN_NAME='RowVer')
	ALTER TABLE DeployedExtractionFilter							ADD RowVer rowversion

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='DeployedExtractionFilterParameter' AND COLUMN_NAME='RowVer')
	ALTER TABLE DeployedExtractionFilterParameter					ADD RowVer rowversion

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='ExternalCohortTable' AND COLUMN_NAME='RowVer')
	ALTER TABLE ExternalCohortTable									ADD RowVer rowversion

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='ExtractableCohort' AND COLUMN_NAME='RowVer')
	ALTER TABLE ExtractableCohort									ADD RowVer rowversion

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='ExtractableColumn' AND COLUMN_NAME='RowVer')
	ALTER TABLE ExtractableColumn									ADD RowVer rowversion

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='ExtractableDataSet' AND COLUMN_NAME='RowVer')
	ALTER TABLE ExtractableDataSet									ADD RowVer rowversion

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='ExtractionConfiguration' AND COLUMN_NAME='RowVer')
	ALTER TABLE ExtractionConfiguration								ADD RowVer rowversion

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='FilterContainer' AND COLUMN_NAME='RowVer')
	ALTER TABLE FilterContainer										ADD RowVer rowversion

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='FilterContainerSubcontainers' AND COLUMN_NAME='RowVer')
	ALTER TABLE FilterContainerSubcontainers						ADD RowVer rowversion

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='GlobalExtractionFilterParameter' AND COLUMN_NAME='RowVer')
	ALTER TABLE GlobalExtractionFilterParameter						ADD RowVer rowversion

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Project' AND COLUMN_NAME='RowVer')
	ALTER TABLE Project												ADD RowVer rowversion

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Project_DataUser' AND COLUMN_NAME='RowVer')
	ALTER TABLE Project_DataUser									ADD RowVer rowversion

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='ReleaseLog' AND COLUMN_NAME='RowVer')
	ALTER TABLE ReleaseLog											ADD RowVer rowversion

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='SelectedDataSets' AND COLUMN_NAME='RowVer')
	ALTER TABLE SelectedDataSets									ADD RowVer rowversion

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='ExtractableDataSetPackage' AND COLUMN_NAME='RowVer')
	ALTER TABLE ExtractableDataSetPackage							ADD RowVer rowversion

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='ExtractableDataSetPackage_ExtractableDataSet' AND COLUMN_NAME='RowVer')
	ALTER TABLE ExtractableDataSetPackage_ExtractableDataSet		ADD RowVer rowversion

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='ProjectCohortIdentificationConfigurationAssociation' AND COLUMN_NAME='RowVer')
	ALTER TABLE ProjectCohortIdentificationConfigurationAssociation ADD RowVer rowversion

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='SelectedDataSetsForcedJoin' AND COLUMN_NAME='RowVer')
	ALTER TABLE SelectedDataSetsForcedJoin						    ADD RowVer rowversion
