﻿using flashcard.utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using System.Security.Claims;

namespace flashcard.Components.Pages.Auth
{
	public partial class Callback : ComponentBase
	{
		[CascadingParameter]
		public required HttpContext HttpContext { get; set; }

		protected override async Task OnInitializedAsync()
		{
			if (!HttpContext.User.Identity!.IsAuthenticated)
			{
				NavigationManager.NavigateTo("/");
				return;
			}
			var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
			var user = authState.User;

			var email = user.FindFirst(ClaimTypes.Email)?.Value;

			if (email == null)
			{
				NavigationManager.NavigateTo("/");
				return;
			}

			dbAccountService.AddNewAccount(email);
			var claims = new[]
			{
			new Claim(ClaimTypes.Email, email)
			};
			var identity = new ClaimsIdentity(claims, AuthenticationConstants.CookieName);
			var principal = new ClaimsPrincipal(identity);
			await HttpContext.SignInAsync(principal);

			NavigationManager.NavigateTo("/");
		}
	}
}