Refactoriza completamente el módulo de Reservations en Application porque la implementación actual no siguió la estructura CQRS del proyecto.

Problema actual:

Codex creó una clase única:

ReservationCommandHandlers

que implementa simultáneamente:

- IRequestHandler<CreateReservationCommand, ...>
- IRequestHandler<UpdateReservationCommand, ...>
- IRequestHandler<ApproveReservationCommand, ...>
- IRequestHandler<RejectReservationCommand, ...>
- IRequestHandler<CancelReservationCommand, ...>
- IRequestHandler<DeleteReservationCommand, ...>

Además:

- Todos los handlers están en un mismo archivo.
- Las dependencias están declaradas en una sola línea.
- El constructor está comprimido en una sola línea.
- Los métodos Handle están comprimidos y difíciles de leer.
- La clase contiene helpers privados para todas las operaciones.
- Mezcla creación, edición, aprobación, rechazo, cancelación y eliminación.
- Mezcla validación, resolución de TenantLocation, obtención de tarifa y mapeo de response.
- Tiene textos con problemas de codificación como:
  "VehÃ­culo"
  "ubicaciÃ³n"
- No respeta la estructura por feature que ya usa Rentify.

Objetivo:

Dejar cada Command, Handler y Validator en su propia carpeta y archivo, siguiendo el patrón del resto del proyecto.

No cambiar el comportamiento funcional de Reservations salvo para corregir problemas evidentes.
No modificar frontend.
No cambiar endpoints.
No cambiar rutas.
No cambiar contratos del API sin necesidad.
No modificar la entidad Reservation salvo que exista una incompatibilidad real.
No meter lógica en Minimal APIs.

====================================================================
1. Revisar convenciones existentes
====================================================================

Antes de modificar:

- Leer AGENTS.md.
- Revisar la estructura real de módulos como:
  - Vehicles
  - Customers
  - Locations
  - TenantLocations
  - Tenants
- Copiar exactamente la convención existente para:
  - Commands
  - Queries
  - Handlers
  - Validators
  - Responses
  - Repositories
  - Services
  - Namespaces

No inventar una arquitectura nueva.

La estructura esperada debe parecerse a:

Application/
  Modules/
    Reservations/
      Commands/
        CreateReservation/
          CreateReservationCommand.cs
          CreateReservationCommandHandler.cs
          CreateReservationCommandValidator.cs

        UpdateReservation/
          UpdateReservationCommand.cs
          UpdateReservationCommandHandler.cs
          UpdateReservationCommandValidator.cs

        ApproveReservation/
          ApproveReservationCommand.cs
          ApproveReservationCommandHandler.cs
          ApproveReservationCommandValidator.cs

        RejectReservation/
          RejectReservationCommand.cs
          RejectReservationCommandHandler.cs
          RejectReservationCommandValidator.cs

        CancelReservation/
          CancelReservationCommand.cs
          CancelReservationCommandHandler.cs
          CancelReservationCommandValidator.cs

        DeleteReservation/
          DeleteReservationCommand.cs
          DeleteReservationCommandHandler.cs
          DeleteReservationCommandValidator.cs

      Queries/
        GetReservations/
          GetReservationsQuery.cs
          GetReservationsQueryHandler.cs
          GetReservationsQueryValidator.cs
          GetReservationsResponse.cs

        GetReservationById/
          GetReservationByIdQuery.cs
          GetReservationByIdQueryHandler.cs
          GetReservationByIdResponse.cs

        CheckVehicleReservationAvailability/
          CheckVehicleReservationAvailabilityQuery.cs
          CheckVehicleReservationAvailabilityQueryHandler.cs
          CheckVehicleReservationAvailabilityQueryValidator.cs

      Contracts/
        Repositories/
        Services/

      Dtos/
      Mappers/
      Services/

Ajustar nombres a la convención real del proyecto si difiere.

====================================================================
2. Eliminar clase agregadora
====================================================================

Eliminar completamente:

ReservationCommandHandlers

No debe quedar ninguna clase que implemente múltiples IRequestHandler para commands distintos.

Cada handler debe implementar únicamente un IRequestHandler.

Ejemplo:

public sealed class CreateReservationCommandHandler
    : IRequestHandler<CreateReservationCommand, ResultReponse<ReservationResponse>>

public sealed class ApproveReservationCommandHandler
    : IRequestHandler<ApproveReservationCommand, ResultReponse<ReservationResponse>>

Nunca:

public sealed class ReservationCommandHandlers :
    IRequestHandler<CreateReservationCommand, ...>,
    IRequestHandler<UpdateReservationCommand, ...>,
    IRequestHandler<ApproveReservationCommand, ...>

====================================================================
3. Crear handler de CreateReservation
====================================================================

Archivo:

Commands/CreateReservation/CreateReservationCommandHandler.cs

Responsabilidades:

1. Validar cliente.
2. Obtener vehículo reservable.
3. Resolver ubicación de entrega.
4. Resolver ubicación de devolución.
5. Calcular quantity.
6. Obtener VehicleRate.
7. Generar código.
8. Crear Reservation.
9. Guardar.
10. Retornar response.

Dependencias solo necesarias:

- IReservationRepository
- ICustomerRepository
- IVehicleRepository
- ITenantLocationRepository
- IReservationCodeGenerator
- IUnitOfWork
- servicios compartidos que se definan para no duplicar lógica

No incluir lógica de aprobar, rechazar, cancelar o eliminar.

Código legible:
- una dependencia por línea
- constructor formateado
- una operación por línea
- nombres claros
- sin métodos gigantes

====================================================================
4. Crear handler de UpdateReservation
====================================================================

Archivo:

Commands/UpdateReservation/UpdateReservationCommandHandler.cs

Responsabilidades:

1. Obtener reserva por tenant.
2. Validar que pueda actualizarse.
3. Obtener vehículo actual.
4. Resolver locations.
5. Obtener tarifa.
6. Recalcular quantity.
7. Llamar UpdatePendingReservation.
8. Guardar.
9. Retornar response.

No incluir lógica de crear o aprobar.

====================================================================
5. Crear handler de ApproveReservation
====================================================================

Archivo:

Commands/ApproveReservation/ApproveReservationCommandHandler.cs

Responsabilidades:

1. Obtener reserva.
2. Validar cliente activo.
3. Validar vehículo reservable.
4. Validar solapamiento.
5. Llamar reservation.Approve.
6. Guardar.
7. Retornar response.

Dependencias solo necesarias:

- IReservationRepository
- ICustomerRepository
- IVehicleRepository
- IUnitOfWork

No necesita:
- ITenantLocationRepository
- IReservationCodeGenerator

====================================================================
6. Crear handlers simples separados
====================================================================

Crear:

RejectReservationCommandHandler
CancelReservationCommandHandler
DeleteReservationCommandHandler

Cada uno debe:

- Obtener Reservation.
- Ejecutar método de dominio correspondiente.
- Guardar.
- Retornar response.

No duplicar lógica innecesaria.

====================================================================
7. Extraer lógica compartida
====================================================================

La clase actual tiene helpers como:

- GetReservationAsync
- ValidateCustomerAsync
- GetReservableVehicleAsync
- GetRate
- ResolveDeliveryAsync
- ResolveReturnAsync
- RequireLocationName
- ToResponse

No copiar todos esos helpers dentro de cada handler.

Extraerlos correctamente.

Crear servicios internos de Application si el patrón del proyecto lo permite.

Sugerencia:

IReservationApplicationService
ReservationApplicationService

Responsabilidades posibles:

- GetReservationOrThrowAsync
- ValidateCustomerAsync
- GetReservableVehicleAsync
- GetVehicleRateOrThrow
- ResolveDeliveryLocationAsync
- ResolveReturnLocationAsync

O separar aún mejor:

ReservationLocationResolver
ReservationVehicleService
ReservationMapper

No crear una clase enorme nueva que repita el mismo problema.
Cada servicio debe tener una responsabilidad clara.

Preferencia:

Services/
  ReservationLocationResolver.cs
  ReservationVehicleResolver.cs

====================================================================
8. ReservationLocationResolver
====================================================================

Crear contrato:

IReservationLocationResolver

Métodos sugeridos:

Task<ResolvedReservationLocation> ResolveDeliveryAsync(
    Guid tenantId,
    Guid? tenantLocationId,
    string? customName,
    decimal customFee,
    CancellationToken cancellationToken);

Task<ResolvedReservationLocation> ResolveReturnAsync(
    Guid tenantId,
    Guid? tenantLocationId,
    string? customName,
    decimal customFee,
    CancellationToken cancellationToken);

Crear modelo:

public sealed record ResolvedReservationLocation(
    Guid? TenantLocationId,
    string Name,
    decimal Fee);

Reglas:

Delivery:
- Si tenantLocationId null:
  - nombre personalizado requerido
  - fee personalizado >= 0
- Si tiene tenantLocationId:
  - buscar TenantLocation por tenant
  - validar activa/no eliminada
  - validar AllowsDelivery
  - usar DisplayName y DeliveryFee

Return:
- Si tenantLocationId null:
  - nombre personalizado requerido
  - fee personalizado >= 0
- Si tiene tenantLocationId:
  - validar AllowsPickup
  - usar DisplayName y PickupFee

Mensajes correctamente codificados en UTF-8:

- "La ubicación de entrega no está disponible."
- "La ubicación de entrega no permite entregas."
- "La ubicación de devolución no está disponible."
- "La ubicación de devolución no permite recogidas."
- "El nombre de la ubicación es requerido."

No dejar textos como:
- "ubicaciÃ³n"
- "vehÃ­culo"

====================================================================
9. ReservationVehicleResolver
====================================================================

Crear contrato:

IReservationVehicleResolver

Métodos sugeridos:

Task<Vehicle> GetReservableVehicleAsync(
    Guid tenantId,
    Guid vehicleId,
    CancellationToken cancellationToken);

VehicleRate GetRate(
    Vehicle vehicle,
    RentalType rentalType);

Reglas:

- Vehicle debe existir.
- Debe pertenecer al tenant.
- Debe estar activo.
- No eliminado.
- No Maintenance.
- No OutOfService.
- Debe tener VehicleRate activo para RentalType.

Mensajes:

- "Vehículo no encontrado."
- "El vehículo no está disponible para reservas."
- "El vehículo no tiene una tarifa configurada para este tipo de renta."

====================================================================
10. Customer validation
====================================================================

No duplicar validación del cliente en Create y Approve.

Crear servicio pequeño o reutilizar uno existente:

IReservationCustomerValidator

Método:

Task ValidateAsync(
    Guid tenantId,
    Guid customerId,
    CancellationToken cancellationToken);

Reglas:

- Debe existir.
- Debe pertenecer al tenant.
- Debe estar activo.
- No eliminado.

Mensaje:

"Cliente no encontrado."

Si ya existe un servicio equivalente, reutilizarlo.

====================================================================
11. Mapper separado
====================================================================

No dejar:

private static ReservationResponse ToResponse(...)

repetido en handlers.

Crear:

ReservationMapper

o extension:

ReservationMappings

Ejemplo:

public static ReservationResponse ToResponse(this Reservation reservation)

Ubicación sugerida:

Modules/Reservations/Mappers/ReservationMapper.cs

No mezclar mapeo con lógica de dominio o persistencia.

====================================================================
12. Commands separados
====================================================================

Verificar que cada command esté en su propio archivo:

CreateReservationCommand.cs
UpdateReservationCommand.cs
ApproveReservationCommand.cs
RejectReservationCommand.cs
CancelReservationCommand.cs
DeleteReservationCommand.cs

No colocar múltiples records/classes de commands en un mismo archivo.

Cada archivo debe tener:
- namespace correcto
- un solo command principal
- formato limpio

====================================================================
13. Validators separados
====================================================================

Cada validator debe estar en el folder del command correspondiente.

Ejemplo:

CreateReservationCommandValidator.cs

UpdateReservationCommandValidator.cs

ApproveReservationCommandValidator.cs

RejectReservationCommandValidator.cs

CancelReservationCommandValidator.cs

DeleteReservationCommandValidator.cs

No crear un archivo ReservationCommandValidators.cs con todos juntos.

====================================================================
14. Queries separadas
====================================================================

Revisar si Codex también agrupó queries.

Cada query debe tener su propia carpeta y archivos:

GetReservationsQuery
GetReservationsQueryHandler
GetReservationsQueryValidator
GetReservationsResponse

GetReservationByIdQuery
GetReservationByIdQueryHandler
GetReservationByIdResponse

CheckVehicleReservationAvailabilityQuery
CheckVehicleReservationAvailabilityQueryHandler
CheckVehicleReservationAvailabilityQueryValidator

No crear una clase ReservationQueryHandlers implementando múltiples IRequestHandler.

====================================================================
15. Formato y calidad de código
====================================================================

Aplicar formato profesional.

No permitir:

private readonly IReservationRepository _reservations; private readonly ICustomerRepository _customers;

Debe ser:

private readonly IReservationRepository _reservationRepository;
private readonly ICustomerRepository _customerRepository;

No permitir constructores en una sola línea.

No permitir múltiples statements en una sola línea:

await ValidateCustomerAsync(...); Vehicle vehicle = await ...

Separar:

await _customerValidator.ValidateAsync(...);

Vehicle vehicle = await _vehicleResolver.GetReservableVehicleAsync(...);

No usar nombres abreviados innecesarios:
- _reservations
- _customers
- _vehicles
- _locations
- _codes

Usar:
- _reservationRepository
- _customerRepository
- _vehicleRepository
- _tenantLocationRepository
- _reservationCodeGenerator

No usar `var` si el proyecto prefiere tipos explícitos.
Seguir AGENTS.md.

====================================================================
16. Corregir codificación de caracteres
====================================================================

Buscar en todo el módulo Reservations textos dañados:

- VehÃ­culo
- ubicaciÃ³n
- devoluciÃ³n
- estÃ¡
- cualquier secuencia Ã

Reemplazar por español correcto:

- Vehículo
- ubicación
- devolución
- está

Guardar archivos como UTF-8.

====================================================================
17. Registro de dependencias
====================================================================

Registrar servicios nuevos en DI siguiendo el patrón actual:

- IReservationLocationResolver
- IReservationVehicleResolver
- IReservationCustomerValidator

No registrar handlers manualmente si MediatR los descubre por assembly.

Verificar que no existan registros duplicados.

====================================================================
18. No cambiar comportamiento existente
====================================================================

Mantener:

- ResultReponse<T>
- ApiException
- Status codes actuales
- IUnitOfWork
- lógica de tarifas
- lógica de depósito
- lógica de locations
- validación de overlap
- multi-tenancy

No cambiar nombres públicos de endpoints.
No cambiar rutas.
No cambiar response DTO salvo error existente.

====================================================================
19. Pruebas requeridas
====================================================================

Agregar o ajustar unit tests por handler.

CreateReservationCommandHandler:
- crea reserva correctamente
- customer no existe
- vehicle no existe
- vehicle no disponible
- rate no existe
- delivery location inválida
- return location inválida

UpdateReservationCommandHandler:
- actualiza Pending
- no actualiza Approved
- recalcula tarifa y total

ApproveReservationCommandHandler:
- aprueba Pending
- falla si overlap
- falla si vehículo no disponible

RejectReservationCommandHandler:
- rechaza Pending

CancelReservationCommandHandler:
- cancela Pending o Approved

DeleteReservationCommandHandler:
- soft delete permitido
- falla en Approved/ConvertedToRental

Usar mocks/fakes según patrón actual.

====================================================================
20. Verificaciones finales
====================================================================

Al finalizar:

1. Eliminar ReservationCommandHandlers.
2. Confirmar que no existe ningún handler que implemente múltiples commands.
3. Confirmar que cada command tiene su carpeta.
4. Confirmar que cada query tiene su carpeta.
5. Confirmar que cada validator está separado.
6. Confirmar que no hay archivos gigantes con varios casos de uso.
7. Confirmar que no hay textos corruptos.
8. Ejecutar:
   dotnet format
9. Ejecutar:
   dotnet build
10. Ejecutar tests del módulo Reservations.

====================================================================
21. Resultado esperado
====================================================================

La estructura final debe ser legible, mantenible y coherente con CQRS:

- Un caso de uso por carpeta.
- Un command por archivo.
- Un handler por archivo.
- Un validator por archivo.
- Una query por archivo.
- Un query handler por archivo.
- Lógica compartida extraída a servicios pequeños.
- Mapeo separado.
- Sin clase ReservationCommandHandlers.
- Sin archivos comprimidos o ilegibles.
- Sin caracteres dañados.
- El proyecto compila y las pruebas pasan.