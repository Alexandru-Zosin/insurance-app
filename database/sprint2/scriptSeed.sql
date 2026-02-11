USE InsuranceDb;
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRAN;

--------------------------------------------------------------------
-- 0) Clear data (keep schema)
--------------------------------------------------------------------
DELETE FROM core.RiskBuilding;
DELETE FROM core.Policy;
DELETE FROM core.Building;
DELETE FROM core.PremiumRules;
DELETE FROM core.RiskCategory;
DELETE FROM core.Currency;
DELETE FROM core.Broker;
DELETE FROM core.Client;
DELETE FROM core.City;
DELETE FROM core.County;
DELETE FROM core.Country;
DELETE FROM core.AuditLog;

--------------------------------------------------------------------
-- Helpers
--------------------------------------------------------------------
DECLARE @Today date = CAST(GETDATE() AS date);
DECLARE @SpanStart date = DATEADD(year, -2, @Today);  -- start dates from past 2 years
DECLARE @SpanEnd date   = DATEADD(year,  1, @Today);  -- through next 1 year (3-year span)

;WITH
N30 AS
(
    SELECT TOP (30) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS n
    FROM sys.all_objects a
),
N10 AS
(
    SELECT TOP (10) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS n
    FROM sys.all_objects a
),
N40 AS
(
    SELECT TOP (40) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS n
    FROM sys.all_objects a
),
N100 AS
(
    SELECT TOP (100) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS n
    FROM sys.all_objects a
)
SELECT 1 AS Dummy
INTO #Dummy;

DROP TABLE #Dummy;

--------------------------------------------------------------------
-- 1) Geography: 5 countries, 15 counties, 30 cities
--------------------------------------------------------------------
CREATE TABLE #Country
(
    CountryId int NOT NULL PRIMARY KEY,
    Name nvarchar(100) NOT NULL UNIQUE
);

INSERT INTO core.Country (Name)
OUTPUT inserted.CountryId, inserted.Name INTO #Country (CountryId, Name)
VALUES
(N'Romania'),
(N'Bulgaria'),
(N'Hungary'),
(N'Serbia'),
(N'Greece');

CREATE TABLE #County
(
    CountyId int NOT NULL PRIMARY KEY,
    CountryId int NOT NULL,
    Name nvarchar(100) NOT NULL
);

;WITH Countries AS
(
    SELECT CountryId, Name, ROW_NUMBER() OVER (ORDER BY CountryId) AS rn
    FROM #Country
),
GenCounties AS
(
    SELECT
        c.CountryId,
        CAST(CONCAT(c.Name, N' County ', v.k) AS nvarchar(100)) AS CountyName
    FROM Countries c
    CROSS JOIN (VALUES (1),(2),(3)) v(k)
)
INSERT INTO core.County (CountryId, Name)
OUTPUT inserted.CountyId, inserted.CountryId, inserted.Name
INTO #County (CountyId, CountryId, Name)
SELECT CountryId, CountyName
FROM GenCounties
ORDER BY CountryId, CountyName;

CREATE TABLE #City
(
    CityId int NOT NULL PRIMARY KEY,
    CountyId int NOT NULL,
    Name nvarchar(100) NOT NULL
);

;WITH Counties AS
(
    SELECT CountyId, CountryId, Name, ROW_NUMBER() OVER (ORDER BY CountyId) AS rn
    FROM #County
),
GenCities AS
(
    SELECT
        co.CountyId,
        CAST(CONCAT(REPLACE(co.Name, N' County ', N' '), N'City ', v.k) AS nvarchar(100)) AS CityName
    FROM Counties co
    CROSS JOIN (VALUES (1),(2)) v(k)
)
INSERT INTO core.City (CountyId, Name)
OUTPUT inserted.CityId, inserted.CountyId, inserted.Name
INTO #City (CityId, CountyId, Name)
SELECT CountyId, CityName
FROM GenCities
ORDER BY CountyId, CityName;

--------------------------------------------------------------------
-- 2) Currency (3)
--------------------------------------------------------------------
INSERT INTO core.Currency (Code, Name, ExchangeRateToBase, IsActive)
VALUES
(N'RON', N'Romanian Leu', 1.00000000, 1),
(N'EUR', N'Euro',         4.95000000, 1),
(N'USD', N'US Dollar',    4.60000000, 1);

--------------------------------------------------------------------
-- 3) RiskCategory (6)
--------------------------------------------------------------------
INSERT INTO core.RiskCategory (Code, Name, IsActive)
VALUES
('FloodZone',        N'Flood zone', 1),
('EarthquakeZone',   N'Earthquake zone', 1),
('FireRisk',         N'Fire risk', 1),
('TheftRisk',        N'Theft risk', 1),
('LandslideRisk',    N'Landslide risk', 1),
('StormRisk',        N'Storm risk', 1);

CREATE TABLE #RiskCategory
(
    RiskCategoryId int NOT NULL PRIMARY KEY,
    Code varchar(50) NOT NULL UNIQUE
);

INSERT INTO #RiskCategory (RiskCategoryId, Code)
SELECT RiskCategoryId, Code
FROM core.RiskCategory;

--------------------------------------------------------------------
-- 4) Clients (30)
--------------------------------------------------------------------
CREATE TABLE #Client
(
    ClientKey uniqueidentifier NOT NULL PRIMARY KEY,
    ClientType varchar(20) NOT NULL,
    IdentificationNumber nvarchar(50) NOT NULL UNIQUE
);

;WITH Gen AS
(
    SELECT n,
           CASE WHEN n % 5 = 0 THEN 'Company' ELSE 'Individual' END AS ClientType,
           CAST(CONCAT(N'Client ', RIGHT(CONCAT(N'000', n), 3)) AS nvarchar(100)) AS ClientName,
           CAST(CONCAT(N'ID-', RIGHT(CONCAT(N'00000', n), 5)) AS nvarchar(50)) AS IdNo,
           CAST(CONCAT(N'client', n, N'@example.test') AS nvarchar(100)) AS Email,
           CAST(CONCAT(N'+40-700-', RIGHT(CONCAT(N'000', n), 3), N'-', RIGHT(CONCAT(N'000', n*7), 3)) AS nvarchar(50)) AS Phone,
           CAST(CONCAT(N'Street ', ((n-1) % 20) + 1) AS nvarchar(100)) AS Street,
           CAST(((n - 1) % 200) + 1 AS nvarchar(20)) AS Number
    FROM (SELECT TOP (30) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS n FROM sys.all_objects) x
)
INSERT INTO core.Client (ClientType, Name, IdentificationNumber, Email, Phone, Street, Number)
OUTPUT inserted.ClientKey, inserted.ClientType, inserted.IdentificationNumber
INTO #Client (ClientKey, ClientType, IdentificationNumber)
SELECT ClientType, ClientName, IdNo, Email, Phone, Street, Number
FROM Gen
ORDER BY n;

--------------------------------------------------------------------
-- 5) Brokers (10)
--------------------------------------------------------------------
CREATE TABLE #Broker
(
    BrokerKey uniqueidentifier NOT NULL PRIMARY KEY,
    Code nvarchar(50) NOT NULL UNIQUE
);

;WITH Gen AS
(
    SELECT n,
           CAST(CONCAT(N'BRK-', RIGHT(CONCAT(N'000', n), 3)) AS nvarchar(50)) AS Code,
           CAST(CONCAT(N'Broker ', RIGHT(CONCAT(N'00', n), 2)) AS nvarchar(100)) AS BrokerName,
           CAST(CONCAT(N'broker', n, N'@example.test') AS nvarchar(100)) AS Email,
           CAST(CONCAT(N'+40-701-', RIGHT(CONCAT(N'000', n), 3), N'-', RIGHT(CONCAT(N'000', n*9), 3)) AS nvarchar(50)) AS Phone,
           CAST(CASE WHEN n % 10 = 0 THEN 0 ELSE 1 END AS bit) AS IsActive,
           CAST((0.02 + (n % 6) * 0.005) AS decimal(9,6)) AS CommissionPct
    FROM (SELECT TOP (10) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS n FROM sys.all_objects) x
)
INSERT INTO core.Broker (Code, Name, Email, Phone, IsActive, CommissionPercentage)
OUTPUT inserted.BrokerKey, inserted.Code
INTO #Broker (BrokerKey, Code)
SELECT Code, BrokerName, Email, Phone, IsActive, CommissionPct
FROM Gen
ORDER BY n;

--------------------------------------------------------------------
-- 6) Buildings (40)
--------------------------------------------------------------------
CREATE TABLE #Building
(
    BuildingKey uniqueidentifier NOT NULL PRIMARY KEY,
    OwnerClientId uniqueidentifier NOT NULL,
    CityId int NOT NULL,
    BuildingType varchar(20) NOT NULL,
    InsuredValueCurrencyCode nvarchar(10) NOT NULL
);

;WITH
Clients AS
(
    SELECT ClientKey, ROW_NUMBER() OVER (ORDER BY ClientKey) AS rn, COUNT(*) OVER () AS cnt
    FROM #Client
),
Cities AS
(
    SELECT CityId, ROW_NUMBER() OVER (ORDER BY CityId) AS rn, COUNT(*) OVER () AS cnt
    FROM #City
),
Gen AS
(
    SELECT
        n.n AS Seq,
        ABS(CHECKSUM(NEWID())) AS R1,
        ABS(CHECKSUM(NEWID())) AS R2,
        ABS(CHECKSUM(NEWID())) AS R3
    FROM (SELECT TOP (40) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS n FROM sys.all_objects) n
),
Pick AS
(
    SELECT
        g.Seq,
        c.ClientKey AS OwnerClientId,
        ci.CityId,
        CASE (g.R1 % 3)
            WHEN 0 THEN 'Residential'
            WHEN 1 THEN 'Office'
            ELSE 'Industrial'
        END AS BuildingType,
        CAST(CONCAT(N'Street ', (g.R2 % 50) + 1) AS nvarchar(100)) AS Street,
        CAST(((g.R3 % 250) + 1) AS nvarchar(20)) AS Number,
        (1950 + (g.R1 % 75)) AS ConstructionYear,
        (40 + (g.R2 % 1200)) AS SurfaceArea,
        CAST((50000.00 + (g.R3 % 1450000)) AS decimal(19,2)) AS InsuredValueAmount,
        CASE (g.R2 % 3)
            WHEN 0 THEN N'RON'
            WHEN 1 THEN N'EUR'
            ELSE N'USD'
        END AS InsuredValueCurrencyCode
    FROM Gen g
    CROSS APPLY
    (
        SELECT TOP (1) ClientKey
        FROM Clients
        WHERE rn = ((g.R1 % (SELECT MAX(cnt) FROM Clients)) + 1)
    ) c
    CROSS APPLY
    (
        SELECT TOP (1) CityId
        FROM Cities
        WHERE rn = ((g.R2 % (SELECT MAX(cnt) FROM Cities)) + 1)
    ) ci
)
INSERT INTO core.Building
(
    OwnerClientId, CityId, Street, Number,
    ConstructionYear, BuildingType, SurfaceArea,
    InsuredValueAmount, InsuredValueCurrencyCode
)
OUTPUT inserted.BuildingKey, inserted.OwnerClientId, inserted.CityId, inserted.BuildingType, inserted.InsuredValueCurrencyCode
INTO #Building (BuildingKey, OwnerClientId, CityId, BuildingType, InsuredValueCurrencyCode)
SELECT
    OwnerClientId, CityId, Street, Number,
    ConstructionYear, BuildingType, SurfaceArea,
    InsuredValueAmount, InsuredValueCurrencyCode
FROM Pick
ORDER BY Seq;

--------------------------------------------------------------------
-- 7) RiskBuilding mappings (1-3 categories per building)
--------------------------------------------------------------------
;WITH RB AS
(
    SELECT
        b.BuildingKey,
        rc.RiskCategoryId,
        ABS(CHECKSUM(NEWID())) % 100 AS R
    FROM #Building b
    CROSS JOIN #RiskCategory rc
)
INSERT INTO core.RiskBuilding (BuildingKey, RiskCategoryId)
SELECT BuildingKey, RiskCategoryId
FROM RB
WHERE
    (R < 18)      -- approx 1 category on average across all pairs
    OR (R BETWEEN 55 AND 60)  -- add some extra density
OPTION (RECOMPILE);

-- Ensure at least 1 category per building (fill gaps)
;WITH Missing AS
(
    SELECT b.BuildingKey
    FROM #Building b
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM core.RiskBuilding rb
        WHERE rb.BuildingKey = b.BuildingKey
    )
),
PickOne AS
(
    SELECT m.BuildingKey,
           (SELECT TOP (1) rc.RiskCategoryId FROM #RiskCategory rc ORDER BY NEWID()) AS RiskCategoryId
    FROM Missing m
)
INSERT INTO core.RiskBuilding (BuildingKey, RiskCategoryId)
SELECT BuildingKey, RiskCategoryId
FROM PickOne;

--------------------------------------------------------------------
-- 8) PremiumRules (a mix of fee + risk)
--------------------------------------------------------------------
DECLARE @FeeFrom date = DATEADD(day, -30, @Today);
DECLARE @FeeTo date   = DATEADD(day, 730, @Today);

-- Fee rules
INSERT INTO core.PremiumRules
(
    RuleKind, Name, Percentage, IsActive,
    FeeType, EffectiveFrom, EffectiveTo
)
VALUES
('Fee', N'Admin fee (standard)',         0.020000, 1, 'AdminFee',         @FeeFrom, @FeeTo),
('Fee', N'Broker commission (standard)', 0.030000, 1, 'BrokerCommission', @FeeFrom, @FeeTo),
('Fee', N'Admin fee (legacy)',           0.010000, 0, 'AdminFee',         DATEADD(day, -730, @FeeFrom), DATEADD(day, -31, @FeeFrom));

-- Risk rules (use a few real ids)
DECLARE @AnyRomaniaId int = (SELECT TOP (1) CountryId FROM #Country WHERE Name = N'Romania');
DECLARE @AnyBulgariaId int = (SELECT TOP (1) CountryId FROM #Country WHERE Name = N'Bulgaria');

DECLARE @AnyRomaniaCountyId int = (SELECT TOP (1) CountyId FROM #County WHERE CountryId = @AnyRomaniaId ORDER BY CountyId);
DECLARE @AnyBulgariaCountyId int = (SELECT TOP (1) CountyId FROM #County WHERE CountryId = @AnyBulgariaId ORDER BY CountyId);

DECLARE @AnyCityRo int = (SELECT TOP (1) CityId FROM #City WHERE CountyId = @AnyRomaniaCountyId ORDER BY CityId);
DECLARE @AnyCityBg int = (SELECT TOP (1) CityId FROM #City WHERE CountyId = @AnyBulgariaCountyId ORDER BY CityId);

INSERT INTO core.PremiumRules
(
    RuleKind, Name, Percentage, IsActive,
    CountryId, CountyId, CityId, BuildingType, ZoneRiskCategoryCode
)
VALUES
('RiskCountry',      N'Country risk Romania',       0.020000, 1, @AnyRomaniaId, NULL, NULL, NULL,     NULL),
('RiskCountry',      N'Country risk Bulgaria',      0.018000, 1, @AnyBulgariaId, NULL, NULL, NULL,    NULL),
('RiskCounty',       N'County risk (RO sample)',    0.012000, 1, NULL, @AnyRomaniaCountyId, NULL, NULL, NULL),
('RiskCity',         N'City risk (RO sample)',      0.010000, 1, NULL, NULL, @AnyCityRo, NULL, NULL),
('RiskBuildingType', N'Office building risk',       0.025000, 1, NULL, NULL, NULL, 'Office', NULL),
('RiskBuildingType', N'Industrial building risk',   0.030000, 1, NULL, NULL, NULL, 'Industrial', NULL),
('RiskZoneCategory', N'Flood zone risk',            0.030000, 1, NULL, NULL, NULL, NULL, 'FloodZone'),
('RiskZoneCategory', N'Earthquake zone risk',       0.040000, 1, NULL, NULL, NULL, NULL, 'EarthquakeZone'),
('RiskCity',         N'City risk (BG legacy)',      0.050000, 0, NULL, NULL, @AnyCityBg, NULL, NULL);

--------------------------------------------------------------------
--------------------------------------------------------------------
-- 9) Policies (100) spanning ~3 years, with status variety
--------------------------------------------------------------------
CREATE TABLE #ClientList
(
    rn int NOT NULL PRIMARY KEY,
    ClientKey uniqueidentifier NOT NULL
);

INSERT INTO #ClientList (rn, ClientKey)
SELECT ROW_NUMBER() OVER (ORDER BY ClientKey), ClientKey
FROM #Client;

CREATE TABLE #BrokerList
(
    rn int NOT NULL PRIMARY KEY,
    BrokerKey uniqueidentifier NOT NULL
);

INSERT INTO #BrokerList (rn, BrokerKey)
SELECT ROW_NUMBER() OVER (ORDER BY BrokerKey), BrokerKey
FROM #Broker;

CREATE TABLE #BuildingList
(
    rn int NOT NULL PRIMARY KEY,
    BuildingKey uniqueidentifier NOT NULL,
    CurrencyCode nvarchar(10) NOT NULL
);

INSERT INTO #BuildingList (rn, BuildingKey, CurrencyCode)
SELECT ROW_NUMBER() OVER (ORDER BY BuildingKey), BuildingKey, InsuredValueCurrencyCode
FROM #Building;

DECLARE @ClientCnt int = (SELECT COUNT(*) FROM #ClientList);
DECLARE @BrokerCnt int = (SELECT COUNT(*) FROM #BrokerList);
DECLARE @BuildingCnt int = (SELECT COUNT(*) FROM #BuildingList);

;WITH Gen AS
(
    SELECT
        n.n AS Seq,
        ABS(CHECKSUM(NEWID())) AS R1,
        ABS(CHECKSUM(NEWID())) AS R2,
        ABS(CHECKSUM(NEWID())) AS R3,
        ABS(CHECKSUM(NEWID())) AS R4
    FROM (SELECT TOP (100) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS n FROM sys.all_objects) n
),
Pick AS
(
    SELECT
        g.Seq,

        cl.ClientKey,
        bl.BuildingKey,
        br.BrokerKey,

        bl.CurrencyCode,

        -- carry RNG fields forward for later CTEs
        g.R1 AS R1,
        g.R2 AS R2,
        g.R4 AS R4,

        DATEADD(day, (g.R1 % (DATEDIFF(day, @SpanStart, @SpanEnd) + 1)), @SpanStart) AS StartDateRaw,

        CAST((200.00 + (g.R2 % 4801)) AS decimal(19,2)) AS BasePremiumAmount,

        CAST((5 + (g.R3 % 21)) AS int) AS UpliftPct,       -- 5..25

        CAST((g.R4 % 100) AS int) AS StatusRoll           -- 0..99
    FROM Gen g
    CROSS APPLY (SELECT ClientKey FROM #ClientList   WHERE rn = ((g.R1 % @ClientCnt) + 1)) cl
    CROSS APPLY (SELECT BrokerKey FROM #BrokerList   WHERE rn = ((g.R2 % @BrokerCnt) + 1)) br
    CROSS APPLY (SELECT BuildingKey, CurrencyCode FROM #BuildingList WHERE rn = ((g.R3 % @BuildingCnt) + 1)) bl
),
Dates AS
(
    SELECT
        p.*,

        -- EndDate = StartDate + ~1 year (335..395 days)
        DATEADD(day, (335 + (p.R2 % 61)), p.StartDateRaw) AS EndDateRaw
    FROM Pick p
),
Final AS
(
    SELECT
        Seq,
        ClientKey,
        BuildingKey,
        BrokerKey,
        CurrencyCode,

        StartDateRaw AS StartDate,
        EndDateRaw   AS EndDate,

        BasePremiumAmount,

        CAST(ROUND(BasePremiumAmount * (1.0 + (UpliftPct / 100.0)), 2) AS decimal(19,2)) AS FinalPremiumAmount,

        CASE
            WHEN StatusRoll < 12 THEN 'Cancelled'
            WHEN StartDateRaw > @Today THEN 'Draft'
            WHEN EndDateRaw < @Today THEN 'Expired'
            ELSE 'Active'
        END AS Status,

        -- Creation: for future-start drafts, anchor on today; otherwise anchor on start date
        CASE
            WHEN StartDateRaw > @Today THEN DATEADD(day, -(1 + (R1 % 30)), @Today)
            ELSE DATEADD(day, -(1 + (R1 % 60)), StartDateRaw)
        END AS CreationDate,

        CASE
            WHEN StatusRoll % 3 = 0 THEN @Today
            ELSE NULL
        END AS LastUpdateDate,

        -- Cancel fields (filled only when Cancelled)
        CASE
            WHEN StatusRoll < 12 THEN CAST(CONCAT(N'Cancelled reason ', Seq) AS nvarchar(200))
            ELSE NULL
        END AS CancellationReason,

        CASE
            WHEN StatusRoll < 12
                 AND DATEDIFF(day, StartDateRaw, EndDateRaw) > 10
            THEN DATEADD(day, (1 + (R4 % (DATEDIFF(day, StartDateRaw, EndDateRaw) - 1))), StartDateRaw)
            WHEN StatusRoll < 12
            THEN DATEADD(day, 1, StartDateRaw)
            ELSE NULL
        END AS CancellationEffectiveDate
    FROM Dates
)
INSERT INTO core.Policy
(
    ClientKey, BuildingKey, BrokerKey,
    Status,
    StartDate, EndDate,
    BasePremiumAmount, FinalPremiumAmount, CurrencyCode,
    CreationDate, LastUpdateDate,
    CancellationReason, CancellationEffectiveDate
)
SELECT
    ClientKey, BuildingKey, BrokerKey,
    Status,
    StartDate, EndDate,
    BasePremiumAmount, FinalPremiumAmount, CurrencyCode,
    CreationDate, LastUpdateDate,
    CancellationReason, CancellationEffectiveDate
FROM Final
ORDER BY Seq;


--------------------------------------------------------------------
-- 10) Optional: a few audit log rows (lightweight)
--------------------------------------------------------------------
;WITH Gen AS
(
    SELECT TOP (20)
        ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS n
    FROM sys.all_objects
),
AnyClient AS
(
    SELECT TOP (1) ClientKey AS EntityId
    FROM #Client
    ORDER BY NEWID()
)
INSERT INTO core.AuditLog
(
    Id, EntityType, EntityId, Action,
    OldValue, NewValue,
    PerformedBy, PerformedAtUtc
)
SELECT
    NEWID(),
    N'Client',
    (SELECT EntityId FROM AnyClient),
    N'SeedData',
    NULL,
    CAST(CONCAT(N'Row ', n) AS nvarchar(512)),
    NEWID(),
    DATEADD(minute, -n, SYSUTCDATETIME())
FROM Gen;

COMMIT;

--------------------------------------------------------------------
-- Sanity checks
--------------------------------------------------------------------
SELECT COUNT(*) AS Countries     FROM core.Country;
SELECT COUNT(*) AS Counties      FROM core.County;
SELECT COUNT(*) AS Cities        FROM core.City;
SELECT COUNT(*) AS Currencies    FROM core.Currency;
SELECT COUNT(*) AS RiskCategories FROM core.RiskCategory;
SELECT COUNT(*) AS Clients       FROM core.Client;
SELECT COUNT(*) AS Brokers       FROM core.Broker;
SELECT COUNT(*) AS Buildings     FROM core.Building;
SELECT COUNT(*) AS RiskBuilding  FROM core.RiskBuilding;
SELECT COUNT(*) AS PremiumRules  FROM core.PremiumRules;
SELECT COUNT(*) AS Policies      FROM core.Policy;
SELECT COUNT(*) AS AuditLogs     FROM core.AuditLog;

SELECT TOP (10)
    PolicyNumber, Status, StartDate, EndDate, BasePremiumAmount, FinalPremiumAmount, CurrencyCode,
    CancellationReason, CancellationEffectiveDate
FROM core.Policy
ORDER BY PolicyId DESC;
GO
