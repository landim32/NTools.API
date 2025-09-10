import { useState } from "react"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { CodeExample } from "@/components/CodeExample"
import { Mail } from "lucide-react"

export default function RecoverPassword() {
  const [email, setEmail] = useState("")

  const reactCode = `// Password Recovery Component
import { useState } from 'react'
import { useAuth } from '@nauth/react'

export function RecoverPasswordForm() {
  const [email, setEmail] = useState('')
  const [isSubmitted, setIsSubmitted] = useState(false)
  const { recoverPassword, loading, error } = useAuth()

  const handleSubmit = async (e) => {
    e.preventDefault()
    
    try {
      await recoverPassword({ email })
      setIsSubmitted(true)
    } catch (err) {
      console.error('Password recovery failed:', err)
    }
  }

  if (isSubmitted) {
    return (
      <div className="success-message">
        <h2>Check Your Email</h2>
        <p>
          If an account with email {email} exists, we've sent you a password reset link.
        </p>
        <p>
          Please check your email and follow the instructions to reset your password.
        </p>
      </div>
    )
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <div>
        <label htmlFor="email">Email Address</label>
        <input
          id="email"
          type="email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          placeholder="Enter your email address"
          required
        />
      </div>
      <button type="submit" disabled={loading}>
        {loading ? 'Sending recovery email...' : 'Send Recovery Email'}
      </button>
      {error && <p className="error">{error}</p>}
    </form>
  )
}`

  const dotnetCode = `// RecoverPasswordController.cs
using Microsoft.AspNetCore.Mvc;
using NAuth.Services;
using NAuth.Models;

[ApiController]
[Route("api/[controller]")]
public class RecoverPasswordController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IEmailService _emailService;
    private readonly ITokenService _tokenService;
    private readonly ILogger<RecoverPasswordController> _logger;

    public RecoverPasswordController(
        IAuthService authService,
        IEmailService emailService,
        ITokenService tokenService,
        ILogger<RecoverPasswordController> logger)
    {
        _authService = authService;
        _emailService = emailService;
        _tokenService = tokenService;
        _logger = logger;
    }

    [HttpPost("request")]
    public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Email) || !IsValidEmail(request.Email))
            {
                return BadRequest(new { message = "Valid email address is required" });
            }

            // Always return success for security (don't reveal if email exists)
            var user = await _authService.GetUserByEmailAsync(request.Email);
            
            if (user != null)
            {
                // Generate password reset token
                var resetToken = await _tokenService.GeneratePasswordResetTokenAsync(user.Id);
                
                // Create reset URL
                var resetUrl = $"{Request.Scheme}://{Request.Host}/reset-password?token={resetToken}&email={Uri.EscapeDataString(user.Email)}";
                
                // Send password reset email
                var emailResult = await _emailService.SendPasswordResetEmailAsync(
                    user.Email, 
                    user.Name ?? "User", 
                    resetUrl
                );

                if (emailResult.Success)
                {
                    // Log password reset request
                    await _authService.LogSecurityEventAsync(user.Id, "password_reset_requested", 
                        HttpContext.Connection.RemoteIpAddress?.ToString());
                }
            }

            // Always return the same response for security
            return Ok(new { 
                message = "If an account with that email exists, we've sent you a password reset link." 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Password reset request failed for email: {Email}", request.Email);
            return StatusCode(500, new { message = "An error occurred while processing your request" });
        }
    }

    [HttpPost("reset")]
    public async Task<IActionResult> ResetPassword([FromBody] PasswordResetConfirmRequest request)
    {
        try
        {
            var validationResult = ValidateResetRequest(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            // Verify reset token
            var tokenResult = await _tokenService.ValidatePasswordResetTokenAsync(request.Token);
            if (!tokenResult.IsValid)
            {
                return BadRequest(new { message = "Invalid or expired reset token" });
            }

            var user = await _authService.GetUserByIdAsync(tokenResult.UserId);
            if (user == null)
            {
                return BadRequest(new { message = "Invalid reset request" });
            }

            // Reset password
            var resetResult = await _authService.ResetPasswordAsync(user, request.NewPassword);
            
            if (resetResult.Success)
            {
                // Invalidate the reset token
                await _tokenService.InvalidatePasswordResetTokenAsync(request.Token);
                
                // Log successful password reset
                await _authService.LogSecurityEventAsync(user.Id, "password_reset_completed", 
                    HttpContext.Connection.RemoteIpAddress?.ToString());

                // Send confirmation email
                await _emailService.SendPasswordResetConfirmationAsync(user.Email, user.Name ?? "User");

                return Ok(new { message = "Password reset successfully" });
            }

            return BadRequest(new { message = resetResult.ErrorMessage });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Password reset failed for token: {Token}", request.Token);
            return StatusCode(500, new { message = "An error occurred while resetting password" });
        }
    }

    private ValidationResult ValidateResetRequest(PasswordResetConfirmRequest request)
    {
        var errors = new List<string>();

        if (string.IsNullOrEmpty(request.Token))
            errors.Add("Reset token is required");

        if (string.IsNullOrEmpty(request.NewPassword))
            errors.Add("New password is required");
        else if (request.NewPassword.Length < 8)
            errors.Add("Password must be at least 8 characters");

        if (request.NewPassword != request.ConfirmPassword)
            errors.Add("Passwords do not match");

        return new ValidationResult { IsValid = !errors.Any(), Errors = errors };
    }
}

public class PasswordResetRequest
{
    public string Email { get; set; }
}

public class PasswordResetConfirmRequest
{
    public string Token { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
}`

  return (
    <div className="container mx-auto max-w-6xl py-8 px-4">
      <div className="mb-8">
        <h1 className="text-3xl font-bold tracking-tight mb-2">Recover Password</h1>
        <p className="text-muted-foreground">
          Secure password recovery with email verification and token validation
        </p>
      </div>

      <div className="grid gap-8 lg:grid-cols-2">
        {/* Password Recovery Form */}
        <div>
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <Mail className="h-5 w-5" />
                Password Recovery
              </CardTitle>
              <CardDescription>
                Enter your email address and we'll send you a link to reset your password
              </CardDescription>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="email">Email Address</Label>
                <Input
                  id="email"
                  type="email"
                  placeholder="john@example.com"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                />
              </div>
              <Button className="w-full bg-gradient-to-r from-brand-primary to-brand-secondary hover:shadow-brand transition-all duration-300">
                Send Recovery Email
              </Button>
              <div className="text-center text-sm text-muted-foreground">
                <p>
                  Remember your password?{" "}
                  <a href="/login" className="text-brand-primary hover:underline">
                    Sign in instead
                  </a>
                </p>
              </div>
            </CardContent>
          </Card>
        </div>

        {/* Code Examples */}
        <div>
          <CodeExample
            reactCode={reactCode}
            dotnetCode={dotnetCode}
            title="Password Recovery Implementation"
          />
        </div>
      </div>
    </div>
  )
}