using CRUD_BlazorApp.Common;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace CRUD_BlazorApp.RepositoryServices
{
	public class CustomAuthenticationStateProvider : AuthenticationStateProvider
	{
		private readonly UserSession userSession;

		public CustomAuthenticationStateProvider(UserSession userSession)
		{
			this.userSession = userSession;
		}

		public override Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			if (userSession.IsAuthenticated)
			{
				var claims = new List<Claim>
				{
					new Claim(ClaimTypes.NameIdentifier, userSession.UserId.ToString()),
					new Claim(ClaimTypes.Name, userSession.UserName),
					new Claim("AuthenticationType", userSession.AuthenticationType)
				};

				// Add role claims
				foreach (var role in userSession.Roles)
				{
					claims.Add(new Claim(ClaimTypes.Role, role));
				}

				// Add permission claims
				foreach (var permission in userSession.Permissions)
				{
					claims.Add(new Claim("Permission", permission));
				}

				var identity = new ClaimsIdentity(claims, "Custom");
				var principal = new ClaimsPrincipal(identity);
				var authenticationState = new AuthenticationState(principal);

				return Task.FromResult(authenticationState);
			}
			else
			{
				var anonymousPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
				var authenticationState = new AuthenticationState(anonymousPrincipal);

				return Task.FromResult(authenticationState);
			}
		}

		public void NotifyUserAuthenticated(string userName)
		{
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, userSession.UserId.ToString()),
				new Claim(ClaimTypes.Name, userName),
				new Claim("AuthenticationType", userSession.AuthenticationType)
			};

			foreach (var role in userSession.Roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, role));
			}

			foreach (var permission in userSession.Permissions)
			{
				claims.Add(new Claim("Permission", permission));
			}

			var identity = new ClaimsIdentity(claims, "Custom");
			var user = new ClaimsPrincipal(identity);
			var authenticationState = new AuthenticationState(user);

			NotifyAuthenticationStateChanged(Task.FromResult(authenticationState));
		}

		public void NotifyUserLogout()
		{
			var anonymousPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
			var authenticationState = new AuthenticationState(anonymousPrincipal);

			NotifyAuthenticationStateChanged(Task.FromResult(authenticationState));
		}
	}
}
