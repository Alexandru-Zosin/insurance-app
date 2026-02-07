USE InsuranceDb;
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRAN;

-- Child tables first (FK order)
DELETE FROM core.RiskBuilding;
DELETE FROM core.Policy;
DELETE FROM core.Building;
DELETE FROM core.PremiumRules;

-- Parents
DELETE FROM core.RiskCategory;
DELETE FROM core.Currency;
DELETE FROM core.Broker;
DELETE FROM core.Client;
DELETE FROM core.City;
DELETE FROM core.County;
DELETE FROM core.Country;

COMMIT;
GO
