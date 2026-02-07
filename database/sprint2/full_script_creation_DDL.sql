USE InsuranceDb;
GO

IF SCHEMA_ID('core') IS NULL
    EXEC('CREATE SCHEMA core');
GO

/* Drop in dependency order (idempotent reruns) */
IF OBJECT_ID('core.RiskBuilding', 'U') IS NOT NULL DROP TABLE core.RiskBuilding;
IF OBJECT_ID('core.Policy', 'U') IS NOT NULL DROP TABLE core.Policy;
IF OBJECT_ID('core.Building', 'U') IS NOT NULL DROP TABLE core.Building;
IF OBJECT_ID('core.PremiumRules', 'U') IS NOT NULL DROP TABLE core.PremiumRules;
IF OBJECT_ID('core.RiskCategory', 'U') IS NOT NULL DROP TABLE core.RiskCategory;
IF OBJECT_ID('core.Currency', 'U') IS NOT NULL DROP TABLE core.Currency;
IF OBJECT_ID('core.Broker', 'U') IS NOT NULL DROP TABLE core.Broker;
IF OBJECT_ID('core.Client', 'U') IS NOT NULL DROP TABLE core.Client;
IF OBJECT_ID('core.City', 'U') IS NOT NULL DROP TABLE core.City;
IF OBJECT_ID('core.County', 'U') IS NOT NULL DROP TABLE core.County;
IF OBJECT_ID('core.Country', 'U') IS NOT NULL DROP TABLE core.Country;
GO

/* Geography */
CREATE TABLE core.Country
(
    CountryId int IDENTITY(1,1) NOT NULL,
    Name nvarchar(100) NOT NULL,

    CONSTRAINT PK_Country PRIMARY KEY CLUSTERED (CountryId),
    CONSTRAINT UQ_Country_Name UNIQUE (Name),
    CONSTRAINT CK_Country_Name_NotBlank CHECK (LEN(LTRIM(RTRIM(Name))) > 0)
);
GO

CREATE TABLE core.County
(
    CountyId int IDENTITY(1,1) NOT NULL,
    CountryId int NOT NULL,
    Name nvarchar(100) NOT NULL,

    CONSTRAINT PK_County PRIMARY KEY CLUSTERED (CountyId),
    CONSTRAINT UQ_County UNIQUE (CountryId, Name),
    CONSTRAINT CK_County_Name_NotBlank CHECK (LEN(LTRIM(RTRIM(Name))) > 0),
    CONSTRAINT FK_County_Country FOREIGN KEY (CountryId) REFERENCES core.Country(CountryId)
);
GO

CREATE TABLE core.City
(
    CityId int IDENTITY(1,1) NOT NULL,
    CountyId int NOT NULL,
    Name nvarchar(100) NOT NULL,

    CONSTRAINT PK_City PRIMARY KEY CLUSTERED (CityId),
    CONSTRAINT UQ_City UNIQUE (CountyId, Name),
    CONSTRAINT CK_City_Name_NotBlank CHECK (LEN(LTRIM(RTRIM(Name))) > 0),
    CONSTRAINT FK_City_County FOREIGN KEY (CountyId) REFERENCES core.County(CountyId)
);
GO

/* Broker
- Internal surrogate key: BrokerId int IDENTITY clustered PK
- External/app key: BrokerKey uniqueidentifier UNIQUE (fast lookup + stable external id)
- Business key: Code UNIQUE
*/
CREATE TABLE core.Broker
(
    BrokerId int IDENTITY(1,1) NOT NULL,
    BrokerKey uniqueidentifier NOT NULL CONSTRAINT DF_Broker_BrokerKey DEFAULT (NEWSEQUENTIALID()),
    Code nvarchar(50) NOT NULL,
    Name nvarchar(100) NOT NULL,
    Email nvarchar(100) NOT NULL,
    Phone nvarchar(50) NOT NULL,
    IsActive bit NOT NULL,
    CommissionPercentage decimal(9,6) NULL, /* 0..1 */

    CONSTRAINT PK_Broker PRIMARY KEY CLUSTERED (BrokerId),
    CONSTRAINT UQ_Broker_BrokerKey UNIQUE (BrokerKey),
    CONSTRAINT UQ_Broker_Code UNIQUE (Code),
    CONSTRAINT CK_Broker_Code_NotBlank CHECK (LEN(LTRIM(RTRIM(Code))) > 0),
    CONSTRAINT CK_Broker_Name_NotBlank CHECK (LEN(LTRIM(RTRIM(Name))) > 0),
    CONSTRAINT CK_Broker_Email_NotBlank CHECK (LEN(LTRIM(RTRIM(Email))) > 0),
    CONSTRAINT CK_Broker_Phone_NotBlank CHECK (LEN(LTRIM(RTRIM(Phone))) > 0),
    CONSTRAINT CK_Broker_CommissionPct_0_1 CHECK (CommissionPercentage IS NULL OR (CommissionPercentage >= 0.0 AND CommissionPercentage <= 1.0))
);
GO

/* Client
- Internal surrogate key: ClientId int IDENTITY clustered PK
- External/app key: ClientKey uniqueidentifier UNIQUE
- Business identifier: IdentificationNumber UNIQUE
*/
CREATE TABLE core.Client
(
    ClientId int IDENTITY(1,1) NOT NULL,
    ClientKey uniqueidentifier NOT NULL CONSTRAINT DF_Client_ClientKey DEFAULT (NEWSEQUENTIALID()),
    ClientType varchar(20) NOT NULL, /* Individual, Company */
    Name nvarchar(100) NOT NULL,
    IdentificationNumber nvarchar(50) NOT NULL,
    Email nvarchar(100) NOT NULL,
    Phone nvarchar(50) NOT NULL,
    Street nvarchar(100) NULL,
    Number nvarchar(20) NULL,

    CONSTRAINT PK_Client PRIMARY KEY CLUSTERED (ClientId),
    CONSTRAINT UQ_Client_ClientKey UNIQUE (ClientKey),
    CONSTRAINT UQ_Client_IdentificationNumber UNIQUE (IdentificationNumber),
    CONSTRAINT CK_Client_Type CHECK (ClientType IN ('Individual', 'Company')),
    CONSTRAINT CK_Client_Name_NotBlank CHECK (LEN(LTRIM(RTRIM(Name))) > 0),
    CONSTRAINT CK_Client_IdNumber_NotBlank CHECK (LEN(LTRIM(RTRIM(IdentificationNumber))) > 0),
    CONSTRAINT CK_Client_Email_NotBlank CHECK (LEN(LTRIM(RTRIM(Email))) > 0),
    CONSTRAINT CK_Client_Phone_NotBlank CHECK (LEN(LTRIM(RTRIM(Phone))) > 0),
    CONSTRAINT CK_Client_Address_BothOrNone CHECK
    (
        (Street IS NULL AND Number IS NULL)
        OR (LEN(LTRIM(RTRIM(Street))) > 0 AND LEN(LTRIM(RTRIM(Number))) > 0)
    )
);
GO

/* Currency (metadata) */
CREATE TABLE core.Currency
(
    Code nvarchar(10) NOT NULL,
    Name nvarchar(100) NOT NULL,
    ExchangeRateToBase decimal(19,8) NOT NULL,
    IsActive bit NOT NULL,

    CONSTRAINT PK_Currency PRIMARY KEY CLUSTERED (Code),
    CONSTRAINT CK_Currency_Code_NotBlank CHECK (LEN(LTRIM(RTRIM(Code))) > 0),
    CONSTRAINT CK_Currency_Name_NotBlank CHECK (LEN(LTRIM(RTRIM(Name))) > 0),
    CONSTRAINT CK_Currency_ExchangeRate_Positive CHECK (ExchangeRateToBase > 0)
);
GO

/* Building
- Internal surrogate key: BuildingId int IDENTITY clustered PK
- External/app key: BuildingKey uniqueidentifier UNIQUE
- Internal FK to owner client: OwnerClientId uniqueidentifier
*/
CREATE TABLE core.Building
(
    BuildingId int IDENTITY(1,1) NOT NULL,
    BuildingKey uniqueidentifier NOT NULL CONSTRAINT DF_Building_BuildingKey DEFAULT (NEWSEQUENTIALID()),
    OwnerClientId uniqueidentifier NOT NULL,
    CityId int NOT NULL,

    Street nvarchar(100) NOT NULL,
    Number nvarchar(20) NOT NULL,

    ConstructionYear int NOT NULL,
    BuildingType varchar(20) NOT NULL, /* Residential, Office, Industrial */
    SurfaceArea int NOT NULL,

    InsuredValueAmount decimal(19,2) NOT NULL,
    InsuredValueCurrencyCode nvarchar(10) NOT NULL,

    CONSTRAINT PK_Building PRIMARY KEY CLUSTERED (BuildingId),
    CONSTRAINT UQ_Building_BuildingKey UNIQUE (BuildingKey),

    CONSTRAINT FK_Building_Client FOREIGN KEY (OwnerClientId) REFERENCES core.Client(ClientKey),
    CONSTRAINT FK_Building_City FOREIGN KEY (CityId) REFERENCES core.City(CityId),
    CONSTRAINT FK_Building_InsuredValueCurrency FOREIGN KEY (InsuredValueCurrencyCode) REFERENCES core.Currency(Code),

    CONSTRAINT CK_Building_Address_NotBlank CHECK (LEN(LTRIM(RTRIM(Street))) > 0 AND LEN(LTRIM(RTRIM(Number))) > 0),
    CONSTRAINT CK_Building_ConstructionYear CHECK (ConstructionYear >= 1600 AND ConstructionYear <= 3000),
    CONSTRAINT CK_Building_Type CHECK (BuildingType IN ('Residential', 'Office', 'Industrial')),
    CONSTRAINT CK_Building_SurfaceArea_Positive CHECK (SurfaceArea > 0),
    CONSTRAINT CK_Building_InsuredValue_Positive CHECK (InsuredValueAmount >= 0),
    CONSTRAINT CK_Building_Currency_NotBlank CHECK (LEN(LTRIM(RTRIM(InsuredValueCurrencyCode))) > 0)
);
GO

CREATE TABLE core.RiskCategory
(
    RiskCategoryId int IDENTITY(1,1) NOT NULL,
    Code varchar(50) NOT NULL,     /* e.g. FloodZone, EarthquakeZone */
    Name nvarchar(100) NOT NULL,   /* display name */
    IsActive bit NOT NULL CONSTRAINT DF_RiskCategory_IsActive DEFAULT (1),

    CONSTRAINT PK_RiskCategory PRIMARY KEY CLUSTERED (RiskCategoryId),
    CONSTRAINT UQ_RiskCategory_Code UNIQUE (Code),
    CONSTRAINT UQ_RiskCategory_Name UNIQUE (Name),
    CONSTRAINT CK_RiskCategory_Code_NotBlank CHECK (LEN(LTRIM(RTRIM(Code))) > 0),
    CONSTRAINT CK_RiskCategory_Name_NotBlank CHECK (LEN(LTRIM(RTRIM(Name))) > 0)
);
GO

/* Building risk categories (junction) */
CREATE TABLE core.RiskBuilding
(
    BuildingKey uniqueidentifier NOT NULL,
    RiskCategoryId int NOT NULL,

    CONSTRAINT PK_RiskBuilding PRIMARY KEY CLUSTERED (BuildingKey, RiskCategoryId),
    CONSTRAINT FK_RiskBuilding_Building FOREIGN KEY (BuildingKey) REFERENCES core.Building(BuildingKey),
    CONSTRAINT FK_RiskBuilding_RiskCategory FOREIGN KEY (RiskCategoryId) REFERENCES core.RiskCategory(RiskCategoryId)
);
GO

/* PremiumRules (TPH)
- Internal surrogate key: PremiumRuleId int IDENTITY clustered PK
- External/app key: PremiumRuleKey uniqueidentifier UNIQUE
*/
CREATE TABLE core.PremiumRules
(
    PremiumRuleId int IDENTITY(1,1) NOT NULL,
    PremiumRuleKey uniqueidentifier NOT NULL CONSTRAINT DF_PremiumRules_PremiumRuleKey DEFAULT (NEWSEQUENTIALID()),

    RuleKind varchar(30) NOT NULL,
    Name nvarchar(200) NOT NULL,
    Percentage decimal(9,6) NOT NULL, /* 0..1 */
    IsActive bit NOT NULL,

    /* Fee-only */
    FeeType varchar(30) NULL, /* BrokerCommission, AdminFee */
    EffectiveFrom date NULL,
    EffectiveTo date NULL,

    /* Risk-only (nullable where not used) */
    CityId int NULL,
    CountyId int NULL,
    CountryId int NULL,
    BuildingType varchar(20) NULL,
    ZoneRiskCategoryCode varchar(50) NULL, /* e.g. FloodZone, EarthquakeZone */

    CONSTRAINT PK_PremiumRules PRIMARY KEY CLUSTERED (PremiumRuleId),
    CONSTRAINT UQ_PremiumRules_PremiumRuleKey UNIQUE (PremiumRuleKey),

    CONSTRAINT CK_PremiumRules_RuleKind CHECK
    (
        RuleKind IN ('Fee', 'RiskCity', 'RiskCounty', 'RiskCountry', 'RiskBuildingType', 'RiskZoneCategory')
    ),
    CONSTRAINT CK_PremiumRules_Name_NotBlank CHECK (LEN(LTRIM(RTRIM(Name))) > 0),
    CONSTRAINT CK_PremiumRules_Percentage_0_1 CHECK (Percentage >= 0.0 AND Percentage <= 1.0),
    CONSTRAINT CK_PremiumRules_FeeType CHECK (FeeType IS NULL OR FeeType IN ('BrokerCommission', 'AdminFee')),
    CONSTRAINT CK_PremiumRules_EffectiveDates CHECK (EffectiveFrom IS NULL OR EffectiveTo IS NULL OR EffectiveTo >= EffectiveFrom),
    CONSTRAINT CK_PremiumRules_BuildingType CHECK (BuildingType IS NULL OR BuildingType IN ('Residential', 'Office', 'Industrial')),

    /* Discriminator-driven column requirements */
    CONSTRAINT CK_PremiumRules_Fee_RequiresFeeColumns CHECK
    (
        (RuleKind <> 'Fee')
        OR (FeeType IS NOT NULL AND EffectiveFrom IS NOT NULL AND EffectiveTo IS NOT NULL)
    ),
    CONSTRAINT CK_PremiumRules_RiskCity_RequiresCity CHECK
    (
        (RuleKind <> 'RiskCity') OR (CityId IS NOT NULL)
    ),
    CONSTRAINT CK_PremiumRules_RiskCounty_RequiresCounty CHECK
    (
        (RuleKind <> 'RiskCounty') OR (CountyId IS NOT NULL)
    ),
    CONSTRAINT CK_PremiumRules_RiskCountry_RequiresCountry CHECK
    (
        (RuleKind <> 'RiskCountry') OR (CountryId IS NOT NULL)
    ),
    CONSTRAINT CK_PremiumRules_RiskBuildingType_RequiresBuildingType CHECK
    (
        (RuleKind <> 'RiskBuildingType') OR (BuildingType IS NOT NULL)
    ),
    CONSTRAINT CK_PremiumRules_RiskTag_RequiresRiskCategory CHECK
    (
        (RuleKind <> 'RiskZoneCategory') OR (ZoneRiskCategoryCode IS NOT NULL AND LEN(LTRIM(RTRIM(ZoneRiskCategoryCode))) > 0)
    ),

    /* FKs (nullable columns allowed) */
    CONSTRAINT FK_PremiumRules_City FOREIGN KEY (CityId) REFERENCES core.City(CityId),
    CONSTRAINT FK_PremiumRules_County FOREIGN KEY (CountyId) REFERENCES core.County(CountyId),
    CONSTRAINT FK_PremiumRules_Country FOREIGN KEY (CountryId) REFERENCES core.Country(CountryId),
    CONSTRAINT FK_PremiumRules_ZoneRiskCategoryCode FOREIGN KEY (ZoneRiskCategoryCode) REFERENCES core.RiskCategory(Code)
)
GO

/* Policy
- Internal surrogate key: PolicyId int IDENTITY clustered PK
- External/app key: PolicyNumber uniqueidentifier UNIQUE
- Internal FKs: ClientId/BuildingId/BrokerId int
*/
CREATE TABLE core.Policy
(
    PolicyId int IDENTITY(1,1) NOT NULL,
    PolicyNumber uniqueidentifier NOT NULL CONSTRAINT DF_Policy_PolicyNumber DEFAULT (NEWSEQUENTIALID()),

    ClientKey uniqueidentifier NOT NULL,
    BuildingKey uniqueidentifier NOT NULL,
    BrokerKey uniqueidentifier NOT NULL,

    Status varchar(20) NOT NULL, /* Draft, Active, Expired, Cancelled */

    StartDate date NOT NULL,
    EndDate date NOT NULL,

    BasePremiumAmount decimal(19,2) NOT NULL,
    FinalPremiumAmount decimal(19,2) NOT NULL,
    CurrencyCode nvarchar(10) NOT NULL,

    CreationDate date NOT NULL,
    LastUpdateDate date NULL,

    CancellationReason nvarchar(200) NULL,
    CancellationEffectiveDate date NULL,

    CONSTRAINT PK_Policy PRIMARY KEY CLUSTERED (PolicyId),
    CONSTRAINT UQ_Policy_PolicyNumber UNIQUE (PolicyNumber),

    CONSTRAINT FK_Policy_Client FOREIGN KEY (ClientKey) REFERENCES core.Client(ClientKey),
    CONSTRAINT FK_Policy_Building FOREIGN KEY (BuildingKey) REFERENCES core.Building(BuildingKey),
    CONSTRAINT FK_Policy_Broker FOREIGN KEY (BrokerKey) REFERENCES core.Broker(BrokerKey),
    CONSTRAINT FK_Policy_Currency FOREIGN KEY (CurrencyCode) REFERENCES core.Currency(Code),

    CONSTRAINT CK_Policy_Status CHECK (Status IN ('Draft', 'Active', 'Expired', 'Cancelled')),
    CONSTRAINT CK_Policy_Dates CHECK (EndDate > StartDate),
    CONSTRAINT CK_Policy_BasePremium_NonNegative CHECK (BasePremiumAmount >= 0),
    CONSTRAINT CK_Policy_FinalPremium_NonNegative CHECK (FinalPremiumAmount >= 0),
    CONSTRAINT CK_Policy_Currency_NotBlank CHECK (LEN(LTRIM(RTRIM(CurrencyCode))) > 0),

    CONSTRAINT CK_Policy_CancelFields CHECK
    (
        (Status <> 'Cancelled' AND CancellationReason IS NULL AND CancellationEffectiveDate IS NULL)
        OR
        (Status = 'Cancelled' AND LEN(LTRIM(RTRIM(CancellationReason))) > 0 AND CancellationEffectiveDate IS NOT NULL)
    )
);
GO

/* Indexes (keep existing intent; adjust to surrogate keys + add fast lookups on GUID keys) */

/* Geography FK indexes (not automatic in SQL Server) */
CREATE INDEX IX_County_CountryId ON core.County(CountryId);
CREATE INDEX IX_City_CountyId ON core.City(CountyId);
GO

/* Building access paths */
CREATE INDEX IX_Building_OwnerClientKey ON core.Building(OwnerClientId);
CREATE INDEX IX_Building_CityId ON core.Building(CityId);
GO

/* RiskBuilding reverse lookup */
CREATE INDEX IX_RiskBuilding_RiskCategoryId ON core.RiskBuilding(RiskCategoryId);
GO

/* Policy access paths */
CREATE INDEX IX_Policy_ClientKey ON core.Policy(ClientKey);
CREATE INDEX IX_Policy_BuildingKey ON core.Policy(BuildingKey);
CREATE INDEX IX_Policy_BrokerKey ON core.Policy(BrokerKey);
CREATE INDEX IX_Policy_Status_Dates ON core.Policy(Status, StartDate, EndDate);
GO

/* PremiumRules access paths */
CREATE INDEX IX_PremiumRules_RuleKind_Active ON core.PremiumRules(RuleKind, IsActive);
GO