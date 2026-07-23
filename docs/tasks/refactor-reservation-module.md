Audita y corrige completamente la estructura del módulo Reservations tomando AGENTS.md como fuente obligatoria de verdad.

Problema:
El módulo Reservations fue generado sin respetar completamente las reglas arquitectónicas y de estilo del proyecto. Aunque algunos Commands, Queries y Handlers fueron separados, todavía pueden existir:

- Archivos en carpetas incorrectas.
- Commands y Queries fuera de la estructura exigida.
- Handlers con namespaces genéricos.
- Múltiples tipos principales en un mismo archivo.
- Validators faltantes.
- Responses o DTOs colocados en carpetas incorrectas.
- Servicios compartidos innecesarios o demasiado grandes.
- Código minificado o mal formateado.
- Dependencias innecesarias en handlers.
- Mapeos dentro de handlers.
- Reglas de negocio duplicadas.
- Implementaciones incompletas respecto a AGENTS.md.
- Clases, contratos o archivos antiguos del módulo Reservations.
- Nombres inconsistentes.
- Textos con errores de codificación.
- Tests faltantes.

No asumas la estructura.
Primero debes leer AGENTS.md completo y comparar cada regla contra el estado real del módulo Reservations.

No modificar frontend.
No modificar endpoints públicos salvo que exista una violación clara de AGENTS.md.
No cambiar el comportamiento funcional del módulo salvo errores reales.
No modificar módulos ajenos innecesariamente.

====================================================================
1. Leer AGENTS.md obligatoriamente
====================================================================

Antes de modificar cualquier archivo:

1. Localizar y leer AGENTS.md completo.
2. Revisar si existen AGENTS.md adicionales en subcarpetas.
3. Determinar qué instrucciones aplican a:
   - Core.Domain
   - Core.Application
   - Infrastructure
   - Presentation
   - Tests
4. Crear internamente una lista de verificación basada en esas reglas.
5. Auditar Reservations contra esa lista.

No empezar a refactorizar antes de leer AGENTS.md.

En el resultado final, resumir:
- Qué reglas de AGENTS.md no se estaban cumpliendo.
- Qué archivos fueron movidos, creados, eliminados o corregidos.

====================================================================
2. Auditar todo el módulo Reservations
====================================================================

Revisar todo archivo relacionado con Reservations en:

- Core.Domain
- Core.Application
- Infrastructure
- Presentation
- Tests
- Persistence/Migrations si aplica

Buscar por:

Reservation
Reservations
ReservationStatus
ReservationChannel
IReservationRepository
ReservationRepository
ReservationConfiguration
ReservationEndpoints
ReservationResponse
ReservationMapper
ReservationCodeGenerator
ReservationLocationResolver
ReservationVehicleResolver
ReservationCustomerValidator

También buscar archivos antiguos:

- ReservationCommandHandlers
- ReservationQueryHandlers
- ReservationVehicle
- ReservationPayment
- ReservationVehicles
- ReservationPayments
- Commands.cs
- Queries.cs
- Handlers.cs
- Validators.cs

No limitar la revisión a la carpeta Application.

====================================================================
3. Estructura esperada por caso de uso
====================================================================

La estructura debe seguir AGENTS.md.

Si AGENTS.md exige una estructura por feature/use case, organizar aproximadamente así:

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
        CheckVehicleReservationAvailabilityResponse.cs

    Contracts/
      Repositories/
        IReservationRepository.cs
      Services/
        IReservationCodeGenerator.cs
        IReservationLocationResolver.cs
        IReservationVehicleResolver.cs
        IReservationCustomerValidator.cs

    Services/
      ReservationCodeGenerator.cs
      ReservationLocationResolver.cs
      ReservationVehicleResolver.cs
      ReservationCustomerValidator.cs

    Mappers/
      ReservationMapper.cs

    Dtos/
      Solo DTOs realmente compartidos por varios casos de uso.

La estructura exacta debe adaptarse a AGENTS.md y al patrón real de Customers/Vehicles.

No mantener todos los Commands directamente en:

Reservations/Commands

si AGENTS.md exige una carpeta por caso de uso.

====================================================================
4. Un tipo principal por archivo
====================================================================

Verificar que cada archivo tenga un único tipo principal.

No permitir archivos que contengan juntos:

- varios Commands
- varios Queries
- varios Handlers
- varios Validators
- varios Responses
- varias interfaces no relacionadas

Cada Command debe estar en su propio archivo.

Cada Query debe estar en su propio archivo.

Cada Handler debe estar en su propio archivo.

Cada Validator debe estar en su propio archivo.

Cada Response principal debe estar en su propio archivo si así lo exige AGENTS.md.

Eliminar cualquier clase agregadora como:

- ReservationCommandHandlers
- ReservationQueryHandlers

Ningún handler debe implementar más de un IRequestHandler.

====================================================================
5. Namespaces correctos
====================================================================

Los namespaces deben reflejar las carpetas.

Ejemplo:

namespace Rentify.Backend.Core.Application.Modules.Reservations.Commands.ApproveReservation;

No dejar todos los casos de uso con:

namespace Rentify.Backend.Core.Application.Modules.Reservations.Commands;

si AGENTS.md establece namespaces por carpeta.

Aplicar lo mismo a Queries, Validators, Services y Responses.

Actualizar todos los using y referencias después de mover archivos.

====================================================================
6. Commands
====================================================================

Auditar estos Commands:

- CreateReservationCommand
- UpdateReservationCommand
- ApproveReservationCommand
- RejectReservationCommand
- CancelReservationCommand
- DeleteReservationCommand

Verificar para cada uno:

- Archivo propio.
- Carpeta propia.
- Namespace correcto.
- Solo contiene datos de entrada.
- No contiene lógica de negocio.
- No contiene acceso a repositorios.
- No recibe datos calculados por backend como UnitRate, RentalAmount o depósito.
- TenantId y usuario siguen el patrón actual del proyecto.
- Implementa IRequest con el response correcto.
- Nombres coherentes.

No mezclar request HTTP con Command si AGENTS.md los separa.

====================================================================
7. Handlers
====================================================================

Auditar cada handler individualmente.

Cada handler debe:

- Implementar un solo IRequestHandler.
- Inyectar únicamente dependencias que realmente utiliza.
- Tener formato estándar C#.
- Tener pasos claros.
- Delegar reglas de dominio a Reservation.
- No contener mapeos manuales enormes.
- No contener queries complejas inline si corresponden al repository.
- No repetir helpers privados en múltiples handlers.
- No convertirse en un “god handler”.

Revisar especialmente:

CreateReservationCommandHandler:
- Customer validator.
- Vehicle resolver.
- Location resolver.
- Code generator.
- Repository.
- UnitOfWork.

UpdateReservationCommandHandler:
- No debe depender de generador de código.
- Debe resolver tarifa y ubicaciones.
- Solo actualiza Pending.

ApproveReservationCommandHandler:
- Solo dependencias necesarias.
- Valida customer, vehicle y overlap.
- No depende de locations ni code generator.

RejectReservationCommandHandler:
- Repository + UnitOfWork, salvo servicio compartido necesario.

CancelReservationCommandHandler:
- Repository + UnitOfWork.

DeleteReservationCommandHandler:
- Repository + UnitOfWork.

No duplicar `GetReservationOrThrowAsync` en todos si AGENTS.md permite un servicio/repository helper. Pero tampoco crear un servicio gigante para todo.

====================================================================
8. Queries
====================================================================

Auditar:

- GetReservationsQuery
- GetReservationByIdQuery
- CheckVehicleReservationAvailabilityQuery

Cada Query debe tener:

- Archivo propio.
- Handler propio.
- Validator propio cuando reciba filtros/parámetros validables.
- Response propio.
- Namespace correcto.
- Proyección eficiente.
- AsNoTracking para lectura si el patrón lo usa.
- Filtro obligatorio por TenantId.
- Exclusión de IsDeleted.
- Paginación usando el modelo estándar del proyecto.

No cargar entidades completas y mapear en memoria si puede proyectarse desde EF.

No usar repositorios de comandos para consultas si el proyecto separa read/write.

====================================================================
9. Validators
====================================================================

Confirmar que existan y estén registrados:

- CreateReservationCommandValidator
- UpdateReservationCommandValidator
- ApproveReservationCommandValidator
- RejectReservationCommandValidator
- CancelReservationCommandValidator
- DeleteReservationCommandValidator
- GetReservationsQueryValidator
- GetReservationByIdQueryValidator, si el patrón lo requiere
- CheckVehicleReservationAvailabilityQueryValidator

Verificar:

- No duplicar reglas de dominio innecesariamente.
- FluentValidation para formato y entrada.
- Dominio para invariantes.
- Mensajes en español correcto.
- Longitudes alineadas con EF Configuration.
- Fechas coherentes.
- Enums válidos.
- Guids no vacíos.

No poner todos los validators en un solo archivo.

====================================================================
10. Responses y DTOs
====================================================================

Auditar ReservationResponse y otros DTOs.

Determinar según AGENTS.md si debe existir:

- Un response compartido.
- Responses específicos por query.
- List item response.
- Detail response.
- Availability response.

No usar un response mínimo de:

Id, Code, Status, TotalAmount

para todos los endpoints si cada caso requiere información distinta.

Separar al menos:

- CreateReservationResponse o ReservationResponse básico.
- ReservationListItemResponse.
- ReservationDetailResponse.
- VehicleReservationAvailabilityResponse.

No exponer:
- TenantId innecesariamente.
- Datos internos.
- Campos sensibles del cliente que no correspondan.

====================================================================
11. Mappers
====================================================================

Revisar ReservationMapper.

Debe:

- Estar en archivo propio.
- Tener métodos claros.
- No consultar base de datos.
- No contener reglas de negocio.
- No depender de servicios.
- Mapear solo datos ya disponibles.

Si los Queries proyectan directamente, no obligarlos a usar mapper en memoria.

No repetir `ToResponse()` en varios handlers.

====================================================================
12. Servicios compartidos
====================================================================

Auditar:

- ReservationLocationResolver
- ReservationVehicleResolver
- ReservationCustomerValidator
- ReservationCodeGenerator

Verificar que:

- Cada servicio tenga una responsabilidad.
- Las interfaces estén en Contracts/Services.
- Las implementaciones estén en Services.
- No exista un ReservationApplicationService gigante.
- No dupliquen comportamiento de repositorios existentes.
- No contengan SaveChanges.
- No muten Reservation.
- No dependan de Presentation.
- Se registren una sola vez en DI.

ReservationLocationResolver:
- Resuelve delivery y return.
- Valida TenantLocation.
- Usa fee del backend si LocationId existe.
- Soporta ubicación personalizada.
- No acepta manipulación de fee para ubicaciones configuradas.

ReservationVehicleResolver:
- Obtiene vehículo.
- Valida estado.
- Obtiene tarifa activa.

ReservationCustomerValidator:
- Valida existencia, tenant, active y no deleted.

ReservationCodeGenerator:
- Genera código único por tenant.
- No depende del frontend.

====================================================================
13. Domain
====================================================================

Auditar Reservation en Core.Domain.

Confirmar que:

- No conserva modelo legado.
- No contiene ReservationVehicle.
- No contiene ReservationPayment.
- No contiene colecciones antiguas.
- Estado inicial es Pending.
- No tiene InProgress ni Completed si esos estados pertenecen a Rental.
- Tiene métodos:
  - Create
  - UpdatePendingReservation
  - Approve
  - Reject
  - Cancel
  - MarkAsConvertedToRental
  - Delete
- Sus transiciones de estado son válidas.
- Recalcula totales internamente.
- Normaliza strings.
- Valida importes.
- No accede a repositorios.
- No conoce MediatR, EF, HTTP ni Application.

Revisar ReservationStatus y ReservationChannel.

Eliminar código comentado y restos del modelo anterior.

====================================================================
14. Infrastructure
====================================================================

Auditar:

- ReservationConfiguration
- ReservationRepository
- Configuración de DbSet
- DI

ReservationConfiguration debe coincidir con Domain:

- Precisiones numeric(18,2).
- Longitudes.
- Nullable.
- Índices.
- FK Customer Restrict.
- FK Vehicle Restrict.
- Enums según convención.
- Sin relación a ReservationVehicle/ReservationPayment.

ReservationRepository debe:

- Filtrar por TenantId.
- Excluir IsDeleted.
- Usar tracking solo cuando corresponde.
- Implementar overlap correctamente:
  newStart < existingEnd && newEnd > existingStart
- Considerar solo Approved.
- Excluir la reserva actual cuando aplique.
- No exponer IQueryable fuera si AGENTS.md lo prohíbe.

Eliminar configuraciones y DbSets antiguos de:

- ReservationVehicle
- ReservationPayment

No eliminar Payment ni Invoice.

====================================================================
15. Presentation
====================================================================

Auditar ReservationEndpoints.

Debe:

- Estar en Presentation.
- No contener lógica de negocio.
- Obtener TenantId/UserId desde current context.
- Construir Command/Query.
- Enviar con ISender.
- Retornar respuesta estándar.
- Tener rutas coherentes.
- Tener tags.
- Tener autorización.
- No recibir TenantId del cliente.
- No recibir UnitRate, RentalAmount, SecurityDepositAmount ni TotalAmount del frontend.

Endpoints esperados:

GET    /api/v1/reservations
GET    /api/v1/reservations/{reservationId}
POST   /api/v1/reservations
PUT    /api/v1/reservations/{reservationId}
PUT    /api/v1/reservations/{reservationId}/approve
PUT    /api/v1/reservations/{reservationId}/reject
PUT    /api/v1/reservations/{reservationId}/cancel
DELETE /api/v1/reservations/{reservationId}
GET    /api/v1/reservations/availability

Revisar orden de rutas para evitar que `/{reservationId}` capture `/availability`.

Mapear `/availability` antes de `/{reservationId:guid}` o usar constraint guid.

====================================================================
16. Seguridad y multi-tenancy
====================================================================

Auditar cada camino.

Confirmar:

- TenantId nunca se toma del body.
- TenantId viene del contexto autenticado.
- Toda consulta filtra TenantId.
- Un tenant no puede obtener/modificar reservas de otro.
- Customer pertenece al tenant.
- Vehicle pertenece al tenant.
- TenantLocation pertenece al tenant.
- IDs de otro tenant producen 404 o mensaje estándar del proyecto.
- No hay fugas de información en availability.

====================================================================
17. Limpieza de archivos antiguos
====================================================================

Buscar y eliminar, si ya no se usan:

- ReservationCommandHandlers.cs
- ReservationQueryHandlers.cs
- Commands agrupados.
- Queries agrupadas.
- Validators agrupados.
- ReservationVehicle.
- ReservationPayment.
- Configuraciones antiguas.
- DTOs duplicados.
- Mappers duplicados.
- Helpers sin referencias.
- Archivos vacíos.
- Código comentado.
- Namespaces antiguos.

No dejar dos implementaciones del mismo handler.

No dejar clases huérfanas compilando solo por accidente.

====================================================================
18. Formato
====================================================================

Todos los archivos deben cumplir formato C# profesional.

No aceptar:

- Clases completas en una línea.
- Varios statements por línea.
- Campos privados en una sola línea.
- Constructores comprimidos.
- `if (...) throw ...` en líneas enormes.
- Usings desordenados.
- Caracteres dañados como:
  - VehÃ­culo
  - ubicaciÃ³n
  - estÃ¡

Guardar archivos en UTF-8.

Ejecutar:

dotnet format

Después revisar el diff para asegurar que el formateador sí modificó los archivos del módulo.

====================================================================
19. Pruebas
====================================================================

Revisar AGENTS.md para la estructura exigida de tests.

Crear o corregir tests para:

Domain:
- Create Pending.
- Cálculo de cantidad.
- Cálculo de total.
- Approve.
- Reject.
- Cancel.
- Update solo Pending.
- Delete permitido/no permitido.
- ConvertedToRental solo Approved.

Handlers:
- Create.
- Update.
- Approve con y sin overlap.
- Reject.
- Cancel.
- Delete.

Queries:
- listado filtra tenant.
- detalle filtra tenant.
- availability detecta overlap.

Resolvers:
- Location global/configurada.
- Location personalizada.
- Delivery no permitido.
- Pickup no permitido.
- Vehicle sin tarifa.
- Vehicle no disponible.

No crear tests inútiles que solo comprueben constructores.

====================================================================
20. Validación final obligatoria
====================================================================

Antes de terminar:

1. Mostrar el árbol final del módulo Reservations.
2. Confirmar que se siguió AGENTS.md.
3. Ejecutar dotnet format.
4. Ejecutar dotnet build.
5. Ejecutar tests relacionados.
6. Buscar clases que implementen más de un IRequestHandler.
7. Buscar archivos del módulo con múltiples Commands/Queries/Validators.
8. Buscar código minificado.
9. Buscar caracteres `Ã`.
10. Buscar referencias a ReservationVehicle y ReservationPayment.
11. Verificar registros DI.
12. Verificar endpoints.
13. Verificar namespaces.
14. Verificar que no se tocaron módulos ajenos innecesariamente.

No declarar la tarea terminada si build o tests fallan.

====================================================================
21. Entregable final de Codex
====================================================================

Al finalizar, responde con:

1. Reglas relevantes encontradas en AGENTS.md.
2. Problemas detectados en Reservations.
3. Archivos movidos.
4. Archivos creados.
5. Archivos eliminados.
6. Servicios extraídos o eliminados.
7. Estado de build.
8. Estado de tests.
9. Árbol final de carpetas.
10. Cualquier deuda técnica que quede, explicando por qué no se resolvió.

No respondas únicamente “hecho”.
Necesito una auditoría verificable y una estructura final consistente.