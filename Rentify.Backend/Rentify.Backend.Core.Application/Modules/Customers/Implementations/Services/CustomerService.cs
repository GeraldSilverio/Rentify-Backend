using Microsoft.AspNetCore.Http;
using Rentify.Backend.Core.Application.Modules.Customers.Commands.CreateCustomer;
using Rentify.Backend.Core.Application.Modules.Customers.Commands.DeleteCustomer;
using Rentify.Backend.Core.Application.Modules.Customers.Commands.UpdateCustomer;
using Rentify.Backend.Core.Application.Modules.Customers.Commands.UploadCustomerDocument;
using Rentify.Backend.Core.Application.Modules.Customers.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Customers.Contracts.Services;
using Rentify.Backend.Core.Application.Shared.Exceptions;
using Rentify.Backend.Core.Application.Shared.Storage;
using Rentify.Backend.Core.Application.Shared.UnitOfWork;
using Rentify.Backend.Core.Domain.Entities;

namespace Rentify.Backend.Core.Application.Modules.Customers.Implementations.Services;

public sealed class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IUnitOfWork _unitOfWork;

    public CustomerService(
        ICustomerRepository customerRepository,
        IFileStorageService fileStorageService,
        IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _fileStorageService = fileStorageService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> CreateAsync(CreateCustomerCommand command, CancellationToken cancellationToken = default)
    {
        if (await _customerRepository.LicenseNumberExistsAsync(command.TenantId, command.LicenseNumber, cancellationToken: cancellationToken))
            throw new ApiException("Customer license number already exists for this tenant.", StatusCodes.Status400BadRequest);

        Customer customer = Customer.Create(
            command.TenantId,
            command.FirstName,
            command.LastName,
            command.Email,
            command.PhoneNumber,
            command.LicenseNumber,
            command.LicenseExpirationDate,
            command.CreatedBy);

        await _customerRepository.AddAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return customer.Id;
    }

    public async Task<Guid> UpdateAsync(UpdateCustomerCommand command, CancellationToken cancellationToken = default)
    {
        Customer customer = await GetCustomerOrThrowAsync(command.TenantId, command.CustomerId, cancellationToken);

        if (await _customerRepository.LicenseNumberExistsAsync(command.TenantId, command.LicenseNumber, customer.Id, cancellationToken))
            throw new ApiException("Customer license number already exists for this tenant.", StatusCodes.Status400BadRequest);

        customer.Update(
            command.FirstName,
            command.LastName,
            command.Email,
            command.PhoneNumber,
            command.LicenseNumber,
            command.LicenseExpirationDate,
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
                command.CreatedBy);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return document.Id;
        }
        catch
        {
            await _fileStorageService.DeleteAsync(storedFile.PublicId, CancellationToken.None);
            throw;
        }
    }

    private async Task<Customer> GetCustomerOrThrowAsync(Guid tenantId, Guid customerId, CancellationToken cancellationToken)
    {
        return await _customerRepository.GetByIdAsync(tenantId, customerId, cancellationToken)
               ?? throw new ApiException("Customer not found.", StatusCodes.Status404NotFound);
    }
}
