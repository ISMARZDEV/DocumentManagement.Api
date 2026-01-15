# 🧪 Pruebas Unitarias - Document Management API

## 📚 Descripción

Pruebas unitarias para gestión documental con **Clean Architecture** y **.NET 8**.

## 📋 Casos de Prueba Detallados

Consulta los test cases completos de pruebas unitarias e integración en el GitHub Project:

🔗 **[Ver Test Cases en GitHub Project](https://github.com/users/ISMARZDEV/projects/4/views/1?visibleFields=%5B%22Title%22%2C%22Sub-issues+progress%22%2C%22Labels%22%2C%22Status%22%2C%22Linked+pull+requests%22%2C%22Parent+issue%22%2C%22Repository%22%2C%22Assignees%22%5D&pane=issue&itemId=149707812&issue=ISMARZDEV%7CDocumentManagement.Api%7C19)**

<img width="3024" height="1964" alt="Image" src="https://github.com/user-attachments/assets/b58af903-2075-4342-a8ec-7a4a2c0de2dc" />

## 🛠️ Tecnologías

- **xUnit** - Framework de pruebas
- **Moq** - Mocking de dependencias
- **FluentAssertions** - Assertions fluidas
- **EF Core InMemory** - BD en memoria

## ✅ Pruebas Implementadas (13 tests)

### CreateDocumentCommandHandler (12 tests)

**Grupo 1: Validación de Roles** (4 tests)
- ✅ Cliente sin CustomerId → Se usa UserId automáticamente
- ✅ Operador sin CustomerId → BadRequestException
- ✅ Operador con CustomerId → Documento creado
- ✅ Rol inválido → UnauthorizedAccessException

**Grupo 2: Procesamiento de Archivos** (3 tests)
- ✅ Guardar archivo Base64 en staging
- ✅ Base64 inválido → BadRequestException
- ✅ Calcular tamaño correctamente

**Grupo 3: Creación de Document** (3 tests)
- ✅ Propiedades iniciales correctas (Status=RECEIVED, FileUrl=null)
- ✅ CreatedAt con DateTime.UtcNow
- ✅ Cada documento recibe Guid único

**Grupo 4: Hangfire** (2 tests)
- ✅ Encolar job y asignar CorrelationId automáticamente
- ✅ Preservar CorrelationId si ya existe

### UserService (1 test)
- ✅ **LoginAsync** - Usuario inicia sesión y recibe JWT válido

## 🚀 Comandos

```bash
# Todas las pruebas
dotnet test

# Project específico
dotnet test test/Bhd.Infrastructure.UnitTests/Bhd.Infrastructure.UnitTests.csproj

# Con verbosidad
dotnet test --verbosity normal

# Con cobertura
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
```

## 📊 Resultados

```
✅ Total: 13 tests
✅ Correctas: 13
❌ Fallidas: 0
⏱️ Tiempo: ~50ms
```

## 🎯 Patrones Utilizados

- **AAA Pattern**: Arrange-Act-Assert
- **Test Fixtures**: `IClassFixture<ApplicationDbContextFixture>`
- **Builders**: `TestDataBuilder` para datos de prueba
- **Mocking**: Moq para aislar dependencias
- **In-Memory DB**: EF Core para pruebas rápidas

## 📝 Nomenclatura

`Should_[ExpectedBehavior]_When[Condition]`

---

**Framework**: .NET 8.0 | **Fecha**: 15 de enero de 2026