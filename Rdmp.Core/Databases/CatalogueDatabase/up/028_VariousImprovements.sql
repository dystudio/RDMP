--Version:1.22.0.0
--Description: Fixes max length on filter descriptions, makes it possible to track which extractionfilter an aggregate filter was imported from and adds column order to SET operations in cohort generation
if (select max_length from sys.columns where name = 'Description' and OBJECT_NAME(object_id) = 'ExtractionFilter') <> -1
	alter table ExtractionFilter alter column Description varchar(max)

if (select max_length from sys.columns where name = 'Description' and OBJECT_NAME(object_id) = 'AggregateFilter') <> -1
	alter table AggregateFilter alter column Description varchar(max)

if not exists (select 1 from sys.columns where name = 'ClonedFromExtractionFilter_ID' and OBJECT_NAME(object_id) = 'AggregateFilter')
	alter table AggregateFilter add ClonedFromExtractionFilter_ID int null

--Add orderability to the set containers UNION EXCEPT etc (this is continuous order axis with the configurations in the same container i.e. you can have container,configuration,container, configuration or container,container,configuration,configuration
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE name = 'Order' and OBJECT_NAME(object_id) = 'CohortAggregateContainer')
BEGIN
alter table CohortAggregateContainer add [Order] int null
END
GO

IF (SELECT columnproperty(object_id('CohortAggregateContainer'), 'Order', 'AllowsNull')) = 1
BEGIN
	UPDATE CohortAggregateContainer set [Order] = 0
END
GO

IF (SELECT columnproperty(object_id('CohortAggregateContainer'), 'Order', 'AllowsNull')) = 1
BEGIN
	alter table CohortAggregateContainer alter column [Order] int not null
END
GO

