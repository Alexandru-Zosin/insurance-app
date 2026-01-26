USE InsuranceDb;
GO

/* =========================
   core.Client
   ========================= */

/* Add Street if missing */
IF COL_LENGTH('core.Client', 'Street') IS NULL
BEGIN
    ALTER TABLE core.Client
    ADD Street NVARCHAR(100) NOT NULL CONSTRAINT df_client_street DEFAULT '';
END
GO

/* Add StreetNumber if missing */
IF COL_LENGTH('core.Client', 'StreetNumber') IS NULL
BEGIN
    ALTER TABLE core.Client
    ADD StreetNumber NVARCHAR(20) NOT NULL CONSTRAINT df_client_street_number DEFAULT '';
END
GO

/* Drop Address if it exists */
IF COL_LENGTH('core.Client', 'Address') IS NOT NULL
BEGIN
    ALTER TABLE core.Client
    DROP COLUMN Address;
END
GO

/* Drop defaults if they exist */
IF EXISTS (
    SELECT 1 FROM sys.default_constraints
    WHERE name = 'df_client_street'
)
BEGIN
    ALTER TABLE core.Client DROP CONSTRAINT df_client_street;
END

IF EXISTS (
    SELECT 1 FROM sys.default_constraints
    WHERE name = 'df_client_street_number'
)
BEGIN
    ALTER TABLE core.Client DROP CONSTRAINT df_client_street_number;
END
GO

/* =========================
   core.Building
   ========================= */

/* Drop Address CHECK constraint if it exists */
IF EXISTS (
    SELECT 1 FROM sys.check_constraints
    WHERE name = 'ck_building_address'
)
BEGIN
    ALTER TABLE core.Building DROP CONSTRAINT ck_building_address;
END
GO

/* Add Street if missing */
IF COL_LENGTH('core.Building', 'Street') IS NULL
BEGIN
    ALTER TABLE core.Building
    ADD Street NVARCHAR(100) NOT NULL CONSTRAINT df_building_street DEFAULT '';
END
GO

/* Add StreetNumber if missing */
IF COL_LENGTH('core.Building', 'StreetNumber') IS NULL
BEGIN
    ALTER TABLE core.Building
    ADD StreetNumber NVARCHAR(20) NOT NULL CONSTRAINT df_building_street_number DEFAULT '';
END
GO

/* Drop Address if it exists */
IF COL_LENGTH('core.Building', 'Address') IS NOT NULL
BEGIN
    ALTER TABLE core.Building
    DROP COLUMN Address;
END
GO

/* Drop defaults if they exist */
IF EXISTS (
    SELECT 1 FROM sys.default_constraints
    WHERE name = 'df_building_street'
)
BEGIN
    ALTER TABLE core.Building DROP CONSTRAINT df_building_street;
END

IF EXISTS (
    SELECT 1 FROM sys.default_constraints
    WHERE name = 'df_building_street_number'
)
BEGIN
    ALTER TABLE core.Building DROP CONSTRAINT df_building_street_number;
END
GO

USE InsuranceDb;
GO

/* Rename core.Client.StreetNumber -> core.Client.Number (idempotent) */
IF COL_LENGTH('core.Client', 'StreetNumber') IS NOT NULL
   AND COL_LENGTH('core.Client', 'Number') IS NULL
BEGIN
    EXEC sp_rename 'core.Client.StreetNumber', 'Number', 'COLUMN';
END
GO

/* Rename core.Building.StreetNumber -> core.Building.Number (idempotent) */
IF COL_LENGTH('core.Building', 'StreetNumber') IS NOT NULL
   AND COL_LENGTH('core.Building', 'Number') IS NULL
BEGIN
    EXEC sp_rename 'core.Building.StreetNumber', 'Number', 'COLUMN';
END
GO
