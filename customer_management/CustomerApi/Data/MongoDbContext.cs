using CustomerApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CustomerApi.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IConfiguration configuration)
    {
        var client = new MongoClient(configuration["MongoDb:ConnectionString"]);
        _database = client.GetDatabase(configuration["MongoDb:Database"]);
    }

    public IMongoCollection<Customer> Customers => _database.GetCollection<Customer>("Customers");
public IMongoCollection<User> Users => _database.GetCollection<User>("Users");

}
