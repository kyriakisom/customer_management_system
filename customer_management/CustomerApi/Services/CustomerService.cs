using CustomerApi.Data;
using CustomerApi.Models;
using MongoDB.Driver;

namespace CustomerApi.Services;

// Service class to handle business logic and database operations for Customers
public class CustomerService
{
    private readonly IMongoCollection<Customer> _customers; // MongoDB collection for customers

    // Constructor injects the MongoDbContext and gets the Customers collection
    public CustomerService(MongoDbContext context)
    {
        _customers = context.Customers;
    }

    // Retrieve all customers from the database
    public async Task<List<Customer>> GetAllAsync() =>
        await _customers.Find(_ => true).ToListAsync();

    // Retrieve a single customer by their GUID Id
    public async Task<Customer?> GetByIdAsync(Guid id) =>
        await _customers.Find(x => x.Id == id).FirstOrDefaultAsync();

    // Retrieve a single customer by their unique IdentityNumber
    public async Task<Customer?> GetByIdentityAsync(string idNumber) =>
        await _customers.Find(x => x.IdentityNumber == idNumber).FirstOrDefaultAsync();

    // Search customers by matching query against FirstName, LastName, or IdentityNumber (case-insensitive)
    public async Task<List<Customer>> SearchAsync(string query)
    {
        // Build a filter combining regex matches on multiple fields with case-insensitive option ("i")
        var filter = Builders<Customer>.Filter.Or(
            Builders<Customer>.Filter.Regex("FirstName", new MongoDB.Bson.BsonRegularExpression(query, "i")),
            Builders<Customer>.Filter.Regex("LastName", new MongoDB.Bson.BsonRegularExpression(query, "i")),
            Builders<Customer>.Filter.Regex("IdentityNumber", new MongoDB.Bson.BsonRegularExpression(query, "i"))
        );

        // Execute the filtered query and get matching customers
        var customers = await _customers.Find(filter).ToListAsync();

        // Sort results by LastName, then FirstName, then IdentityNumber
        return customers.OrderBy(c => c.LastName)
                        .ThenBy(c => c.FirstName)
                        .ThenBy(c => c.IdentityNumber)
                        .ToList();
    }

    // Create a new customer if valid and doesn't exist yet
    public async Task<string> CreateAsync(Customer customer)
    {
        // Check if a customer with the same IdentityNumber already exists
        if (await GetByIdentityAsync(customer.IdentityNumber) is not null)
            return "There is already a customer with this AT."; // AT presumably means "IdentityNumber"

        // Check if customer is at least 16 years old
        if ((DateTime.UtcNow - customer.BirthDate).TotalDays / 365 < 16)
            return "The customer must be at least 16 years old.";

        // Insert the new customer document into MongoDB
        await _customers.InsertOneAsync(customer);
        return "The client was successfully added.";
    }

    // Update an existing customer by Id with new data
    public async Task<bool> UpdateAsync(Guid id, Customer update)
    {
        // Check if customer exists before updating
        var existing = await GetByIdAsync(id);
        if (existing is null) return false;

        // Preserve original Id, CreatedAt, and BirthDate (immutable fields)
        update.Id = existing.Id;
        update.CreatedAt = existing.CreatedAt;
        update.BirthDate = existing.BirthDate;

        // Replace the customer document with the updated one
        var result = await _customers.ReplaceOneAsync(c => c.Id == id, update);

        // Return true if any document was modified
        return result.ModifiedCount > 0;
    }

    // Delete a customer by Id
    public async Task<bool> DeleteAsync(Guid id)
    {
        // Delete the document matching the Id
        var result = await _customers.DeleteOneAsync(c => c.Id == id);

        // Return true if a document was deleted
        return result.DeletedCount > 0;
    }
}
