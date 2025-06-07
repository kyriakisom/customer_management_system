using CustomerApi.Data;
using CustomerApi.Models;
using MongoDB.Driver;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace CustomerApi.Services;

public class UserService
{
    private readonly IMongoCollection<User> _users;

    public UserService(MongoDbContext context)
    {
        _users = context.Users;
    }

    public async Task<string?> RegisterAsync(string email, string username, string fullName, string password)
    {
        if (await _users.Find(u => u.Email == email || u.Username == username).AnyAsync())
            return null;

        var passwordHash = HashPassword(password);
        var user = new User
        {
            Email = email,
            Username = username,
            FullName = fullName,
            PasswordHash = passwordHash
        };

        await _users.InsertOneAsync(user);
        return user.Id;
    }

    public async Task<User?> AuthenticateAsync(string username, string password)
    {
        var user = await _users.Find(u => u.Username == username).FirstOrDefaultAsync();
        return user != null && VerifyPassword(password, user.PasswordHash) ? user : null;
    }

    private static string HashPassword(string password)
    {
        byte[] salt = new byte[128 / 8];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(salt);

        var hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password, salt, KeyDerivationPrf.HMACSHA256, 10000, 256 / 8));

        return $"{Convert.ToBase64String(salt)}.{hash}";
    }

    private static bool VerifyPassword(string password, string stored)
    {
        var parts = stored.Split('.');
        if (parts.Length != 2) return false;

        var salt = Convert.FromBase64String(parts[0]);
        var expectedHash = parts[1];

        var actualHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password, salt, KeyDerivationPrf.HMACSHA256, 10000, 256 / 8));

        return actualHash == expectedHash;
    }
}
