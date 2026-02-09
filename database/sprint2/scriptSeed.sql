USE InsuranceDb;
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRAN;

-- Clear data (keep schema)
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

-- --------------------------------
-- Geography
-- --------------------------------
INSERT INTO core.Country (Name)
VALUES (N'Romania'), (N'Bulgaria');

DECLARE @RomaniaId int = (SELECT CountryId FROM core.Country WHERE Name = N'Romania');
DECLARE @BulgariaId int = (SELECT CountryId FROM core.Country WHERE Name = N'Bulgaria');

INSERT INTO core.County (CountryId, Name)
VALUES
(@RomaniaId, N'Bucuresti'),
(@RomaniaId, N'Cluj'),
(@BulgariaId, N'Sofia');

DECLARE @BucurestiCountyId int = (SELECT CountyId FROM core.County WHERE CountryId = @RomaniaId AND Name = N'Bucuresti');
DECLARE @ClujCountyId int = (SELECT CountyId FROM core.County WHERE CountryId = @RomaniaId AND Name = N'Cluj');
DECLARE @SofiaCountyId int = (SELECT CountyId FROM core.County WHERE CountryId = @BulgariaId AND Name = N'Sofia');

INSERT INTO core.City (CountyId, Name)
VALUES
(@BucurestiCountyId, N'Bucharest'),
(@ClujCountyId, N'Cluj-Napoca'),
(@SofiaCountyId, N'Sofia');

DECLARE @BucharestCityId int = (SELECT CityId FROM core.City WHERE CountyId = @BucurestiCountyId AND Name = N'Bucharest');
DECLARE @ClujNapocaCityId int = (SELECT CityId FROM core.City WHERE CountyId = @ClujCountyId AND Name = N'Cluj-Napoca');
DECLARE @SofiaCityId int = (SELECT CityId FROM core.City WHERE CountyId = @SofiaCountyId AND Name = N'Sofia');

-- --------------------------------
-- Currency
-- --------------------------------
INSERT INTO core.Currency (Code, Name, ExchangeRateToBase, IsActive)
VALUES
(N'RON', N'Romanian Leu', 1.00000000, 1),
(N'EUR', N'Euro',         4.95000000, 1),
(N'USD', N'US Dollar',    4.60000000, 1);

-- --------------------------------
-- RiskCategory (for ZoneRiskCategoryCode FK)
-- --------------------------------
INSERT INTO core.RiskCategory (Code, Name, IsActive)
VALUES
('FloodZone',      N'Flood zone', 1),
('EarthquakeZone', N'Earthquake zone', 1);

DECLARE @FloodId int = (SELECT RiskCategoryId FROM core.RiskCategory WHERE Code = 'FloodZone');
DECLARE @QuakeId int = (SELECT RiskCategoryId FROM core.RiskCategory WHERE Code = 'EarthquakeZone');

-- --------------------------------
-- Clients
-- --------------------------------
INSERT INTO core.Client (ClientType, Name, IdentificationNumber, Email, Phone, Street, Number)
VALUES
('Individual', N'Ion Popescu',    N'RO-IND-0001', N'ion.popescu@example.test',    N'+40-700-000-001', N'Splaiul Independentei', N'1'),
('Individual', N'Maria Ionescu',  N'RO-IND-0002', N'maria.ionescu@example.test',  N'+40-700-000-002', N'Calea Victoriei',       N'10'),
('Company',    N'ACME SRL',       N'RO-COMP-0001',N'office@acme.example.test',    N'+40-700-000-010', N'Bulevardul Unirii',     N'99');

DECLARE @ClientKey1 uniqueidentifier = (SELECT ClientKey FROM core.Client WHERE IdentificationNumber = N'RO-IND-0001');
DECLARE @ClientKey2 uniqueidentifier = (SELECT ClientKey FROM core.Client WHERE IdentificationNumber = N'RO-IND-0002');
DECLARE @ClientKey3 uniqueidentifier = (SELECT ClientKey FROM core.Client WHERE IdentificationNumber = N'RO-COMP-0001');

-- --------------------------------
-- Brokers
-- --------------------------------
INSERT INTO core.Broker (Code, Name, Email, Phone, IsActive, CommissionPercentage)
VALUES
(N'BRK-001', N'Broker One', N'broker.one@example.test', N'+40-700-100-001', 1, 0.050000),
(N'BRK-002', N'Broker Two', N'broker.two@example.test', N'+40-700-100-002', 1, 0.030000);

DECLARE @BrokerKey1 uniqueidentifier = (SELECT BrokerKey FROM core.Broker WHERE Code = N'BRK-001');
DECLARE @BrokerKey2 uniqueidentifier = (SELECT BrokerKey FROM core.Broker WHERE Code = N'BRK-002');

-- --------------------------------
-- Buildings
-- --------------------------------
INSERT INTO core.Building
(
    OwnerClientId, CityId, Street, Number,
    ConstructionYear, BuildingType, SurfaceArea,
    InsuredValueAmount, InsuredValueCurrencyCode
)
VALUES
(@ClientKey1, @BucharestCityId,  N'Splaiul Independentei', N'100', 1985, 'Residential',  85, 120000.00, N'RON'),
(@ClientKey1, @BucharestCityId,  N'Calea Victoriei',       N'200', 2008, 'Office',      220, 450000.00, N'EUR'),
(@ClientKey2, @ClujNapocaCityId, N'Strada Memorandumului', N'5',   1975, 'Residential',  60,  90000.00, N'RON'),
(@ClientKey3, @SofiaCityId,      N'Boulevard Vitosha',     N'15',  2012, 'Industrial',  900, 900000.00, N'USD');

DECLARE @BuildingKey1 uniqueidentifier =
(
    SELECT TOP (1) BuildingKey FROM core.Building
    WHERE OwnerClientId = @ClientKey1 AND Street = N'Splaiul Independentei' AND Number = N'100'
);

DECLARE @BuildingKey2 uniqueidentifier =
(
    SELECT TOP (1) BuildingKey FROM core.Building
    WHERE OwnerClientId = @ClientKey1 AND Street = N'Calea Victoriei' AND Number = N'200'
);

DECLARE @BuildingKey3 uniqueidentifier =
(
    SELECT TOP (1) BuildingKey FROM core.Building
    WHERE OwnerClientId = @ClientKey2 AND Street = N'Strada Memorandumului' AND Number = N'5'
);

DECLARE @BuildingKey4 uniqueidentifier =
(
    SELECT TOP (1) BuildingKey FROM core.Building
    WHERE OwnerClientId = @ClientKey3 AND Street = N'Boulevard Vitosha' AND Number = N'15'
);

-- --------------------------------
-- RiskBuilding (building-zone category mapping)
-- --------------------------------
INSERT INTO core.RiskBuilding (BuildingKey, RiskCategoryId)
VALUES
(@BuildingKey1, @FloodId),
(@BuildingKey1, @QuakeId),
(@BuildingKey2, @QuakeId),
(@BuildingKey3, @FloodId),
(@BuildingKey4, @QuakeId);

-- --------------------------------
-- PremiumRules: Fees
-- --------------------------------
DECLARE @Today date = CAST(GETUTCDATE() AS date);
DECLARE @From date = DATEADD(day, -30, @Today);
DECLARE @To date = DATEADD(day, 365, @Today);

INSERT INTO core.PremiumRules
(
    RuleKind, Name, Percentage, IsActive,
    FeeType, EffectiveFrom, EffectiveTo
)
VALUES
('Fee', N'Admin fee (active)',         0.020000, 1, 'AdminFee',         @From, @To),
('Fee', N'Broker commission (active)', 0.030000, 1, 'BrokerCommission', @From, @To),
('Fee', N'Old admin fee (inactive)',   0.010000, 0, 'AdminFee',         DATEADD(day, -365, @From), DATEADD(day, -31, @From));

-- --------------------------------
-- PremiumRules: Risks
-- --------------------------------
INSERT INTO core.PremiumRules
(
    RuleKind, Name, Percentage, IsActive,
    CountryId, CountyId, CityId, BuildingType, ZoneRiskCategoryCode
)
VALUES
('RiskCountry',      N'Country risk Romania',         0.020000, 1, @RomaniaId, NULL, NULL, NULL,     NULL),
('RiskCounty',       N'County risk Bucuresti',        0.015000, 1, NULL, @BucurestiCountyId, NULL, NULL, NULL),
('RiskCity',         N'City risk Bucharest',          0.010000, 1, NULL, NULL, @BucharestCityId, NULL, NULL),
('RiskBuildingType', N'Building type Office risk',    0.025000, 1, NULL, NULL, NULL, 'Office',      NULL),
('RiskZoneCategory', N'Flood zone risk',              0.030000, 1, NULL, NULL, NULL, NULL,         'FloodZone'),
('RiskZoneCategory', N'Earthquake zone risk',         0.040000, 1, NULL, NULL, NULL, NULL,         'EarthquakeZone'),
('RiskCity',         N'Old city risk Sofia inactive', 0.050000, 0, NULL, NULL, @SofiaCityId, NULL, NULL);

-- --------------------------------
-- Policies (couple)
-- --------------------------------
DECLARE @PolicyStart date = DATEFROMPARTS(YEAR(@Today), MONTH(@Today), 1);
DECLARE @PolicyEnd date = DATEADD(year, 1, @PolicyStart);

INSERT INTO core.Policy
(
    ClientKey, BuildingKey, BrokerKey,
    Status,
    StartDate, EndDate,
    BasePremiumAmount, FinalPremiumAmount, CurrencyCode,
    CreationDate, LastUpdateDate,
    CancellationReason, CancellationEffectiveDate
)
VALUES
(@ClientKey1, @BuildingKey1, @BrokerKey1,
 'Draft',
 @PolicyStart, @PolicyEnd,
 1000.00, 1100.00, N'RON',
 @Today, NULL,
 NULL, NULL),

(@ClientKey2, @BuildingKey3, @BrokerKey2,
 'Active',
 DATEADD(day, -10, @PolicyStart), DATEADD(year, 1, DATEADD(day, -10, @PolicyStart)),
 800.00, 900.00, N'RON',
 DATEADD(day, -10, @Today), @Today,
 NULL, NULL);

COMMIT;

-- Sanity checks
SELECT TOP (10) * FROM core.Country ORDER BY CountryId;
SELECT TOP (10) * FROM core.County ORDER BY CountyId;
SELECT TOP (10) * FROM core.City ORDER BY CityId;

SELECT TOP (10) Code, Name, IsActive FROM core.Currency ORDER BY Code;
SELECT TOP (10) Code, Name, IsActive FROM core.RiskCategory ORDER BY RiskCategoryId;

SELECT TOP (10) BuildingKey, OwnerClientId, CityId, BuildingType, InsuredValueAmount, InsuredValueCurrencyCode
FROM core.Building ORDER BY BuildingId;

SELECT TOP (20) RuleKind, Name, Percentage, IsActive, FeeType, CountryId, CountyId, CityId, BuildingType, ZoneRiskCategoryCode
FROM core.PremiumRules ORDER BY PremiumRuleId;

SELECT TOP (10) PolicyNumber, Status, ClientKey, BuildingKey, BrokerKey, BasePremiumAmount, FinalPremiumAmount, CurrencyCode
FROM core.Policy ORDER BY PolicyId;

GO
