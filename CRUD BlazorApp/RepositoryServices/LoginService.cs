using CRUD_BlazorApp.IDataService;
using CRUD_BlazorApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using CRUD_BlazorApp.Common;

namespace CRUD_BlazorApp.RepositoryServices
{
	public class LoginService : ILoginService
	{
		private readonly IDbContextFactory<ApplicationDbContext> dbContextFactory;
		private readonly ILogger<LoginService> logger;
		private readonly IPasswordHasher<UserModel> passwordHasher;
		private readonly IAuditService auditService;


		public LoginService(
			IDbContextFactory<ApplicationDbContext> dbContextFactory,
			ILogger<LoginService> logger,
			IPasswordHasher<UserModel> passwordHasher,
			IAuditService auditService)   // added parameter
		{
			this.dbContextFactory = dbContextFactory;
			this.logger = logger;
			this.passwordHasher = passwordHasher;
			this.auditService = auditService; // assign field
		}

		public async Task<UserModel?> VerifyUserAsync(
			string userName,
			string password)
		{
			try
			{
				await using var dbContext =
					await dbContextFactory.CreateDbContextAsync();

				var user = await dbContext.Users
					.FirstOrDefaultAsync(x =>
						x.UserName == userName &&
						x.IsActive);

				if (user == null)
				{
					await auditService.LogAsync(
							 null,
							userName,
				   Common.Constant.Emergency,
						 "Login",
						 "Failed",
						 "Invalid username or password");

					// removed undefined 'errorMessage' and return null
					return null;
				}

				var verificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
				if (verificationResult != PasswordVerificationResult.Success)
				{
					return null;
				}

				// Check if user has any active roles
				var hasActiveRole = await dbContext.UserRoles
					.Include(ur => ur.Role)
					.AnyAsync(ur => ur.UserId == user.Id && ur.Role!.IsActive);

				if (!hasActiveRole)
				{
					await auditService.LogAsync(
						user.Id,
						user.UserName,
						Common.Constant.Emergency,
						"Login",
						"Failed",
						"No active role assigned");

					return null;
				}

				await auditService.LogAsync(
					user.Id,
					user.UserName,
					Common.Constant.Emergency,
					"Login",
					"Success");

				return user;
			}
			catch (Exception ex)
			{
				logger.LogError(
					ex,
					"Error occurred while validating user {UserName}",
					userName);

				throw;
			}
		}
	}
}
