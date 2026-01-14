-- Script de inicialización de base de datos
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'BhdDocumentDb')
BEGIN
    CREATE DATABASE BhdDocumentDb;
    PRINT 'Base de datos BhdDocumentDb creada exitosamente';
END
ELSE
BEGIN
    PRINT 'Base de datos BhdDocumentDb ya existe';
END
GO