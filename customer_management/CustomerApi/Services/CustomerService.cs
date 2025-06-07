using CustomerApi.Data;
using CustomerApi.Models;
using MongoDB.Driver;

namespace CustomerApi.Services;

public class CustomerService
{
    private readonly IMongoCollection<Customer> _customers;

    public CustomerService(MongoDbContext context)
    {
        _customers = context.Customers;
    }

    public async Task<List<Customer>> GetAllAsync() =>
        await _customers.Find(_ => true).ToListAsync();

    public async Task<Customer?> GetByIdAsync(Guid id) =>
        await _customers.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<Customer?> GetByIdentityAsync(string idNumber) =>
        await _customers.Find(x => x.IdentityNumber == idNumber).FirstOrDefaultAsync();

    public async Task<List<Customer>> SearchAsync(string query)
    {
        var filter = Builders<Customer>.Filter.Or(
            Builders<Customer>.Filter.Regex("FirstName", new MongoDB.Bson.BsonRegularExpression(query, "i")),
            Builders<Customer>.Filter.Regex("LastName", new MongoDB.Bson.BsonRegularExpression(query, "i")),
            Builders<Customer>.Filter.Regex("IdentityNumber", new MongoDB.Bson.BsonRegularExpression(query, "i"))
        );

        var customers = await _customers.Find(filter).ToListAsync();

        return customers.OrderBy(c => c.LastName)
                        .ThenBy(c => c.FirstName)
                        .ThenBy(c => c.IdentityNumber)
                        .ToList();
    }

    public async Task<string> CreateAsync(Customer customer)
    {
        if (await GetByIdentityAsync(customer.IdentityNumber) is not null)
            return "There is already a customer with this AT.";

        if ((DateTime.UtcNow - customer.BirthDate).TotalDays / 365 < 16)
            return "The customer must be at least 16 years old.";

        await _customers.InsertOneAsync(customer);
        return "The client was successfully added.";
    }

    public async Task<bool> UpdateAsync(Guid id, Customer update)
    {
        var existing = await GetByIdAsync(id);
        if (existing is null) return false;

        update.Id = existing.Id;
        update.CreatedAt = existing.CreatedAt;
        update.BirthDate = existing.BirthDate;

        var result = await _customers.ReplaceOneAsync(c => c.Id == id, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var result = await _customers.DeleteOneAsync(c => c.Id == id);
        return result.DeletedCount > 0;
    }
}
