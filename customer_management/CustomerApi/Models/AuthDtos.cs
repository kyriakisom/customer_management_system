using System.ComponentModel.DataAnnotations;

namespace CustomerApi.Models;

// Data Transfer Object for user signup/registration
public class SignupDto
{
    [Required] // Email is required
    [EmailAddress] // Must be a valid email format
    public string Email { get; set; } = string.Empty;

    [Required] // Username is required
    public string Username { get; set; } = string.Empty;

    [Required] // Full name is required
    public string FullName { get; set; } = string.Empty;

    [Required] // Password is required
    [DataType(DataType.Password)] // Treated as a password field (e.g. hides input in UI)
    public string Password { get; set; } = string.Empty;

    [Required] // Confirm password is required
    [DataType(DataType.Password)] // Treated as a password field
    [Compare("Password", ErrorMessage = "Passwords do not match")] 
    // Must match the value of the Password property; custom error message if not
    public string ConfirmPassword { get; set; } = string.Empty;
}

// Data Transfer Object for user login
public class LoginDto
{
    [Required] // Username is required
    public string Username { get; set; } = string.Empty;

    [Required] // Password is required
    [DataType(DataType.Password)] // Treated as a password field
    public string Password { get; set; } = string.Empty;
}
