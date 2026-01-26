USE [InsuranceDb];
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRAN;

    -- Disable all foreign keys
    EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL';

    -- Delete all data
    EXEC sp_MSforeachtable 'DELETE FROM ?';

    -- Reseed all identity columns
    DECLARE @sql NVARCHAR(MAX) = N'';

    SELECT @sql +=
        'DBCC CHECKIDENT (''' +
        QUOTENAME(SCHEMA_NAME(t.schema_id)) + '.' + QUOTENAME(t.name) +
        ''', RESEED, 0);'
    FROM sys.tables t
    JOIN sys.identity_columns ic ON t.object_id = ic.object_id;

    EXEC sp_executesql @sql;

    -- Re-enable foreign keys
    EXEC sp_MSforeachtable 'ALTER TABLE ? CHECK CONSTRAINT ALL';

    COMMIT TRAN;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRAN;
    THROW;
END CATCH;
GO
