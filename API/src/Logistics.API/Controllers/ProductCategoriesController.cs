using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/product-categories")]
[Authorize]
public class ProductCategoriesController : ControllerBase
{
    private readonly IProductCategoryRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ProductCategoriesController(IProductCategoryRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var categories = await _repository.GetAllAsync();
        return Ok(categories);
    }

    [HttpGet("active")]
    public async Task<ActionResult> GetActive()
    {
        var categories = await _repository.GetActiveAsync();
        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(Guid id)
    {
        var category = await _repository.GetByIdAsync(id);
        if (category == null)
            return NotFound();
        return Ok(category);
    }

    [HttpGet("by-code/{code}")]
    public async Task<ActionResult> GetByCode(string code)
    {
        var category = await _repository.GetByCodeAsync(code);
        if (category == null)
            return NotFound();
        return Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateProductCategoryRequest request)
    {
        if (await _repository.GetByCodeAsync(request.Code) != null)
            return BadRequest("Código de categoria já existe");

        var category = new ProductCategory(request.Name, request.Code, request.Description);

        if (!string.IsNullOrWhiteSpace(request.Barcode))
            category.SetBarcode(request.Barcode);

        if (!string.IsNullOrWhiteSpace(request.Reference))
            category.SetReference(request.Reference);

        if (request.IsMaintenance.HasValue)
            category.SetMaintenance(request.IsMaintenance.Value);

        if (!string.IsNullOrWhiteSpace(request.Attributes))
            category.SetAttributes(request.Attributes);

        await _repository.AddAsync(category);
        await _unitOfWork.CommitAsync();

        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateProductCategoryRequest request)
    {
        var category = await _repository.GetByIdAsync(id);
        if (category == null)
            return NotFound();

        var existingWithCode = await _repository.GetByCodeAsync(request.Code);
        if (existingWithCode != null && existingWithCode.Id != id)
            return BadRequest("Código de categoria já existe em outra categoria");

        category.Update(request.Name, request.Code, request.Description);

        if (!string.IsNullOrWhiteSpace(request.Barcode))
            category.SetBarcode(request.Barcode);

        if (!string.IsNullOrWhiteSpace(request.Reference))
            category.SetReference(request.Reference);

        if (request.IsMaintenance.HasValue)
            category.SetMaintenance(request.IsMaintenance.Value);

        if (!string.IsNullOrWhiteSpace(request.Attributes))
            category.SetAttributes(request.Attributes);

        await _repository.UpdateAsync(category);
        await _unitOfWork.CommitAsync();

        return Ok(category);
    }

    [HttpPost("{id}/activate")]
    public async Task<ActionResult> Activate(Guid id)
    {
        var category = await _repository.GetByIdAsync(id);
        if (category == null)
            return NotFound();

        category.SetActive(true);

        await _repository.UpdateAsync(category);
        await _unitOfWork.CommitAsync();

        return Ok(category);
    }

    [HttpPost("{id}/deactivate")]
    public async Task<ActionResult> Deactivate(Guid id)
    {
        var category = await _repository.GetByIdAsync(id);
        if (category == null)
            return NotFound();

        category.SetActive(false);

        await _repository.UpdateAsync(category);
        await _unitOfWork.CommitAsync();

        return Ok(category);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var category = await _repository.GetByIdAsync(id);
        if (category == null)
            return NotFound();

        if (category.Products.Any())
            return BadRequest("Não é possível excluir categoria com produtos vinculados");

        await _repository.DeleteAsync(id);
        await _unitOfWork.CommitAsync();

        return NoContent();
    }
}

public record CreateProductCategoryRequest(
    string Name,
    string Code,
    string? Description,
    string? Barcode,
    string? Reference,
    bool? IsMaintenance,
    string? Attributes
);

public record UpdateProductCategoryRequest(
    string Name,
    string Code,
    string? Description,
    string? Barcode,
    string? Reference,
    bool? IsMaintenance,
    string? Attributes
);
