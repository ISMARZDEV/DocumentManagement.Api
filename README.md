# Document Management API - Technical Evaluation

![NET8](https://img.shields.io/badge/.NET-8.0-purple) ![Docker](https://img.shields.io/badge/Docker-Ready-blue) ![SQLServer](https://img.shields.io/badge/SQL_Server-2022-red)

API REST de gestión de carga y búsqueda de documentos. Diseñada para proporcionar capacidades de búsqueda avanzada, carga asíncrona y almacenamiento de metadatos.

---

## 📄 Documentación - Arquitectura de solución

<div align="center">
  <a href="TU_ENLACE_AQUI">
    <img width="400" height="1100" alt="Diagrama de Arquitectura" src="https://github.com/user-attachments/assets/91badbb3-64fc-4172-90e3-17d086c48efa" />
  </a>
  <p><i>Figura 1: Documentación - Arquitectura de solución</i></p>
</div>

<div align="center">

  [Ver documentación de arquitectura completo](./docs/architecture.md) (Acceder mediante este enlace)

</div>


## 🛠️ Stack Tecnológico

* **Framework:** .NET 8 (ASP.NET Core Web API)
* **Lenguaje:** C\#
* **Base de Datos:** SQL Server 2022
* **Jobs en segundo plano:** Hangfire (Background Job)
* **ORM:** Entity Framework Core 8
* **Resiliencia:** Polly (Reintentos y Circuit Breakers)
* **Containerización:** Docker & Docker Compose
* **Testing:** xUnit & Moq
* **Seguridad:** JWT (JSON Web Tokens)

---

## 🏛️ Arquitectura: Clean Architecture

El desarrollo de esta API REST sigue estrictamente los principios de **Clean Architecture** (Arquitectura Limpia), separando las responsabilidades en capas concéntricas para garantizar la independencia de frameworks, UI y bases de datos.

<div align="center">
  <img width="300" height="300" alt="Diagrama de Arquitectura" src="https://github.com/user-attachments/assets/61813877-5182-42a2-ab9f-f553ab376b8c" />
  <p><i>Figura 2: Diagrama de la arquitectura del sistema</i></p>
</div>

### Capas del Sistema

1.  **Domain (Núcleo):** Contiene las Entidades (`Document`, `User`), Value Objects, Enums y las Interfaces de los Repositorios. **No tiene dependencias externas.**
2.  **Application (Casos de Uso):** Contiene la lógica de negocio pura, DTOs, Validaciones y las Interfaces de Servicios. Orquesta el flujo de datos entre el Dominio y la Infraestructura.
3.  **Infrastructure (Adaptadores):** Implementa las interfaces definidas en Domain/Application. Aquí residen:
    * `DbContext` (EF Core).
    * Implementación de Repositorios.
    * Background Workers (HostedService / Hangfire jobs).
    * Servicios de Archivos (FileStorage).
    * Background Workers (`HostedService`).
4.  **Web.Api (Presentación):** Punto de entrada HTTP (REST). Contiene los Controllers, Configuración de Inyección de Dependencias (IoC) y Middlewares.

---

## 📂 Estructura de la Solución

La solución mantiene una separación física clara para respetar la arquitectura:

```text
📦 DocumentManagement.Api
 ┣ 📂 src
 ┃ ┣ 📂 Bhd.Domain             # Logica de negocio (Entidades, Interfaces, Repositorio...)
 ┃ ┣ 📂 Bhd.Application        # Casos de uso (DTOs, Services, Interfaces, Validators..)
 ┃ ┣ 📂 Bhd.Infrastructure     # Servicios externos (EF Core, Hangerfire, Handlers, Jobs, Persistance, Migrations...)
 ┃ ┗ 📂 Bhd.WebApi             # Presentacion (Controllers, Program.cs, Dockerfile...)
 ┃
 ┣ 📂 tests                    # Pruebas Unitarias e Integración
 ┣ 📜 .dockerignore            # Ignorar archivos y carpetas que no son necesarias en la imagen
 ┣ 📜 docker-compose.yml       # Orquestación de contenedores
 ┣ 📘 LICENSE                  # Copyright(c) 2026 Ismael Martínez
 ┗ 📜 README.md                # Esta documentación
 ```

---

## ⚙️ Ejecución "One Command" (Docker)

La aplicación está contenerizada y configurada para autogestionarse (Migraciones y Seeding automáticos al iniciar).

**Requisitos**

Docker Desktop instalado y corriendo.

**Paso para ejecutar**

Para correr ejecutar el comando en la raíz del proyecto (📦 DocumentManagement.Api):

```bash
docker compose up -d
```

Esto levantará:

**SQL Server (Puerto 2500)**

**Web API (Puerto 8080)** - Espera automáticamente a que la BD y los servicios dependientes estén listos.

Una vez levantado, puede acceder a la documentación de la API:

- Swagger (HTTP): http://localhost:5017/swagger/index.html
- Swagger (HTTPS, perfil `https` / desarrollo): https://localhost:7008/swagger/index.html
- Hangfire Dashboard (Jobs): http://localhost:5017/hangfire

Si usas HTTPS localmente y no confías el certificado aún, en macOS/Windows ejecuta antes:

```bash
dotnet dev-certs https --trust
```

Para ejecutar la API localmente con el perfil HTTPS (desde `src/Bhd.WebApi`):

```bash
cd src/Bhd.WebApi
dotnet run --launch-profile "https"
```

- Nota: si ejecutas en Docker, los puertos están mapeados por `docker-compose.yml` a `5017` (HTTP) y `7008` (HTTPS); si tienes problemas con certificados, usa la URL HTTP en `:5017`.
---

## 🧪 Otros Comandos Útiles


### Ver logs en tiempo real del contenedor API

```docker
docker-compose logs -f bhd-api
```

### Ver logs del SQL Server

```docker
docker-compose logs -f bhd-db
```

### Detener el contenedor (no los borra)

```docker
docker-compose stop
```

### Detener el contenedor (no los borra)

```docker
docker-compose start
```
### Borrar contenedor

```docker
docker-compose down
```
---

## 🌿 Estrategia de Ramas y GitHub (Gitflow) - Commits y Pull Request

Se utilizo un flujo de trabajo estructurado para garantizar la calidad del código.

**main:** Rama de Producción. Código estable y listo para desplegar.

**dev:** Rama de Integración. Aquí se une todo el trabajo desarrollado.

**testing:** Rama para QA. Despliegues para pruebas.

**feature/nombre-funcionalidad:** Ramas temporales para cada tarea del backlog.

**Reglas:** No esta permitido hacer commit directo a main o dev. Siempre usar Pull Requests.

---

## Documentación de Código

Se utilizo **Comentarios XML (`///`)** obligatorios en interfaces y servicios públicos. Esto debe describir el método, qué entra y qué sale.

### Formato Requerido:

```csharp
/// <summary>
/// Descripción breve y clara de QUÉ hace el método.
/// </summary>
/// <param name="nombreParametro">Descripción de qué es este parámetro.</param>
/// <returns>Descripción de qué devuelve el método al finalizar.</returns>
/// <exception cref="TipoExcepcion">Descripción de errores controlados que puede lanzar.</exception>
```

## 🚀 Flujo de Carga Asíncrona (Event-Driven)

La solución mantiene una separación física clara para respetar la arquitectura:

El sistema utiliza un patrón de **Productor-Consumidor** para no bloquear al cliente durante cargas pesadas:

**Recepción (API):** El usuario envía el archivo (POST /upload). La API guarda los metadatos con estado RECEIVED en SQL Server y encola un job de procesamiento (Hangfire). Retorna 202 Accepted inmediatamente.

**Procesamiento (Worker / Job):** Un servicio en segundo plano (Hangfire worker) procesa el job y realiza el almacenamiento (Ej. Azure Blob Storage, AWS S3, etc.).

**Resiliencia:** Si falla el almacenamiento, Polly se encarga de reintentar la operación antes de marcarlo como fallido.

---

## 🔁 Hangfire (Jobs en segundo plano) - Core Asíncrono 

Se utilizó Hangfire para la Carga Asíncrona (Core Asíncrono) y para encolar tareas de procesamiento de documentos desde la API.

### POST /api/bhd/mgmt/1/documents/actions/upload

Se utilizó Hangfire para la Carga Asíncrona (Core Asíncrono) y para encolar tareas de procesamiento (procesado y almacenamiento de documentos) desde la API.

<div align="center">
 <img width="511" height="369" alt="Image" src="https://github.com/user-attachments/assets/a0a4a0ac-4ccd-443d-9e67-e96c386fc82f" />
  <p><i>Figura 3: Dashboard Hangfire (monitorizar jobs)</i></p>
</div>

1. **Authenticación y Autorización** — POST Login para acceder mediante uno de los siguientes usuarios:

**Admin**

Acceso total. Puede cargar documentos para cualquier cliente, crear nuevos usuarios, ver la lista de usuarios y ver la lista completa de documentos cargados.

```json
{
  "email": "admin@prueba.com",
  "password": "Candado6947!"
}

```

* **Operador**

Acceso limitado. Puede cargar documentos de clientes y ver el listado de clientes.

```json
{
  "email": "operador@prueba.com",
  "password": "Candado6947!"
}
```

**Cliente**

Usuario final. Solo puede cargar sus propios documentos y ver sus documentos.

```json
{
  "email": "cliente@prueba.com",
  "password": "Candado6947!"
}
```

- **Nota:** Copiar y pegar token en la varible Globals (bearerToken)

<div align="center">
<img width="1000" height="487" alt="Image" src="https://github.com/user-attachments/assets/c4de5409-f610-42ee-89a0-8ddb77c0b0c3" />
</div>

2. **Cliente envía archivo** — POST con archivo multipart/form-data y token de seguridad JWT válido.

<div align="center">
  <img width="931" height="627" alt="Image" src="https://github.com/user-attachments/assets/6ceb05ca-1d68-429e-9c2a-355482549334" />
</div>

3. **API valida y codifica** — El servidor valida las credenciales y convierte el archivo a Base64.

4. **Handler crea Document en BD** — Se persiste el documento en SQL Server con estado `RECEIVED`.

5. **Handler guarda en staging** — El archivo se almacena temporalmente en `/temp`.

<div align="center">
<img width="518" height="384" alt="Image" src="https://github.com/user-attachments/assets/c891ac27-b250-4936-84bf-66c9280105a9" />
</div>

6. **Handler encola job** — Hangfire crea un job en la tabla `HangfireJob` de SQL Server.


7. **API retorna 202 Accepted** — El cliente recibe inmediatamente `documentId` y `jobId`.

<div align="center">
<img width="653" height="186" alt="Image" src="https://github.com/user-attachments/assets/b76b9746-c9b8-4b6a-b927-e21371c146cc"/>
</div>

8. **Hangfire Worker procesa** — Un servicio en background obtiene el job de la cola de SQL Server.

<div align="center">
<img width="1768" height="1022" alt="Image" src="https://github.com/user-attachments/assets/a5a6df5c-183c-432b-9a23-9cb738284c4b" />
</div>

9. **Worker ejecuta DocumentUploadJob** — Mueve el archivo de `/temp` a `DocumentStorage/{year}/{month}/` y actualiza el estado a `SENT`.

<div align="center">
<img width="1127" height="534" alt="Image" src="https://github.com/user-attachments/assets/20cb72d1-c63b-4341-a171-e87192372178" />
</div>

<div align="center">
  <img width="530" height="362" alt="Image" src="https://github.com/user-attachments/assets/74bdbef3-8294-4db3-9f68-b0a5eb7f088e" />
</div>


10. **Reintentos (si falla)** — Si hay error, reintenta 5 veces con delays de **1 min, 2 min, 3 min, 5 min y 10 min**. Si agota reintentos, actualiza el estado a `FAILED`.

---

## 📁 Carpetas en la raíz del proyecto

- **DocumentStorage/**: es la carpeta destinada al almacenamiento final (Servicio externo de almacenamiento de ejemplo) de documentos (estructura por año/mes, p.ej. `DocumentStorage/2026/01/`).
- **temp/**: carpeta temporal usada durante el procesamiento y decodificación antes de mover al almacenamiento definitivo (Servicio externo).

Ambas carpetas están en la raíz del repositorio: [DocumentStorage](DocumentStorage) y [temp](temp).

<div align="center">
 <img width="511" height="369" alt="Image" src="https://github.com/user-attachments/assets/e4f841a5-cea9-4f75-bff4-a66033442a2d" />
</div>

---

## ⏱️ Configuración: Delay Demo, Tamaño y Tipos de Documentos

**Delay de prueba (DEMO):** 30 segundos — Tiempo que el archivo permanece en `/tmp` antes de procesarse. Comentar en producción.

**Tamaño máximo de archivo:** 50 MB — Configurable en `RequestSizeLimit` (Bhd.WebApi). Validar en la capa Application.

**Tipos MIME permitidos:**
- `application/pdf` — PDF (Portable Document Format)
- `application/vnd.openxmlformats-officedocument.wordprocessingml.document` — DOCX (Microsoft Word)
- `application/vnd.openxmlformats-officedocument.spreadsheetml.sheet` — XLSX (Microsoft Excel)
- `image/png` — PNG (Portable Network Graphics)
- `image/jpeg` — JPG/JPEG (Joint Photographic Experts Group)
- `text/plain` — TXT (Plain Text)

**Reintentos de Hangfire:** 5 intentos de procesamiento con delays de 1 min, 2 min, 3 min, 5 min y 10 min. Tras agotarse, el documento se marca como `FAILED`.
