--Version:2.0.0.1
--Description: Support for user favourite objects and naming CohortAggregateContainers and not having Count Sql on aggregates
 if not exists (select 1 from sys.tables where name = 'Favourite')
 begin

CREATE TABLE [dbo].[Favourite](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[TypeName] [varchar](500) NOT NULL,
	[ObjectID] [int] NOT NULL,
	[RepositoryTypeName] [varchar](500) NOT NULL,
	[Username] [varchar](500) NOT NULL,
	[FavouritedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_Favourite] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
))
 end


 if not exists (select 1 from sys.columns where name = 'Name' and  object_name(object_id) = 'CohortAggregateContainer')
begin
alter table CohortAggregateContainer add Name varchar(1000) null
end
GO

if exists (select 1 from sys.columns where name = 'Name' and  object_name(object_id) = 'CohortAggregateContainer' and is_nullable = 1)
begin
update CohortAggregateContainer set Name = Operation where Name is null
alter table CohortAggregateContainer alter column Name varchar(1000) not null
end

--null countSQL is now allowed
if exists (select 1 from sys.columns where name = 'CountSQL' and  object_name(object_id) = 'AggregateConfiguration' and is_nullable = 0)
begin
alter table AggregateConfiguration alter column CountSQL varchar(1000) null

update 
ac
set
ac.CountSQL = null
from  AggregateConfiguration ac
where exists (select 1 from JoinableCohortAggregateConfiguration where AggregateConfiguration_ID = ac.ID) --any previous patient index tables that had redundant unused count columns now don't

--all previous cohort aggregates also get to have their CountSQL cleared
update AggregateConfiguration set CountSQL = null where Name like 'cic_%'
end

if not exists (select 1 from sys.tables where name = 'AggregateTopX') 
begin
CREATE TABLE [AggregateTopX](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[AggregateConfiguration_ID] [int] NOT NULL,
	[TopX] [int] NOT NULL CONSTRAINT [DF_AggregateTopX_TopX]  DEFAULT ((1)),
	[OrderByDimensionIfAny_ID] [int] NULL,
	[OrderByDirection] [varchar](100) NOT NULL CONSTRAINT [DF_AggregateTopX_OrderByDirection]  DEFAULT ('Descending'),

	CONSTRAINT [FK_AggregateTopX_AggregateConfiguration] FOREIGN KEY([AggregateConfiguration_ID]) REFERENCES [AggregateConfiguration] ([ID]) ON DELETE CASCADE,
	CONSTRAINT [FK_AggregateTopX_AggregateDimension] FOREIGN KEY(OrderByDimensionIfAny_ID) REFERENCES [AggregateDimension] ([ID]),
	CONSTRAINT [ix_OneTopXPerAggregateConfiguration] UNIQUE ([AggregateConfiguration_ID]) ,
 CONSTRAINT [PK_AggregateTopX] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

end