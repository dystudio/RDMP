--Version:2.11.0.1
--Description: Changes to support FAnsi Sql library changes

UPDATE ExternalDatabaseServer set DatabaseType = 'MySql' where DatabaseType = 'MYSQLServer'
UPDATE TableInfo set DatabaseType = 'MySql'  where DatabaseType = 'MYSQLServer'
UPDATE ConnectionStringKeyword set DatabaseType = 'MySql' where DatabaseType = 'MYSQLServer'