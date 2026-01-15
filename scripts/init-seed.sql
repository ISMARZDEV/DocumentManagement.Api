-- Script de inicialización con datos del seeder
-- Este script se ejecuta automáticamente cuando se crea el contenedor SQL Server

USE [BhdDocumentDb];
GO

-- Insertar Usuarios
INSERT INTO [dbo].[Users]
    ([Id], [Email], [UserName], [Name], [PasswordHash], [Role], [CreatedAt])
VALUES
    ('aebdcc30-a004-42b7-8c9c-cba97a500780', 'admin@prueba.com', 'admin', 'Administrador', 'AQAAAAIAAYagAAAAEMMaDiJmYNcCJEp5cOvnLZW6R4KMaUx91VfG+Kx/oFU+7E1X4ZF5JK51ND5VBbCRXA==', 'Admin', '2026-01-15T00:00:00.0000000'),
    ('550e8400-e29b-41d4-a716-446655440010', 'operador@prueba.com', 'operador', 'María de Jesús', 'AQAAAAIAAYagAAAAEMMaDiJmYNcCJEp5cOvnLZW6R4KMaUx91VfG+Kx/oFU+7E1X4ZF5JK51ND5VBbCRXA==', 'Operator', '2026-01-15T00:00:00.0000000'),
    ('550e8400-e29b-41d4-a716-446655440011', 'cliente@prueba.com', 'cliente', 'José Pérez', 'AQAAAAIAAYagAAAAEMMaDiJmYNcCJEp5cOvnLZW6R4KMaUx91VfG+Kx/oFU+7E1X4ZF5JK51ND5VBbCRXA==', 'Client', '2026-01-15T00:00:00.0000000');
GO

-- Insertar Documentos
INSERT INTO [dbo].[Documents]
    ([Id], [UserId], [Filename], [ContentType], [DocumentType], [Channel], [CustomerId], [FileUrl], [Size], [Status], [CorrelationId], [CreatedAt])
VALUES
    ('cc2fef10-2bab-42a6-b665-79248d985265', '550e8400-e29b-41d4-a716-446655440011', 'fail.pdf', 'application/pdf', 1, 0, '550e8400-e29b-41d4-a716-446655440011', NULL, 89446, 4, '80020', '2026-01-15T05:41:20.8335273'),
    ('1b036a5c-82d7-46e1-aaf3-bd9b23d792f3', '550e8400-e29b-41d4-a716-446655440011', 'test document.docx', 'application/vnd.openxmlformats-officedocument.wordprocessingml.document', 3, 1, '550e8400-e29b-41d4-a716-446655440011', '/app/DocumentStorage/2026/01/1b036a5c-82d7-46e1-aaf3-bd9b23d792f3_test document.docx', 2285500, 2, '80019', '2026-01-15T05:36:48.9631863'),
    ('da2154e8-8a0b-478c-bd8d-91cb25f5d9a5', '550e8400-e29b-41d4-a716-446655440011', 'test document.xlsx', 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet', 4, 0, '550e8400-e29b-41d4-a716-446655440011', '/app/DocumentStorage/2026/01/da2154e8-8a0b-478c-bd8d-91cb25f5d9a5_test document.xlsx', 9479, 2, '80018', '2026-01-15T05:36:33.0352655'),
    ('ebc74bc8-a771-4cbf-b2b6-c821c6a964b3', '550e8400-e29b-41d4-a716-446655440011', 'test_document.pdf', 'application/pdf', 5, 2, '550e8400-e29b-41d4-a716-446655440011', '/app/DocumentStorage/2026/01/ebc74bc8-a771-4cbf-b2b6-c821c6a964b3_test_document.pdf', 89446, 2, '80015', '2026-01-15T05:19:40.6844382'),
    ('35973e79-8ba6-492c-bb1e-e9ab2fd4d684', '550e8400-e29b-41d4-a716-446655440010', 'test_image.jpg', 'image/jpeg', 0, 1, '550e8400-e29b-41d4-a716-446655440011', '/app/DocumentStorage/2026/01/35973e79-8ba6-492c-bb1e-e9ab2fd4d684_test_image.jpg', 180110, 2, '80016', '2026-01-15T05:32:47.8042926'),
    ('d1de9bc0-04ee-4dcc-93f7-6317c59d74bc', '550e8400-e29b-41d4-a716-446655440011', 'test_image.png', 'image/png', 1, 0, '550e8400-e29b-41d4-a716-446655440011', '/app/DocumentStorage/2026/01/d1de9bc0-04ee-4dcc-93f7-6317c59d74bc_test_image.png', 1255233, 1, '80017', '2026-01-15T05:33:48.4928443');
GO
