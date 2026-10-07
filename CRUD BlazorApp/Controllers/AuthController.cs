using Microsoft.AspNetCore.Mvc;
using CRUD_BlazorApp.IDataService;
using CRUD_BlazorApp.DTOs;
using System.Security.Claims;

namespace CRUD_BlazorApp.Controllers
{
	[ApiController]
	[Route("api/auth")]
	public class AuthController : ControllerBase
	{
		private readonly ILoginService loginService;
		private readonly IAuthenticationServices authenticationService;
		private readonly ILogger<AuthController> logger;

		public AuthController(
			ILoginService loginService,
			IAuthenticationServices authenticationService,
			ILogger<AuthController> logger)
		{
			this.loginService = loginService;
			this.authenticationService = authenticationService;
			this.logger = logger;
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequest request)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				// Verify user credentials
				var user = await loginService.VerifyUserAsync(
					request.UserName,
					request.Password);

				if (user == null)
				{
					return Unauthorized(new { error = "Invalid username or password" });
				}

				// Create authentication session
				var sessionCreated = await authenticationService.CreateSessionAsync(
					user.Id,
					user.UserName,
					"API");

				if (!sessionCreated)
				{
					return Unauthorized(new { error = "Access denied. No active role assigned" });
				}

				return Ok(new
				{
					message = "Login successful",
					data = new
					{
						userId = user.Id,
						userName = user.UserName
					}
				});
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Login error for user {UserName}", request.UserName);
				return StatusCode(500, new { error = "Internal server error" });
			}
		}

		[HttpPost("logout")]
		public async Task<IActionResult> Logout()
		{
			try
			{
				await authenticationService.LogoutAsync();
				return Ok(new { message = "Logout successful" });
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Logout error");
				return StatusCode(500, new { error = "Internal server error" });
			}
		}
	}

	public class LoginRequest
	{
		public string UserName { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;
	}
}
