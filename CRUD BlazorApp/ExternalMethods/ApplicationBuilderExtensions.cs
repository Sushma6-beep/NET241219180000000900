using CRUD_BlazorApp.Components;

namespace CRUD_BlazorApp.ExternalMethods
{
	public static class ApplicationBuilderExtensions
	{
		public static WebApplication
		   UseApplicationPipeline(
			   this WebApplication app)
		{
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Error");
				app.UseHsts();
			}

			app.UseHttpsRedirection();

			app.UseAntiforgery();

			app.MapStaticAssets();

			app.MapRazorComponents<App>()
				.AddInteractiveServerRenderMode();

			return app;
		}
	}
}
