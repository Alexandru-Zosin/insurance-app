use InsuranceDb;
go

create table core.Building(
	BuildingId INT IDENTITY(1,1) PRIMARY KEY, -- -- clustered index auto. created to obtain bId with bKey
	BuildingKey UNIQUEIDENTIFIER UNIQUE NOT NULL,  -- noncl ind. to later use clustered index from pk
	ClientId INT NOT NULL,
	CityId INT NOT NULL,
	ConstructionYear INT NOT NULL,
	Address nvarchar(100) NOT NULL,
	BuildingType nvarchar(50) not null,
	NumberOfFloors INT NOT NULL,
	SurfaceArea INT NOT NULL,
	InsuredValue INT NOT NULL,
	FloodRiskZone INT NULL,
	EarthquakeRiskZone INT NULL,
	
	constraint ck_construction_year
		check (ConstructionYear between 1600 and 3000),

	constraint ck_building_specs_positive_values
		check (NumberOfFloors > 0 and SurfaceArea > 0 and InsuredValue > 0),

	constraint ck_building_address
		check (len(ltrim(rtrim(Address))) > 0),

	constraint ck_building_type 
		check (BuildingType in ('Residential', 'Office', 'Industrial')),

	constraint fk_client
		foreign key (ClientId) references core.Client (ClientId),

	constraint fk_city
		foreign key (CityId) references core.City (CityId)
);