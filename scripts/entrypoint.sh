#!/bin/bash
set -e

# Esperar a que SQL Server esté listo
echo "Esperando a que SQL Server inicie..."
sleep 30s

# Ejecutar el script de inicialización
echo "Ejecutando script de inicialización de base de datos..."
/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P $MSSQL_SA_PASSWORD -C -i /scripts/init-db.sql

echo "Base de datos inicializada correctamente"

docker-compose logs -f bhd-api