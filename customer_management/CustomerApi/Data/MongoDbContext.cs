using CustomerApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CustomerApi.Data;

// MongoDbContext handles the connection to MongoDB and exposes collections
public class MongoDbContext
{
    private readonly IMongoDatabase _database; // Reference to the MongoDB database

    // Constructor accepts IConfiguration to read MongoDB connection settings
    public MongoDbContext(IConfiguration configuration)
    {
        // Create a MongoClient using the connection string from configuration
        var client = new MongoClient(configuration["MongoDb:ConnectionString"]);
        
        // Get the specific database by name from configuration
        _database = client.GetDatabase(configuration["MongoDb:Database"]);
    }

    // Property to access the Customers collection in MongoDB
    public IMongoCollection<Customer> Customers => _database.GetCollection<Customer>("Customers");

    // Property to access the Users collection in MongoDB
    public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
}
