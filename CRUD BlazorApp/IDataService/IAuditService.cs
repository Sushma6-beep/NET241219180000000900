namespace CRUD_BlazorApp.IDataService
{
	public interface IAuditService
	{
		Task LogAsync(
		 int? userId,
		 string? userName,
		 string? authenticationType,
		 string action,
		 string status,
		 string? details = null);
	}
}
