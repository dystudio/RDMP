--Version:2.11.0.1
--Description: Added support for a disabled state flag on cohort identification objects
  
 if not exists (select 1 from sys.columns where name = 'IsDisabled' and OBJECT_NAME(object_id) = 'CohortAggregateContainer')
	alter table CohortAggregateContainer add IsDisabled bit not null default(0) with values

if not exists (select 1 from sys.columns where name = 'IsDisabled' and OBJECT_NAME(object_id) = 'AggregateConfiguration')
	alter table AggregateConfiguration add IsDisabled bit not null default(0) with values

if not exists (select 1 from sys.columns where name = 'IsDisabled' and OBJECT_NAME(object_id) = 'AggregateFilterContainer')
	alter table AggregateFilterContainer add IsDisabled bit not null default(0) with values

if not exists (select 1 from sys.columns where name = 'IsDisabled' and OBJECT_NAME(object_id) = 'AggregateFilter')
	alter table AggregateFilter add IsDisabled bit not null default(0) with values
	