#!/bin/bash
echo "Construyendo e iniciando contenedores..."
docker-compose up --build -d

echo ""
echo "Esperando que los contenedores estén listos..."
sleep 3

echo ""
echo "Mostrando logs de bhd-api (Presiona Ctrl+C para salir)..."
echo "================================================"
docker logs bhd-api -f
