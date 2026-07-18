Implementa el módulo de Locations en el backend de Rentify.



Objetivo:

Necesitamos soportar ubicaciones de entrega/recogida para reservas futuras.



Hay dos niveles:



1\. Location:

&#x20;  Catálogo global administrado por el SuperAdmin de Rentify.

&#x20;  Ejemplos:

&#x20;  - Aeropuerto Internacional Las Américas

&#x20;  - Aeropuerto Internacional del Cibao

&#x20;  - Aeropuerto Internacional de Punta Cana

&#x20;  - Oficina principal

&#x20;  - Hotel / Plaza / Punto conocido



2\. TenantLocation:

&#x20;  Configuración por rent car/tenant.

&#x20;  Cada rent car decide en qué ubicaciones entrega o recoge vehículos y cuánto cobra adicionalmente.



También debe quedar preparado para que más adelante las reservas puedan usar:

\- Una ubicación predefinida configurada por el tenant.

\- Una ubicación personalizada escrita por el cliente.

\- Un costo adicional por entrega.

\- Un costo adicional por recogida.



No implementar todavía el módulo Reservations en esta tarea.

No conectar todavía reservas con Location.

Solo crear los módulos Location y TenantLocation con sus endpoints, entidades, configuraciones, repositorios, CQRS y migraciones.



====================================================================

1\. Contexto de arquitectura

====================================================================



El proyecto usa:

\- Clean Architecture / Modular Monolith.

\- Domain / Application / Infrastructure / Presentation.

\- Minimal APIs.

\- MediatR.

\- EF Core.

\- PostgreSQL.

\- Multi-tenant.

\- BaseEntity con:

&#x20; - CreatedBy

&#x20; - ModifiedBy

&#x20; - CreatedDate

&#x20; - ModifiedDate

&#x20; - IsDeleted

&#x20; - IsActive



Seguir los patrones existentes del proyecto.



Antes de modificar:

\- Leer AGENTS.md.

\- Revisar la estructura actual de módulos como Vehicles, Customers y Tenants.

\- Revisar cómo se crean entidades, configuraciones EF, repositorios, handlers, validators y endpoints.

\- No crear un estilo diferente.

\- Respetar namespaces existentes.



====================================================================

2\. Módulo 1: Location global

====================================================================



Crear entidad global:



Rentify.Backend.Core.Domain.Entities.Locations.Location



Esta entidad NO pertenece a un tenant.

Es catálogo global administrado por SuperAdmin.



Campos:



public sealed class Location : BaseEntity

{

&#x20;   public Guid Id { get; private set; }

&#x20;   public string Name { get; private set; } = null!;

&#x20;   public LocationType Type { get; private set; }

&#x20;   public string? City { get; private set; }

&#x20;   public string? Province { get; private set; }

&#x20;   public string Country { get; private set; } = "República Dominicana";

&#x20;   public string? Address { get; private set; }

&#x20;   public string? Notes { get; private set; }

}



Crear enum:



namespace Rentify.Backend.Core.Domain.Enums;



public enum LocationType

{

&#x20;   Airport = 1,

&#x20;   Office = 2,

&#x20;   Hotel = 3,

&#x20;   Mall = 4,

&#x20;   Port = 5,

&#x20;   BusStation = 6,

&#x20;   Other = 99

}



Notas:

\- No usar Custom como tipo global. La ubicación personalizada será manejada luego a nivel de reserva/tenant.

\- Location representa ubicaciones conocidas y reutilizables.



====================================================================

3\. Reglas de dominio para Location

====================================================================



Agregar factory:



Location.Create(

&#x20;   string name,

&#x20;   LocationType type,

&#x20;   string? city,

&#x20;   string? province,

&#x20;   string? country,

&#x20;   string? address,

&#x20;   string? notes,

&#x20;   string createdBy)



Agregar método Update:



Update(

&#x20;   string name,

&#x20;   LocationType type,

&#x20;   string? city,

&#x20;   string? province,

&#x20;   string? country,

&#x20;   string? address,

&#x20;   string? notes,

&#x20;   string modifiedBy)



Agregar métodos:

\- Activate(string modifiedBy)

\- Deactivate(string modifiedBy)

\- Delete(string modifiedBy)



Reglas:

\- Name requerido, máximo 150.

\- Type debe ser enum válido.

\- City opcional, máximo 100.

\- Province opcional, máximo 100.

\- Country requerido. Si viene null/empty, usar "República Dominicana".

\- Country máximo 100.

\- Address opcional, máximo 250.

\- Notes opcional, máximo 500.

\- CreatedBy/ModifiedBy requerido.

\- Trim a todos los strings.

\- String vacío debe guardarse como null en campos opcionales.



No permitir borrar físicamente. Usar soft delete.



====================================================================

4\. EF Core Configuration para Location

====================================================================



Crear LocationConfiguration.



Tabla:

"Locations"



Configurar:



Id primary key.

Name varchar(150) required.

Type required.

City varchar(100) nullable.

Province varchar(100) nullable.

Country varchar(100) required.

Address varchar(250) nullable.

Notes varchar(500) nullable.

CreatedBy required.

ModifiedBy required.

CreatedDate required.

ModifiedDate required.

IsDeleted required.

IsActive required.



Enum LocationType:

\- Si el proyecto usa enums como string, usar HasConversion<string>().

\- Si usa int, seguir convención actual.

Preferencia si no hay convención clara: string para legibilidad.



Índices:

\- Name + City + Province + Country único parcial donde IsDeleted = false.

\- Type.

\- IsActive.



Ejemplo índice único:



builder.HasIndex(x => new

{

&#x20;   x.Name,

&#x20;   x.City,

&#x20;   x.Province,

&#x20;   x.Country

})

.IsUnique()

.HasFilter("\\"IsDeleted\\" = false");



Ajustar según convención actual de PostgreSQL y EF en el proyecto.



====================================================================

5\. Endpoints SuperAdmin para Location

====================================================================



Crear endpoints bajo rutas de SuperAdmin.



Rutas sugeridas:



GET    /api/v1/admin/locations

GET    /api/v1/admin/locations/{locationId}

POST   /api/v1/admin/locations

PUT    /api/v1/admin/locations/{locationId}

PUT    /api/v1/admin/locations/{locationId}/activate

PUT    /api/v1/admin/locations/{locationId}/deactivate

DELETE /api/v1/admin/locations/{locationId}



Estos endpoints deben requerir rol SuperAdmin o la política equivalente actual del proyecto.



Si el proyecto usa roles:

\- OWNER para tenant

\- SUPER\_ADMIN o SuperAdmin para admin global



Usar la convención actual.



====================================================================

6\. Query: listar Locations

====================================================================



GET /api/v1/admin/locations



Debe soportar paginación y filtros:



Query params:

\- search opcional

\- type opcional

\- isActive opcional

\- pageNumber

\- pageSize



Filtros:

\- search busca por Name, City, Province, Country.

\- type filtra por LocationType.

\- isActive filtra activos/inactivos.

\- siempre excluir IsDeleted = true.



Response debe incluir:



LocationListItemResponse:

\- Id

\- Name

\- Type

\- City

\- Province

\- Country

\- Address

\- IsActive



Usar el modelo de paginación existente del proyecto.



====================================================================

7\. Query: obtener Location por Id

====================================================================



GET /api/v1/admin/locations/{locationId}



Response:

\- Id

\- Name

\- Type

\- City

\- Province

\- Country

\- Address

\- Notes

\- IsActive

\- CreatedDate

\- ModifiedDate



Si no existe o está eliminado:

404 "Ubicación no encontrada."



====================================================================

8\. Command: crear Location

====================================================================



POST /api/v1/admin/locations



Request:



{

&#x20; "name": "Aeropuerto Internacional Las Américas",

&#x20; "type": "Airport",

&#x20; "city": "Santo Domingo Este",

&#x20; "province": "Santo Domingo",

&#x20; "country": "República Dominicana",

&#x20; "address": "Ruta 66, Boca Chica",

&#x20; "notes": "Principal aeropuerto de Santo Domingo."

}



Validaciones:

\- name requerido max 150.

\- type requerido y válido.

\- city max 100.

\- province max 100.

\- country max 100.

\- address max 250.

\- notes max 500.

\- no permitir duplicado activo/no eliminado por Name + City + Province + Country.



Mensaje duplicado:

"Ya existe una ubicación con estos datos."



Response:

\- Id

\- Name

\- Type

\- City

\- Province

\- Country

\- Address

\- Notes

\- IsActive



====================================================================

9\. Command: actualizar Location

====================================================================



PUT /api/v1/admin/locations/{locationId}



Request igual a crear.



Validaciones:

\- locationId requerido.

\- Debe existir y no estar eliminado.

\- mismas validaciones de create.

\- no permitir duplicado excluyendo el mismo locationId.



Mensaje no encontrada:

"Ubicación no encontrada."



Mensaje duplicado:

"Ya existe una ubicación con estos datos."



====================================================================

10\. Activar, desactivar y eliminar Location

====================================================================



PUT /api/v1/admin/locations/{locationId}/activate

\- Marca IsActive = true.

\- IsDeleted debe seguir false.



PUT /api/v1/admin/locations/{locationId}/deactivate

\- Marca IsActive = false.

\- No borrar.



DELETE /api/v1/admin/locations/{locationId}

\- Soft delete.

\- IsDeleted = true.

\- IsActive = false.



Reglas:

\- Si Location está usada por TenantLocation, permitir desactivar, pero no eliminar físicamente.

\- Como DELETE es soft delete, permitirlo.

\- Más adelante se podría bloquear si hay reservas históricas. No implementarlo ahora.



====================================================================

11\. Seed opcional de aeropuertos RD

====================================================================



Crear seed opcional o script SQL separado para ubicaciones globales de República Dominicana.



No ejecutar automáticamente si el proyecto no maneja seeds así.

Puede quedar como migration seed si el proyecto usa HasData, o como script SQL en docs/database/seeds.



Aeropuertos sugeridos:

\- Aeropuerto Internacional Las Américas

\- Aeropuerto Internacional del Cibao

\- Aeropuerto Internacional de Punta Cana

\- Aeropuerto Internacional Gregorio Luperón

\- Aeropuerto Internacional La Romana

\- Aeropuerto Internacional El Catey

\- Aeropuerto Internacional La Isabela / Dr. Joaquín Balaguer



Todos Type = Airport.

Country = República Dominicana.



====================================================================

12\. Módulo 2: TenantLocation

====================================================================



Crear entidad:



Rentify.Backend.Core.Domain.Entities.Locations.TenantLocation



Esta entidad pertenece a un tenant.

Representa las ubicaciones que un rent car habilita para entrega/recogida y sus costos.



Campos:



public sealed class TenantLocation : BaseEntity

{

&#x20;   public Guid Id { get; private set; }

&#x20;   public Guid TenantId { get; private set; }

&#x20;   public Guid? LocationId { get; private set; }



&#x20;   public string DisplayName { get; private set; } = null!;



&#x20;   public bool AllowsDelivery { get; private set; }

&#x20;   public bool AllowsPickup { get; private set; }



&#x20;   public decimal DeliveryFee { get; private set; }

&#x20;   public decimal PickupFee { get; private set; }



&#x20;   public bool IsCustom { get; private set; }



&#x20;   public Tenant Tenant { get; private set; } = null!;

&#x20;   public Location? Location { get; private set; }

}



Explicación:

\- LocationId apunta a una Location global cuando el tenant configura una ubicación del catálogo.

\- IsCustom indica si es una configuración personalizada del tenant.

\- DisplayName es el nombre visible para el rent car y para futuras reservas.

\- Para Location global, DisplayName puede copiarse desde Location.Name, pero debe guardarse como histórico configurable.

\- IsCustom = true permite que el rent car cree una ubicación propia no global, como "Oficina principal", "Sucursal Santiago", etc.

\- No confundir esto con la dirección personalizada que el cliente escribirá en una reserva futura. Esta entidad es configuración del tenant.



====================================================================

13\. Reglas de dominio para TenantLocation

====================================================================



Crear factory para Location global:



TenantLocation.CreateFromGlobalLocation(

&#x20;   Guid tenantId,

&#x20;   Guid locationId,

&#x20;   string displayName,

&#x20;   bool allowsDelivery,

&#x20;   bool allowsPickup,

&#x20;   decimal deliveryFee,

&#x20;   decimal pickupFee,

&#x20;   string createdBy)



Crear factory para ubicación personalizada del tenant:



TenantLocation.CreateCustom(

&#x20;   Guid tenantId,

&#x20;   string displayName,

&#x20;   bool allowsDelivery,

&#x20;   bool allowsPickup,

&#x20;   decimal deliveryFee,

&#x20;   decimal pickupFee,

&#x20;   string createdBy)



Update:



Update(

&#x20;   string displayName,

&#x20;   bool allowsDelivery,

&#x20;   bool allowsPickup,

&#x20;   decimal deliveryFee,

&#x20;   decimal pickupFee,

&#x20;   string modifiedBy)



Métodos:

\- Activate(string modifiedBy)

\- Deactivate(string modifiedBy)

\- Delete(string modifiedBy)



Reglas:

\- TenantId requerido.

\- Si IsCustom = false, LocationId requerido.

\- Si IsCustom = true, LocationId debe ser null.

\- DisplayName requerido, máximo 150.

\- Debe permitir delivery o pickup al menos uno.

\- DeliveryFee no puede ser negativo.

\- PickupFee no puede ser negativo.

\- Si AllowsDelivery = false, DeliveryFee debe guardarse como 0.

\- Si AllowsPickup = false, PickupFee debe guardarse como 0.

\- CreatedBy/ModifiedBy requerido.

\- Soft delete para eliminar.



Ejemplo:

\- AllowsDelivery = true, DeliveryFee = 1500.

\- AllowsPickup = true, PickupFee = 1000.

\- AllowsDelivery = false, DeliveryFee final = 0.



====================================================================

14\. EF Core Configuration para TenantLocation

====================================================================



Crear TenantLocationConfiguration.



Tabla:

"TenantLocations"



Configurar:

\- Id primary key.

\- TenantId required.

\- LocationId nullable.

\- DisplayName varchar(150) required.

\- AllowsDelivery required.

\- AllowsPickup required.

\- DeliveryFee numeric(18,2) required default 0.

\- PickupFee numeric(18,2) required default 0.

\- IsCustom required.

\- CreatedBy required.

\- ModifiedBy required.

\- CreatedDate required.

\- ModifiedDate required.

\- IsDeleted required.

\- IsActive required.



Relaciones:

\- TenantLocation belongs to Tenant.

\- TenantLocation optionally belongs to Location.



OnDelete:

\- Tenant: Restrict.

\- Location: Restrict.



Índices:

1\. TenantId + LocationId único parcial donde IsDeleted = false AND LocationId IS NOT NULL.

&#x20;  Para evitar que un tenant configure dos veces el mismo aeropuerto global.



2\. TenantId + DisplayName único parcial donde IsDeleted = false.

&#x20;  Para evitar duplicados visuales dentro del tenant.



3\. TenantId + IsActive.



Ejemplo:



builder.HasIndex(x => new { x.TenantId, x.LocationId })

&#x20;   .IsUnique()

&#x20;   .HasFilter("\\"IsDeleted\\" = false AND \\"LocationId\\" IS NOT NULL");



builder.HasIndex(x => new { x.TenantId, x.DisplayName })

&#x20;   .IsUnique()

&#x20;   .HasFilter("\\"IsDeleted\\" = false");



====================================================================

15\. Endpoints tenant para TenantLocation

====================================================================



Crear endpoints internos para rent car/tenant:



GET    /api/v1/tenant-locations

GET    /api/v1/tenant-locations/{tenantLocationId}

POST   /api/v1/tenant-locations

PUT    /api/v1/tenant-locations/{tenantLocationId}

PUT    /api/v1/tenant-locations/{tenantLocationId}/activate

PUT    /api/v1/tenant-locations/{tenantLocationId}/deactivate

DELETE /api/v1/tenant-locations/{tenantLocationId}



Además, crear endpoint para que el tenant pueda ver las ubicaciones globales disponibles:



GET /api/v1/locations/available



Este endpoint devuelve Locations globales activas y no eliminadas para que el rent car pueda elegir cuáles habilitar.



No debe permitir modificar Locations globales.

Solo lectura.



====================================================================

16\. GET /api/v1/locations/available

====================================================================



Endpoint interno para tenants.



Query params:

\- search opcional

\- type opcional



Debe devolver solo:

\- Locations globales activas.

\- IsDeleted = false.

\- IsActive = true.



Response:

\- Id

\- Name

\- Type

\- City

\- Province

\- Country

\- Address



Esto es para llenar un select del frontend:

"Seleccione una ubicación del catálogo."



No requiere SuperAdmin, pero sí usuario autenticado del tenant.



====================================================================

17\. GET /api/v1/tenant-locations

====================================================================



Lista las ubicaciones configuradas por el rent car.



Query params:

\- search opcional.

\- isActive opcional.

\- supportsDelivery opcional.

\- supportsPickup opcional.

\- pageNumber.

\- pageSize.



Response:

\- Id

\- LocationId

\- DisplayName

\- LocationType si viene de global.

\- City

\- Province

\- AllowsDelivery

\- AllowsPickup

\- DeliveryFee

\- PickupFee

\- IsCustom

\- IsActive



Filtrar siempre por TenantId actual.

Excluir IsDeleted = true.



====================================================================

18\. GET /api/v1/tenant-locations/{tenantLocationId}

====================================================================



Devuelve detalle de configuración del tenant.



Response:

\- Id

\- TenantId no incluir si los responses normalmente no lo exponen.

\- LocationId

\- DisplayName

\- LocationType

\- City

\- Province

\- Country

\- Address

\- AllowsDelivery

\- AllowsPickup

\- DeliveryFee

\- PickupFee

\- IsCustom

\- IsActive



Si no existe o pertenece a otro tenant:

404 "Ubicación del tenant no encontrada."



====================================================================

19\. POST /api/v1/tenant-locations

====================================================================



Debe permitir dos modos:



A. Configurar ubicación global:



Request:

{

&#x20; "locationId": "uuid-global-location",

&#x20; "displayName": "Aeropuerto Las Américas",

&#x20; "allowsDelivery": true,

&#x20; "allowsPickup": true,

&#x20; "deliveryFee": 1500,

&#x20; "pickupFee": 1500,

&#x20; "isCustom": false

}



Reglas:

\- locationId requerido.

\- La Location global debe existir, estar activa y no eliminada.

\- No permitir duplicar la misma Location global dentro del mismo tenant.

\- displayName puede venir vacío/null. Si viene vacío, usar Location.Name.

\- IsCustom debe ser false.



B. Crear ubicación personalizada del tenant:



Request:

{

&#x20; "locationId": null,

&#x20; "displayName": "Oficina principal",

&#x20; "allowsDelivery": true,

&#x20; "allowsPickup": true,

&#x20; "deliveryFee": 0,

&#x20; "pickupFee": 0,

&#x20; "isCustom": true

}



Reglas:

\- locationId debe ser null.

\- displayName requerido.

\- IsCustom debe ser true.

\- No permitir DisplayName duplicado dentro del tenant.



Validaciones comunes:

\- Debe permitir delivery o pickup al menos uno.

\- deliveryFee >= 0.

\- pickupFee >= 0.

\- Si allowsDelivery = false, deliveryFee se guarda 0.

\- Si allowsPickup = false, pickupFee se guarda 0.



Response:

TenantLocationResponse.



====================================================================

20\. PUT /api/v1/tenant-locations/{tenantLocationId}

====================================================================



Permite actualizar configuración del rent car:



Request:

{

&#x20; "displayName": "Aeropuerto Las Américas",

&#x20; "allowsDelivery": true,

&#x20; "allowsPickup": false,

&#x20; "deliveryFee": 1500,

&#x20; "pickupFee": 0

}



Reglas:

\- No permitir cambiar LocationId.

\- No permitir cambiar IsCustom.

\- Solo actualizar:

&#x20; - DisplayName

&#x20; - AllowsDelivery

&#x20; - AllowsPickup

&#x20; - DeliveryFee

&#x20; - PickupFee

\- Validar duplicado de DisplayName excluyendo el registro actual.

\- Si pertenece a otro tenant, 404.



====================================================================

21\. Activar/desactivar/eliminar TenantLocation

====================================================================



PUT /api/v1/tenant-locations/{tenantLocationId}/activate

\- IsActive = true.



PUT /api/v1/tenant-locations/{tenantLocationId}/deactivate

\- IsActive = false.



DELETE /api/v1/tenant-locations/{tenantLocationId}

\- Soft delete:

&#x20; - IsDeleted = true.

&#x20; - IsActive = false.



No eliminar físicamente.



Si luego una reserva histórica usó esta ubicación, no se debe perder el histórico porque la reserva copiará nombre/tarifa. Eso se implementará luego.



====================================================================

22\. CQRS requerido

====================================================================



Crear estructura siguiendo AGENTS.md.



Para Location global SuperAdmin:



Queries:

\- GetAdminLocationsQuery

\- GetAdminLocationByIdQuery



Commands:

\- CreateLocationCommand

\- UpdateLocationCommand

\- ActivateLocationCommand

\- DeactivateLocationCommand

\- DeleteLocationCommand



Para tenant:



Queries:

\- GetAvailableLocationsQuery

\- GetTenantLocationsQuery

\- GetTenantLocationByIdQuery



Commands:

\- CreateTenantLocationCommand

\- UpdateTenantLocationCommand

\- ActivateTenantLocationCommand

\- DeactivateTenantLocationCommand

\- DeleteTenantLocationCommand



Validators:

Crear validators para todos los commands y queries que reciban parámetros.



Responses:

\- LocationResponse

\- LocationListItemResponse

\- TenantLocationResponse

\- TenantLocationListItemResponse



====================================================================

23\. Repositorios

====================================================================



Crear interfaces y repositorios siguiendo patrón actual.



Sugeridos:



ILocationRepository:

\- Task<Location?> GetByIdAsync(Guid id, CancellationToken cancellationToken)

\- Task<Location?> GetActiveByIdAsync(Guid id, CancellationToken cancellationToken)

\- Task<bool> ExistsDuplicateAsync(string name, string? city, string? province, string country, Guid? excludeId, CancellationToken cancellationToken)

\- Queryable o método paginado según patrón actual.



ITenantLocationRepository:

\- Task<TenantLocation?> GetByIdAsync(Guid tenantId, Guid id, CancellationToken cancellationToken)

\- Task<bool> ExistsByLocationAsync(Guid tenantId, Guid locationId, Guid? excludeId, CancellationToken cancellationToken)

\- Task<bool> ExistsByDisplayNameAsync(Guid tenantId, string displayName, Guid? excludeId, CancellationToken cancellationToken)

\- AddAsync

\- métodos paginados/listados según patrón actual.



Si el proyecto usa repositorios genéricos o DbContext directo en handlers, seguir la convención existente.



====================================================================

24\. Tenancy y seguridad

====================================================================



Location global:

\- Solo SuperAdmin puede crear/editar/activar/desactivar/eliminar.

\- Tenant puede consultar locations globales activas vía /api/v1/locations/available.



TenantLocation:

\- Todas las operaciones filtran por TenantId actual.

\- Nunca permitir que un tenant vea o modifique configuraciones de otro tenant.

\- TenantId sale de ICurrentTenantService o ICurrentRequestContext según convención actual.

\- CreatedBy/ModifiedBy sale del usuario actual.



====================================================================

25\. Migración

====================================================================



Crear migración EF Core.



Nombre sugerido:

AddLocationsAndTenantLocations



Debe crear:

\- Locations

\- TenantLocations



Con relaciones, índices y restricciones.



Validar que compile.



====================================================================

26\. Preparación para Reservations futura

====================================================================



No implementar Reservations en esta tarea.



Pero dejar preparado el diseño para que luego Reservation pueda guardar:



DeliveryTenantLocationId nullable

DeliveryLocationName

DeliveryAddressDetails

DeliveryFee



PickupTenantLocationId nullable

PickupLocationName

PickupAddressDetails

PickupFee



La reserva futura debe copiar el nombre y fee aplicado para mantener histórico.



No crear estos campos todavía en Reservation en esta tarea, salvo que ya exista el módulo y sea estrictamente necesario. Preferencia: no tocar Reservation ahora.



====================================================================

27\. Pruebas manuales SuperAdmin

====================================================================



Probar:



1\. Crear Location global aeropuerto.

POST /api/v1/admin/locations



2\. Listar Locations globales.

GET /api/v1/admin/locations



3\. Filtrar por type Airport.



4\. Obtener Location por Id.



5\. Actualizar Location.



6\. Crear duplicado con mismos Name/City/Province/Country.

Resultado:

\- Debe fallar:

"Ya existe una ubicación con estos datos."



7\. Desactivar Location.

Resultado:

\- IsActive false.



8\. Activar Location.

Resultado:

\- IsActive true.



9\. Eliminar Location.

Resultado:

\- IsDeleted true, IsActive false.



====================================================================

28\. Pruebas manuales TenantLocation

====================================================================



Probar:



1\. Tenant consulta ubicaciones disponibles.

GET /api/v1/locations/available



Resultado:

\- Devuelve solo Locations globales activas.



2\. Tenant configura Aeropuerto Las Américas.

POST /api/v1/tenant-locations



3\. Tenant intenta configurar el mismo LocationId otra vez.

Resultado:

\- Debe fallar por duplicado.



Mensaje sugerido:

"Esta ubicación ya está configurada para tu empresa."



4\. Tenant crea ubicación personalizada:

"Oficina principal"



5\. Tenant intenta crear otra ubicación personalizada con el mismo DisplayName.

Resultado:

\- Debe fallar.



Mensaje sugerido:

"Ya existe una ubicación con este nombre para tu empresa."



6\. Tenant lista sus ubicaciones.

GET /api/v1/tenant-locations



7\. Tenant actualiza fee de entrega.

PUT /api/v1/tenant-locations/{id}



8\. Tenant desactiva ubicación.

PUT /api/v1/tenant-locations/{id}/deactivate



9\. Tenant activa ubicación.

PUT /api/v1/tenant-locations/{id}/activate



10\. Tenant elimina ubicación.

DELETE /api/v1/tenant-locations/{id}



11\. Validar tenancy:

\- Tenant A no puede ver/modificar TenantLocation de Tenant B.



====================================================================

29\. Resultado esperado

====================================================================



Al finalizar:

\- Existe módulo global Locations para SuperAdmin.

\- Existe módulo TenantLocations para rent cars.

\- SuperAdmin puede administrar catálogo global de ubicaciones.

\- Tenant puede ver ubicaciones globales disponibles.

\- Tenant puede configurar dónde entrega/recoge y sus costos.

\- Tenant puede crear ubicaciones personalizadas propias.

\- Hay validaciones de duplicados.

\- Todo respeta multi-tenancy.

\- Hay EF configurations y migración.

\- Hay endpoints Minimal APIs organizados.

\- Hay CQRS/MediatR siguiendo el patrón del proyecto.

\- El proyecto compila sin errores.
