use InsuranceDb;
go

alter table core.Building
	add InsuredValueCurrency nvarchar(10) NOT NULL
	constraint df_building_currency default ('RON')
	constraint ck_building_currency
		check (len(ltrim(rtrim(InsuredValueCurrency))) > 0);
