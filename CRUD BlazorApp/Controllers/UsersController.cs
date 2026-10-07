using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CRUD_BlazorApp.Common;
using CRUD_BlazorApp.IDataService;
using CRUD_BlazorApp.DTOs;
using CRUD_BlazorApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace CRUD_BlazorApp.Controllers
{
	[ApiController]
	[Route("api/users")]
	public class UsersController : ControllerBase
	{
		private readonly ApplicationDbContext dbContext;
		private readonly ILoginService loginService;
		private readonly IRoleService roleService;
		private readonly IPermissionService permissionService;
		private readonly IPasswordHasher<UserModel> passwordHasher;
		private readonly ILogger<UsersController> logger;

		public UsersController(
			ApplicationDbContext dbContext,
			ILoginService loginService,
			IRoleService roleService,
			IPermissionService permissionService,
			IPasswordHasher<UserModel> passwordHasher,
			ILogger<UsersController> logger)
		{
			this.dbContext = dbContext;
			this.loginService = loginService;
			this.roleService = roleService;
			this.permissionService = permissionService;
			this.passwordHasher = passwordHasher;
			this.logger = logger;
		}

		[HttpGet]
		[Authorize(Policy = PermissionConstants.UserView)]
		public async Task<IActionResult> GetUsers()
		{
			try
			{
				var users = await dbContext.Users
					.Where(u => u.IsActive)
					.Select(u => new UserResponse
					{
						Id = u.Id,
						UserName = u.UserName,
						Email = u.Email,
						IsActive = u.IsActive,
						CreatedAt = u.CreatedAt
					})
					.ToListAsync();

				if (!users.Any())
				{
					return Ok(new { message = "No users found", data = new List<UserResponse>() });
				}

				return Ok(new { message = "Users retrieved successfully", data = users });
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error getting users");
				return StatusCode(500, new { error = "Internal server error" });
			}
		}

		[HttpPost]
		[Authorize(Policy = PermissionConstants.UserCreate)]
		public async Task<IActionResult> CreateUser(
			[FromBody] CreateUserRequest request)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				// Check if user already exists
				var existingUser = await dbContext.Users
					.FirstOrDefaultAsync(u => u.UserName == request.UserName || u.Email == request.Email);

				if (existingUser != null)
				{
					return BadRequest(new { error = "Username or email already exists" });
				}

				// Create new user
				var newUser = new UserModel
				{
					UserName = request.UserName,
					Email = request.Email,
					IsActive = true,
					CreatedAt = DateTime.UtcNow
				};

				// Hash the password
				newUser.PasswordHash = passwordHasher.HashPassword(newUser, request.Password);

				// Add to database
				dbContext.Users.Add(newUser);
				await dbContext.SaveChangesAsync();

				// Return created response
				var response = new UserResponse
				{
					Id = newUser.Id,
					UserName = newUser.UserName,
					Email = newUser.Email,
					IsActive = newUser.IsActive,
					CreatedAt = newUser.CreatedAt
				};

				return CreatedAtAction(nameof(GetUsers), new { id = newUser.Id }, response);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error creating user");
				return StatusCode(500, new { error = "Internal server error" });
			}
		}

		[HttpPut("{id}")]
		[Authorize(Policy = PermissionConstants.UserEdit)]
		public async Task<IActionResult> UpdateUser(
			int id,
			[FromBody] UpdateUserRequest request)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				// Find user
				var user = await dbContext.Users.FindAsync(id);
				if (user == null)
				{
					return NotFound(new { error = "User not found" });
				}

				// Check if email is already taken by another user
				var emailExists = await dbContext.Users
					.AnyAsync(u => u.Email == request.Email && u.Id != id);

				if (emailExists)
				{
					return BadRequest(new { error = "Email already exists" });
				}

				// Update user
				user.UserName = request.UserName;
				user.Email = request.Email;

				dbContext.Users.Update(user);
				await dbContext.SaveChangesAsync();

				// Return updated response
				var response = new UserResponse
				{
					Id = user.Id,
					UserName = user.UserName,
					Email = user.Email,
					IsActive = user.IsActive,
					CreatedAt = user.CreatedAt
				};

				return Ok(new { message = "User updated successfully", data = response });
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error updating user {UserId}", id);
				return StatusCode(500, new { error = "Internal server error" });
			}
		}

		[HttpDelete("{id}")]
		[Authorize(Policy = PermissionConstants.UserDelete)]
		public async Task<IActionResult> DeleteUser(
			int id)
		{
			try
			{
				// Find user
				var user = await dbContext.Users.FindAsync(id);
				if (user == null)
				{
					return NotFound(new { error = "User not found" });
				}

				// Soft delete: mark as inactive instead of removing
				user.IsActive = false;

				dbContext.Users.Update(user);
				await dbContext.SaveChangesAsync();

				return Ok(new { message = "User deleted successfully" });
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error deleting user {UserId}", id);
				return StatusCode(500, new { error = "Internal server error" });
			}
		}
	}
}
