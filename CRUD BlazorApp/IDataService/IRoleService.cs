namespace CRUD_BlazorApp.IDataService
{
		public interface IRoleService
		{
			Task<List<string>> GetActiveRolesAsync(int userId);
		}
	
}
