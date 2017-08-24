--Version:1.43.0.1
--Description: Adds cache load triggering overlap e.g. don't trigger new load for at least 1 day after your cache is up to date
if not exists (select * from sys.columns where name ='CacheLagPeriodLoadDelay')
	alter table [CacheProgress] add CacheLagPeriodLoadDelay varchar(10) null