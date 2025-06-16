using CustomerApi.Data;
using CustomerApi.Models;
using MongoDB.Driver;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace CustomerApi.Services;

// Service class to handle user registration and authentication logic
public class UserService
{
    private readonly IMongoCollection<User> _users; // MongoDB collection for users

    // Constructor injects the MongoDbContext and gets the Users collection
    public UserService(MongoDbContext context)
    {
        _users = context.Users;
    }

    // Registers a new user with provided details and hashed password
    // Returns the new user's Id if successful, or null if email/username already exists
    public async Task<string?> RegisterAsync(string email, string username, string fullName, string password)
    {
        // Check if email or username already exists in the database
        if (await _users.Find(u => u.Email == email || u.Username == username).AnyAsync())
            return null; // User already exists

        // Hash the password securely before storing
        var passwordHash = HashPassword(password);

        // Create a new User instance with hashed password
        var user = new User
        {
            Email = email,
            Username = username,
            FullName = fullName,
            PasswordHash = passwordHash
        };

        // Insert the new user document into MongoDB
        await _users.InsertOneAsync(user);

        // Return the Id of the newly created user
        return user.Id;
    }

    // Authenticates a user by username and password
    // Returns the User if authentication succeeds, otherwise null
    public async Task<User?> AuthenticateAsync(string username, string password)
    {
        // Retrieve user by username
        var user = await _users.Find(u => u.Username == username).FirstOrDefaultAsync();

        // Verify password hash matches the provided password
        return user != null && VerifyPassword(password, user.PasswordHash) ? user : null;
    }

    // Helper method to hash passwords securely with a random salt using PBKDF2
    private static string HashPassword(string password)
    {
        // Generate a 128-bit (16-byte) salt using a secure RNG
        byte[] salt = new byte[128 / 8];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(salt);

        // Derive a 256-bit subkey (hash) using PBKDF2 with HMACSHA256 and 10,000 iterations
        var hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password, salt, KeyDerivationPrf.HMACSHA256, 10000, 256 / 8));

        // Return the salt and hash concatenated with a separator (.)
        return $"{Convert.ToBase64String(salt)}.{hash}";
    }

    // Helper method to verify a provided password against the stored salt+hash
    private static bool VerifyPassword(string password, string stored)
    {
        // Split the stored string into salt and hash parts
        var parts = stored.Split('.');
        if (parts.Length != 2) return false;

        var salt = Convert.FromBase64String(parts[0]); // Extract salt
        var expectedHash = parts[1]; // Extract stored hash

        // Recompute hash from the input password and stored salt
        var actualHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password, salt, KeyDerivationPrf.HMACSHA256, 10000, 256 / 8));

        // Compare computed hash with the stored hash securely
        return actualHash == expectedHash;
    }
}
