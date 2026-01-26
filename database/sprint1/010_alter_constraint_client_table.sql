ALTER TABLE core.Client DROP CONSTRAINT ck_client_type;
GO

ALTER TABLE core.Client WITH CHECK
ADD CONSTRAINT ck_client_type
CHECK (ClientType IN ('Company', 'Individual'));
GO
