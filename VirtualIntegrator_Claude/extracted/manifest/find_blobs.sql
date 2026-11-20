SET NOCOUNT ON;
SELECT t.name + '.' + c.name AS blob_column,
       ty.name AS data_type,
       c.max_length
FROM sys.columns c
JOIN sys.tables t ON c.object_id = t.object_id
JOIN sys.types ty ON c.user_type_id = ty.user_type_id
WHERE ty.name IN ('varbinary','image','binary')
ORDER BY t.name, c.column_id;
