--Version:1.14.0.0
--Description: Adds the Description column into the Pipeline table so that data analysts can document what the pipeline is supposed to do including the context it is to be used in (data load, extraction etc)

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Pipeline' AND COLUMN_NAME='Description')
BEGIN
	ALTER TABLE Pipeline ADD Description varchar(max) NULL
END
