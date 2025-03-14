USE master;
GO
EXEC sp_configure 'show advanced options', 1;
RECONFIGURE;
GO
EXEC sp_configure 'force encryption', 0;
RECONFIGURE;
GO

CREATE DATABASE PizzaCatalog;
GO