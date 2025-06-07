using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace CustomerApi.Models;

public class Customer
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    [RegularExpression(@"^[A-Z]{2}\d{6}$")]
    public string IdentityNumber { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^[Α-ΩA-Z][α-ωa-zΑ-Ωάέήίόύώϊϋΐΰ]{2,}$")]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^[Α-ΩA-Z][α-ωa-zΑ-Ωάέήίόύώϊϋΐΰ]{2,}$")]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^(male|female)$")]
    public string Gender { get; set; } = string.Empty;

    [Required]
    public DateTime BirthDate { get; set; }

    [Required]
    public List<string> Addresses { get; set; } = new();

    [Required]
    public List<string> PhoneNumbers { get; set; } = new();
}
