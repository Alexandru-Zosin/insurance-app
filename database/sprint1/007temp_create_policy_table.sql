use InsuranceDb;
go

CREATE TABLE core.Policy (
    PolicyId INT IDENTITY(1,1) PRIMARY KEY,
    PolicyKey UNIQUEIDENTIFIER UNIQUE NOT NULL,

    ClientId INT NOT NULL,
    BuildingId INT NOT NULL,
    BrokerId INT NOT NULL,

    PremiumAmount DECIMAL(18,2) NOT NULL,
    PremiumCurrency NVARCHAR(10) NOT NULL,

    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,

    CONSTRAINT fk_policy_client
        FOREIGN KEY (ClientId) REFERENCES core.Client(ClientId),

    CONSTRAINT fk_policy_building
        FOREIGN KEY (BuildingId) REFERENCES core.Building(BuildingId),

    CONSTRAINT [fk_policy_broker]
        FOREIGN KEY (BrokerId) REFERENCES core.Broker(BrokerId),

    CONSTRAINT ck_policy_dates
        CHECK (EndDate > StartDate),

    CONSTRAINT ck_policy_premium
        CHECK (PremiumAmount > 0 AND LEN(LTRIM(RTRIM(PremiumCurrency))) > 0)
);
