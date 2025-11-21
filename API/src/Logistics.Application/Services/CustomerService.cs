using Logistics.Application.DTOs.Customer;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;

namespace Logistics.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CustomerService(ICustomerRepository customerRepository, ICompanyRepository companyRepository, IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CustomerResponse> CreateAsync(CustomerRequest request)
    {
        if (await _companyRepository.GetByIdAsync(request.CompanyId) == null)
            throw new KeyNotFoundException("Empresa não encontrada");
        if (await _customerRepository.DocumentExistsAsync(request.Document))
            throw new InvalidOperationException("Documento já existe");

        var customer = new Customer(request.CompanyId, request.Name, request.Document, request.Phone, request.Email);
        customer.Update(request.Name, request.Document, request.Phone, request.Email, request.Address);
        
        await _customerRepository.AddAsync(customer);
        await _unitOfWork.CommitAsync();
        return MapToResponse(customer);
    }

    public async Task<CustomerResponse> GetByIdAsync(Guid id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null) throw new KeyNotFoundException("Cliente não encontrado");
        return MapToResponse(customer);
    }

    public async Task<IEnumerable<CustomerResponse>> GetAllAsync()
    {
        var customers = await _customerRepository.GetAllAsync();
        return customers.Select(MapToResponse);
    }

    public async Task<IEnumerable<CustomerResponse>> GetByCompanyIdAsync(Guid companyId)
    {
        var customers = await _customerRepository.GetByCompanyIdAsync(companyId);
        return customers.Select(MapToResponse);
    }

    public async Task<CustomerResponse> UpdateAsync(Guid id, CustomerRequest request)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null) throw new KeyNotFoundException("Cliente não encontrado");
        if (await _customerRepository.DocumentExistsAsync(request.Document, id))
            throw new InvalidOperationException("Documento já existe");

        customer.Update(request.Name, request.Document, request.Phone, request.Email, request.Address);
        await _customerRepository.UpdateAsync(customer);
        await _unitOfWork.CommitAsync();
        return MapToResponse(customer);
    }

    public async Task DeleteAsync(Guid id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null) throw new KeyNotFoundException("Cliente não encontrado");
        await _customerRepository.DeleteAsync(id);
        await _unitOfWork.CommitAsync();
    }

    private static CustomerResponse MapToResponse(Customer c) => new CustomerResponse
    {
        Id = c.Id,
        CompanyId = c.CompanyId,
        Name = c.Name,
        Document = c.Document,
        Phone = c.Phone,
        Email = c.Email,
        Address = c.Address,
        IsActive = c.IsActive,
        CreatedAt = c.CreatedAt
    };
}
