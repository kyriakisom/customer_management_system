using CustomerApi.Models;
using CustomerApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CustomerApi.Controllers;

// Mark this class as an API controller and set route prefix as "api/auth"
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserService _userService;  // Service to handle user-related operations
    private readonly IConfiguration _config;    // To access configuration values like JWT key

    // Constructor with dependency injection of UserService and IConfiguration
    public AuthController(UserService userService, IConfiguration config)
    {
        _userService = userService;
        _config = config;
    }

    // POST api/auth/signup
    // Endpoint to register a new user
    [HttpPost("signup")]
    public async Task<IActionResult> Signup([FromBody] SignupDto dto)
    {
        // Check if the incoming model is valid
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Attempt to register the user via UserService
        var userId = await _userService.RegisterAsync(dto.Email, dto.Username, dto.FullName, dto.Password);
        if (userId == null)
            // If registration failed (e.g. email or username already exists), return BadRequest
            return BadRequest("Email or Username already in use");

        // Return success message if registration is successful
        return Ok("User registered successfully");
    }

    // POST api/auth/login
    // Endpoint to authenticate a user and generate a JWT token
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        // Validate the incoming model
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Authenticate user credentials using UserService
        var user = await _userService.AuthenticateAsync(dto.Username, dto.Password);
        if (user == null)
            // If authentication fails, return Unauthorized
            return Unauthorized("Invalid credentials");

        // Generate JWT token for authenticated user
        var token = GenerateJwtToken(user);

        // Return the token in the response
        return Ok(new { Token = token });
    }

    // Private method to generate JWT token based on user details
    private string GenerateJwtToken(User user)
    {
        // Retrieve secret key from configuration and convert to bytes
        var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]!);

        // Define token descriptor including claims, expiry and signing credentials
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] {
                // Claims to include in the token payload
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("FullName", user.FullName)
            }),
            // Token expires in 1 hour from the current UTC time
            Expires = DateTime.UtcNow.AddHours(1),
            // Use HMAC SHA256 algorithm and the secret key to sign the token
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        // Create token handler to generate token
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        // Return the serialized JWT token string
        return tokenHandler.WriteToken(token);
    }
}
