using CRUD_BlazorApp.Models;

namespace CRUD_BlazorApp.IDataService
{
		public interface ILoginService
		{
			Task<UserModel?> VerifyUserAsync(
				string userName,
				string password);
		}
	
}
