--Version:1.41.0.1
--Description: Adds support for global timeouts in Automation, Gets rid of redundant fields in LoadMetadata

  if not exists (select * from sys.columns where name ='GlobalTimeoutPeriod'AND object_id = OBJECT_ID('AutomationServiceSlot'))
begin
	alter table AutomationServiceSlot add GlobalTimeoutPeriod int null
end

  if exists (select * from sys.columns where name ='IncludeDataset' AND object_id = OBJECT_ID('LoadMetadata'))
  begin
     alter table LoadMetadata drop constraint DF_LoadMetadata_IncludeDataset
     alter table LoadMetadata drop column IncludeDataset
	 
  end

  if exists (select * from sys.columns where name ='UsesStandardisedLoadProcess'AND object_id = OBJECT_ID('LoadMetadata'))
  begin
  alter table LoadMetadata drop constraint DF_LoadMetadata_UsesStandardisedLoadProcess
     alter table LoadMetadata drop column UsesStandardisedLoadProcess
  end

    if exists (select * from sys.columns where name ='ScheduleStartDate'AND object_id = OBJECT_ID('LoadMetadata'))
  begin
     alter table LoadMetadata drop column ScheduleStartDate
  end

      if exists (select * from sys.columns where name ='SchedulePeriod'AND object_id = OBJECT_ID('LoadMetadata'))
  begin
     alter table LoadMetadata drop column SchedulePeriod
  end
