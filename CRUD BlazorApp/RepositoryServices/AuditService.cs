using CRUD_BlazorApp.RepositoryServices;
using CRUD_BlazorApp.Models;
using CRUD_BlazorApp;
using CRUD_BlazorApp.IDataService;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace CRUD_BlazorApp.RepositoryServices;

public class AuditService : IAuditService
{
	private readonly Microsoft.EntityFrameworkCore.IDbContextFactory<ApplicationDbContext> dbContextFactory;
	private readonly ILogger<AuditService> logger;

	public AuditService(
		IDbContextFactory<ApplicationDbContext> dbContextFactory,
		ILogger<AuditService> logger)
	{
		this.dbContextFactory = dbContextFactory;
		this.logger = logger;
	}

	public async Task LogAsync(
		int? userId,
		string? userName,
		string? authenticationType,
		string action,
		string status,
		string? details = null)
	{
		try
		{
			await using var dbContext =
				await dbContextFactory.CreateDbContextAsync();

			var audit = new AuditModel
			{
				UserId = userId,
				UserName = userName,
				AuthenticationType =
					authenticationType,

				Action = action,
				Status = status,
				Details = details,
				CreatedOn = DateTime.UtcNow
			};

			dbContext.AuditLogs.Add(audit);

			await dbContext.SaveChangesAsync();

		}
		catch (Exception ex)
		{
			logger.LogError(
				ex,
				"Failed to create audit log for {Action}",
				action);

			throw;
		}
	}
}