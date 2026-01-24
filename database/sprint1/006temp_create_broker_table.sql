use InsuranceDb;
go

create table core.Broker (
	BrokerId INT IDENTITY(1,1) PRIMARY KEY,
	BrokerKey UNIQUEIDENTIFIER UNIQUE NOT NULL,
	Name nvarchar(100) NOT NULL,
	IsActive bit not null,

	constraint ck_broker_name check (len(ltrim(rtrim(name))) > 0)
);
