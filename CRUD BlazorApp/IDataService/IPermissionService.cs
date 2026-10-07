namespace CRUD_BlazorApp.IDataService
{
	public interface IPermissionService
	{
		Task<List<string>> GetActivePermissionsAsync(
		int userId);
	}
}
