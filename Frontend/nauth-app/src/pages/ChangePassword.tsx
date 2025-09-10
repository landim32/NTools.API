import { useState } from "react"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { CodeExample } from "@/components/CodeExample"
import { KeyRound } from "lucide-react"

export default function ChangePassword() {
  const [currentPassword, setCurrentPassword] = useState("")
  const [newPassword, setNewPassword] = useState("")
  const [confirmPassword, setConfirmPassword] = useState("")

  const reactCode = `// Change Password Component
import { useState } from 'react'
import { useAuth } from '@nauth/react'

export function ChangePasswordForm() {
  const [currentPassword, setCurrentPassword] = useState('')
  const [newPassword, setNewPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')
  const { changePassword, loading, error } = useAuth()

  const handleSubmit = async (e) => {
    e.preventDefault()
    
    if (newPassword !== confirmPassword) {
      alert('New passwords do not match')
      return
    }

    if (newPassword === currentPassword) {
      alert('New password must be different from current password')
      return
    }

    try {
      await changePassword({
        currentPassword,
        newPassword,
        confirmPassword
      })
      
      alert('Password changed successfully!')
      setCurrentPassword('')
      setNewPassword('')
      setConfirmPassword('')
    } catch (err) {
      console.error('Password change failed:', err)
    }
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <div>
        <label htmlFor="currentPassword">Current Password</label>
        <input
          id="currentPassword"
          type="password"
          value={currentPassword}
          onChange={(e) => setCurrentPassword(e.target.value)}
          required
        />
      </div>
      <div>
        <label htmlFor="newPassword">New Password</label>
        <input
          id="newPassword"
          type="password"
          value={newPassword}
          onChange={(e) => setNewPassword(e.target.value)}
          minLength={8}
          required
        />
      </div>
      <div>
        <label htmlFor="confirmPassword">Confirm New Password</label>
        <input
          id="confirmPassword"
          type="password"
          value={confirmPassword}
          onChange={(e) => setConfirmPassword(e.target.value)}
          required
        />
      </div>
      <button type="submit" disabled={loading}>
        {loading ? 'Changing password...' : 'Change Password'}
      </button>
      {error && <p className="error">{error}</p>}
    </form>
  )
}`

  const dotnetCode = `// ChangePasswordController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using NAuth.Services;
using NAuth.Models;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ChangePasswordController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IPasswordValidator _passwordValidator;
    private readonly ILogger<ChangePasswordController> _logger;

    public ChangePasswordController(
        IAuthService authService,
        IPasswordValidator passwordValidator,
        ILogger<ChangePasswordController> logger)
    {
        _authService = authService;
        _passwordValidator = passwordValidator;
        _logger = logger;
    }

    [HttpPost("change")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        try
        {
            var userId = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var validationResult = ValidateChangePasswordRequest(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            // Verify current password
            var user = await _authService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            var isCurrentPasswordValid = await _authService.VerifyPasswordAsync(user, request.CurrentPassword);
            if (!isCurrentPasswordValid)
            {
                return BadRequest(new { message = "Current password is incorrect" });
            }

            // Validate new password
            var passwordValidation = _passwordValidator.Validate(request.NewPassword);
            if (!passwordValidation.IsValid)
            {
                return BadRequest(passwordValidation.Errors);
            }

            // Change password
            var result = await _authService.ChangePasswordAsync(user, request.NewPassword);
            
            if (result.Success)
            {
                // Log password change
                await _authService.LogSecurityEventAsync(user.Id, "password_changed", 
                    HttpContext.Connection.RemoteIpAddress?.ToString());

                // Send notification email
                await _authService.SendPasswordChangedNotificationAsync(user);

                return Ok(new { message = "Password changed successfully" });
            }

            return BadRequest(new { message = result.ErrorMessage });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Password change failed for user: {UserId}", User.FindFirst("userId")?.Value);
            return StatusCode(500, new { message = "An error occurred while changing password" });
        }
    }

    private ValidationResult ValidateChangePasswordRequest(ChangePasswordRequest request)
    {
        var errors = new List<string>();

        if (string.IsNullOrEmpty(request.CurrentPassword))
            errors.Add("Current password is required");

        if (string.IsNullOrEmpty(request.NewPassword))
            errors.Add("New password is required");
        else if (request.NewPassword.Length < 8)
            errors.Add("New password must be at least 8 characters");

        if (string.IsNullOrEmpty(request.ConfirmPassword))
            errors.Add("Password confirmation is required");

        if (request.NewPassword != request.ConfirmPassword)
            errors.Add("New passwords do not match");

        if (request.CurrentPassword == request.NewPassword)
            errors.Add("New password must be different from current password");

        return new ValidationResult { IsValid = !errors.Any(), Errors = errors };
    }
}

public class ChangePasswordRequest
{
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
}`

  return (
    <div className="container mx-auto max-w-6xl py-8 px-4">
      <div className="mb-8">
        <h1 className="text-3xl font-bold tracking-tight mb-2">Change Password</h1>
        <p className="text-muted-foreground">
          Secure password updating with validation and security logging
        </p>
      </div>

      <div className="grid gap-8 lg:grid-cols-2">
        {/* Change Password Form */}
        <div>
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <KeyRound className="h-5 w-5" />
                Update Password
              </CardTitle>
              <CardDescription>
                Enter your current password and choose a new secure password
              </CardDescription>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="current-password">Current Password</Label>
                <Input
                  id="current-password"
                  type="password"
                  placeholder="••••••••"
                  value={currentPassword}
                  onChange={(e) => setCurrentPassword(e.target.value)}
                />
              </div>
              <div className="space-y-2">
                <Label htmlFor="new-password">New Password</Label>
                <Input
                  id="new-password"
                  type="password"
                  placeholder="••••••••"
                  value={newPassword}
                  onChange={(e) => setNewPassword(e.target.value)}
                />
                <p className="text-xs text-muted-foreground">
                  Password must be at least 8 characters long
                </p>
              </div>
              <div className="space-y-2">
                <Label htmlFor="confirm-new-password">Confirm New Password</Label>
                <Input
                  id="confirm-new-password"
                  type="password"
                  placeholder="••••••••"
                  value={confirmPassword}
                  onChange={(e) => setConfirmPassword(e.target.value)}
                />
              </div>
              <Button className="w-full bg-gradient-to-r from-brand-primary to-brand-secondary hover:shadow-brand transition-all duration-300">
                Change Password
              </Button>
            </CardContent>
          </Card>
        </div>

        {/* Code Examples */}
        <div>
          <CodeExample
            reactCode={reactCode}
            dotnetCode={dotnetCode}
            title="Change Password Implementation"
          />
        </div>
      </div>
    </div>
  )
}