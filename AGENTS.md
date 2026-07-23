# Rentify Backend

## 1. Descripción del proyecto

Rentify es un backend SaaS multi-tenant para rent cars y gestión de flotas. La solución actual está construida en .NET 8 con ASP.NET Core, MediatR, FluentValidation, Entity Framework Core, PostgreSQL, ASP.NET Core Identity, Hangfire y Cloudinary.

El proyecto maneja onboarding de tenants, autenticación JWT, suscripciones, perfiles de empresa, vehículos, clientes, reservas, pagos, plantillas de correo y procesamiento de outbox.

## 2. Mapa de la solución

Solución activa: `Rentify.slnx`

- `Rentify.Backend.Core.Domain`
  - Ruta: `Rentify.Backend/Rentify.Backend.Core.Domain`
  - Responsabilidad: entidades, enums, value objects, invariantes y constantes compartidas del dominio.
  - Dependencias: ninguna referencia a otros proyectos.

- `Rentify.Backend.Core.Application`
  - Ruta: `Rentify.Backend/Rentify.Backend.Core.Application`
  - Responsabilidad: commands, queries, handlers, validators, DTOs, contratos y servicios de aplicación.
  - Dependencias: `Rentify.Backend.Core.Domain`.

- `Rentify.Backend.Infrastructure.Identity`
  - Ruta: `Rentify.Backend/Rentify.Backend.Infrastructure.Identity`
  - Responsabilidad: `IdentityContext`, `ApplicationUser`, JWT, autenticación, seeds y migraciones de Identity.
  - Dependencias: `Rentify.Backend.Core.Application`, `Rentify.Backend.Core.Domain`.

- `Rentify.Backend.Infrastructure.Persistence`
  - Ruta: `Rentify.Backend/Rentify.Backend.Infrastructure.Persistence`
  - Responsabilidad: `RentifyContext`, configuraciones EF Core, repositorios, UnitOfWork y migraciones del dominio.
  - Dependencias: `Rentify.Backend.Core.Application`, `Rentify.Backend.Core.Domain`.

- `Rentify.Backend.Infrastructure.Shared`
  - Ruta: `Rentify.Backend/Rentify.Backend.Infrastructure.Shared`
  - Responsabilidad: integraciones técnicas compartidas: Cloudinary, correo, Hangfire y outbox.
  - Dependencias: `Rentify.Backend.Core.Application`, `Rentify.Backend.Core.Domain`, `Rentify.Backend.Infrastructure.Persistence`.

- `Rentify.Backend.Presentation.WebApi`
  - Ruta: `Rentify.Backend/Rentify.Backend.Presentation.WebApi`
  - Responsabilidad: `Program.cs`, composición de DI, middleware, `CurrentRequestContext`, endpoints HTTP y arranque de la API.
  - Dependencias: `Rentify.Backend.Infrastructure.Identity`, `Rentify.Backend.Infrastructure.Persistence`, `Rentify.Backend.Infrastructure.Shared`.

## 3. Dirección de dependencias

Dirección real en el repositorio:

- `Core.Domain` no depende de ninguna otra capa.
- `Core.Application` depende de `Core.Domain`.
- `Infrastructure.Identity` depende de `Core.Application` y `Core.Domain`.
- `Infrastructure.Persistence` depende de `Core.Application` y `Core.Domain`.
- `Infrastructure.Shared` depende de `Core.Application`, `Core.Domain` y `Infrastructure.Persistence`.
- `Presentation.WebApi` depende directamente de los proyectos de infraestructura porque registra la composición de la aplicación desde `Program.cs`.

Evitar dependencias circulares. No agregar dependencias desde Domain hacia Infrastructure o Presentation.

## 4. Convenciones de nombres

- Namespace raíz esperado para proyectos normalizados: `Rentify.Backend.*`.
- La marca correcta es `Rentify`. No reintroducir el typo histórico de la marca.
- La solución actual es `Rentify.slnx`.
- Los proyectos de infraestructura deben usar la grafía normalizada `Infrastructure`.
- El proyecto compartido actual es `Rentify.Backend.Infrastructure.Shared`; no crear `Rentify.Backend.Shared` como proyecto aparte.

Convenciones ya presentes en el código:

- `Modules/<Modulo>/Commands/...` y `Modules/<Modulo>/Queries/...`.
- DTOs dentro de `Dtos`.
- Repositorios dentro de `Infrastructure.Persistence/Repositories`.
- Configuraciones EF dentro de `Infrastructure.Persistence/EntityConfiguration`.

Excepciones históricas que siguen existiendo en rutas internas y deben respetarse salvo refactor explícito:

- `Modules/Secutiry`
- `Repositories/Vehicules`
- `ResultReponse`

No crear variantes “corregidas” en paralelo sin migrar todos los consumidores.

## 5. Comandos principales

Comandos validados en este repositorio:

```powershell
dotnet restore .\Rentify.slnx
dotnet build .\Rentify.slnx --configuration Release --no-restore
dotnet test .\Rentify.slnx --configuration Release --no-build
dotnet format .\Rentify.slnx --verify-no-changes
dotnet run --project .\Rentify.Backend\Rentify.Backend.Presentation.WebApi\Rentify.Backend.Presentation.WebApi.csproj
```

Entity Framework Core:

```powershell
dotnet ef migrations list `
  --project .\Rentify.Backend\Rentify.Backend.Infrastructure.Persistence\Rentify.Backend.Infrastructure.Persistence.csproj `
  --startup-project .\Rentify.Backend\Rentify.Backend.Presentation.WebApi\Rentify.Backend.Presentation.WebApi.csproj `
  --context RentifyContext

dotnet ef migrations list `
  --project .\Rentify.Backend\Rentify.Backend.Infrastructure.Identity\Rentify.Backend.Infrastructure.Identity.csproj `
  --startup-project .\Rentify.Backend\Rentify.Backend.Presentation.WebApi\Rentify.Backend.Presentation.WebApi.csproj `
  --context IdentityContext
```

No hay Dockerfile ni workflows versionados de CI en el baseline actual.

## 6. Arquitectura de aplicación

Patrones verificados:

- CQRS con MediatR.
- FluentValidation registrado desde `Core.Application`.
- Minimal APIs.
- `ResultReponse<T>` como contrato de respuesta de aplicación.
- `PaginatedResponse<T>` para listados paginados.
- Repositorios por módulo.
- `IUnitOfWork`.
- Outbox con Hangfire para procesamiento recurrente.

Estado actual importante:

- La mayor parte de endpoints vive en `Presentation.WebApi/Endpoints`.
- Aún existen endpoints dentro de `Core.Application` (`AuthEndpoints`, `SubscriptionEndpoints`, varios endpoints de Vehicles/Payments/Dashboard, etc.). Eso es estado actual del repositorio; no duplicar ni mover endpoints sin una tarea específica de refactor.

## 7. Multi-tenancy y seguridad

Hechos verificados:

- `CurrentRequestContext` está en `Presentation.WebApi/Services/CurrentRequestContext.cs`.
- La interfaz `ICurrentRequestContext` vive en `Core.Application/Modules/Shared/Context`.
- `TenantId` se obtiene desde claims JWT; `CurrentRequestContext` soporta `ApplicationClaimTypes.TenantId`, `TenantId` y `tenant_id`.
- `UserId` soporta `ApplicationClaimTypes.UserId`, `ClaimTypes.NameIdentifier`, `sub`.
- `Roles` se obtienen de claims de rol y `IsSuperAdmin` compara contra `ApplicationRoles.SuperAdmin`.
- La policy `AuthorizationPolicies.RequiredRoles` se registra en `Infrastructure.Identity` y hoy exige `Owner` y `Secretary`.
- Varias operaciones filtran por `TenantId` en repositorios y handlers.

Regla operativa para nuevos cambios:

- No confiar en `TenantId` enviado por el cliente cuando el endpoint ya puede resolverlo desde `ICurrentRequestContext`.
- Si tocas endpoints legacy que todavía reciben `tenantId` por ruta, no cambies contratos sin pedido explícito; documenta la inconsistencia y mantén compatibilidad.

## 8. Persistencia y EF Core

- Proveedor: PostgreSQL con `Npgsql`.
- Contexto principal: `Infrastructure.Persistence/Context/RentifyContext.cs`.
- Contexto de identidad: `Infrastructure.Identity/Context/IdentityContext.cs`.
- Proyecto de migraciones del dominio: `Rentify.Backend.Infrastructure.Persistence`.
- Proyecto de migraciones de identidad: `Rentify.Backend.Infrastructure.Identity`.
- Configuraciones EF: `Infrastructure.Persistence/EntityConfiguration/*`.
- El código usa `IsDeleted` e `IsActive` como flags de soft delete/estado en varias entidades.
- `ReservationConfiguration` usa `timestamp with time zone` para fechas de reserva.
- `VehicleUnavailableDate` usa `DateOnly`.
- `MigrationsAssembly` se resuelve con `typeof(RentifyContext).Assembly.FullName` y `typeof(IdentityContext).Assembly.FullName`.

No crear migraciones nuevas por cambios solo de namespace, solución o nombre de proyecto.

## 9. Reglas para Reservations

Reglas verificadas en `Core.Domain/Entities/Reservations/Reservation.cs` y handlers:

- Estados actuales: `Pending`, `Approved`, `Rejected`, `Cancelled`, `ConvertedToRental`, `Expired`.
- Una reserva nueva inicia en `Pending`.
- Solo reservas `Pending` pueden actualizarse, aprobarse o rechazarse.
- Solo reservas `Pending` o `Approved` pueden cancelarse.
- Solo reservas `Approved` pueden convertirse a rental.
- Solo reservas `Pending`, `Rejected` o `Cancelled` pueden eliminarse.
- `ExpectedReturnDateTime` debe ser mayor que `DeliveryDateTime`.
- `Quantity` se calcula con `Reservation.CalculateQuantity(...)`.
- El cálculo usa diferencia entre `expectedReturn.Date` y `delivery.Date`, con redondeo hacia arriba:
  - `Daily`: `ceil(days)`
  - `Weekly`: `ceil(days / 7)`
  - `Monthly`: `ceil(days / 30)`
- `RentalAmount = UnitRate * Quantity`.
- `TotalAmount = RentalAmount + SecurityDeposit + DeliveryFee + ReturnFee - DiscountAmount`.
- Al aprobar, además del cambio de estado, se crea un mensaje de outbox y un `VehicleUnavailableDate`.

## 10. Disponibilidad de vehículos

Hechos verificados:

- La entidad de bloqueo es `VehicleUnavailableDate`.
- Se persiste en la tabla `VehicleUnavailableDates`.
- La lógica de solapamiento de `VehicleUnavailableDate.Overlaps` es inclusiva:

```text
existing.StartDate <= requestedEndDate
&& requestedStartDate <= existing.EndDate
```

- Eso representa un intervalo cerrado en ambos extremos para `DateOnly`, no `[start, end)`.
- `Vehicle.AddUnavailableDate(...)` rechaza bloques que se solapen con otros bloques no borrados.
- `Vehicle.IsAvailableFor(...)` exige `Status == Available` y ausencia de bloques solapados.
- `ReservationRepository.HasApprovedOverlapAsync(...)` usa solapamiento de `DateTime` tipo half-open:

```text
requestedStart < existing.ExpectedReturnDateTime
&& requestedEnd > existing.DeliveryDateTime
```

- Endpoint verificado para consultar periodos no disponibles:
  - `GET /api/v1/vehicles/{vehicleId:guid}/unavailable-periods`

## 11. Infrastructure.Shared

Proyecto real:

- Nombre: `Rentify.Backend.Infrastructure.Shared`
- Ruta: `Rentify.Backend/Rentify.Backend.Infrastructure.Shared`

Responsabilidad actual:

- Registro de `Hangfire`.
- Procesamiento de outbox (`OutboxProcessor`, `OutboxService`, handlers).
- Integraciones de correo.
- Integraciones de almacenamiento con Cloudinary.
- Carga de `.env` desde `EnvFileLoader`.

Qué sí puede ir aquí:

- Integraciones técnicas reutilizables.
- Adaptadores a servicios externos.
- Jobs técnicos.
- Implementaciones de contratos transversales de infraestructura.

Qué no debería ir aquí:

- Reglas de negocio del dominio.
- Endpoints HTTP.
- DTOs de API.
- Dependencias hacia `Presentation.WebApi`.

Referencias actuales:

- `Presentation.WebApi` lo referencia.
- `Infrastructure.Shared` referencia `Infrastructure.Persistence`; eso hoy es necesario por el outbox. Evitar agregar dependencias inversas desde `Infrastructure.Persistence` hacia `Infrastructure.Shared`.

## 12. Manejo de errores

- El patrón de respuesta principal es `ResultReponse<T>`.
- Los errores HTTP también se manejan con `ApiException` y middleware en `Presentation.WebApi/Middlewares/ErrorHandlerMiddleware.cs`.
- `Infrastructure.Identity/ServiceRegistration.cs` devuelve errores JSON controlados para fallos de autenticación JWT.
- No introducir respuestas paralelas incompatibles con `ResultReponse<T>` sin una tarea específica.

## 13. Pruebas

- No hay proyectos de prueba en la solución actual.
- `dotnet test .\Rentify.slnx --configuration Release --no-build` devuelve éxito, pero no ejecuta suites porque no hay proyectos test cargados.
- Si agregas pruebas en el futuro, deben entrar explícitamente en `Rentify.slnx`.

## 14. Flujo para agregar una nueva funcionalidad

Checklist pragmático según la estructura actual:

1. Confirmar si la lógica pertenece a Domain, Application, Infrastructure o Presentation.
2. Reutilizar módulo existente antes de crear uno nuevo.
3. Agregar/ajustar entidades o invariantes en Domain solo si la regla es realmente de negocio.
4. Crear command/query, handler, validator y DTOs en Application.
5. Agregar contratos en Application y sus implementaciones en infraestructura si hace falta persistencia o integración externa.
6. Exponer el caso de uso desde `Presentation.WebApi/Endpoints` o, si estás tocando una zona legacy, seguir el patrón actual de ese módulo sin duplicar endpoints.
7. Validar referencias DI en `ServiceRegistration`/`Program.cs`.
8. Ejecutar restore, build, test y format.

## 15. Reglas para agentes de código

- Leer este archivo antes de modificar.
- Buscar implementación existente antes de crear servicios, helpers, DTOs o endpoints nuevos.
- No duplicar `ServiceRegistration`, repositorios ni contratos.
- Mantener `TenantId` aislado por tenant.
- Respetar `CancellationToken`.
- No hardcodear secretos.
- No cambiar contratos HTTP o DTOs públicos sin solicitud explícita.
- No crear migraciones destructivas por renombrados o limpieza.
- No declarar éxito si fallan build o validaciones.
- Reportar archivos modificados y cualquier error preexistente.
- Evitar refactors masivos fuera del alcance.
- Mantener la marca `Rentify`.
- Mantener `Infrastructure.Shared` dentro de infraestructura; no crear un shared global paralelo.

## 16. Git

Patrones observados:

- Ramas presentes en el repo original: `main`, `dev`, `feature/*`.
- Mensajes recientes mezclan Conventional Commits en inglés (`feat(...)`, `fix(...)`, `refactor(...)`, `style(...)`) con algún commit legacy en español.

Reglas prácticas:

- Preferir Conventional Commits.
- No versionar `.env`, secretos, `bin`, `obj`, `.vs`, `.idea` ni artefactos temporales.
- Revisar `git status --short`, `git diff --stat` y `git diff` antes de commit.
- No reintroducir typos históricos en la marca ni en los nombres de infraestructura.
