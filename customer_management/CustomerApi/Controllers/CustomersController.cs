using CustomerApi.Models;
using CustomerApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CustomerApi.Controllers;

// Require JWT authentication for all endpoints in this controller
[Authorize] // 🔐
[ApiController]
// Route pattern: api/customers
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly CustomerService _service;  // Service to handle customer-related logic

    // Constructor with dependency injection of CustomerService
    public CustomersController(CustomerService service)
    {
        _service = service;
    }

    // GET api/customers
    // Retrieve all customers (optionally user-specific)
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        // Get authenticated user's ID from JWT claims
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        Console.WriteLine($"Authenticated user ID: {userId}");

        // If customers are linked to users, use userId to filter customers:
        // var customers = await _service.GetAllByUserAsync(userId);

        // For now, get all customers regardless of user
        var customers = await _service.GetAllAsync();

        // Return 200 OK with the list of customers
        return Ok(customers);
    }

    // GET api/customers/{id}
    // Retrieve a specific customer by their GUID
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var customer = await _service.GetByIdAsync(id);

        // Return 404 if not found, otherwise 200 OK with the customer data
        return customer == null ? NotFound("Customer not found") : Ok(customer);
    }

    // GET api/customers/search?q=keyword
    // Search customers by a query string passed as a query parameter
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string q)
    {
        var results = await _service.SearchAsync(q);

        // Return 200 OK with search results (can be empty list)
        return Ok(results);
    }

    // POST api/customers
    // Create a new customer record
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Customer customer)
    {
        // Validate incoming model data
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Retrieve the current user's ID from JWT claims
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        Console.WriteLine($"Creating customer for user: {userId}");

        // Optionally associate the customer with the authenticated user:
        // customer.UserId = userId;

        // Attempt to create the customer in the service
        var result = await _service.CreateAsync(customer);

        // If creation is successful (result string starts with "Customer"), return 200 OK
        if (result.StartsWith("Customer")) 
            return Ok(result);

        // Otherwise, return 400 Bad Request with error message
        return BadRequest(result);
    }

    // PUT api/customers/{id}
    // Update an existing customer by their GUID
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Customer updated)
    {
        // Call service to update the customer
        var success = await _service.UpdateAsync(id, updated);

        // Return 200 OK if updated successfully, else 404 Not Found
        return success
            ? Ok("Customer info updated successfully")
            : NotFound("The customer was not found or the data is wrong.");
    }

    // DELETE api/customers/{id}
    // Delete a customer by their GUID
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        // Call service to delete the customer
        var deleted = await _service.DeleteAsync(id);

        // Return 200 OK if deleted, else 404 Not Found
        return deleted ? Ok("Customer deleted successfully") : NotFound("Customer not found");
    }
}
