USE [InsuranceDb];
GO

/* =========================
   Countries
   ========================= */

INSERT INTO core.Country (Name)
VALUES 
    ('Romania'), 
    ('France'), 
    ('Germany');
GO

/* =========================
   Counties
   ========================= */

DECLARE @countryIdRom INT = (SELECT CountryId FROM core.Country WHERE Name = 'Romania');
DECLARE @countryIdFra INT = (SELECT CountryId FROM core.Country WHERE Name = 'France');
DECLARE @countryIdGer INT = (SELECT CountryId FROM core.Country WHERE Name = 'Germany');

INSERT INTO core.County (CountryId, Name)
VALUES
    (@countryIdRom, 'Cluj'), (@countryIdRom, 'Timis'), (@countryIdRom, 'Iasi'), (@countryIdRom, 'Brasov'),
    (@countryIdRom, 'Sibiu'), (@countryIdRom, 'Bihor'), (@countryIdRom, 'Constanta'), (@countryIdRom, 'Dolj'),
    (@countryIdRom, 'Arad'), (@countryIdRom, 'Bucuresti'),

    (@countryIdFra, 'Ile-de-France'), (@countryIdFra, 'Provence-Alpes-Cote d''Azur'),
    (@countryIdFra, 'Auvergne-Rhone-Alpes'), (@countryIdFra, 'Nouvelle-Aquitaine'),
    (@countryIdFra, 'Occitanie'), (@countryIdFra, 'Bretagne'),
    (@countryIdFra, 'Normandie'), (@countryIdFra, 'Hauts-de-France'),
    (@countryIdFra, 'Grand Est'), (@countryIdFra, 'Pays de la Loire'),

    (@countryIdGer, 'Bavaria'), (@countryIdGer, 'Berlin'), (@countryIdGer, 'Saxony'),
    (@countryIdGer, 'Hesse'), (@countryIdGer, 'North Rhine-Westphalia'),
    (@countryIdGer, 'Hamburg'), (@countryIdGer, 'Baden-Wurttemberg'),
    (@countryIdGer, 'Brandenburg'), (@countryIdGer, 'Lower Saxony'),
    (@countryIdGer, 'Saarland');
GO

/* =========================
   Cities
   ========================= */

DECLARE @countyId INT;

DECLARE city_cursor CURSOR FOR
    SELECT CountyId FROM core.County;

OPEN city_cursor;
FETCH NEXT FROM city_cursor INTO @countyId;

WHILE @@FETCH_STATUS = 0
BEGIN
    INSERT INTO core.City (Name, CountyId)
    VALUES 
        (CONCAT('CityA_', @countyId), @countyId),
        (CONCAT('CityB_', @countyId), @countyId);

    FETCH NEXT FROM city_cursor INTO @countyId;
END

CLOSE city_cursor;
DEALLOCATE city_cursor;
GO

/* =========================
   Brokers
   ========================= */

INSERT INTO core.Broker (BrokerKey, Name, IsActive)
VALUES
    (NEWID(), 'Alpha Broker', 1),
    (NEWID(), 'Beta Insurance', 1),
    (NEWID(), 'Gamma Cover', 0),
    (NEWID(), 'Delta Protect', 1),
    (NEWID(), 'Omega Assurance', 1);
GO

/* =========================
   Clients
   ========================= */

DECLARE @i INT = 1;

WHILE @i <= 20
BEGIN
    INSERT INTO core.Client (
        ClientKey,
        ClientType,
        Name,
        RegistrationNumber,
        Email,
        Phone,
        Street,
        Number
    )
    VALUES (
        NEWID(),
        CASE WHEN @i % 2 = 0 THEN 'Company' ELSE 'Individual' END,
        CONCAT('Client_', @i),
        CONCAT('REG', 1000 + @i),
        CONCAT('client', @i, '@example.com'),
        CONCAT('+40123', FORMAT(@i, '00')),
        'Example Street',
        CAST(@i AS NVARCHAR(20))
    );

    SET @i += 1;
END
GO

/* =========================
   Buildings
   ========================= */

DECLARE @clientId INT;
DECLARE @j INT;
DECLARE @cityId INT;

DECLARE client_cursor CURSOR FOR
    SELECT ClientId FROM core.Client;

OPEN client_cursor;
FETCH NEXT FROM client_cursor INTO @clientId;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @j = 0;

    WHILE @j < (1 + ABS(CHECKSUM(NEWID())) % 3)
    BEGIN
        SELECT TOP 1 @cityId = CityId FROM core.City ORDER BY NEWID();

        INSERT INTO core.Building (
            BuildingKey,
            ClientId,
            CityId,
            ConstructionYear,
            Street,
            Number,
            BuildingType,
            NumberOfFloors,
            SurfaceArea,
            InsuredValue,
            FloodRiskZone,
            EarthquakeRiskZone,
            InsuredValueCurrency
        )
        VALUES (
            NEWID(),
            @clientId,
            @cityId,
            1980 + (@j * 5),
            CONCAT('Building Street ', @clientId),
            CAST(@j + 1 AS NVARCHAR(20)),
            CASE
                WHEN @j % 3 = 0 THEN 'Residential'
                WHEN @j % 3 = 1 THEN 'Office'
                ELSE 'Industrial'
            END,
            1 + (@j % 5),
            100 + (@j * 20),
            50000 + (@j * 10000),
            NULL,
            NULL,
            'EUR'
        );

        SET @j += 1;
    END

    FETCH NEXT FROM client_cursor INTO @clientId;
END

CLOSE client_cursor;
DEALLOCATE client_cursor;
GO

/* =========================
   Policies
   ========================= */

INSERT INTO core.Policy (
    PolicyKey,
    ClientId,
    BuildingId,
    BrokerId,
    PremiumAmount,
    PremiumCurrency,
    StartDate,
    EndDate
)
SELECT
    NEWID(),
    b.ClientId,
    b.BuildingId,
    (SELECT TOP 1 BrokerId FROM core.Broker ORDER BY NEWID()),
    500 + (ABS(CHECKSUM(NEWID())) % 1000),
    'EUR',
    DATEFROMPARTS(2025, 1 + (ABS(CHECKSUM(NEWID())) % 12), 1),
    DATEFROMPARTS(2026, 1 + (ABS(CHECKSUM(NEWID())) % 12), 1)
FROM core.Building b;
GO
