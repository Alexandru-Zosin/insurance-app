use InsuranceDb;
go

create table core.Country (
	CountryId INT IDENTITY(1,1) PRIMARY KEY,
	Name nvarChar(100) NOT NULL UNIQUE
	
	constraint ck_country_name
		check (len(ltrim(rtrim(Name))) > 0)
);
go

create table core.County (
	CountyId INT IDENTITY(1, 1) NOT NULL PRIMARY KEY,
	CountryId INT NOT NULL,
	Name nvarchar(100) NOT NULL,

	constraint ck_county_name
		check (len(ltrim(rtrim(Name))) > 0),

	constraint uq_county
		unique (CountryId, Name),

	constraint fk_country
		foreign key (countryId) references core.Country (CountryId)
);
go

create table core.City (
	CityId INT IDENTITY(1,1) PRIMARY KEY,
	Name nvarchar(100) NOT NULL,
	CountyId INT NOT NULL,

	constraint ck_city_name
		check (len(ltrim(rtrim(Name))) > 0),

	constraint fk_county
		foreign key (CountyId) references core.County(CountyId),

	constraint uq_city
		unique (Name, CountyId)
);
go