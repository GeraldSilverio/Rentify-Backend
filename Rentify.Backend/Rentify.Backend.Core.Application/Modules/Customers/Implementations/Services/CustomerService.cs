using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Customers.Commands.CreateCustomer;
using Rentify.Backend.Core.Application.Modules.Customers.Commands.DeleteCustomer;
using Rentify.Backend.Core.Application.Modules.Customers.Commands.UpdateCustomer;
using Rentify.Backend.Core.Application.Modules.Customers.Commands.UploadCustomerDocument;
using Rentify.Backend.Core.Application.Modules.Customers.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Customers.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Shared.Exceptions;
using Rentify.Backend.Core.Application.Modules.Shared.Storage;
using Rentify.Backend.Core.Application.Modules.Shared.UnitOfWork;
using Rentify.Backend.Core.Domain.Entities.Customers;
using Rentify.Backend.Core.Application.Modules.Customers.Commands.VerifyCustomer;
using Rentify.Backend.Core.Application.Modules.Customers.Commands.UnverifyCustomer;
using Rentify.Backend.Core.Application.Modules.Customers.Commands.DeleteCustomerDocument;
using Rentify.Backend.Core.Application.Modules.Customers.Dtos;
using Microsoft.Extensions.Logging;

namespace Rentify.Backend.Core.Application.Modules.Customers.Implementations.Services;

public sealed class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(
        ICustomerRepository customerRepository,
        IFileStorageService fileStorageService,
        IUnitOfWork unitOfWork,
        ILogger<CustomerService> logger)
    {
        _customerRepository = customerRepository;
        _fileStorageService = fileStorageService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Guid> CreateAsync(CreateCustomerCommand command, CancellationToken cancellationToken = default)
    {
        string identificationNumberNormalized = Customer.NormalizeIdentificationNumber(command.IdentificationNumber);

        if (await _customerRepository.IdentificationExistsAsync(
                command.TenantId,
                command.IdentificationType,
                identificationNumberNormalized,
                null,
                cancellationToken))
        {
            throw new ApiException("Ya existe un cliente con esta identificación.", StatusCodes.Status400BadRequest);
        }

        if (await _customerRepository.EmailExistsAsync(
                command.TenantId,
                command.Email,
                null,
                cancellationToken))
        {
            throw new ApiException("Ya existe un cliente con este correo electrónico.", StatusCodes.Status400BadRequest);
        }

        Customer customer = Customer.Create(
            command.TenantId,
            command.CustomerType,
            command.IdentificationType,
            command.IdentificationNumber,
            command.BirthDate,
            command.FirstName,
            command.LastName,
            command.Email,
            command.PhoneNumber,
            command.AddressLine,
            command.Sector,
            command.City,
            command.Province,
            command.AddressReference,
            command.CreatedBy);

        await _customerRepository.AddAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return customer.Id;
    }

    public async Task<Guid> UpdateAsync(UpdateCustomerCommand command, CancellationToken cancellationToken = default)
    {
        Customer customer = await GetCustomerOrThrowAsync(command.TenantId, command.CustomerId, cancellationToken);
        string identificationNumberNormalized = Customer.NormalizeIdentificationNumber(command.IdentificationNumber);
        if (await _customerRepository.IdentificationExistsAsync(
                command.TenantId,
                command.IdentificationType,
                identificationNumberNormalized,
                command.CustomerId,
                cancellationToken))
        {
            throw new ApiException("Ya existe un cliente con esta identificación.", StatusCodes.Status400BadRequest);
        }

        if (await _customerRepository.EmailExistsAsync(
                command.TenantId,
                command.Email,
                command.CustomerId,
                cancellationToken))
        {
            throw new ApiException("Ya existe un cliente con este correo electrónico.", StatusCodes.Status400BadRequest);
        }

        customer.Update(
            command.CustomerType,
            command.IdentificationType,
            command.IdentificationNumber,
            command.BirthDate,
            command.FirstName,
            command.LastName,
            command.Email,
            command.PhoneNumber,
            command.AddressLine,
            command.Sector,
            command.City,
            command.Province,
            command.AddressReference,
            command.ModifiedBy);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return customer.Id;
    }

    public async Task DeleteAsync(DeleteCustomerCommand command, CancellationToken cancellationToken = default)
    {
        Customer customer = await GetCustomerOrThrowAsync(command.TenantId, command.CustomerId, cancellationToken);
        customer.Delete(command.ModifiedBy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Guid> UploadDocumentAsync(UploadCustomerDocumentCommand command, CancellationToken cancellationToken = default)
    {
        Customer customer = await GetCustomerOrThrowAsync(command.TenantId, command.CustomerId, cancellationToken);
        StoredFileResult storedFile = await _fileStorageService.UploadAsync(
            command.Document,
            "customer-documents",
            cancellationToken);

        try
        {
            CustomerDocument document = customer.AddDocument(
                command.Document.FileName,
                storedFile.Url,
                storedFile.PublicId,
                command.DocumentType,
                command.CreatedBy,
                command.DocumentSide);

            await _customerRepository.AddDocumentAsync(document, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return document.Id;
        }
        catch (Exception exception)
        {
            try
            {
                await _fileStorageService.DeleteAsync(storedFile.PublicId, CancellationToken.None);
            }
            catch (Exception deleteException)
            {
                exception.Data["CustomerDocumentStorageCleanupFailed"] = deleteException.Message;
            }

            throw;
        }
    }

    public async Task<VerifyCustomerResponse> VerifyAsync(
        VerifyCustomerCommand command,
        CancellationToken cancellationToken = default)
    {
        Customer customer = await GetCustomerOrThrowAsync(command.TenantId, command.CustomerId, cancellationToken);
        customer.MarkAsVerified(command.VerifiedBy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new VerifyCustomerResponse(customer.Id, customer.IsVerified, customer.VerifiedAt, customer.VerifiedBy);
    }

    public async Task<UnverifyCustomerResponse> UnverifyAsync(
        UnverifyCustomerCommand command,
        CancellationToken cancellationToken = default)
    {
        Customer customer = await GetCustomerOrThrowAsync(command.TenantId, command.CustomerId, cancellationToken);
        customer.MarkAsUnverified(command.ModifiedBy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new UnverifyCustomerResponse(customer.Id, customer.IsVerified, customer.VerifiedAt, customer.VerifiedBy);
    }

    public async Task DeleteDocumentAsync(
        DeleteCustomerDocumentCommand command,
        CancellationToken cancellationToken = default)
    {
        CustomerDocument document = await _customerRepository.GetDocumentByIdAsync(
            command.TenantId,
            command.CustomerId,
            command.DocumentId,
            cancellationToken)
            ?? throw new ApiException("Customer document not found.", StatusCodes.Status404NotFound);

        string publicId = document.PublicId;
        document.Delete(command.ModifiedBy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        try
        {
            await _fileStorageService.DeleteAsync(publicId, cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogWarning(
                exception,
                "Customer document was soft-deleted but remote storage cleanup failed for document {DocumentId}.",
                command.DocumentId);
        }
    }

    private async Task<Customer> GetCustomerOrThrowAsync(Guid tenantId, Guid customerId, CancellationToken cancellationToken)
    {
        return await _customerRepository.GetByIdAsync(tenantId, customerId, cancellationToken)
               ?? throw new ApiException("Customer not found.", StatusCodes.Status404NotFound);
    }
}
