import { useState } from "react"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Checkbox } from "@/components/ui/checkbox"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { CodeExample } from "@/components/CodeExample"
import { UserCheck } from "lucide-react"

export default function Register() {
  const [formData, setFormData] = useState({
    name: "",
    email: "",
    phone: "",
    street: "",
    number: "",
    zip: "",
    city: "",
    state: "",
    country: "",
    password: "",
    confirmPassword: "",
    keepSignedIn: false
  })

  const handleInputChange = (field: string, value: string | boolean) => {
    setFormData(prev => ({ ...prev, [field]: value }))
  }

  const reactCode = `// Full Registration Component
import { useState } from 'react'
import { useAuth } from '@nauth/react'

export function FullRegistrationForm() {
  const [formData, setFormData] = useState({
    name: '',
    email: '',
    phone: '',
    address: {
      street: '',
      number: '',
      zip: '',
      city: '',
      state: '',
      country: ''
    },
    password: '',
    confirmPassword: '',
    keepSignedIn: false
  })
  const { register, loading, error } = useAuth()

  const handleSubmit = async (e) => {
    e.preventDefault()
    
    if (formData.password !== formData.confirmPassword) {
      alert('Passwords do not match')
      return
    }

    try {
      await register({
        name: formData.name,
        email: formData.email,
        phone: formData.phone,
        address: formData.address,
        password: formData.password,
        rememberMe: formData.keepSignedIn
      })
    } catch (err) {
      console.error('Registration failed:', err)
    }
  }

  const handleInputChange = (field, value) => {
    if (field.startsWith('address.')) {
      const addressField = field.split('.')[1]
      setFormData(prev => ({
        ...prev,
        address: { ...prev.address, [addressField]: value }
      }))
    } else {
      setFormData(prev => ({ ...prev, [field]: value }))
    }
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-6">
      {/* Personal Information */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <input
          placeholder="Full Name"
          value={formData.name}
          onChange={(e) => handleInputChange('name', e.target.value)}
          required
        />
        <input
          type="email"
          placeholder="Email"
          value={formData.email}
          onChange={(e) => handleInputChange('email', e.target.value)}
          required
        />
        <input
          type="tel"
          placeholder="Phone Number"
          value={formData.phone}
          onChange={(e) => handleInputChange('phone', e.target.value)}
        />
      </div>

      {/* Address Information */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        <input
          placeholder="Street"
          value={formData.address.street}
          onChange={(e) => handleInputChange('address.street', e.target.value)}
        />
        <input
          placeholder="Number"
          value={formData.address.number}
          onChange={(e) => handleInputChange('address.number', e.target.value)}
        />
        <input
          placeholder="ZIP Code"
          value={formData.address.zip}
          onChange={(e) => handleInputChange('address.zip', e.target.value)}
        />
        <input
          placeholder="City"
          value={formData.address.city}
          onChange={(e) => handleInputChange('address.city', e.target.value)}
        />
        <input
          placeholder="State"
          value={formData.address.state}
          onChange={(e) => handleInputChange('address.state', e.target.value)}
        />
        <input
          placeholder="Country"
          value={formData.address.country}
          onChange={(e) => handleInputChange('address.country', e.target.value)}
        />
      </div>

      {/* Password Fields */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <input
          type="password"
          placeholder="Password"
          value={formData.password}
          onChange={(e) => handleInputChange('password', e.target.value)}
          minLength={8}
          required
        />
        <input
          type="password"
          placeholder="Confirm Password"
          value={formData.confirmPassword}
          onChange={(e) => handleInputChange('confirmPassword', e.target.value)}
          required
        />
      </div>

      <div>
        <input
          type="checkbox"
          checked={formData.keepSignedIn}
          onChange={(e) => handleInputChange('keepSignedIn', e.target.checked)}
        />
        <label>Keep me signed in</label>
      </div>

      <button type="submit" disabled={loading}>
        {loading ? 'Creating account...' : 'Create Account'}
      </button>
    </form>
  )
}`

  const dotnetCode = `// FullRegistrationController.cs
using Microsoft.AspNetCore.Mvc;
using NAuth.Services;
using NAuth.Models;

[ApiController]
[Route("api/[controller]")]
public class FullRegistrationController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IAddressService _addressService;
    private readonly ILogger<FullRegistrationController> _logger;

    public FullRegistrationController(
        IAuthService authService,
        IAddressService addressService,
        ILogger<FullRegistrationController> logger)
    {
        _authService = authService;
        _addressService = addressService;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> FullRegister([FromBody] FullRegistrationRequest request)
    {
        try
        {
            var validationResult = await ValidateFullRegistrationAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            // Create address
            var address = new Address
            {
                Street = request.Address.Street,
                Number = request.Address.Number,
                ZipCode = request.Address.Zip,
                City = request.Address.City,
                State = request.Address.State,
                Country = request.Address.Country
            };

            var addressResult = await _addressService.CreateAddressAsync(address);
            if (!addressResult.Success)
            {
                return BadRequest(new { message = "Failed to create address" });
            }

            // Create user with full profile
            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                AddressId = addressResult.Address.Id,
                CreatedAt = DateTime.UtcNow,
                IsEmailVerified = false,
                Profile = new UserProfile
                {
                    FirstName = ExtractFirstName(request.Name),
                    LastName = ExtractLastName(request.Name),
                    PhoneNumber = request.Phone
                }
            };

            var result = await _authService.CreateUserAsync(user, request.Password);
            
            if (result.Success)
            {
                // Send welcome email
                await _authService.SendWelcomeEmailAsync(user);
                
                // Generate token if remember me is checked
                if (request.RememberMe)
                {
                    var token = await _authService.GenerateTokenAsync(result.User);
                    return Ok(new FullRegistrationResponse
                    {
                        Message = "Account created successfully",
                        Token = token,
                        User = result.User
                    });
                }

                return Ok(new { message = "Account created successfully. Welcome!" });
            }

            return BadRequest(new { message = result.ErrorMessage });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Full registration failed for email: {Email}", request.Email);
            return StatusCode(500, new { message = "An error occurred during registration" });
        }
    }

    private async Task<ValidationResult> ValidateFullRegistrationAsync(FullRegistrationRequest request)
    {
        var errors = new List<string>();

        // Validate required fields
        if (string.IsNullOrEmpty(request.Name))
            errors.Add("Name is required");

        if (string.IsNullOrEmpty(request.Email))
            errors.Add("Email is required");
        else if (!IsValidEmail(request.Email))
            errors.Add("Invalid email format");
        else if (await _authService.EmailExistsAsync(request.Email))
            errors.Add("Email already exists");

        // Validate address
        if (request.Address != null)
        {
            if (string.IsNullOrEmpty(request.Address.City))
                errors.Add("City is required");
            if (string.IsNullOrEmpty(request.Address.Country))
                errors.Add("Country is required");
        }

        // Validate password
        if (string.IsNullOrEmpty(request.Password))
            errors.Add("Password is required");
        else if (request.Password.Length < 8)
            errors.Add("Password must be at least 8 characters");

        if (request.Password != request.ConfirmPassword)
            errors.Add("Passwords do not match");

        return new ValidationResult { IsValid = !errors.Any(), Errors = errors };
    }
}

public class FullRegistrationRequest
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public AddressRequest Address { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public bool RememberMe { get; set; }
}

public class AddressRequest
{
    public string Street { get; set; }
    public string Number { get; set; }
    public string Zip { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Country { get; set; }
}`

  return (
    <div className="container mx-auto max-w-6xl py-8 px-4">
      <div className="mb-8">
        <h1 className="text-3xl font-bold tracking-tight mb-2">Full Registration</h1>
        <p className="text-muted-foreground">
          Comprehensive user registration with complete profile information
        </p>
      </div>

      <div className="grid gap-8 lg:grid-cols-2">
        {/* Registration Form */}
        <div>
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <UserCheck className="h-5 w-5" />
                Complete Registration
              </CardTitle>
              <CardDescription>
                Create your account with full profile information
              </CardDescription>
            </CardHeader>
            <CardContent className="space-y-6">
              {/* Personal Information */}
              <div className="space-y-4">
                <h3 className="text-sm font-medium text-muted-foreground uppercase tracking-wider">
                  Personal Information
                </h3>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div className="space-y-2">
                    <Label htmlFor="name">Full Name</Label>
                    <Input
                      id="name"
                      placeholder="John Doe"
                      value={formData.name}
                      onChange={(e) => handleInputChange('name', e.target.value)}
                    />
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="email">Email</Label>
                    <Input
                      id="email"
                      type="email"
                      placeholder="john@example.com"
                      value={formData.email}
                      onChange={(e) => handleInputChange('email', e.target.value)}
                    />
                  </div>
                </div>
                <div className="space-y-2">
                  <Label htmlFor="phone">Phone Number</Label>
                  <Input
                    id="phone"
                    type="tel"
                    placeholder="+1 (555) 123-4567"
                    value={formData.phone}
                    onChange={(e) => handleInputChange('phone', e.target.value)}
                  />
                </div>
              </div>

              {/* Address Information */}
              <div className="space-y-4">
                <h3 className="text-sm font-medium text-muted-foreground uppercase tracking-wider">
                  Address Information
                </h3>
                <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                  <div className="space-y-2">
                    <Label htmlFor="street">Street</Label>
                    <Input
                      id="street"
                      placeholder="Main Street"
                      value={formData.street}
                      onChange={(e) => handleInputChange('street', e.target.value)}
                    />
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="number">Number</Label>
                    <Input
                      id="number"
                      placeholder="123"
                      value={formData.number}
                      onChange={(e) => handleInputChange('number', e.target.value)}
                    />
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="zip">ZIP Code</Label>
                    <Input
                      id="zip"
                      placeholder="12345"
                      value={formData.zip}
                      onChange={(e) => handleInputChange('zip', e.target.value)}
                    />
                  </div>
                </div>
                <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                  <div className="space-y-2">
                    <Label htmlFor="city">City</Label>
                    <Input
                      id="city"
                      placeholder="New York"
                      value={formData.city}
                      onChange={(e) => handleInputChange('city', e.target.value)}
                    />
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="state">State</Label>
                    <Input
                      id="state"
                      placeholder="NY"
                      value={formData.state}
                      onChange={(e) => handleInputChange('state', e.target.value)}
                    />
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="country">Country</Label>
                    <Input
                      id="country"
                      placeholder="United States"
                      value={formData.country}
                      onChange={(e) => handleInputChange('country', e.target.value)}
                    />
                  </div>
                </div>
              </div>

              {/* Password Fields */}
              <div className="space-y-4">
                <h3 className="text-sm font-medium text-muted-foreground uppercase tracking-wider">
                  Security
                </h3>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div className="space-y-2">
                    <Label htmlFor="password">Password</Label>
                    <Input
                      id="password"
                      type="password"
                      placeholder="••••••••"
                      value={formData.password}
                      onChange={(e) => handleInputChange('password', e.target.value)}
                    />
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="confirm-password">Confirm Password</Label>
                    <Input
                      id="confirm-password"
                      type="password"
                      placeholder="••••••••"
                      value={formData.confirmPassword}
                      onChange={(e) => handleInputChange('confirmPassword', e.target.value)}
                    />
                  </div>
                </div>
                <div className="flex items-center space-x-2">
                  <Checkbox
                    id="keep-signed-in"
                    checked={formData.keepSignedIn}
                    onCheckedChange={(checked) => handleInputChange('keepSignedIn', checked as boolean)}
                  />
                  <Label 
                    htmlFor="keep-signed-in" 
                    className="text-sm font-medium leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70"
                  >
                    Keep me signed in
                  </Label>
                </div>
              </div>

              <Button className="w-full bg-gradient-to-r from-brand-primary to-brand-secondary hover:shadow-brand transition-all duration-300">
                Create Complete Account
              </Button>
            </CardContent>
          </Card>
        </div>

        {/* Code Examples */}
        <div>
          <CodeExample
            reactCode={reactCode}
            dotnetCode={dotnetCode}
            title="Full Registration Implementation"
          />
        </div>
      </div>
    </div>
  )
}