# Document Management API - Technical Evaluation

![NET8](https://img.shields.io/badge/.NET-8.0-purple) ![Docker](https://img.shields.io/badge/Docker-Ready-blue) ![RabbitMQ](https://img.shields.io/badge/RabbitMQ-Messaging-orange) ![SQLServer](https://img.shields.io/badge/SQL_Server-2022-red)

Sistema robusto de gestión de carga y búsqueda de documentos. Diseñado para proporcionar capacidades de carga asíncrona, almacenamiento de metadatos y orquestación resiliente mediante mensajería.

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
* **Mensajería:** RabbitMQ (Orquestación asíncrona)
* **ORM:** Entity Framework Core 8
* **Resiliencia:** Polly (Reintentos y Circuit Breakers)
* **Containerización:** Docker & Docker Compose
* **Testing:** xUnit & Moq
* **Autenticación y Autorización:** JWT (JSON Web Tokens)

---

## 🏛️ Arquitectura: Clean Architecture

Este proyecto sigue estrictamente los principios de **Clean Architecture** (Arquitectura Limpia), separando las responsabilidades en capas concéntricas para garantizar la independencia de frameworks, UI y bases de datos.

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
    * Clientes de RabbitMQ (Producer/Consumer).
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
 ┃ ┣ 📂 Bhd.Infrastructure     # Servicios externos (EF Core, RabbitMQ, Workers, Migrations...)
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

**RabbitMQ (Puerto 5672/15672)**

**Web API (Puerto 8080)** - Espera automáticamente a que la BD y RabbitMQ estén listos.

Una vez levantado, accede a la documentación de la API:

👉 **Swagger UI:** http://localhost:8080/swagger

---

## 🧪 Comandos Útiles

Ejecutarlo manualmente (sin Docker)

### Ejecutar Migraciones (desde carpeta src/)
dotnet ef database update -s Bhd.WebApi -p Bhd.Infrastructure

### Correr Pruebas
dotnet test

---

## 🚀 Flujo de Carga Asíncrona (Event-Driven)

La solución mantiene una separación física clara para respetar la arquitectura:

El sistema utiliza un patrón de **Productor-Consumidor** para no bloquear al cliente durante cargas pesadas:

**Recepción (API):** El usuario envía el archivo (POST /upload). La API guarda los metadatos con estado RECEIVED en SQL Server y envía un mensaje a RabbitMQ. Retorna 202 Accepted inmediatamente.

**Procesamiento (Worker):** Un servicio en segundo plano (BackgroundService en Infraestructura) escucha la cola.

**Ejecución:** El Worker toma el mensaje, decodifica el archivo, lo almacena físicamente y actualiza el estado en la BD a AVAILABLE.

**Resiliencia:** Si falla el almacenamiento, Polly se encarga de reintentar la operación antes de marcarlo como fallido.

---

## 🌿 Estrategia de Git (Gitflow)

Se utilizo un flujo de trabajo estructurado para garantizar la calidad del código.

**main:** Rama de Producción. Código estable y listo para desplegar.

**dev:** Rama de Integración. Aquí se une todo el trabajo desarrollado.

**testing:** Rama para QA. Despliegues para pruebas.

**feature/nombre-funcionalidad:** Ramas temporales para cada tarea del backlog.

**Reglas:** No esta permitido hacer commit directo a main o dev. Siempre usar Pull Requests.

---

## 📘 Guía de Estándares de Desarrollo

## 1\. Principios

* **SOLID:** Respeta rigurosamente la Inyección de Dependencias e Inversión de Control.

* **Async/Await:** Todo el I/O (Base de datos, Archivos, Mensajería) debe es asíncrono.

* **Fail Fast:** Se valida los inputs al inicio del método (Cláusulas de Guarda).

## 2\. Convenciones de Nombres (Naming Conventions)

| Elemento | Convención | Ejemplo |
| :--- | :--- | :--- |
| **Clase Async / Método** | `PascalCase...Async` | `GetDocumentByIdAsync`, `UploadFileAsync` |
| **Interfaz** | `IPascalCase` | `IDocumentRepository`, `IMessageProducer` |
| **Variable Local** | `camelCase` | `documentsUpload` |
| **Parámetro** | `camelCase` | `userId`, `idDocument` |
| **Campo Privado** | `_camelCase` | `_dbContext`, `_logger` |
| **Constante** | `SCREAMING_SNAKE_CASE` | `MAX_DOCUMENTS`, `PAGES` |
| **DTO** | `Accion + Entidad + Dto` | `CreateDocumentDto`, `DocumentResponseDto` |
| **Controller** | `Plural + Controller` | `DocumentsController` |


## 3\. Documentación de Código

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

## 4\. Buenas prácticas .NET aplicadas

1.  **Inyección de Dependencias:** Siempre por constructor. Nunca usar `new Service()`.
2.  **Async/Await:** Todo I/O (Base de datos, API calls) es asíncrono. Evita `.Result` o `.Wait()`.
3.  **Manejo de Excepciones:** Se evita el uso de `try/catch` vacíos. Dejando que las excepciones suban al Middleware global a menos que se puedan corregir el error en el momento.
4.  **LINQ:** Uso preferido de LINQ (`Where`, `Select`) sobre bucles `foreach` manuales para transformaciones de listas.