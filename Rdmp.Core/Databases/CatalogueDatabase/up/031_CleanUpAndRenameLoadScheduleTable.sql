--Version:1.25.0.0
--Description: Removes obsolete fields from the LoadSchedule table and renames to LoadProgress

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='LoadSchedule' AND COLUMN_NAME='LastSuccesfulDataLoadRunID')
BEGIN
  ALTER TABLE LoadSchedule DROP COLUMN LastSuccesfulDataLoadRunID
END

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='LoadSchedule' AND COLUMN_NAME='LastSuccesfulDataLoadRunIDServer')
BEGIN
  ALTER TABLE LoadSchedule DROP COLUMN LastSuccesfulDataLoadRunIDServer
END

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='LoadSchedule' AND COLUMN_NAME='Healthboard')
BEGIN
  ALTER TABLE LoadSchedule DROP COLUMN Healthboard
END

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='LoadSchedule')
BEGIN
  EXEC sp_rename 'LoadSchedule', 'LoadProgress'
END

