use InsuranceDb;
go

create table core.Client (
	ClientId INT IDENTITY(1, 1) PRIMARY KEY,
	ClientKey UNIQUEIDENTIFIER UNIQUE NOT NULL,
	ClientType VARCHAR(20) NOT NULL,
	Name nvarchar(100) NOT NULL,
	RegistrationNumber nvarchar(50) UNIQUE NOT NULL, -- CNP or CUI
	Email nvarchar(100) NOT NULL,
	Phone nvarchar(50) NOT NULL,
	Address nvarchar(100) NOT NULL,

	constraint ck_client_type 
		check (ClientType in ('Individual', 'Company')),

	constraint ck_client_name
		check (len(ltrim(rtrim(Name))) > 0),

	constraint ck_registration_number
		check (len(ltrim(rtrim(RegistrationNumber))) > 0)
);
go