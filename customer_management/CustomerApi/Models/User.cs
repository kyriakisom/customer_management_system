using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace CustomerApi.Models;

// Represents a User entity stored in MongoDB
public class User
{
    [BsonId] // Marks this property as the primary key in MongoDB
    [BsonRepresentation(BsonType.ObjectId)] 
    // Store the Id as a MongoDB ObjectId (hex string)
    public string Id { get; set; } = string.Empty;

    [Required] // Email is required
    [EmailAddress] // Must be a valid email format
    public string Email { get; set; } = string.Empty;

    [Required] // Username is required
    public string Username { get; set; } = string.Empty;

    [Required] // FullName is required
    public string FullName { get; set; } = string.Empty;

    [Required] // PasswordHash is required
    // Stores the hashed password, not the plain text password for security
    public string PasswordHash { get; set; } = string.Empty;
}
