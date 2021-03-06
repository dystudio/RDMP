--Version:1.47.0.1
--Description: removes dynamic contexts from Caching, now you just derive from ICacheChunk.  Also adds option to enable/disable Automation of LoadProgresses
if exists (select 1 from sys.columns where name = 'PipelineContextField')
begin
 alter table CacheProgress drop column PipelineContextField

  end

 if not exists (select 1 from sys.columns where name = 'AllowAutomation')
begin
 alter table LoadProgress add AllowAutomation bit null
 end
 GO

 if exists (select 1 from sys.columns where name = 'AllowAutomation' and is_nullable = 1)
 begin

 update LoadProgress set AllowAutomation = 0

 alter table LoadProgress alter column AllowAutomation bit not null
 
ALTER TABLE [dbo].[LoadProgress] ADD  CONSTRAINT [DF_LoadProgress_AllowAutomation]  DEFAULT ((0)) FOR AllowAutomation

 end