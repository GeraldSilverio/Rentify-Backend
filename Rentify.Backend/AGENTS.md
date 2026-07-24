# AGENTS.md — Rentify Backend

## 1. Propósito

Este repositorio contiene el backend de Rentify, una plataforma SaaS multi-tenant para rent cars y flotas de vehículos.

Trabaja sobre el código existente, conserva los contratos públicos y respeta la arquitectura actual. Antes de modificar archivos, inspecciona los patrones ya implementados en módulos equivalentes, especialmente `Vehicles`, `Tenants`, `Security` y `Subscriptions`.

No generes pseudocódigo, archivos incompletos, comentarios `TODO` ni implementaciones de ejemplo. Toda modificación debe quedar compilable y lista para ejecutarse.

---

## 2. Estructura de la solución

La solución está organizada en las siguientes capas:

```text
Core/
├── Rentify.Backend.Core.Application
└── Rentify.Backend.Core.Domain

Infrastructure/
├── Rentify.Backend.Infrastructure.Identity
├── Rentify.Backend.Infrastructure.Persistence
└── Rentify.Backend.Infrastructure.Shared

Presentation/
└── Rentify.Backend.Presentation.WebApi
```

### Responsabilidad de cada capa

#### `Rentify.Backend.Core.Domain`

Contiene:

- Entidades.
- Value Objects.
- Enumeraciones.
- Reglas e invariantes del dominio.
- Eventos de dominio, cuando apliquen.
- Comportamiento propio del negocio.

No debe depender de Application, Infrastructure ni Presentation.

#### `Rentify.Backend.Core.Application`

Contiene:

- Commands.
- Queries.
- Requests.
- Handlers.
- Validators.
- DTOs.
- Contracts e interfaces.
- Casos de uso.
- Servicios de aplicación.
- Abstracciones compartidas.

No debe contener endpoints HTTP, `HttpContext`, `IResult`, `Results`, `StatusCodes`, route groups ni detalles de ASP.NET Core relacionados con la exposición web.

#### `Rentify.Backend.Infrastructure.Identity`

Contiene:

- ASP.NET Core Identity.
- Entidades de identidad.
- Configuraciones de Identity.
- Servicios relacionados con autenticación y usuarios.
- Migraciones y seeds de Identity.

#### `Rentify.Backend.Infrastructure.Persistence`

Contiene:

- `DbContext` del negocio.
- Configuraciones de Entity Framework Core.
- Repositorios.
- Migraciones.
- Persistencia de entidades del dominio.
- Implementaciones de contratos de acceso a datos.

#### `Rentify.Backend.Infrastructure.Shared`

Contiene implementaciones técnicas compartidas, por ejemplo:

- Cloudinary.
- Resend.
- Hangfire.
- Almacenamiento de archivos.
- Servicios externos.
- Integraciones técnicas reutilizables.

#### `Rentify.Backend.Presentation.WebApi`

Contiene:

- Minimal API endpoints.
- Configuración de rutas.
- Middlewares.
- Servicios dependientes de `HttpContext`.
- Implementaciones que leen claims HTTP.
- Configuración de autenticación/autorización.
- Registro y mapeo final de endpoints.
- `Program.cs`.

---

## 3. Regla de dependencias

Respeta este flujo:

```text
Presentation ───────► Application
Infrastructure ─────► Application
Application ────────► Domain
```

Reglas obligatorias:

- Domain no conoce ninguna otra capa.
- Application no depende de Presentation.
- Application no contiene endpoints.
- Presentation puede usar Application y registrar implementaciones.
- Infrastructure implementa contratos definidos en Application.
- No agregues referencias circulares.
- No muevas lógica de negocio a Presentation.

---

## 4. Organización de módulos en Application

Se utiliza una organización modular y vertical por caso de uso.

Ejemplo objetivo:

```text
Modules/
└── Customers/
    ├── Commands/
    │   ├── CreateCustomer/
    │   │   ├── CreateCustomerCommand.cs
    │   │   ├── CreateCustomerHandler.cs
    │   │   ├── CreateCustomerRequest.cs
    │   │   └── CreateCustomerValidator.cs
    │   ├── UpdateCustomer/
    │   │   ├── UpdateCustomerCommand.cs
    │   │   ├── UpdateCustomerHandler.cs
    │   │   ├── UpdateCustomerRequest.cs
    │   │   └── UpdateCustomerValidator.cs
    │   ├── DeleteCustomer/
    │   │   ├── DeleteCustomerCommand.cs
    │   │   ├── DeleteCustomerHandler.cs
    │   │   └── DeleteCustomerValidator.cs
    │   └── UploadCustomerDocument/
    │       ├── UploadCustomerDocumentCommand.cs
    │       ├── UploadCustomerDocumentHandler.cs
    │       └── UploadCustomerDocumentValidator.cs
    ├── Queries/
    │   ├── GetCustomerById/
    │   │   ├── GetCustomerByIdQuery.cs
    │   │   └── GetCustomerByIdHandler.cs
    │   └── SearchCustomers/
    │       ├── SearchCustomersQuery.cs
    │       └── SearchCustomersHandler.cs
    ├── Contracts/
    ├── Dtos/
    └── Implementations/
```

Dentro de Application no deben existir archivos como:

```text
CreateCustomerEndpoint.cs
UpdateCustomerEndpoint.cs
DeleteCustomerEndpoint.cs
SearchCustomersEndpoint.cs
UploadCustomerDocumentEndpoint.cs
CustomerEndpoints.cs
```

Los endpoints pertenecen a Presentation.

---

## 5. Organización de endpoints en Presentation

Los endpoints deben agruparse por módulo dentro de:

```text
Rentify.Backend.Presentation.WebApi/
└── Endpoints/
    ├── Admin/
    ├── Tenants/
    ├── Vehicles/
    │   ├── VehiclesEndpoints.cs
    │   └── VehicleCatalogEndpoints.cs
    └── Customers/
        ├── CustomersEndpoints.cs
        └── CustomerDocumentsEndpoints.cs
```

### Reglas para endpoints

- Usa clases estáticas de extensión para mapear rutas.
- Agrupa las operaciones relacionadas en archivos coherentes.
- Usa nombres plurales: `CustomersEndpoints`, `VehiclesEndpoints`.
- Mantén los handlers y validadores en Application.
- El endpoint solo debe:
  1. Recibir/parsing de datos HTTP.
  2. Invocar el caso de uso correspondiente.
  3. Traducir el resultado a una respuesta HTTP.
- No coloques lógica de negocio en lambdas de endpoints.
- No accedas directamente al `DbContext` desde Presentation.
- No dupliques validaciones del Application.
- Conserva rutas, contratos, status codes, autorización, tags y metadatos OpenAPI existentes, salvo que la tarea indique explícitamente cambiarlos.
- Para cargas de archivos, el binding HTTP permanece en Presentation y la operación se delega a Application mediante su command correspondiente.
- Registra cada grupo desde `Program.cs` o desde el mecanismo central ya utilizado por el proyecto.

Ejemplo de estilo esperado:

```csharp
namespace Rentify.Backend.Presentation.WebApi.Endpoints.Customers;

public static class CustomersEndpoints
{
    public static IEndpointRouteBuilder MapCustomersEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder group = endpoints
            .MapGroup("/api/v1/customers")
            .WithTags("Customers")
            .RequireAuthorization();

        // MapGet, MapPost, MapPut y MapDelete.
        // Cada operación delega su ejecución al caso de uso de Application.

        return endpoints;
    }
}
```

No copies este ejemplo sin revisar las firmas y abstracciones reales del proyecto.

---

## 6. Contexto único del usuario autenticado

Debe existir un solo contrato para consultar los datos del usuario y del tenant actual.

### Nombre requerido

```text
ICurrentRequestContext
CurrentRequestContext
```

### Ubicación

```text
Application:
Rentify.Backend.Core.Application/
└── Modules/
    └── Shared/
        └── Context/
            └── ICurrentRequestContext.cs

Presentation:
Rentify.Backend.Presentation.WebApi/
└── Services/
    └── CurrentRequestContext.cs
```

### Servicios que reemplaza

Elimina y sustituye:

```text
ICurrentUserService
ICurrentTenantService
CurrentUserService
CurrentTenantService
```

Actualiza todos los consumidores para que inyecten solamente:

```csharp
ICurrentRequestContext
```

### Responsabilidades mínimas

El contrato unificado debe exponer, de acuerdo con las necesidades reales del proyecto:

```csharp
bool IsAuthenticated { get; }
Guid UserId { get; }
Guid TenantId { get; }
bool HasTenant { get; }
string? UserName { get; }
string? Email { get; }
IReadOnlyCollection<string> Roles { get; }
bool IsSuperAdmin { get; }
string ModifiedBy { get; }
bool TryGetTenantId(out Guid tenantId);
```

`UserId` y `TenantId` son valores requeridos. Cuando el claim esté ausente, sea inválido o contenga `Guid.Empty`, deben lanzar `ApiException` con estado HTTP 401.

`HasTenant` y `TryGetTenantId` no deben lanzar excepciones.

### Claims soportados

Para `UserId`, conserva compatibilidad con:

```text
ApplicationClaimTypes.UserId
ClaimTypes.NameIdentifier
JwtRegisteredClaimNames.Sub
"sub"
```

Para `TenantId`, conserva compatibilidad con:

```text
ApplicationClaimTypes.TenantId
"TenantId"
"tenant_id"
```

Para nombre de usuario:

```text
ClaimTypes.Name
JwtRegisteredClaimNames.UniqueName
"preferred_username"
"name"
```

Para correo:

```text
ClaimTypes.Email
JwtRegisteredClaimNames.Email
"email"
```

Para roles:

```text
ApplicationClaimTypes.Roles
ClaimTypes.Role
"role"
"roles"
```

Los roles deben:

- Separarse por coma cuando un claim contenga varios valores.
- Eliminar espacios.
- Ignorar valores vacíos.
- Eliminar duplicados sin distinguir mayúsculas de minúsculas.

`IsSuperAdmin` debe comparar contra `ApplicationRoles.SuperAdmin` sin distinguir mayúsculas de minúsculas.

`ModifiedBy` debe usar este orden:

```text
UserName → Email → UserId
```

### Implementación

- La interfaz permanece en Application.
- La implementación permanece en Presentation porque depende de `IHttpContextAccessor`.
- Centraliza la lectura de `ClaimsPrincipal`.
- No repitas la lógica de resolución de claims en varias propiedades o servicios.
- No uses `HttpContext` directamente desde handlers de Application.
- Registra el servicio como `Scoped`.
- Registra `IHttpContextAccessor` una sola vez.
- Elimina registros de DI obsoletos.

Registro esperado:

```csharp
services.AddHttpContextAccessor();
services.AddScoped<ICurrentRequestContext, CurrentRequestContext>();
```

Adapta el lugar exacto del registro a la composición actual del proyecto.

---



## 7. Reglas del módulo Customers

### Identificación y licencia de conducir

En Rentify, el número de licencia de conducir no forma parte de los datos estructurados del cliente.

Reglas obligatorias:

- No agregues ni mantengas propiedades como `LicenseNumber`.
- No agregues ni mantengas propiedades como `LicenseExpirationDate`.
- No recibas esos valores en requests HTTP.
- No los incluyas en commands, validators, DTOs ni responses.
- No los persistas como columnas del cliente.
- La identificación personal del cliente debe usar el campo de cédula o identificación ya existente en el modelo.
- No crees un nuevo campo de cédula si el modelo ya posee `IdentificationNumber`, `DocumentNumber`, `NationalId` o un equivalente.
- No uses el número de cédula como si fuera un `LicenseNumber`.
- La licencia se administra únicamente como imagen o documento asociado al cliente.
- Usa el tipo de documento existente para licencia de conducir. Si no existe, agrega un valor claro al enum o catálogo de tipos de documentos sin almacenar número ni fecha de vencimiento.
- Las imágenes y documentos deben respetar el almacenamiento, seguridad y aislamiento por tenant ya establecidos.

Si `LicenseNumber` o `LicenseExpirationDate` ya están persistidos:

1. Elimínalos de la entidad y su configuración.
2. Elimínalos de requests, commands, handlers, validators, DTOs y mapeos.
3. Crea la migración de Entity Framework Core correspondiente en el proyecto correcto.
4. No elimines ni alteres la tabla de documentos del cliente.
5. No generes migraciones vacías.

### Consulta detallada del cliente

Debe existir una consulta por identificador:

```text
GET /api/v1/customers/{customerId:guid}
```

La consulta debe:

- Obtener `TenantId` desde `ICurrentRequestContext`.
- Buscar por `customerId` y `TenantId`.
- No permitir acceso cross-tenant.
- Devolver 404 cuando el cliente no exista dentro del tenant autenticado.
- Devolver los datos principales del cliente.
- Incluir la colección de documentos asociados.
- Incluir la imagen de licencia dentro de esa colección, identificada por su tipo de documento.
- Proyectar directamente a DTO cuando sea posible.
- Usar `AsNoTracking`.
- Evitar consultas N+1.
- No devolver entidades de Entity Framework directamente.
- No exponer `PublicId` u otros datos técnicos sensibles si los contratos actuales no los exponen.

Estructura esperada:

```text
Modules/Customers/Queries/GetCustomerById/
├── GetCustomerByIdQuery.cs
└── GetCustomerByIdHandler.cs
```

Usa un DTO detallado y reutiliza el DTO de documentos existente cuando sea apropiado.

---

## 8. Paginación obligatoria

Todo endpoint de lectura que devuelva una colección debe estar paginado.

Esto aplica, entre otros, a operaciones nombradas:

```text
GetAll
GetList
List
Search
GetCustomers
GetVehicles
GetReservations
GetPayments
```

No devuelvas colecciones de negocio potencialmente ilimitadas sin paginación.

### Parámetros

Usa el contrato compartido de paginación existente en el proyecto. Si ya existen tipos como:

```text
PaginationRequest
PaginationParameters
PagedRequest
PagedResult<T>
PagedResponse<T>
```

reutilízalos. No crees una segunda implementación equivalente.

Cuando el proyecto no tenga un contrato compartido, crea uno en `Modules/Shared` y úsalo de manera consistente.

Como mínimo, la solicitud debe soportar:

```text
pageNumber
pageSize
```

Convenciones:

- `pageNumber` inicia en 1.
- Usa valores predeterminados razonables siguiendo el patrón existente del proyecto.
- Define un máximo de `pageSize` para evitar consultas abusivas.
- Valida que `pageNumber` y `pageSize` sean mayores que cero.
- Mantén filtros y término de búsqueda junto con la paginación.
- No uses paginación basada en memoria.

### Implementación en consultas

La consulta debe:

1. Aplicar filtro por `TenantId`.
2. Aplicar filtros funcionales.
3. Aplicar un `OrderBy` estable.
4. Calcular `TotalCount`.
5. Aplicar `Skip` y `Take`.
6. Proyectar a DTO.
7. Usar `AsNoTracking` para lecturas.

Nunca ejecutes `ToListAsync` antes de aplicar `Skip` y `Take`.

La respuesta paginada debe seguir el tipo compartido existente e incluir los metadatos que ya use el proyecto, por ejemplo:

```text
Items
TotalCount
PageNumber
PageSize
TotalPages
HasPreviousPage
HasNextPage
```

No inventes una forma de respuesta diferente para cada módulo.

### Excepciones

Los catálogos pequeños, cerrados y explícitamente definidos como catálogos pueden conservar el patrón ya existente. Cualquier colección de entidades de negocio debe paginarse.

---

## 9. Convención obligatoria de rutas multi-tenant

El `TenantId` nunca debe formar parte de las rutas de los módulos operativos del tenant autenticado.

No uses rutas como:

```text
/api/v1/tenants/{tenantId}/customers
/api/v1/tenants/{tenantId}/vehicles
/api/v1/tenants/{tenantId}/reservations
```

Usa rutas como:

```text
/api/v1/customers
/api/v1/vehicles
/api/v1/reservations
```

El `TenantId` debe obtenerse exclusivamente desde el JWT mediante:

```csharp
ICurrentRequestContext.TenantId
```

Reglas obligatorias:

- No recibas `tenantId` por route, query string, body, header personalizado ni formulario.
- No confíes en un `TenantId` enviado por el cliente.
- No permitas que el consumidor seleccione el tenant sobre el cual ejecutará la operación.
- Todos los commands y queries tenant-aware deben recibir el `TenantId` resuelto desde `ICurrentRequestContext`.
- Toda consulta o modificación debe filtrar y validar la pertenencia al tenant autenticado.
- Las operaciones globales exclusivas de SuperAdmin deben vivir en endpoints administrativos separados.
- `CreatedBy` y `ModifiedBy` tampoco deben recibirse desde el cliente cuando puedan obtenerse desde `ICurrentRequestContext.ModifiedBy`.
- Elimina `CreatedBy` y `ModifiedBy` de requests HTTP cuando solo se utilicen como campos de auditoría.
- No cambies rutas públicas globales o administrativas que legítimamente requieran identificar otro tenant.

### Grupos de endpoints

Cada módulo debe definir un `RouteGroupBuilder`.

Ejemplo esperado:

```csharp
RouteGroupBuilder group = app
    .MapGroup("/api/v1/customers")
    .WithTags("Customers")
    .RequireAuthorization(AuthorizationPolicies.RequiredRoles);
```

Después, mapea rutas relativas:

```csharp
group.MapPost("/", ...);
group.MapPut("/{customerId:guid}", ...);
group.MapDelete("/{customerId:guid}", ...);
group.MapGet("/", ...);
```

Usa el nombre real de la constante de policy definida por el proyecto. No hardcodees el nombre de la policy en varios archivos.

### Policy `RequiredRoles`

Debe existir una policy nombrada `RequiredRoles` para los módulos operativos de Rentify.

La policy debe:

- Requerir un usuario autenticado.
- Incluir como mínimo los roles `Owner` y `Secretary`.
- Incluir los demás roles operativos ya definidos en `ApplicationRoles` que legítimamente administren clientes.
- Usar constantes de `ApplicationRoles`; no usar strings repetidos.
- Mantener `SuperAdmin` únicamente si las reglas actuales del proyecto permiten que opere dentro de endpoints tenant-aware.
- Configurarse una sola vez en la composición de autenticación/autorización.

Ejemplo conceptual:

```csharp
options.AddPolicy(
    AuthorizationPolicies.RequiredRoles,
    policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole(
            ApplicationRoles.Owner,
            ApplicationRoles.Secretary);
    });
```

Antes de agregar o modificar la policy, inspecciona los roles reales disponibles y las reglas de acceso existentes. No inventes nombres de roles que no existan.


## 10. Multi-tenancy y seguridad

- Toda operación de negocio perteneciente a una empresa debe quedar limitada al `TenantId` autenticado.
- Nunca aceptes el `TenantId` del cliente cuando puede obtenerse del token.
- No permitas acceso cross-tenant.
- Los superadministradores deben seguir las reglas ya definidas para operaciones globales.
- No confíes en IDs enviados por el cliente sin validar pertenencia al tenant.
- Mantén `[Authorize]`, `RequireAuthorization`, roles o policies existentes.
- No expongas información sensible en mensajes de error.
- Devuelve 401 cuando el contexto autenticado requerido sea inválido.
- Devuelve 403 cuando el usuario esté autenticado pero no tenga permisos.
- Devuelve 404 cuando un recurso del tenant no exista o no sea accesible según el patrón actual.

---

## 11. Convenciones de C#

- Usa namespaces file-scoped.
- Usa `sealed` para clases que no deben heredarse.
- Usa tipos explícitos cuando mejoren la legibilidad.
- Evita `var` cuando el tipo no sea evidente.
- Usa nombres descriptivos en inglés.
- Usa `Async` en métodos asíncronos.
- Propaga `CancellationToken`.
- Usa `ArgumentNullException.ThrowIfNull` cuando corresponda.
- Evita métodos extensos y responsabilidades mezcladas.
- No agregues dependencias nuevas sin necesidad.
- No silencies warnings con casts inseguros.
- No uses `dynamic`.
- No agregues `#pragma warning disable` para esconder problemas.
- No cambies contratos públicos sin revisar todos sus consumidores.
- Conserva el estilo ya establecido en módulos maduros del proyecto.

---

## 12. Commands, Queries, Handlers y Validators

- Cada command/query representa una intención clara.
- Cada handler ejecuta un solo caso de uso.
- Los validators validan entrada, no sustituyen reglas del dominio.
- Las reglas invariantes deben permanecer en Domain.
- Los handlers no deben leer `HttpContext`.
- Para datos del usuario o tenant, inyecta `ICurrentRequestContext`.
- No devuelvas entidades EF directamente desde endpoints.
- Usa los DTOs y resultados ya definidos por el proyecto.
- No mezcles lectura y escritura cuando la estructura existente las separa.
- Mantén consistencia con la forma actual de despachar commands y queries.

---

## 13. Entity Framework Core y persistencia

- Usa consultas `AsNoTracking` cuando sean solo de lectura.
- Filtra por `TenantId` en operaciones tenant-aware.
- Evita N+1 queries.
- Proyecta a DTO cuando sea apropiado.
- No llames `SaveChangesAsync` desde Presentation.
- Respeta transacciones existentes.
- No agregues migraciones salvo que el modelo de datos realmente cambie.
- No cambies relaciones o restricciones de base de datos como efecto colateral de un refactor.
- Mantén compatibilidad con PostgreSQL.

---

## 14. Refactors

Cuando la tarea sea un refactor:

1. Inspecciona referencias y consumidores antes de borrar archivos.
2. Conserva el comportamiento observable.
3. Evita cambios no relacionados.
4. Elimina código muerto.
5. Actualiza namespaces y registros de DI.
6. Actualiza `Program.cs` o el registro central de endpoints.
7. Ejecuta búsqueda global de los tipos eliminados.
8. Compila toda la solución.
9. Ejecuta pruebas existentes.
10. Reporta cualquier error preexistente por separado.

No dejes simultáneamente la implementación nueva y la obsoleta.

---


## 15. Flujo obligatorio de Git y commits

Cada instrucción independiente debe terminar en un commit atómico que sirva como punto de recuperación.

### Antes de modificar

Ejecuta:

```bash
git status --short
git branch --show-current
```

Reglas:

- No descartes cambios preexistentes del usuario.
- No uses `git reset --hard`.
- No uses `git clean -fd`.
- No hagas rebase.
- No hagas amend de commits existentes.
- No incluyas archivos ajenos a la instrucción actual.
- Si el working tree ya contiene cambios, identifica cuáles son preexistentes antes de trabajar.
- Conserva los cambios preexistentes y agrega al commit únicamente los archivos relacionados con la instrucción.

### Un commit por instrucción

Cuando un prompt contenga varias instrucciones numeradas o varios objetivos independientes:

1. Implementa la primera instrucción.
2. Valídala.
3. Crea su commit.
4. Continúa con la siguiente instrucción.

No combines cambios independientes en un solo commit.

Los commits deben:

- Ser pequeños y coherentes.
- Contener únicamente una intención.
- Usar mensajes claros.
- Preferir Conventional Commits.

Ejemplos:

```text
refactor(customers): remove license metadata
feat(customers): add customer details query
feat(customers): paginate customer search
docs(agents): enforce pagination and atomic commits
```

### Validación previa al commit

Antes de cada commit:

```bash
dotnet build
dotnet test
git diff --check
git status --short
```

Cuando ejecutar toda la solución sea costoso, al menos compila y prueba los proyectos afectados, y al terminar ejecuta la validación completa.

No crees el commit si tus cambios introducen errores de compilación.

Si existen errores preexistentes no relacionados:

- Confirma que no fueron introducidos por la instrucción actual.
- Documenta el error.
- Ejecuta la validación más específica posible sobre los proyectos afectados.
- Crea el commit solamente cuando el cambio solicitado esté completo y no agregue errores nuevos.

### Después del commit

Ejecuta:

```bash
git status --short
git log -1 --oneline
```

Incluye el hash corto y el mensaje del commit en el reporte final.

No ejecutes `git push` salvo que el usuario lo solicite explícitamente.

---

## 16. Validación obligatoria

Después de cada cambio relevante:

```bash
dotnet restore
dotnet build
dotnet test
```

Si no hay proyectos de pruebas, indícalo en el reporte final.

También verifica:

```text
- No existen endpoints dentro de Core.Application.
- No quedan referencias a ICurrentUserService.
- No quedan referencias a ICurrentTenantService.
- No quedan registros DI de CurrentUserService.
- No quedan registros DI de CurrentTenantService.
- Los endpoints de Customers se mapean correctamente.
- Existe `GET /api/v1/customers/{customerId:guid}`.
- `GetCustomerById` devuelve los datos del cliente y sus documentos.
- `LicenseNumber` no existe en código productivo ni contratos HTTP.
- `LicenseExpirationDate` no existe en código productivo ni contratos HTTP.
- La licencia se representa únicamente como documento o imagen del cliente.
- Los listados de entidades de negocio usan paginación.
- La consulta de Customers aplica `OrderBy`, `Skip` y `Take` en base de datos.
- La respuesta de Customers usa el contrato paginado compartido.
- Ninguna ruta operativa de Customers contiene `{tenantId}`.
- Ningún endpoint de Customers recibe `tenantId` desde HTTP.
- Customers usa un `RouteGroupBuilder` con base `/api/v1/customers`.
- El grupo de Customers usa la policy `RequiredRoles`.
- La policy `RequiredRoles` incluye como mínimo `Owner` y `Secretary`.
- `TenantId`, `CreatedBy` y `ModifiedBy` se obtienen del contexto autenticado cuando corresponda.
- Las rutas públicas no cambiaron accidentalmente.
- La solución compila sin errores nuevos.
```

---

## 17. Entrega de resultados

Al finalizar una tarea:

- Implementa los cambios; no te limites a describirlos.
- Muestra la lista de archivos creados, modificados, movidos y eliminados.
- Resume las decisiones importantes.
- Indica los comandos de validación ejecutados.
- Reporta el resultado de build y tests.
- Señala riesgos o asuntos pendientes reales.
- Enumera cada commit creado con su hash corto y mensaje.
- Confirma que no se ejecutó `git push`.
- Cuando muestres código en la conversación, presenta archivos completos, no fragmentos aislados ni diffs parciales.

---

## 18. Logging estructurado

Serilog es el proveedor central de logging del host y debe configurarse desde la sección `Serilog` de `appsettings.json` y sus archivos por ambiente. Las variables de entorno pueden sobrescribir cualquier valor usando la convención estándar de configuración de .NET.

### Uso por capa

- Application e Infrastructure deben inyectar `Microsoft.Extensions.Logging.ILogger<T>`.
- Los tipos específicos de Serilog permanecen en Presentation, el host o Infrastructure cuando sean necesarios para la composición técnica.
- Domain no referencia Serilog ni contiene logging técnico.
- No uses `Console.WriteLine` para logs de aplicación.

### Niveles

- `Debug`: decisiones internas y consultas frecuentes sin datos sensibles.
- `Information`: inicio y cierre, operaciones exitosas, commands, eventos de negocio, correos y jobs completados.
- `Warning`: validaciones o conflictos esperados, respuestas 4xx, tokens inválidos o expirados y fallos recuperables de proveedores.
- `Error`: excepciones inesperadas, respuestas 5xx y fallos definitivos de jobs o integraciones.
- `Fatal`: errores de arranque que impiden continuar.

No registres errores esperados de validación como `Error`.

### Eventos y propiedades

- Usa message templates con propiedades nombradas; no interpolación de strings.
- Usa nombres consistentes como `CorrelationId`, `TenantId`, `UserId`, `ReservationId`, `VehicleId`, `CustomerId`, `RentalId`, `PaymentId`, `EmailTemplateCode`, `JobId`, `Status`, `PreviousStatus`, `NewStatus` y `ElapsedMilliseconds`.
- No serialices requests, responses ni entidades completas para construir logs.
- Commands pueden registrarse en `Information`; queries frecuentes deben preferir `Debug`.
- Los eventos específicos del negocio complementan el pipeline de MediatR y no deben duplicarse en endpoints.

Ejemplo correcto:

```csharp
logger.LogInformation(
    "Reservation {ReservationId} approved for Vehicle {VehicleId} in Tenant {TenantId}",
    reservationId,
    vehicleId,
    tenantId);
```

Está prohibido construir el mensaje con `$"..."` o serializar automáticamente el request.

### HTTP y CorrelationId

- El encabezado oficial es `X-Correlation-ID`.
- `CorrelationIdMiddleware` valida o genera el identificador, lo agrega al response y lo mantiene en el `LogContext` durante toda la petición.
- Cada petición genera un evento de entrada y uno de salida.
- El evento de salida contiene método, ruta, status, duración y, cuando estén disponibles, `TenantId` y `UserId`.
- No registres query strings completas, headers completos, request bodies ni response bodies.
- Las excepciones HTTP no controladas se registran una sola vez con stack trace en el middleware global. El evento de finalización solo registra el resultado técnico.

### Archivos y retención

- Ruta local predeterminada: `Logs/rentify-.json`.
- Ruta Production predeterminada: `/app/logs/rentify-.json`.
- El formato es JSON compacto, con rotación diaria y rotación adicional a 50 MB.
- Production conserva como máximo 30 archivos y aplica además una retención temporal de 30 días; Development conserva como máximo 10 archivos durante 7 días.
- Ruta, límites y retención deben seguir siendo configurables.
- Los despliegues en contenedor deben persistir `/app/logs` en un volumen y mantener habilitada la consola.
- Nunca versions archivos de log.

### Jobs, correo y proveedores

- Cada job importante registra inicio, finalización, duración, `JobId` cuando esté disponible y la entidad o tenant relacionado.
- Un job que registra una excepción debe relanzarla para conservar los retries de Hangfire.
- Los flujos de correo registran `EmailTemplateCode`, tenant, entidad relacionada, duración y el identificador seguro del proveedor.
- No registres destinatarios completos, HTML, texto completo, enlaces de recuperación, tokens, API keys, headers, payloads ni URLs firmadas.
- Las integraciones externas registran proveedor, operación, resultado, duración y códigos de estado seguros; nunca el payload completo del proveedor.

### Datos prohibidos

Nunca registres:

- `Password`, `ConfirmPassword`, `CurrentPassword` o `NewPassword`.
- Access tokens, refresh tokens, reset-password tokens, JWT completos o el header `Authorization`.
- Cookies, API keys, client secrets, connection strings ni variables de entorno completas.
- Cédulas, pasaportes, licencias, documentos personales, imágenes Base64 o archivos subidos.
- Correos o teléfonos completos cuando no sean estrictamente necesarios.
- Direcciones completas, tarjetas, datos de pago, HTML o cuerpos de texto completos.
- Request bodies, response bodies, headers completos, entidades completas o requests serializados.
