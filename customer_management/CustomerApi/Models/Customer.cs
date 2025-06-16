using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace CustomerApi.Models;

// Represents a Customer entity stored in MongoDB
public class Customer
{
    [BsonId] // Marks this property as the primary key in MongoDB
    [BsonRepresentation(BsonType.String)] // Store the Guid as a string in BSON
    public Guid Id { get; set; } = Guid.NewGuid(); // Unique identifier, auto-generated

    // Timestamp when the customer record was created, defaults to current UTC time
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required] // IdentityNumber is required
    [RegularExpression(@"^[A-Z]{2}\d{6}$")] 
    // Must match exactly 2 uppercase letters followed by 6 digits (e.g. AB123456)
    public string IdentityNumber { get; set; } = string.Empty;

    [Required] // FirstName is required
    [RegularExpression(@"^[Α-ΩA-Z][α-ωa-zΑ-Ωάέήίόύώϊϋΐΰ]{2,}$")] 
    // First letter uppercase (Latin or Greek), followed by at least 2 lowercase letters (Latin or Greek),
    // supports accented Greek characters as well
    public string FirstName { get; set; } = string.Empty;

    [Required] // LastName is required
    [RegularExpression(@"^[Α-ΩA-Z][α-ωa-zΑ-Ωάέήίόύώϊϋΐΰ]{2,}$")]
    // Same validation rules as FirstName
    public string LastName { get; set; } = string.Empty;

    [Required] // Gender is required
    [RegularExpression(@"^(male|female)$")] 
    // Only "male" or "female" are allowed values
    public string Gender { get; set; } = string.Empty;

    [Required] // BirthDate is required
    public DateTime BirthDate { get; set; }

    [Required] // At least one address is required
    public List<string> Addresses { get; set; } = new(); // List of customer addresses

    [Required] // At least one phone number is required
    public List<string> PhoneNumbers { get; set; } = new(); // List of customer phone numbers
}
