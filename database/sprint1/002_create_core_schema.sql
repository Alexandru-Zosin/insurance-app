use InsuranceDb;

if not exists (
	select 1 from sys.schemas where name ='core'
)
begin
	exec('create SCHEMA core')
end
go