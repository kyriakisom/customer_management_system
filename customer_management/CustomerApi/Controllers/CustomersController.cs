using CustomerApi.Models;
using CustomerApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CustomerApi.Controllers;

[Authorize] // 🔐 Require JWT auth for all routes in this controller
[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly CustomerService _service;

    public CustomersController(CustomerService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        Console.WriteLine($"Authenticated user ID: {userId}");

        // If customers are user-specific, use:
        // var customers = await _service.GetAllByUserAsync(userId);
        var customers = await _service.GetAllAsync();
        return Ok(customers);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var customer = await _service.GetByIdAsync(id);
        return customer == null ? NotFound("Customer not found") : Ok(customer);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string q)
    {
        var results = await _service.SearchAsync(q);
        return Ok(results);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Customer customer)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        Console.WriteLine($"Creating customer for user: {userId}");

        // You could associate customer.UserId = userId here if needed

        var result = await _service.CreateAsync(customer);
        if (result.StartsWith("Customer")) return Ok(result);
        return BadRequest(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Customer updated)
    {
        var success = await _service.UpdateAsync(id, updated);
        return success
            ? Ok("Customer info updated successfully")
            : NotFound("The customer was not found or the data is wrong.");
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _service.DeleteAsync(id);
        return deleted ? Ok("Customer deleted successfully") : NotFound("Customer not found");
    }
}
