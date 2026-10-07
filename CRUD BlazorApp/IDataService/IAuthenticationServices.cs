namespace CRUD_BlazorApp.IDataService
{
	public interface IAuthenticationServices
	{
		Task<bool> CreateSessionAsync(
		  int userId,
		  string userName,
		  string authenticationType);
		Task LogoutAsync();
	}
}
